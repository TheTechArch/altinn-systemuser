/* 
 * Krav og betalinger API
 *
 * Tjeneste som tilbyr oversikt av krav, innbetalinger og utbetalinger tilhørende en part.
 *
 * OpenAPI spec version: 1.1.1
 * 
 * Generated by: https://github.com/swagger-api/swagger-codegen.git
 */
using System.Runtime.Serialization;
namespace IO.Swagger.Model
{
    /// <summary>
    /// oversikt over krav med gjenstående beløp (åpne krav) og eventuelle uplasserte innbetalinger.
    /// </summary>
    [DataContract]
        public partial class AapneKrav
    {
        /// <summary>
        /// Gets or Sets PartIdentifikator
        /// </summary>
        [DataMember(Name="partIdentifikator", EmitDefaultValue=false)]
        public string PartIdentifikator { get; set; }

        /// <summary>
        /// Vanligvis en debettransaksjon i reskontro, som kjennetegnes av at den skal betales av en debitor. (&lt;a href&#x3D;&#x27;http://data.skatteetaten.no/begrep/35c63129-86e6-11e6-a97e-ba992a0501a6&#x27;&gt;Begrepsreferanse&lt;/a&gt;)
        /// </summary>
        /// <value>Vanligvis en debettransaksjon i reskontro, som kjennetegnes av at den skal betales av en debitor. (&lt;a href&#x3D;&#x27;http://data.skatteetaten.no/begrep/35c63129-86e6-11e6-a97e-ba992a0501a6&#x27;&gt;Begrepsreferanse&lt;/a&gt;)</value>
        [DataMember(Name="aapentKravMedGjenstaaendeBeloep", EmitDefaultValue=false)]
        public List<Krav> AapentKravMedGjenstaaendeBeloep { get; set; }

        /// <summary>
        /// overføring av penger til kreditor eller kreditors representant. (&lt;a href&#x3D;&#x27;http://data.skatteetaten.no/begrep/35c6311e-86e6-11e6-a97e-ba992a0501a6&#x27;&gt;Begrepsreferanse&lt;/a&gt;)
        /// </summary>
        /// <value>overføring av penger til kreditor eller kreditors representant. (&lt;a href&#x3D;&#x27;http://data.skatteetaten.no/begrep/35c6311e-86e6-11e6-a97e-ba992a0501a6&#x27;&gt;Begrepsreferanse&lt;/a&gt;)</value>
        [DataMember(Name="innbetalingMedUplassertBeloep", EmitDefaultValue=false)]
        public List<Innbetaling> InnbetalingMedUplassertBeloep { get; set; }

        /// <summary>
        /// Gets or Sets TotalOversikt
        /// </summary>
        [DataMember(Name="totalOversikt", EmitDefaultValue=false)]
        public TotalOversikt TotalOversikt { get; set; }

        /// <summary>
        /// oversikt over saldo, stipulerte renter og krav (forfallte og ikke forfalte) gruppert per kravgrupe ved forespørsels tidspunkt
        /// </summary>
        /// <value>oversikt over saldo, stipulerte renter og krav (forfallte og ikke forfalte) gruppert per kravgrupe ved forespørsels tidspunkt</value>
        [DataMember(Name="oversiktPerKravgruppe", EmitDefaultValue=false)]
        public List<OversiktPerKravgruppe> OversiktPerKravgruppe { get; set; }

        /// <summary>
        /// Gets or Sets Skjermet
        /// </summary>
        [DataMember(Name="skjermet", EmitDefaultValue=false)]
        public bool? Skjermet { get; set; }
    }
}
