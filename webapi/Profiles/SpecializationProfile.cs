using AutoMapper;
using CopilotChat.WebApi.Models.Request;
using CopilotChat.WebApi.Models.Response;
using CopilotChat.WebApi.Models.Storage;

namespace CopilotChat.WebApi.Profiles;

public class SpecializationProfile : Profile
{
    public SpecializationProfile()
    {
        this.CreateMap<SpecializationWriteModel, Specialization>();
        this.CreateMap<SpecializationWriteModel, CompletionDeploymentModel>()
            .ForMember(
                destination => destination.Name,
                option => option.MapFrom(source => source.CompletionDeploymentName)
            );
        this.CreateMap<Specialization, SpecializationResponse>();
        this.CreateMap<CompletionDeploymentModel, SpecializationResponse>();
    }
}
