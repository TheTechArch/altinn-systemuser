using System.ComponentModel.DataAnnotations;

namespace SmartCloud.Server.Models;

public record AccessPackage
{
    [Required, RegularExpression(@"^urn:altinn:accesspackage:.+$")]
    public string Urn { get; set; } = "";
}

public class VendorRequest : IValidatableObject
{
    [Required] public string SystemId { get; set; } = "";
    [Required, RegularExpression(@"^\d{9}$")] public string PartyOrgNo { get; set; } = "";
    public string? IntegrationTitle { get; set; }
    public string? ExternalRef { get; set; }
    public string? RedirectUrl { get; set; }
    public List<Right> Rights { get; set; } = [];
    public List<AccessPackage> AccessPackages { get; set; } = [];

    public IEnumerable<ValidationResult> Validate(ValidationContext context)
    {
        if (Rights is null || AccessPackages is null)
            yield return new ValidationResult("Tilgangslistene kan ikke være null.");
        else if (Rights.Count + AccessPackages.Count == 0)
            yield return new ValidationResult("Velg minst én ressurs eller tilgangspakke.");
        foreach (var error in RequestValidation.ValidateRights(Rights)) yield return error;
        if (!RequestValidation.ValidRedirect(RedirectUrl))
            yield return new ValidationResult("Returadresse må være en absolutt HTTPS-adresse (HTTP tillates for localhost).");
    }
}

public class VendorChangeRequest : IValidatableObject
{
    public List<Right> RequiredRights { get; set; } = [];
    public List<Right> UnwantedRights { get; set; } = [];
    public List<AccessPackage> RequiredAccessPackages { get; set; } = [];
    public List<AccessPackage> UnwantedAccessPackages { get; set; } = [];
    public string? RedirectUrl { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext context)
    {
        if (RequiredRights is null || UnwantedRights is null || RequiredAccessPackages is null || UnwantedAccessPackages is null)
        {
            yield return new ValidationResult("Tilgangslistene kan ikke være null.");
            yield break;
        }
        if (RequiredRights.Count + UnwantedRights.Count + RequiredAccessPackages.Count + UnwantedAccessPackages.Count == 0)
            yield return new ValidationResult("Velg minst én tilgang som skal legges til eller fjernes.");
        foreach (var error in RequestValidation.ValidateRights(RequiredRights.Concat(UnwantedRights))) yield return error;
        if (RequiredAccessPackages.Select(p => p?.Urn).Intersect(UnwantedAccessPackages.Select(p => p?.Urn)).Any())
            yield return new ValidationResult("Samme tilgangspakke kan ikke både legges til og fjernes.");
        if (RequiredRights.Select(RequestValidation.RightKey).Intersect(UnwantedRights.Select(RequestValidation.RightKey)).Any())
            yield return new ValidationResult("Samme ressurs kan ikke både legges til og fjernes.");
        if (!RequestValidation.ValidRedirect(RedirectUrl))
            yield return new ValidationResult("Ugyldig returadresse.");
    }
}

public static class RequestValidation
{
    public static bool ValidRedirect(string? value) => string.IsNullOrEmpty(value) ||
        (Uri.TryCreate(value, UriKind.Absolute, out var uri) && string.IsNullOrEmpty(uri.UserInfo) &&
        (uri.Scheme == "https" || uri.Scheme == "http" && uri.IsLoopback));

    public static string RightKey(Right right) => string.Join("|", (right?.Resource ?? []).Where(a => a is not null).Select(a => $"{a.Id}={a.Value}").Order());

    public static IEnumerable<ValidationResult> ValidateRights(IEnumerable<Right>? rights)
    {
        if (rights is null) yield break;
        foreach (var right in rights)
            if (right?.Resource is not { Count: > 0 } || right.Resource.Any(a => a is null || string.IsNullOrWhiteSpace(a.Id) || string.IsNullOrWhiteSpace(a.Value)))
                yield return new ValidationResult("En rettighet må inneholde ressursattributter med id og verdi.");
    }
}

public class VendorIdentity
{
    [Required, RegularExpression(@"^0192:\d{9}$")]
    [System.Text.Json.Serialization.JsonPropertyName("ID")]
    public string Id { get; set; } = "";
}

public class RegisterVendorSystem : IValidatableObject
{
    [Required] public string Id { get; set; } = "";
    [Required] public VendorIdentity Vendor { get; set; } = new();
    [Required] public Dictionary<string, string> Name { get; set; } = [];
    [Required] public Dictionary<string, string> Description { get; set; } = [];
    [Required] public List<Right> Rights { get; set; } = [];
    [Required] public List<AccessPackage> AccessPackages { get; set; } = [];
    [Required] public List<string> ClientId { get; set; } = [];
    public bool IsVisible { get; set; } = true;
    [Required] public List<string> AllowedRedirectUrls { get; set; } = [];

    public IEnumerable<ValidationResult> Validate(ValidationContext context)
    {
        if (Vendor?.Id?.Length != 14 || !Id.StartsWith(Vendor.Id[5..] + "_", StringComparison.Ordinal) || Id.Any(char.IsWhiteSpace))
            yield return new ValidationResult("System-ID må starte med leverandørens organisasjonsnummer etterfulgt av _, og kan ikke inneholde mellomrom.");
        if (Name is null || !Name.Values.Any(v => !string.IsNullOrWhiteSpace(v)))
            yield return new ValidationResult("Oppgi minst ett systemnavn.");
        if (AllowedRedirectUrls?.Any(url => string.IsNullOrWhiteSpace(url) || !RequestValidation.ValidRedirect(url)) == true)
            yield return new ValidationResult("Tillatte returadresser må være gyldige absolutte adresser.");
        foreach (var error in RequestValidation.ValidateRights(Rights)) yield return error;
    }
}
