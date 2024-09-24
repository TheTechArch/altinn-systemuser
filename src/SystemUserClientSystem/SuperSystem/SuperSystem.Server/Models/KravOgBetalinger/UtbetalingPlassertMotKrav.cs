/* 
 * Krav og betalinger API
 *
 * Tjeneste som tilbyr oversikt av krav, innbetalinger og utbetalinger tilhørende en part.
 *
 * OpenAPI spec version: 1.1.1
 * 
 * Generated by: https://github.com/swagger-api/swagger-codegen.git
 */
using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.ComponentModel.DataAnnotations;
using SwaggerDateConverter = IO.Swagger.Client.SwaggerDateConverter;
namespace IO.Swagger.Model
{
    /// <summary>
    /// UtbetalingPlassertMotKrav
    /// </summary>
    [DataContract]
        public partial class UtbetalingPlassertMotKrav :  IEquatable<UtbetalingPlassertMotKrav>, IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UtbetalingPlassertMotKrav" /> class.
        /// </summary>
        /// <param name="utbetalingsidentifikator">utbetalingsidentifikator (required).</param>
        /// <param name="utbetaltBeloep">utbetaltBeloep (required).</param>
        /// <param name="utbetaltDato">utbetaltDato (required).</param>
        /// <param name="betaltTil">betaltTil (required).</param>
        /// <param name="plassertBeloep">plassertBeloep (required).</param>
        /// <param name="plassertDato">plassertDato (required).</param>
        public UtbetalingPlassertMotKrav(string utbetalingsidentifikator = default(string), double? utbetaltBeloep = default(double?), DateTime? utbetaltDato = default(DateTime?), Betalingsinformasjon betaltTil = default(Betalingsinformasjon), double? plassertBeloep = default(double?), DateTime? plassertDato = default(DateTime?))
        {
            // to ensure "utbetalingsidentifikator" is required (not null)
            if (utbetalingsidentifikator == null)
            {
                throw new InvalidDataException("utbetalingsidentifikator is a required property for UtbetalingPlassertMotKrav and cannot be null");
            }
            else
            {
                this.Utbetalingsidentifikator = utbetalingsidentifikator;
            }
            // to ensure "utbetaltBeloep" is required (not null)
            if (utbetaltBeloep == null)
            {
                throw new InvalidDataException("utbetaltBeloep is a required property for UtbetalingPlassertMotKrav and cannot be null");
            }
            else
            {
                this.UtbetaltBeloep = utbetaltBeloep;
            }
            // to ensure "utbetaltDato" is required (not null)
            if (utbetaltDato == null)
            {
                throw new InvalidDataException("utbetaltDato is a required property for UtbetalingPlassertMotKrav and cannot be null");
            }
            else
            {
                this.UtbetaltDato = utbetaltDato;
            }
            // to ensure "betaltTil" is required (not null)
            if (betaltTil == null)
            {
                throw new InvalidDataException("betaltTil is a required property for UtbetalingPlassertMotKrav and cannot be null");
            }
            else
            {
                this.BetaltTil = betaltTil;
            }
            // to ensure "plassertBeloep" is required (not null)
            if (plassertBeloep == null)
            {
                throw new InvalidDataException("plassertBeloep is a required property for UtbetalingPlassertMotKrav and cannot be null");
            }
            else
            {
                this.PlassertBeloep = plassertBeloep;
            }
            // to ensure "plassertDato" is required (not null)
            if (plassertDato == null)
            {
                throw new InvalidDataException("plassertDato is a required property for UtbetalingPlassertMotKrav and cannot be null");
            }
            else
            {
                this.PlassertDato = plassertDato;
            }
        }
        
        /// <summary>
        /// Gets or Sets Utbetalingsidentifikator
        /// </summary>
        [DataMember(Name="utbetalingsidentifikator", EmitDefaultValue=false)]
        public string Utbetalingsidentifikator { get; set; }

        /// <summary>
        /// Gets or Sets UtbetaltBeloep
        /// </summary>
        [DataMember(Name="utbetaltBeloep", EmitDefaultValue=false)]
        public double? UtbetaltBeloep { get; set; }

