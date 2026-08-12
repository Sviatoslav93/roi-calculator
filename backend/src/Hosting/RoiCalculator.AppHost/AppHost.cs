var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .AddDatabase("DefaultConnection", "roi_calculator");

builder.AddProject<Projects.RoiForm_Api>("roi-form-api")
    .WithReference(postgres)
    .WaitFor(postgres)
    .WithEnvironment("Features__UseInMemoryDatabase", "false");

await builder.Build().RunAsync();
