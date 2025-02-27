// Copyright (c) Quartech. All rights reserved.

using System;
using CopilotChat.WebApi.Storage;

namespace CopilotChat.WebApi.Models.Storage;

internal class SpecializationIndex : IStorageEntity
{
    public string Id { get; set; }

    public string Partition => this.Id;

    public string Name { get; set; }

    public string Label { get; set; }

    public string QueryType { get; set; }

    public string AISearchDeploymentId { get; set; }

    public string OpenAIDeploymentConnection { get; set; }

    public string EmbeddingDeployment { get; set; }

    public int Order { get; set; }

    public SpecializationIndex(
        string Name,
        string Label,
        string QueryType,
        string AISearchDeploymentId,
        string OpenAIDeploymentConnection,
        string EmbeddingDeployment,
        int Order
    )
    {
        this.Id = Guid.NewGuid().ToString();
        this.Name = Name;
        this.Label = Label;
        this.QueryType = QueryType;
        this.AISearchDeploymentId = AISearchDeploymentId;
        this.OpenAIDeploymentConnection = OpenAIDeploymentConnection;
        this.EmbeddingDeployment = EmbeddingDeployment;
        this.Order = Order;
    }
}
