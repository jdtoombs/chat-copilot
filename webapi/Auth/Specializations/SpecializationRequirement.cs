using Microsoft.AspNetCore.Authorization;

namespace CopilotChat.WebApi.Auth.Specializations;

/// <summary>
/// Used to require the specialization to be owned by the authenticated user.
/// </summary>
internal class SpecializationRequirement : IAuthorizationRequirement { }
