/* 
 * Krav og betalinger API
 *
 * Tjeneste som tilbyr oversikt av krav, innbetalinger og utbetalinger tilhørende en part.
 *
 * OpenAPI spec version: 1.1.1
 * 
 * Generated by: https://github.com/swagger-api/swagger-codegen.git
 */
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text;
namespace IO.Swagger.Model
{
    /// <summary>
    /// overføring av penger fra kreditor eller kreditors representant.
    /// </summary>
    [DataContract]
        public partial class Utbetaling
    {
        /// <summary>
        /// Gets or Sets PartIdentifikator
        /// </summary>
        [DataMember(Name="partIdentifikator", EmitDefaultValue=false)]
        public string PartIdentifikator { get; set; }

        /// <summary>
        /// Gets or Sets Utbetalingsidentifikator
        /// </summary>
        [DataMember(Name="utbetalingsidentifikator", EmitDefaultValue=false)]
        public string Utbetalingsidentifikator { get; set; }

        /// <summary>
        /// Gets or Sets Utbetaltdato
        /// </summary>
        [DataMember(Name="utbetaltdato", EmitDefaultValue=false)]
        public DateTime? Utbetaltdato { get; set; }

        /// <summary>
        /// Gets or Sets UtbetaltBeloep
        /// </summary>
        [DataMember(Name="utbetaltBeloep", EmitDefaultValue=false)]
        public double? UtbetaltBeloep { get; set; }

        /// <summary>
        /// Gets or Sets BetaltTil
        /// </summary>
        [DataMember(Name="betaltTil", EmitDefaultValue=false)]
        public Betalingsinformasjon BetaltTil { get; set; }

        /// <summary>
        /// Gets or Sets GrunnlagForUtbetaling
        /// </summary>
        [DataMember(Name="grunnlagForUtbetaling", EmitDefaultValue=false)]
        public GrunnlagForUtbetaling GrunnlagForUtbetaling { get; set; }

        /// <summary>
        /// Gets or Sets Utbetalingstatus
        /// </summary>
        [DataMember(Name="utbetalingstatus", EmitDefaultValue=false)]
        public Utbetalingstatus Utbetalingstatus { get; set; }
       
    }
}
