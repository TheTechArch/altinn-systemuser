using smartcloud.server.Models;
using SmartCloud.Server.Models;
using SmartCloud.Server.Services.Interfaces;
using Microsoft.Extensions.Options;
using SmartCloud.Server.Config;
using System.Text.Json;

namespace SmartCloud.Server.Services;

// Retains the existing demo endpoints while using the same checked transport as the vendor pages.
public class SystemuserService(AltinnVendorClient api, IOptions<SystemRegisterConfig> options) : ISystemUser
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private SystemRegisterConfig Config => options.Value;
    private static T Read<T>(JsonElement element) => element.Deserialize<T>(JsonOptions)
        ?? throw new AltinnApiException(System.Net.HttpStatusCode.BadGateway, "Tomt svar fra Altinn.");

    public async Task<CreateRequestSystemUserResponse> CreateSystemUserRequest(CreateRequestSystemUser request, string token) =>
        Read<CreateRequestSystemUserResponse>(await api.Send(HttpMethod.Post, Config.RequestSystemUserPath, token, request));

    public async Task<List<SystemUser>> GetSystemUsersForSystem(string systemid, string token) =>
        (await api.List(Config.SystemUserListForSystemPath + Uri.EscapeDataString(systemid), token)).Select(Read<SystemUser>).ToList();

    public async Task<CreateRequestSystemUserResponse> GetRequestStatus(string requestId, string token) =>
        Read<CreateRequestSystemUserResponse>(await api.Send(HttpMethod.Get, Config.RequestSystemUserPath + "/" + Uri.EscapeDataString(requestId), token));

    public async Task<List<RequestSystemResponse>> GetRequestsForSystem(string systemId, string token) =>
        (await api.List(Config.RequestSystemPath + Uri.EscapeDataString(systemId), token)).Select(Read<RequestSystemResponse>).ToList();
}
