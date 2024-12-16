using Altinn.ApiClients.Maskinporten.Interfaces;
using Altinn.ApiClients.Maskinporten.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using SmartCloud.Server.Config;
using SmartCloud.Server.Services;
using SmartCloud.Server.Services.Interfaces;
using SystemUserApi.Clients;
using SystemUserApi.Clients.Interfaces;
using SystemUserApi.Configuration;

var builder = WebApplication.CreateBuilder(args);

ConfigureServices(builder.Services, builder.Configuration);

// Add services to the container.

builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

// Add authentication and uses Maskinporten as the authentication provider
// SystemUser tokens is maskinporten token with some extra claims
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.MetadataAddress = "https://test.maskinporten.no/.well-known/oauth-authorization-server";
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = "https://test.maskinporten.no/",
    };
});
var app = builder.Build();

app.UseAuthentication();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();


void ConfigureServices(IServiceCollection services, IConfiguration config)
{
    services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen();
    services.AddHttpClient<IAuthorization, AuthorizationClient>();
    services.AddTransient<IMaskinportenService, MaskinportenService>();
    services.AddTransient<ITokenExchange, TokenExchange>();
    services.Configure<AuthorizationConfig>(config.GetSection("Authorization"));
    services.Configure<MaskinportenConfig>(config.GetSection("Maskinporten"));
}