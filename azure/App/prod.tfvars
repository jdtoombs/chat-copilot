azure_subscription_id = "b2cba309-26dd-459c-a021-54cb56fe6c49"
azure_tenant_id       = "898fdc18-1bd2-4a3b-84a7-2efb988e3b90"
azure_environment     = "public"

location = {
  name        = "canadacentral",
  region_code = "cnc"
}
location_openai = {
  name        = "canadaeast",
  region_code = "cne"
}

project_code = "copilot"

environment = "prod"

tags = {
  client         = "Quartech"
  owner          = "david.camden@quartech.com"
  project        = "Q-Pilot",
  client-name    = "Quartech",
  project-name   = "Q-Pilot",
  developer-name = "Quartech"
}

kubernetes_azure_subscription_id = "6163682b-8d24-45e8-8352-3fc634febf51"
kubernetes_azure_tenant_id       = "898fdc18-1bd2-4a3b-84a7-2efb988e3b90"
kubernetes_cluster_name          = "aks-sharedaks-cnc-cluster"
kubernetes_resource_group_name   = "rg-sharedaks-cnc-cluster-aks"

acr_name                = "crsharedakscluster"
acr_resource_group_name = "rg-sharedaks-cnc-cluster-cr"

app_api_object_id    = "55f7148f-b09d-4916-ba12-5c3f1ff80b4a"
app_github_object_id = "adc2f406-2df6-4c47-a542-ae55cf73d35f"

cosmosdb_sql_containers = [
  { name = "chatsessions", partition_key_path = "/id" },
  { name = "chatmessages", partition_key_path = "/chatId" },
  { name = "chatmemorysources", partition_key_path = "/chatId" },
  { name = "chatparticipants", partition_key_path = "/chatId" },
  { name = "specialization", partition_key_path = "/id" },
  { name = "chatuser", partition_key_path = "/id" },
  { name = "specializationindex", partition_key_path = "/id" },
  { name = "openaideployment", partition_key_path = "/id" },
  { name = "aisearchdeployment", partition_key_path = "/id" }
  { name = "completiondeploymentmodel", partition_key_path = "/specializationId" }
]

openai_deployments = [
  {
    name       = "gpt-4o-mini",
    model_name = "gpt-4o-mini",
    version    = "2024-07-18",
    sku_name   = "GlobalStandard",
    capacity   = 50000
  },
  {
    name       = "gpt-4o",
    model_name = "gpt-4o",
    version    = "2024-08-06",
    sku_name   = "GlobalStandard",
    capacity   = 10000
  },
  {
    name       = "text-embedding-ada-002",
    model_name = "text-embedding-ada-002",
    version    = 2,
    sku_name   = "Standard",
    capacity   = 100
  }
]

container_names = ["specialization"]
