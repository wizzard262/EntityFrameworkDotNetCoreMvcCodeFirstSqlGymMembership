(toggle render/edit this file in VS Code: Ctrl + Shift + V)

# Gym Membership Demo  
A minimal ASP.NET Core MVC + Entity Framework Core **Code‑First** demo that:
- Creates an **Azure SQL Database** via Terraform  
- Deploys an **Azure App Service**  
- Uses EF Core migrations to build the schema  
- Demonstrates a **many‑to‑many relationship** using a join table **without a CLR class**  
- Seeds test data (members, classes, and join table entries)  
- Displays the data on a single MVC page 

---

## **Technology Stack**
- .NET 10 
- ASP.NET Core MVC  
- Entity Framework Core 8  
- SQL Server / Azure SQL  
- Terraform (AzureRM provider)  
- Azure App Service  

---

## **Database Model Overview**
EF Core generates a join table named: GymMemberClassSession
### **Entities**
- `GymMember`
- `ClassSession`

### **Many‑to‑Many Join Table**
EF Core generates a join table. This is not in the C# Models but the relationships imply it and so it is created:
- `GymMemberClassSession`

## 1. Local SQL Database Setup (for running EF Core migrations locally)
Terraform only creates the **Azure SQL Database**.  (see: /infra/*)

For local development you need a local SQL Server instance and to run the commands below:

### Let EF Core create the database automatically and popualte  the test data

If the database **does not exist**, EF Core will create it when you run:
(in for example VS Code --> Terminal --> Cmd)

Change to the correct folder (and install the EF Core CLI tool if needed):

```bash (i.e)
cd C:\DEV\Repositories\GitHub\EFCoreMvcCodeFirstSqlGymMembers\GymMembers\src
dotnet tool install --global dotnet-ef
```

Run this to create migrations in a new folder: 
(e.g. C:\DEV\Repositories\GitHub\EFCoreMvcCodeFirstSqlGymMembers\GymMembers\src\Migrations)
It should populate the folder with some *.cs files that will run the Migrations.
```
dotnet ef migrations add InitialCreate
```

Run the update command. This is idempotent (It only applies migrations that have not been applied yet. It never re‑runs a migration that already exists in the DB table: `__EFMigrationsHistory`)
This Runs all pending EF Core migrations using the connection string in **appsettings.json**
* It create Migration table: `__EFMigrationsHistory`
* It creates or updates the specified database `GymMembership` so its schema matches the current EF Core model.
* It gets the schema that is defined in : **C:\DEV\Repositories\GitHub\EFCoreMvcCodeFirstSqlGymMembers\GymMembers\src\Data\ApplicationDbContext.cs**
* **Note:** **ApplicationDbContext.cs** also contains some test data, and this is populated into the DB too.
```bash (i.e)
dotnet ef database update
```

This builds the full schema anf test data locally so you can run and test the app before deploying to Azure.
Note the table "__EFMigrationsHistory" now has a row added showing that a Migration has run.

### Run the VS Solution. It renders the table of 
| Member         | Joined      | Classes                                      |
|----------------|-------------|-----------------------------------------------|
| Charlie Brown  | 01/03/2024  | Spin Class (02 Jun 2024 07:00)               |
| Alice Johnson  | 10/01/2024  | Morning Yoga (01 Jun 2024 08:00)<br>HIIT Blast (01 Jun 2024 18:00) |
| Bob Smith      | 05/02/2024  | HIIT Blast (01 Jun 2024 18:00)               |


## 2. Terraform deployment to Azure

This project includes Infrastructure‑as‑Code (IaC) using **Terraform** to deploy:
- Azure SQL Database (General Purpose – Serverless – Standard-series) FREE-TIER!
- Azure App Service (Web App)
- Connection string injection
- App settings configuration

The Terraform file is located in: /GymMembers/infra/inframain.tf

---

## 📦 Prerequisites

Before running Terraform, ensure you have:

- **Terraform CLI** installed  
  https://developer.hashicorp.com/terraform/downloads
   - No MSI! So open the zip and put terraform.exe into **C:\Program Files\Terraform\terraform.exe**
   - Start --> Environmental Variables --> 'Environmental Variables' Button --> System Variables --> Edit PATH entry --> Add: C:\Program Files\Terraform

- **Azure CLI** installed  
  https://learn.microsoft.com/cli/azure/install-azure-cli

- An existing Azure setup containing:
  - Resource Group: **ResourceGroup1**
  - App Service Plan: **ASP-ResourceGroup1-8ac4**
  - SQL Server: **wizzard262**

These are referenced using Terraform `data` blocks and must already exist.

---

### 🔐 1. Authenticate to Azure

Log in to your Azure account:

```bash
az login
```

Note: You might need to (I needed to ) do: 
```bash
az login --use-device-code
```
And then got : "To sign in, use a web browser to open the page https://login.microsoft.com/device and enter the code XXXXXXXX to authenticate."....which worked!"

If you have multiple subscriptions set the correct Subscription: (I only have 1: "Azure subscription 1")
```bash
az account set --subscription "<YOUR_SUBSCRIPTION_ID>"
```

### 📁 2. Navigate to the Terraform folder
```bash
cd C:\DEV\Repositories\GitHub\EFCoreMvcCodeFirstSqlGymMembers\GymMembers\infra
```

### 🧱 3. Initialize Terraform
```bash
terraform init
```

N.B. You must provide you Azure SQL Server Admin password. This is used for the Web App, not for the SQL Server DB Creation
(or if you just go to run **terraform plan** or **terraform apply** it requests the password)
Note that it is Terraform that sets and passes in the DB connection string and sets it as an Azure as:
Web App → Settings → Configuration → Connection strings → DefaultConnection

```bash
terraform apply -var="sql_admin_password=Octopus8"
```

### 📝 4. Review the execution plan
```bash
terraform plan
```
You should see:
- NEW SQL Database: GymMembership
- NEW App Service: gym-membership-webapp
- Connection string injection
- App settings
No existing resources will be recreated because they are referenced via data blocks.

### 🚢 5. Apply the infrastructure
```bash
terraform apply
```
Type yes when prompted.
Terraform will:
- Create the GymMembership SQL Database
- Create the gym-membership-webapp App Service
- Inject the DefaultConnection connection string
- Set ASPNETCORE_ENVIRONMENT=Development
- Link everything to your existing Resource Group, App Service Plan, and SQL Server

🧹 7. Destroy the infrastructure!!! (optional)
If you want to remove everything Terraform created:

```bash
terraform destroy
```
This deletes:
- The SQL Database
- The App Service
- It DOES NOT delete your existing Resource Group, App Service Plan, or SQL Server.

## 3. Deploy the MVC App and run the Entity Framework Migrations
Terraform deployment only created the Azure Resources, now we need to deploy the project.
An Azure DevOps Pipeline will build & deploy the C# Code and run the Entity Framework Migrations.

### NOTE: Terraform CANNOT create the Azure SQL "Free" tier!
Azure does NOT expose the Free tier through ARM, Bicep, or Terraform.
I deleted and recreated DB **GymMembership** via ClickOps (via the portal webpages)
DB Creation commented out of the Terraform file

### NOTE: Terraform CANNOT use any existing AppServicePlan
None match all of the correct type, operating system, region, etc
We had to make a new one
