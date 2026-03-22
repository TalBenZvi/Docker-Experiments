using System;
using System.Collections.Generic;

namespace DockerExperiments.WebAPI.EfModels;

public partial class Item
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;
}
