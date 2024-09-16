// See https://aka.ms/new-console-template for more information
using Altinn.ApiClients.Maskinporten.Models;
using Altinn.ApiClients.Maskinporten.Services;
using Microsoft.IdentityModel.Tokens;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using SystemUserApi.Models;

/// <summary>   
/// Test to show how a client  can use SystemUser Maskinporten to call an API that requires System User
/// Requirements to run this test:
/// 1. You need an test organization that will have the role as system vendor in this scenario. A System vendor offers software solutions that end user can use. Tenor can be used to identify a Test User Like this. 
/// 2. You need to setup a Integration in Maskinporten (https://onboarding.test.maskinporten.no/) with the following settings:
/// - Scope: altinn:systemusersdemo.read
/// - A JWK with matching KID that is presented in 
/// 3. You need to have a system in Altinn Systemregister that is registrated on this test organization with the clientId for the integration just created. Contact Altinn for this. You need to define which resource to use
/// 4. You need a client organization that will create a systemUser and connect to the system in step 3.
/// 5. Log in as admin 
/// 

string clientID = "7ee41fce-9f6e-4c32-8195-0fe2c1517f43";
string scope = "altinn:systembruker.demo";
string systemUserOrg = "210493352";
string pemCertificatePath = @".\mp-key.pem";

//var cert = new X509Certificate2(@".\CertPublicKey.cer");

// Load PEM certificate
var pem = File.ReadAllText(pemCertificatePath);

var rsa = RSA.Create();
rsa.ImportFromPem(pem.ToCharArray());

var privRsa = new RsaSecurityKey(rsa.ExportParameters(true)) { KeyId = "650a4snb5f" };
var privJwk = JsonWebKeyConverter.ConvertFromRSASecurityKey(privRsa);

HttpClient httpClient = new HttpClient();
MaskinportenService maskinportenService = new MaskinportenService(httpClient);

// Read JWK created in step. By default uses the base64 encoded JWK in testjwk.b64 as part of this repo
string filePath = @"testjwk.b64";
string fileContent = File.ReadAllText(filePath);

// Request system user token for Maskinporten test environment with ClientID from 2. 
TokenResponse? systemUserToken = await maskinportenService.GetToken(privJwk, "test", clientID, scope, systemUserOrg);

if(systemUserToken == null)
{
    Console.WriteLine("Not able to generate token");
    return;
}

/// Set up call to SystemUserInfo API
/// In a real world scenario this would be a call to a system that requires system user that performs business logic
/// In this case it just a demo api that returns information about the authenticated system user
HttpClient systemUserInfoApiClient = new HttpClient();
systemUserInfoApiClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", systemUserToken.AccessToken);
systemUserInfoApiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

HttpResponseMessage httpResponseMessage = await systemUserInfoApiClient.GetAsync("https://systemuserapi.azurewebsites.net/api/SystemUserInfo");
httpResponseMessage.EnsureSuccessStatusCode();
SystemUserClaim? systemUser = await httpResponseMessage.Content.ReadFromJsonAsync<SystemUserClaim>();

// Present system user 
if (systemUser != null)
{
    Console.WriteLine($"SystemId: {systemUser.System_id}");
    Console.WriteLine($"SystemUserId: {systemUser.Systemuser_id[0]}");
    Console.WriteLine($"Org: {systemUser.Systemuser_org.ID}");
}

