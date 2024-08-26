// See https://aka.ms/new-console-template for more information
using Altinn.ApiClients.Maskinporten.Interfaces;
using Altinn.ApiClients.Maskinporten.Models;
using Altinn.ApiClients.Maskinporten.Services;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using SystemUserApi.Models;


/// <summary>   
/// Test to show how a client  



HttpClient httpClient = new HttpClient();
MaskinportenService maskinportenService = new MaskinportenService(httpClient);



string filePath = @"c:\jwks\systest6.b64";
string fileContent = File.ReadAllText(filePath);


 TokenResponse systemUserToken = await maskinportenService.GetToken(fileContent, "test", "9fe38c9a-8177-4697-997c-0618eb6213c3", "altinn:instances.read", null, "210493352");


HttpClient httpClient1 = new HttpClient();
httpClient1.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", systemUserToken.AccessToken);
httpClient1.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

HttpResponseMessage httpResponseMessage = await httpClient1.GetAsync("https://systemuserapi.azurewebsites.net/api/SystemUserInfo");

SystemUserClaim? userUser = await httpResponseMessage.Content.ReadFromJsonAsync<SystemUserClaim>();

httpResponseMessage.EnsureSuccessStatusCode();