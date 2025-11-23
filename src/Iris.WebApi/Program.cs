using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IConnectionMultiplexer>(s =>
{
    string connection = builder.Configuration.GetConnectionString("redis")!;
    return ConnectionMultiplexer.Connect(connection);
});
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapIndicatorsEndpoints();

app.UseHttpsRedirection();

app.Run();
