resource "azurerm_cognitive_account" "cognitive_account" {
  name = var.account_name
  custom_subdomain_name = var.account_name
  location = var.account_location
  resource_group_name = var.resource_group_name
  kind = var.account_kind
  sku_name = var.sku_name
  tags = var.tags
}

resource "azurerm_cognitive_deployment" "cognitive_deployment" {
  for_each = { for record in var.openai_deployments : record.name => record }
  name = each.key
  cognitive_account_id = azurerm_cognitive_account.cognitive_account.id
  rai_policy_name = "Microsoft.DefaultV2"
  model {
    format = "OpenAI"
    name = each.value.model_name
    version = each.value.version
  }
  scale {
    type = each.value.sku_name
    capacity = each.value.capacity
  }
}