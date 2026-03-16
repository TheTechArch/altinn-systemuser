namespace OnPremOnboarder.Models;

public class SystemRegisterRequest
{
    public required string Id { get; init; }

    public required Vendor Vendor { get; set; }

    public required IDictionary<string, string> Name { get; set; }

    public required IDictionary<string, string> Description { get; set; }

    public List<Right> Rights { get; set; } = [];

    public List<AccessPackage> AccessPackages { get; set; } = [];

    public bool IsDeleted { get; set; } = false;

    public required List<string> ClientId { get; set; }

    public bool IsVisible { get; set; } = true;

    public List<Uri> AllowedRedirectUrls { get; set; } = [];
}