        /// <summary>
        /// Gets or Sets UtbetaltDato
        /// </summary>
        [DataMember(Name="utbetaltDato", EmitDefaultValue=false)]
        public DateTime? UtbetaltDato { get; set; }

        /// <summary>
        /// Gets or Sets BetaltTil
        /// </summary>
        [DataMember(Name="betaltTil", EmitDefaultValue=false)]
        public Betalingsinformasjon BetaltTil { get; set; }

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
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class UtbetalingPlassertMotKrav {\n");
            sb.Append("  Utbetalingsidentifikator: ").Append(Utbetalingsidentifikator).Append("\n");
            sb.Append("  UtbetaltBeloep: ").Append(UtbetaltBeloep).Append("\n");
            sb.Append("  UtbetaltDato: ").Append(UtbetaltDato).Append("\n");
            sb.Append("  BetaltTil: ").Append(BetaltTil).Append("\n");
            sb.Append("  PlassertBeloep: ").Append(PlassertBeloep).Append("\n");
            sb.Append("  PlassertDato: ").Append(PlassertDato).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }
  
        /// <summary>
        /// Returns the JSON string presentation of the object
        /// </summary>
        /// <returns>JSON string presentation of the object</returns>
        public virtual string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        /// <summary>
        /// Returns true if objects are equal
        /// </summary>
        /// <param name="input">Object to be compared</param>
        /// <returns>Boolean</returns>
        public override bool Equals(object input)
        {
            return this.Equals(input as UtbetalingPlassertMotKrav);
        }

        /// <summary>
        /// Returns true if UtbetalingPlassertMotKrav instances are equal
        /// </summary>
        /// <param name="input">Instance of UtbetalingPlassertMotKrav to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(UtbetalingPlassertMotKrav input)
        {
            if (input == null)
                return false;

            return 
                (
                    this.Utbetalingsidentifikator == input.Utbetalingsidentifikator ||
                    (this.Utbetalingsidentifikator != null &&
                    this.Utbetalingsidentifikator.Equals(input.Utbetalingsidentifikator))
                ) && 
                (
                    this.UtbetaltBeloep == input.UtbetaltBeloep ||
                    (this.UtbetaltBeloep != null &&
                    this.UtbetaltBeloep.Equals(input.UtbetaltBeloep))
                ) && 
                (
                    this.UtbetaltDato == input.UtbetaltDato ||
                    (this.UtbetaltDato != null &&
                    this.UtbetaltDato.Equals(input.UtbetaltDato))
                ) && 
                (
                    this.BetaltTil == input.BetaltTil ||
                    (this.BetaltTil != null &&
                    this.BetaltTil.Equals(input.BetaltTil))
                ) && 
                (
                    this.PlassertBeloep == input.PlassertBeloep ||
                    (this.PlassertBeloep != null &&
                    this.PlassertBeloep.Equals(input.PlassertBeloep))
                ) && 
                (
                    this.PlassertDato == input.PlassertDato ||
                    (this.PlassertDato != null &&
                    this.PlassertDato.Equals(input.PlassertDato))
                );
        }

        /// <summary>
        /// Gets the hash code
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                int hashCode = 41;
                if (this.Utbetalingsidentifikator != null)
                    hashCode = hashCode * 59 + this.Utbetalingsidentifikator.GetHashCode();
                if (this.UtbetaltBeloep != null)
                    hashCode = hashCode * 59 + this.UtbetaltBeloep.GetHashCode();
                if (this.UtbetaltDato != null)
                    hashCode = hashCode * 59 + this.UtbetaltDato.GetHashCode();
                if (this.BetaltTil != null)
                    hashCode = hashCode * 59 + this.BetaltTil.GetHashCode();
                if (this.PlassertBeloep != null)
                    hashCode = hashCode * 59 + this.PlassertBeloep.GetHashCode();
                if (this.PlassertDato != null)
                    hashCode = hashCode * 59 + this.PlassertDato.GetHashCode();
                return hashCode;
            }
        }

        /// <summary>
        /// To validate all properties of the instance
        /// </summary>
        /// <param name="validationContext">Validation context</param>
        /// <returns>Validation Result</returns>
        IEnumerable<System.ComponentModel.DataAnnotations.ValidationResult> IValidatableObject.Validate(ValidationContext validationContext)
        {
            yield break;
        }
    }
}