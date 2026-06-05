# Use existing Resource Group
data "azurerm_resource_group" "rg" {
  name = "ResourceGroup1"
}

# Use existing App Service Plan
data "azurerm_app_service_plan" "plan" {
  name                = "ASP-ResourceGroup1-8ac4"
  resource_group_name = data.azurerm_resource_group.rg.name
}

# Use existing SQL Server
data "azurerm_sql_server" "sql" {
  name                = "wizzard262"
  resource_group_name = data.azurerm_resource_group.rg.name
}

# Create NEW SQL Database
resource "azurerm_sql_database" "db" {
  name                = "GymMembership"
  resource_group_name = data.azurerm_resource_group.rg.name
  location            = data.azurerm_resource_group.rg.location
  server_name         = data.azurerm_sql_server.sql.name
  sku_name            = "Basic"
}

# Create NEW App Service
resource "azurerm_app_service" "app" {
  name                = "gym-membership-webapp"
  location            = data.azurerm_resource_group.rg.location
  resource_group_name = data.azurerm_resource_group.rg.name
  app_service_plan_id = data.azurerm_app_service_plan.plan.id

  connection_string {
    name  = "DefaultConnection"
    type  = "SQLAzure"
    value = "Server=tcp:${data.azurerm_sql_server.sql.name}.database.windows.net,1433;Database=${azurerm_sql_database.db.name};User ID=${data.azurerm_sql_server.sql.administrator_login};Password=${data.azurerm_sql_server.sql.administrator_login_password};Encrypt=true;TrustServerCertificate=False;Connection Timeout=30;"
  }

  app_settings = {
    "ASPNETCORE_ENVIRONMENT" = "Production"
  }
}
