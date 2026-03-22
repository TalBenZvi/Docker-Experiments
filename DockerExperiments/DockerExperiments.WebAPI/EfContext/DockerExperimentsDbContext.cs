using System;
using System.Collections.Generic;
using DockerExperiments.WebAPI.EfModels;
using Microsoft.EntityFrameworkCore;

namespace DockerExperiments.WebAPI.EfContext;

public partial class DockerExperimentsDbContext : DbContext
{
    public DockerExperimentsDbContext(DbContextOptions<DockerExperimentsDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Item> Items { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Item>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("item_pkey");

            entity.ToTable("item");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Name).HasColumnName("name");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
