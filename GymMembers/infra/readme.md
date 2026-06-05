# Infrastructure for GymMembership Demo (Terraform)

This folder contains Terraform configuration that deploys the Azure
infrastructure required for the **GymMembership** ASP.NET Core MVC + EF Core
demo application.

Terraform is configured to **use your existing Azure resources**, and only
provisions what is missing.

---

## Existing Azure Resources (Imported via Data Sources)

Terraform does **not** recreate these:

- **Resource Group:** `ResourceGroup1`
- **SQL Server:** `wizzard262`
- **App Service Plan:** `ASP-ResourceGroup1-8ac4` (UK West)

These are referenced using Terraform `data` blocks so they can be reused safely.

---

## Resources Terraform Creates

### 1. Azure SQL Database
A new SQL database named **`GymMembership`** is created on your existing SQL
Server (`wizzard262`).  
EF Core migrations will populate the schema and seed data on first run.

### 2. Azure App Service
A new Web App named **`gym-membership-webapp`** is created inside your existing
App Service Plan.

Terraform configures:

- The App Service Plan association  
- Production environment mode  
- The **DefaultConnection** connection string pointing to Azure SQL  

This allows EF Core to run migrations automatically on startup.

---

## Deployment Workflow

### Step 1 — Deploy Infrastructure

From the `infra/` folder:

```bash
terraform init
terraform apply
