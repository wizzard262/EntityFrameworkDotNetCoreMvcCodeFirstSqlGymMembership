/*
This Terraform configuration:

1. READS existing Azure resources:
   - Resource Group
   - Azure SQL Server (azurerm_mssql_server)
   - Existing SQL Database (FREE tier created manually)

2. CREATES new resources:
   - Windows App Service Plan (cheapest Windows SKU: B1)
   - Azure Windows Web App (azurerm_windows_web_app)

3. CONFIGURES the Web App:
   - Injects the SQL connection string (pointing to the existing DB)
   - Sets ASP.NET Core environment to Development

IMPORTANT NOTE ABOUT THE DATABASE:

Azure SQL Database **Free Tier** cannot be created using Terraform, ARM, Bicep,
Azure CLI, or any automation. Microsoft only exposes the Free tier through the
Azure Portal UI, and it must be created manually.

Because of this limitation:

- The SQL Database "GymMembership" MUST already exist in Azure
  (created manually via the Portal using the Free tier option)
- Terraform will NOT create, modify, or delete the database
- Terraform only READS the existing database using a `data` block
- The Web App connection string points to this pre-existing DB

If the database does not already exist before running `terraform apply`,
the deployment will fail.
*/

# ---------------------------------------------------------
# Terraform Provider block
# ---------------------------------------------------------
terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 4.0"
    }
  }
}

provider "azurerm" {
  features {}
}

# ---------------------------------------------------------
# Use existing Resource Group
# ---------------------------------------------------------
data "azurerm_resource_group" "rg" {
  name = "ResourceGroup1"
}

# ---------------------------------------------------------
# Use existing SQL Server
# ---------------------------------------------------------
data "azurerm_mssql_server" "sql" {
  name                = "wizzard262"
  resource_group_name = data.azurerm_resource_group.rg.name
}

# ---------------------------------------------------------
# Use existing SQL Database (FREE tier created manually)
# ---------------------------------------------------------
data "azurerm_mssql_database" "db" {
  name      = "GymMembership"
  server_id = data.azurerm_mssql_server.sql.id
}

# ----------------------------------------------------------------
# SQL Admin Password (passed into connection string)
# ----------------------------------------------------------------
variable "sql_admin_password" {
  type      = string
  sensitive = true
}

# ---------------------------------------------------------
# Create NEW Windows App Service Plan (cheapest Windows SKU: B1)
# ---------------------------------------------------------
resource "azurerm_service_plan" "plan" {
  name                = "asp-gym-membership-b1"
  resource_group_name = data.azurerm_resource_group.rg.name
  location            = data.azurerm_resource_group.rg.location

  os_type             = "Windows"
  sku_name            = "B1"   # Cheapest Windows tier
}

# ---------------------------------------------------------
# Create NEW Windows Web App
# ---------------------------------------------------------
resource "azurerm_windows_web_app" "app" {
  name                = "gym-membership-webapp"
  location            = data.azurerm_resource_group.rg.location
  resource_group_name = data.azurerm_resource_group.rg.name

  service_plan_id     = azurerm_service_plan.plan.id

  site_config {
    always_on = false
  }

  # Inject EF Core connection string into Web App
  connection_string {
    name  = "DefaultConnection"
    type  = "SQLAzure"
    value = "Server=tcp:${data.azurerm_mssql_server.sql.fully_qualified_domain_name},1433;Database=${data.azurerm_mssql_database.db.name};User ID=${data.azurerm_mssql_server.sql.administrator_login};Password=${var.sql_admin_password};Encrypt=true;TrustServerCertificate=False;Connection Timeout=30;"
  }

  # App settings for ASP.NET Core
  app_settings = {
    "ASPNETCORE_ENVIRONMENT" = "Development"
  }
}
