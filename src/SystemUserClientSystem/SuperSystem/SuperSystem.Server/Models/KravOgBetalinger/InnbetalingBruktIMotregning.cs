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
    /// InnbetalingBruktIMotregning
    /// </summary>
    [DataContract]
        public partial class InnbetalingBruktIMotregning :  IEquatable<InnbetalingBruktIMotregning>, IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InnbetalingBruktIMotregning" /> class.
        /// </summary>
        /// <param name="innbetalingsidentifikator">innbetalingsidentifikator (required).</param>
        /// <param name="uplassertBeloep">uplassertBeloep (required).</param>
        public InnbetalingBruktIMotregning(string innbetalingsidentifikator = default(string), double? uplassertBeloep = default(double?))
        {
            // to ensure "innbetalingsidentifikator" is required (not null)
            if (innbetalingsidentifikator == null)
            {
                throw new InvalidDataException("innbetalingsidentifikator is a required property for InnbetalingBruktIMotregning and cannot be null");
            }
            else
            {
                this.Innbetalingsidentifikator = innbetalingsidentifikator;
            }
            // to ensure "uplassertBeloep" is required (not null)
            if (uplassertBeloep == null)
            {
                throw new InvalidDataException("uplassertBeloep is a required property for InnbetalingBruktIMotregning and cannot be null");
            }
            else
            {
                this.UplassertBeloep = uplassertBeloep;
            }
        }
        
        /// <summary>
        /// Gets or Sets Innbetalingsidentifikator
        /// </summary>
        [DataMember(Name="innbetalingsidentifikator", EmitDefaultValue=false)]
        public string Innbetalingsidentifikator { get; set; }

        /// <summary>
        /// Gets or Sets UplassertBeloep
        /// </summary>
        [DataMember(Name="uplassertBeloep", EmitDefaultValue=false)]
        public double? UplassertBeloep { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class InnbetalingBruktIMotregning {\n");
            sb.Append("  Innbetalingsidentifikator: ").Append(Innbetalingsidentifikator).Append("\n");
            sb.Append("  UplassertBeloep: ").Append(UplassertBeloep).Append("\n");
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
            return this.Equals(input as InnbetalingBruktIMotregning);
        }

        /// <summary>
        /// Returns true if InnbetalingBruktIMotregning instances are equal
        /// </summary>
        /// <param name="input">Instance of InnbetalingBruktIMotregning to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(InnbetalingBruktIMotregning input)
        {
            if (input == null)
                return false;

            return 
                (
                    this.Innbetalingsidentifikator == input.Innbetalingsidentifikator ||
                    (this.Innbetalingsidentifikator != null &&
                    this.Innbetalingsidentifikator.Equals(input.Innbetalingsidentifikator))
                ) && 
                (
                    this.UplassertBeloep == input.UplassertBeloep ||
                    (this.UplassertBeloep != null &&
                    this.UplassertBeloep.Equals(input.UplassertBeloep))
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
                if (this.Innbetalingsidentifikator != null)
                    hashCode = hashCode * 59 + this.Innbetalingsidentifikator.GetHashCode();
                if (this.UplassertBeloep != null)
                    hashCode = hashCode * 59 + this.UplassertBeloep.GetHashCode();
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