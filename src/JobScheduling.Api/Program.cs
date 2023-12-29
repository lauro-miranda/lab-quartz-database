using JobScheduling.Api.Extensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

builder.Host.UseSerilog();

configuration
    .ConfigureProviders(builder.Environment.EnvironmentName)
    .ConfigureLogging();

services.AddControllers();

services.AddHttpContextAccessor();
services.AddHttpClient("Default");
services.ConfigureDI();
services.ConfigureBD(configuration);

services.AddEndpointsApiExplorer();
services.AddSwaggerGen(options =>
{
    options.CustomSchemaIds(type => type.ToString());
});

services.ConfigureScheduling();

var app = builder.Build();

if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseSerilogRequestLogging();

app.UseAuthorization();

app.MapControllers();

app.Run();