await Host.CreateDefaultBuilder()
    .ConfigureServices(services =>
    {
        services.AddHostedService<LicenseControl>();
        services.AddHostedService<GoldWatcher>();
        services.AddSingleton<CoreLicense>();
        services.AddSingleton<Settings>();
        services.AddSingleton<LogWriter>();
        services.AddSingleton<ServerConnection>();
        services.AddSingleton<ContextBuilder>();
        services.AddScoped<IAccountRepository, AccountRepository>();
        services.AddScoped<IServerRepository, ServerRepository>();
    })
    .Build()
    .RunAsync();