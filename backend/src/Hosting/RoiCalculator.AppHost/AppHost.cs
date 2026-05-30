var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .AddDatabase("DefaultConnection", "roi_calculator");

var roiFormApi = builder.AddProject<Projects.RoiForm_Api>("roi-form-api")
    .WithReference(postgres)
    .WaitFor(postgres)
    .WithEnvironment("Features__UseInMemoryDatabase", "false");

builder.AddViteApp("web", "../../../../web")
    .WithReference(roiFormApi)
    .WaitFor(roiFormApi)
    .WithEnvironment("VITE_API_BASE_URL", roiFormApi.GetEndpoint("http"))
    .WithExternalHttpEndpoints();

builder.Build().Run();
