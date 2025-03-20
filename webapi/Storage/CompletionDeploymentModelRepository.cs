using CopilotChat.WebApi.Models.Storage;

namespace CopilotChat.WebApi.Storage;

#pragma warning disable IDE0055

public class CompletionDeploymentModelRepository(IStorageContext<CompletionDeploymentModel> storageContext)
    : Repository<CompletionDeploymentModel>(storageContext) { }
