using System;

namespace CopilotChat.Shared;
public class DefaultConfiguration
{
  public string DefaultModel { get; set; } = "";
  public string DefaultEmbeddingModel { get; set; } = "";
  public Uri? Endpoint { get; set; } = null;
}
