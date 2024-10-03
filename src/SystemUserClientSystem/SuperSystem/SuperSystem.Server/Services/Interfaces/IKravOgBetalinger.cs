using IO.Swagger.Model;

namespace SuperSystem.Server.Services.Interfaces
{
    /// <summary>
    /// Interface for https://skatteetaten.github.io/api-dokumentasjon/api/kravogbetalinger?tab=Test
    /// </summary>
    public interface IKravOgBetalinger
    {
        /// <summary>
        /// Hent utbetalinger for en part identifisert av organisasjonsnummer, fødselsnummer eller D-nummer innenfor en valgt periode.
        /// </summary>
        Task<Utbetalinger?> GetUtbetalinger(string orgno, string? from, string? to, string? correlation);

        /// <summary>
        /// Hent innbetalinger for en part identifisert av organisasjonsnummer, fødselsnummer eller dnummer innenfor en valgt periode. Du får oversikt over innbetalinger som er gjort, og hvilke krav som har blitt dekket.
        /// </summary>
        Task<Innbetalinger?> GetInnbetalinger(string orgno, string? from, string? to, string? correlation);

        /// <summary>
        /// Hent kravoversikt over gjeld, tilgodebeløp og betalinger mellom part identifisert av organisasjonsnummer, fødselsnummer eller dnummer og Skatteetaten.
        /// </summary>
        Task<AapneKrav?> GetAapneKrav(string orgno, string? from, string? to, string? correlation);

        /// <summary>
        /// Hent kravoversikt over gjeld, tilgodebeløp og betalinger mellom part identifisert av organisasjonsnummer, fødselsnummer eller dnummer og Skatteetaten innenfor en valgt periode.
        /// </summary>
        Task<Krav?> GetKrav(string orgno, string from, string? to, string? correlation);
    }
}
