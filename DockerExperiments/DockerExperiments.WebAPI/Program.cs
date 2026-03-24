using DockerExperiments.WebAPI.Config;
using DockerExperiments.WebAPI.EfContext;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;

Console.WriteLine("Starting");

var builder = WebApplication.CreateBuilder(args);

string configFilePath = Environment.GetEnvironmentVariable("CONFIG_FILE_PATH") ?? "appsettings.json";

builder.Configuration.AddJsonFile(configFilePath);

Console.WriteLine("Set config");

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

DbConfig dbConfig = builder.Configuration.GetSection("Db").Get<DbConfig>()!;
string dbConnectionString =
    $"Host={dbConfig.Host};Database={dbConfig.Database};Username={dbConfig.Username};Password={dbConfig.Password}";
DbContextOptionsBuilder<DockerExperimentsDbContext> optionsBuilder = new DbContextOptionsBuilder<DockerExperimentsDbContext>();
optionsBuilder.UseNpgsql(dbConnectionString);
DockerExperimentsDbContext dbContext = new DockerExperimentsDbContext(optionsBuilder.Options);

Console.WriteLine("Set up db");

builder.Services.AddHealthChecks().AddNpgSql(dbConnectionString, name: "postgres");

WebApplication app = builder.Build();

Console.WriteLine("Built app");

app.UseSwagger();
app.UseSwaggerUI();

Console.WriteLine("Set up swagger");

app.UseHttpsRedirection();

bool isAlive = true;

app.MapHealthChecks("/health/ready", new HealthCheckOptions { Predicate = check => check.Name == "postgres" }).WithOpenApi();
app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = _ => false,
    ResponseWriter = async (context, _) =>
    {
        if (isAlive)
            await context.Response.WriteAsync("Healthy");
        else
        {
            context.Response.StatusCode = 500;
            await context.Response.WriteAsync("I am frozen!");
        }
    }
});

Console.WriteLine("Set up health checks");

app.MapGet("/items", () => dbContext.Items.ToArray()).WithName("GetItems").WithOpenApi();
app.MapPost("/kill", () => isAlive = false).WithName("kill").WithOpenApi();

List<byte[]> leak = new List<byte[]>();
app.MapGet("/stress-memory", () => {
    while(true) {
        var chunk = new byte[10 * 1024 * 1024]; 
        // Filling the array with data ensures the OS actually allocates physical RAM
        Array.Fill(chunk, (byte)1); 
        leak.Add(chunk);
        
        Console.WriteLine($"Current usage: {GC.GetTotalMemory(false) / 1024 / 1024} MB");
        Thread.Sleep(500); 
    }
});

app.MapGet("/stress-cpu", () =>
{
    int i = 0;
    while (true)
    {
        Console.WriteLine(i);
    }
});

Console.WriteLine("Set up resource endpoints");

FileStorageConfig fileStorageConfig = builder.Configuration.GetSection("FileStorage").Get<FileStorageConfig>()!;
string fileStorageDirectoryPath = fileStorageConfig.DirectoryPath;
if (!Directory.Exists(fileStorageDirectoryPath))
    Directory.CreateDirectory(fileStorageDirectoryPath);
const string storageTestFileName = "test.txt";
string storageTestFilePath = Path.Combine(fileStorageDirectoryPath, storageTestFileName);

app.MapPost("/test-storage", () =>
{
    if (File.Exists(storageTestFilePath))
        File.Delete(storageTestFilePath);

    File.WriteAllText(storageTestFilePath, DateTime.Now.ToLongTimeString());
    
    return File.ReadAllText(storageTestFilePath);
});

Console.WriteLine("Set up storage endpoint");

string fileStorageNetworkSharePath = fileStorageConfig.NetworkSharePath;
const string networkShareTestFileName = "test.txt";
string networkShareTestFilePath = Path.Combine(fileStorageNetworkSharePath, networkShareTestFileName);

app.MapPost("/test-network-share", () =>
{
    if (File.Exists(networkShareTestFilePath))
        File.Delete(networkShareTestFilePath);

    File.WriteAllText(networkShareTestFilePath, DateTime.Now.ToLongTimeString());
    
    return File.ReadAllText(networkShareTestFilePath);
});

Console.WriteLine("Set up network share endpoint");

await dbContext.Database.MigrateAsync();

Console.WriteLine("Migrated db");

app.Run();
