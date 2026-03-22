dotnet ef dbcontext scaffold "Host=localhost;Database=DockerExperimentsDB;Username=postgres;Password=password" ^
                  Npgsql.EntityFrameworkCore.PostgreSQL ^
                  --output-dir EfModels ^
                  --context-dir EfContext ^
                  --context DockerExperimentsDbContext ^
                  --no-onconfiguring ^
                  --force