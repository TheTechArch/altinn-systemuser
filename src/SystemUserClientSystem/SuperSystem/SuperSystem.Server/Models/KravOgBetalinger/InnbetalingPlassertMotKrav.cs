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
    /// viser detaljer av hvilken innbetaling er plassert mot krav
    /// </summary>
    [DataContract]
        public partial class InnbetalingPlassertMotKrav
    {
        /// <summary>
        /// Gets or Sets InnbetalingsIdentifikator
        /// </summary>
        [DataMember(Name="innbetalingsIdentifikator", EmitDefaultValue=false)]
        public string InnbetalingsIdentifikator { get; set; }

        /// <summary>
        /// Gets or Sets PlassertBeloep
        /// </summary>
        [DataMember(Name="plassertBeloep", EmitDefaultValue=false)]
        public double? PlassertBeloep { get; set; }

        /// <summary>
        /// Gets or Sets PlassertDato
        /// </summary>
        [DataMember(Name="plassertDato", EmitDefaultValue=false)]
        public DateTime? PlassertDato { get; set; }

        /// <summary>
        /// Gets or Sets Innbetalingsdato
        /// </summary>
        [DataMember(Name="innbetalingsdato", EmitDefaultValue=false)]
        public DateTime? Innbetalingsdato { get; set; }

        /// <summary>
        /// Gets or Sets InnbetaltBeloep
        /// </summary>
        [DataMember(Name="innbetaltBeloep", EmitDefaultValue=false)]
        public double? InnbetaltBeloep { get; set; }

        /// <summary>
        /// Gets or Sets InnbetaltFra
        /// </summary>
        [DataMember(Name="innbetaltFra", EmitDefaultValue=false)]
        public Betalingsinformasjon InnbetaltFra { get; set; }

        /// <summary>
        /// Gets or Sets Innbetalingstype
        /// </summary>
        [DataMember(Name="innbetalingstype", EmitDefaultValue=false)]
        public string Innbetalingstype { get; set; }

     }
}
