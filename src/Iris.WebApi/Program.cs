var builder = WebApplication.CreateSlimBuilder(args);

builder.Logging.AddJsonConsole();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.SerializerOptions.WriteIndented = false;
    options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});

builder.Services.AddSharedInfrastructure(
    builder.Configuration,
    includeTelemetry: !builder.Environment.IsEnvironment("Test"));

builder.Services.AddIndicatorsModule();

builder.Services.AddHealthChecks();
builder.Services.AddOpenApi();

var app = builder.Build();

app.MapOpenApi();
app.MapScalarApiReference();
app.MapHealthChecks("/health");

app.UseOutputCache();
app.UseHangfireDashboard();
app.UseIndicatorsModule();

app.MapIndicatorsEndpoints();

app.UseHttpsRedirection();

app.Run();