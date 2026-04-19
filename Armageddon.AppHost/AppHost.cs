var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.Armageddon_Server>("armageddon-server");

builder.Build().Run();
