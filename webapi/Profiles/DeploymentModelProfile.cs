using AutoMapper;
using CopilotChat.WebApi.Models.Request;
using CopilotChat.WebApi.Models.Storage;

namespace CopilotChat.WebApi.Profiles;

public class DeploytmentModelProfile : Profile
{
    public DeploytmentModelProfile()
    {
        this.CreateMap<SpecializationBase, CompletionDeploymentModel>();
    }
}
