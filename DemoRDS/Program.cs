using DemoRDS;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting web application");

    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((ctx, svc, cfg) => cfg
        .ReadFrom.Configuration(ctx.Configuration)
        .ReadFrom.Services(svc)
        .Enrich.FromLogContext()
        .WriteTo.Console()
    );

    var connectionString = builder.Configuration.GetConnectionString("postgres");
    builder.Services.AddNpgsql<DemoDbContext>(connectionString);

    var app = builder.Build();

    app.UseSerilogRequestLogging();

    if (connectionString != null)
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<DemoDbContext>();
        db.Database.Migrate();
    }

    app.MapGet("/", () => "Hello, world!");
    app.MapGet("/list", (DemoDbContext dbContext) => dbContext.DemoModels.ToListAsync());
    app.MapGet("/env", (IConfiguration configuration) => ((IConfigurationRoot) configuration).GetDebugView());

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}