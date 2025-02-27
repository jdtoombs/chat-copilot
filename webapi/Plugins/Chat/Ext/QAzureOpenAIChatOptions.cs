// Copyright (c) Quartech. All rights reserved.

using System.ComponentModel.DataAnnotations;

namespace CopilotChat.WebApi.Plugins.Chat.Ext;

/// <summary>
/// This class is a representation of Azure AI Chat options.
/// </summary>
public class QAzureOpenAIChatOptions
{
    public const string PropertyName = "QAzureOpenAIChatConfig";
    public bool Enabled { get; set; } = false;
    public string DefaultModel { get; set; } = "";
    public string DefaultEmbeddingModel { get; set; } = "";
    public string DefaultConnection { get; set; } = "";
    public string DefaultSpecializationImage { get; set; } = "";
    public string DefaultSpecializationIcon { get; set; } = "";
    public string AdminGroupMembershipId { get; set; } = "";

    [Required]
    public BlobStorageOption BlobStorage { get; set; } = new BlobStorageOption();

    public class ChatCompletionDeployment
    {
        public string Name { get; set; } = string.Empty;
        public int CompletionTokenLimit { get; set; }
        public int OutputTokens { get; set; }
    }

    public class BlobStorageOption
    {
        public string ConnectionString { get; set; } = string.Empty;
        public string SpecializationContainerName { get; set; } = "specialization";
    }
}
