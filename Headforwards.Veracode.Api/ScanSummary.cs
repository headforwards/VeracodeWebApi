using System;
using System.Globalization;
using System.Runtime.Serialization;
// ReSharper disable InconsistentNaming

namespace Headforwards.Veracode.Api
{
    /// <summary>
    /// Represents the basic information about a Veracode Scan
    /// </summary>
    [DataContract()]
    public class ScanSummary : IScan
    {
        /// <summary>
        /// The identifier of the Veracode scan
        /// </summary>
        [DataMember()]
        public string Id { get; set; }

        /// <summary>
        /// The name given to the Veracode scan, usually a build identifier
        /// </summary>
        [DataMember()]
        public string Name { get; set; }

        /// <summary>
        /// The date the scan was executed
        /// </summary>
        [DataMember()]
        public DateTime ScanDate { get; set; }

        /// <summary>
        /// Constructs a new Veracode scan summary
        /// </summary>
        /// <param name="build_id">The identifier for the scan</param>
        /// <param name="version">The name given to the scan</param>
        /// <param name="policy_updated_date">The date the scan was executed</param>
        public ScanSummary(string build_id, string version, string policy_updated_date)
        {
            var culture = CultureInfo.CreateSpecificCulture("en-US");
            var styles = DateTimeStyles.AssumeLocal;
            DateTime dateTime;

            Id = build_id;
            Name = version;
            var conversionSuccess = DateTime.TryParse(policy_updated_date, culture, styles, out dateTime);
            if (conversionSuccess)
                ScanDate = dateTime;
        }

        public override string ToString()
        {
            return $"{Name} scanned on {ScanDate.ToLongDateString()} at {ScanDate.ToLongTimeString()} with id of {Id}";
        }
    }
}
