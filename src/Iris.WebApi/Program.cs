var builder = WebApplication.CreateSlimBuilder(args);

builder.Logging.AddConsole();
builder.Logging.AddJsonConsole();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.SerializerOptions.WriteIndented = false;
    options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});

builder.Services.AddHealthChecks();

builder.Services.AddOpenApi();

builder.Services.AddRedis(builder.Configuration);

builder.Services.AddHangfire(builder.Configuration);

builder.Services.AddBCBHttpClient(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapHealthChecks("/health");

app.UseIndicatorsBackgroundJobs();

app.UseHangfireDashboard();

app.MapIndicatorsEndpoints();

app.UseHttpsRedirection();

app.Run();

public partial class Program { }
