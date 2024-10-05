using System;

namespace CopilotChat.Shared;
public class DefaultConfiguration
{
  public string DefaultModel { get; set; } = "";
  public string DefaultEmbeddingModel { get; set; } = "";
  public string APIKey { get; set; } = string.Empty;
  public Uri? DefaultEndpoint { get; set; } = null;
}
