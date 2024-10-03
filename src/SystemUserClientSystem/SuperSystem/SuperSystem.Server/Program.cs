using Microsoft.Extensions.Logging;
using SuperSystem.Server.Config;
using SuperSystem.Server.Controllers;
using System.Text.Json.Serialization;
using System.Text.Json;
using Altinn.ApiClients.Maskinporten.Interfaces;
using Altinn.ApiClients.Maskinporten.Services;
using SuperSystem.Server.Services;
using SuperSystem.Server.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

ConfigureServices(builder.Services, builder.Configuration);

// Add services to the container.

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");
app.Run();



void ConfigureServices(IServiceCollection services, IConfiguration config)
{
    services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen();
    services.Configure<MaskinportenConfig>(config.GetSection("Maskinporten"));
    services.Configure<KravOgBetalingerConfig>(config.GetSection("KravOgBetalinger"));
    services.AddHttpClient<IMaskinportenService, MaskinportenService>();
    services.AddHttpClient<ITokenExchange, TokenExchange>();
    services.AddHttpClient<ISystemUser, SystemuserService>();
    services.AddHttpClient<IKravOgBetalinger, KravOgBetalingerService>();
}
