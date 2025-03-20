using AutoMapper;
using CopilotChat.WebApi.Models.Request;
using CopilotChat.WebApi.Models.Response;
using CopilotChat.WebApi.Models.Storage;

namespace CopilotChat.WebApi.Profiles;

public class SpecializationProfile : Profile
{
    public SpecializationProfile()
    {
        this.CreateMap<SpecializationBase, Specialization>();
        this.CreateMap<Specialization, SpecializationResponse>();
    }
}
