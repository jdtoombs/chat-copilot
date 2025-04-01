using AutoMapper;
using CopilotChat.WebApi.Models.Request;
using CopilotChat.WebApi.Models.Response;
using CopilotChat.WebApi.Models.Storage;

namespace CopilotChat.WebApi.Profiles;

public class SpecializationProfile : Profile
{
    public SpecializationProfile()
    {
        this.CreateMap<SpecializationWriteModel, Specialization>().ReverseMap();

        this.CreateMap<SpecializationWriteModel, CompletionDeploymentModel>()
            .ForMember(
                destination => destination.Name,
                option => option.MapFrom(source => source.CompletionDeploymentName)
            )
            .ReverseMap();

        this.CreateMap<Specialization, SpecializationResponse>();

        this.CreateMap<Specialization, SpecializationReadModel>();

        this.CreateMap<CompletionDeploymentModel, SpecializationResponse>()
            .ForMember(destination => destination.Id, option => option.Ignore())
            .ForMember(destination => destination.Name, option => option.Ignore())
            .ForMember(
                destination => destination.CompletionDeploymentName,
                option => option.MapFrom(source => source.Name)
            );
    }
}
