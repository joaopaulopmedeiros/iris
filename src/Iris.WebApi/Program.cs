var builder = WebApplication.CreateBuilder(args);

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

app.UseIndicatorsBackgroundJobs();

app.UseHangfireDashboard();

app.MapIndicatorsEndpoints();

app.UseHttpsRedirection();

app.Run();