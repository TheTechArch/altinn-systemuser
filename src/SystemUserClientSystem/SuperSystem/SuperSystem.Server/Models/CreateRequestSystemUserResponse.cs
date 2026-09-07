namespace SmartCloud.Server.Models;

public class CreateRequestSystemUserResponse
{
    public Guid Id { get; set; }
    public string? IntegrationTitle { get; set; }
    public string? ExternalRef { get; set; }
    public string SystemId { get; set; } = "";
    public string PartyOrgNo { get; set; } = "";
    public List<Right> Rights { get; set; } = [];
    public List<AccessPackage> AccessPackages { get; set; } = [];
    public string Status { get; set; } = "";
    public string? RedirectUrl { get; set; }
    public string? ConfirmUrl { get; set; }
    public DateTime Created { get; set; }
    public bool Escalated { get; set; }
    public bool TimedOut { get; set; }
}
