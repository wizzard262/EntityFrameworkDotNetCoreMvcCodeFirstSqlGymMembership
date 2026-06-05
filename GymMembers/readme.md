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