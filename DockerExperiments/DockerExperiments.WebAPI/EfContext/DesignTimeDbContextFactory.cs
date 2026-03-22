using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DockerExperiments.WebAPI.EfContext
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<DockerExperimentsDbContext>
    {
        public DockerExperimentsDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<DockerExperimentsDbContext>();
            
            // Hardcode a dummy connection string for design-time ONLY.
            // This doesn't need to be your real DB; it's just to satisfy the compiler.
            optionsBuilder.UseNpgsql("Host=localhost;Database=dummy;Username=postgres;Password=password");

            return new DockerExperimentsDbContext(optionsBuilder.Options);
        }
    }
}