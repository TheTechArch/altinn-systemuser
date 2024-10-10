namespace SmartCloud.Server.Models
{
    public record Right
    {

        /// <summary>
        /// For instance: Read, Write, Sign
        /// </summary>                
        public string? Action { get; set; }

        /// <summary>
        /// The list of attributes that identifes a resource part of a right.
        /// </summary>
        public List<AttributePair> Resource { get; set; } = [];
    }
}
