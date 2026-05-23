var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.Armageddon_Server>("armageddon-server");

builder.AddProject<Projects.Armageddon_Mobile>("armageddon-mobile");

builder.Build().Run();
