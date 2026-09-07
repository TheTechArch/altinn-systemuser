namespace Altinn.ApiClients.Maskinporten.Models;

/// <summary>Safe diagnostics for token failures; never contains the assertion or raw provider response.</summary>
public class TokenRequestException : ApplicationException
{
    public TokenRequestException(string message) : base(message) { }

    public TokenRequestException(string code, string environment, string scope, int? upstreamStatus = null, string? providerCode = null)
        : base(Describe(providerCode ?? code))
    {
        Code = code;
        Environment = environment;
        Scope = scope;
        UpstreamStatus = upstreamStatus;
        ProviderCode = providerCode;
    }

    public string? Code { get; }
    public string? Environment { get; }
    public string? Scope { get; }
    public int? UpstreamStatus { get; }
    public string? ProviderCode { get; }

    private static string Describe(string code) => code switch
    {
        "MP-121" => "Maskinporten avviste signeringsnøkkelen fordi den er utløpt. Forny nøkkelen på integrasjonen i Maskinporten.",
        "MP-110" => "Maskinporten avviste målmiljøet (audience) i tokenforespørselen. Kontroller Maskinporten:Environment.",
        "MP-130" => "Tokenforespørselen utløp før Maskinporten behandlet den. Kontroller serverklokken og forsinkelser i nettverket.",
        "MP-200" or "invalid_scope" => "Maskinporten avviste forespurt scope. Kontroller at scopet er registrert på integrasjonen, og at virksomheten har tilgang i valgt miljø.",
        "MP-201" => "Maskinporten avviste integrasjonstypen for scopet. Bruk en integrasjon opprettet for Maskinporten.",
        "MP-250" => "Virksomheten har ikke fått tilgang til scopet. Be API-tilbyderen gi tilgang i valgt miljø.",
        "MP-251" => "Virksomheten har ikke delegert tilgang til scopet til leverandøren. Kontroller delegeringen i valgt miljø.",
        "MP-100" or "invalid_client" => "Maskinporten kunne ikke autentisere integrasjonen. Kontroller ClientId, registrert signeringsnøkkel og at integrasjonen finnes i valgt miljø.",
        "MP-120" or "MP-124" => "Maskinporten avviste signaturen. Kontroller at signeringsnøkkelen eller sertifikatet er gyldig og registrert på riktig integrasjon.",
        "invalid_grant" => "Maskinporten avviste den signerte tokenforespørselen. Kontroller klient-ID, nøkkelens gyldighet, valgt miljø og serverklokken.",
        "invalid_request" => "Maskinporten avviste formatet på tokenforespørselen. Kontroller integrasjonens konfigurasjon.",
        "connection_failed" => "SmartCloud kunne ikke koble til Maskinporten. Kontroller DNS, TLS og utgående nettverkstilgang fra serveren.",
        "timeout" => "Maskinporten svarte ikke innen tidsfristen. Prøv igjen; ved gjentatte feil, kontroller nettverket fra serveren.",
        "invalid_response" => "Maskinporten returnerte et ugyldig svar eller manglet tilgangstoken. Prøv igjen og kontroller tjenestens driftsstatus.",
        _ => "Maskinporten avviste tokenforespørselen. Bruk feilkoden, miljøet og scopet nedenfor ved feilsøking av integrasjonen."
    };
}
