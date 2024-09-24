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
    /// KreditkravBruktIMotregning
    /// </summary>
    [DataContract]
        public partial class KreditkravBruktIMotregning :  IEquatable<KreditkravBruktIMotregning>, IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="KreditkravBruktIMotregning" /> class.
        /// </summary>
        /// <param name="kravforfallsIdentifikator">kravforfallsIdentifikator (required).</param>
        /// <param name="gjenstaaendeBeloep">gjenstaaendeBeloep (required).</param>
        /// <param name="kravbeskrivelse">kravbeskrivelse (required).</param>
        /// <param name="kravperiode">kravperiode.</param>
        public KreditkravBruktIMotregning(string kravforfallsIdentifikator = default(string), double? gjenstaaendeBeloep = default(double?), MultiSpraakTekst kravbeskrivelse = default(MultiSpraakTekst), Kravperiode kravperiode = default(Kravperiode))
        {
            // to ensure "kravforfallsIdentifikator" is required (not null)
            if (kravforfallsIdentifikator == null)
            {
                throw new InvalidDataException("kravforfallsIdentifikator is a required property for KreditkravBruktIMotregning and cannot be null");
            }
            else
            {
                this.KravforfallsIdentifikator = kravforfallsIdentifikator;
            }
            // to ensure "gjenstaaendeBeloep" is required (not null)
            if (gjenstaaendeBeloep == null)
            {
                throw new InvalidDataException("gjenstaaendeBeloep is a required property for KreditkravBruktIMotregning and cannot be null");
            }
            else
            {
                this.GjenstaaendeBeloep = gjenstaaendeBeloep;
            }
            // to ensure "kravbeskrivelse" is required (not null)
            if (kravbeskrivelse == null)
            {
                throw new InvalidDataException("kravbeskrivelse is a required property for KreditkravBruktIMotregning and cannot be null");
            }
            else
            {
                this.Kravbeskrivelse = kravbeskrivelse;
            }
            this.Kravperiode = kravperiode;
        }
        
        /// <summary>
        /// Gets or Sets KravforfallsIdentifikator
        /// </summary>
        [DataMember(Name="kravforfallsIdentifikator", EmitDefaultValue=false)]
        public string KravforfallsIdentifikator { get; set; }

        /// <summary>
        /// Gets or Sets GjenstaaendeBeloep
        /// </summary>
        [DataMember(Name="gjenstaaendeBeloep", EmitDefaultValue=false)]
        public double? GjenstaaendeBeloep { get; set; }

        /// <summary>
        /// Gets or Sets Kravbeskrivelse
        /// </summary>
        [DataMember(Name="kravbeskrivelse", EmitDefaultValue=false)]
        public MultiSpraakTekst Kravbeskrivelse { get; set; }

        /// <summary>
        /// Gets or Sets Kravperiode
        /// </summary>
        [DataMember(Name="kravperiode", EmitDefaultValue=false)]
        public Kravperiode Kravperiode { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class KreditkravBruktIMotregning {\n");
            sb.Append("  KravforfallsIdentifikator: ").Append(KravforfallsIdentifikator).Append("\n");
            sb.Append("  GjenstaaendeBeloep: ").Append(GjenstaaendeBeloep).Append("\n");
            sb.Append("  Kravbeskrivelse: ").Append(Kravbeskrivelse).Append("\n");
            sb.Append("  Kravperiode: ").Append(Kravperiode).Append("\n");
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
            return this.Equals(input as KreditkravBruktIMotregning);
        }

        /// <summary>
        /// Returns true if KreditkravBruktIMotregning instances are equal
        /// </summary>
        /// <param name="input">Instance of KreditkravBruktIMotregning to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(KreditkravBruktIMotregning input)
        {
            if (input == null)
                return false;

            return 
                (
                    this.KravforfallsIdentifikator == input.KravforfallsIdentifikator ||
                    (this.KravforfallsIdentifikator != null &&
                    this.KravforfallsIdentifikator.Equals(input.KravforfallsIdentifikator))
                ) && 
                (
                    this.GjenstaaendeBeloep == input.GjenstaaendeBeloep ||
                    (this.GjenstaaendeBeloep != null &&
                    this.GjenstaaendeBeloep.Equals(input.GjenstaaendeBeloep))
                ) && 
                (
                    this.Kravbeskrivelse == input.Kravbeskrivelse ||
                    (this.Kravbeskrivelse != null &&
                    this.Kravbeskrivelse.Equals(input.Kravbeskrivelse))
                ) && 
                (
                    this.Kravperiode == input.Kravperiode ||
                    (this.Kravperiode != null &&
                    this.Kravperiode.Equals(input.Kravperiode))
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
                if (this.KravforfallsIdentifikator != null)
                    hashCode = hashCode * 59 + this.KravforfallsIdentifikator.GetHashCode();
                if (this.GjenstaaendeBeloep != null)
                    hashCode = hashCode * 59 + this.GjenstaaendeBeloep.GetHashCode();
                if (this.Kravbeskrivelse != null)
                    hashCode = hashCode * 59 + this.Kravbeskrivelse.GetHashCode();
                if (this.Kravperiode != null)
                    hashCode = hashCode * 59 + this.Kravperiode.GetHashCode();
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
