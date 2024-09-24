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
    /// Periode
    /// </summary>
    [DataContract]
        public partial class Periode :  IEquatable<Periode>, IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Periode" /> class.
        /// </summary>
        /// <param name="fraOgMed">fraOgMed (required).</param>
        /// <param name="tilOgMed">tilOgMed.</param>
        public Periode(DateTime? fraOgMed = default(DateTime?), DateTime? tilOgMed = default(DateTime?))
        {
            // to ensure "fraOgMed" is required (not null)
            if (fraOgMed == null)
            {
                throw new InvalidDataException("fraOgMed is a required property for Periode and cannot be null");
            }
            else
            {
                this.FraOgMed = fraOgMed;
            }
            this.TilOgMed = tilOgMed;
        }
        
        /// <summary>
        /// Gets or Sets FraOgMed
        /// </summary>
        [DataMember(Name="fraOgMed", EmitDefaultValue=false)]
        public DateTime? FraOgMed { get; set; }

        /// <summary>
        /// Gets or Sets TilOgMed
        /// </summary>
        [DataMember(Name="tilOgMed", EmitDefaultValue=false)]
        public DateTime? TilOgMed { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class Periode {\n");
            sb.Append("  FraOgMed: ").Append(FraOgMed).Append("\n");
            sb.Append("  TilOgMed: ").Append(TilOgMed).Append("\n");
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
            return this.Equals(input as Periode);
        }

        /// <summary>
        /// Returns true if Periode instances are equal
        /// </summary>
        /// <param name="input">Instance of Periode to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(Periode input)
        {
            if (input == null)
                return false;

            return 
                (
                    this.FraOgMed == input.FraOgMed ||
                    (this.FraOgMed != null &&
                    this.FraOgMed.Equals(input.FraOgMed))
                ) && 
                (
                    this.TilOgMed == input.TilOgMed ||
                    (this.TilOgMed != null &&
                    this.TilOgMed.Equals(input.TilOgMed))
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
                if (this.FraOgMed != null)
                    hashCode = hashCode * 59 + this.FraOgMed.GetHashCode();
                if (this.TilOgMed != null)
                    hashCode = hashCode * 59 + this.TilOgMed.GetHashCode();
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
