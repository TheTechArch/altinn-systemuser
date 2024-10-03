// See https://aka.ms/new-console-template for more information
using Altinn.Platform.Authentication.Core.SystemRegister.Models;
using Altinn.SystemAdmin.Maskinporten.Config;
using Altinn.SystemAdmin.Maskinporten.Interfaces;
using Altinn.SystemAdmin.Maskinporten.Models;
using Altinn.SystemAdmin.Maskinporten.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text.Json;
using SystemAdmin.Config;
using SystemAdmin.Services;
using SystemAdmin.Services.Interfaces;

internal class Program
{

    private static IConfigurationRoot _configuration;

    private static async Task Main(string[] args)
    {
        Console.WriteLine("Hello, World!");

        _configuration = BuildConfiguration();
      
        MaskinportenConfig maskinportenConfig = _configuration.GetRequiredSection("Maskinporten").Get<MaskinportenConfig>();
        SystemRegisterConfig systemRegisterConfig = _configuration.GetRequiredSection("SystemRegister").Get<SystemRegisterConfig>();
        IServiceCollection services = GetAndRegisterServices();
        
        MaskinportenService maskinportenService = new MaskinportenService(new HttpClient(), maskinportenConfig);

        string json = await File.ReadAllTextAsync(systemRegisterConfig.SystemInfoPath);

        SystemRegisterRequest requset = JsonSerializer.Deserialize<SystemRegisterRequest>(json);
        SystemRegister systemRegisterService = new SystemRegister(new HttpClient(), maskinportenService, systemRegisterConfig);
        await systemRegisterService.CreateSystem(requset);

    }


    static IConfigurationRoot BuildConfiguration()
    {
        var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

        Assembly assembly = Assembly.Load(new AssemblyName("SystemAdmin"));
        if (assembly != null)
        {
            builder.AddUserSecrets(assembly, true);
        }

        return builder.Build();
    }

    static IServiceCollection GetAndRegisterServices()
    {
        IServiceCollection services = new ServiceCollection();
        services.Configure<MaskinportenConfig>(_configuration.GetSection("Maskinporten"));
        services.AddTransient<ISystemRegister, SystemRegister>();
        services.AddTransient<IMaskinportenService, MaskinportenService>();
        return services;
    }
}