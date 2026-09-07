namespace SmartCloud.Server.Models;

public class MaskinportenConfigurationException : Exception
{
    public MaskinportenConfigurationException() : base(
        "Maskinporten er ikke konfigurert. Angi gyldig Maskinporten:EncodedJwk, Maskinporten:ClientId og Maskinporten:Environment (test/prod) i serverens user-secrets eller miljøvariabler.")
    {
    }
}
