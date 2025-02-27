// Copyright (c) Quartech. All rights reserved.

using System;
using CopilotChat.WebApi.Storage;

namespace CopilotChat.WebApi.Models.Storage;

public class AISearchDeployment : IStorageEntity
{
    public string Id { get; set; }

    public string Partition => this.Id;

    public string Name { get; set; }

    public string Label { get; set; }

    public string Endpoint { get; set; }

    public string SecretName { get; set; }

    public int Order { get; set; }

    public AISearchDeployment(string Name, string Label, string Endpoint, string SecretName, int Order)
    {
        this.Id = Guid.NewGuid().ToString();
        this.Name = Name;
        this.Label = Label;
        this.Endpoint = Endpoint;
        this.SecretName = SecretName;
        this.Order = Order;
    }
}
