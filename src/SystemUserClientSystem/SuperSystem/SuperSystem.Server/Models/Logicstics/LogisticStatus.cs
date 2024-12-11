namespace SystemUserApi.Models.Logistics
{
    /// <summary>
    /// Dummy model for Logicist
    /// </summary>
    public class LogisticStatus
    {
        /// <summary>
        /// A overalls status for the logistic
        /// </summary>
        public string Status { get; set; }  

        /// <summary>
        /// The shipment status
        /// </summary>
        public string ShipmentStatus { get; set; }

        /// <summary>
        /// The import status
        /// </summary>
        public string ImportStatus { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ExportStatus { get; set; }

        public string TransitStatus { get; set; }   

        /// <summary>
        /// Last changed
        /// </summary>
        public string CreateTime { get; set; }
    }
}
