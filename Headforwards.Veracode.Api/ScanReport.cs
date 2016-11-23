using System;
using System.Globalization;
using System.Runtime.Serialization;
// ReSharper disable InconsistentNaming

namespace Headforwards.Veracode.Api
{
    /// <summary>
    /// Represents the summary information about a Veracode Scan
    /// </summary>
    [DataContract()]
    public class ScanReport : IScan
    {
        const string NO_REPORT_AVAILABLE = "No Report Available";

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
        /// The user who uploaded the code for Veracode to scan
        /// </summary>
        [DataMember()]
        public string SubmittedBy { get; set; }

        /// <summary>
        /// The name of the application being scanned in Veracode 
        /// </summary>
        [DataMember()]
        public string ApplicationName { get; set; }

        /// <summary>
        /// The identifier of the application being scanned in Veracode
        /// </summary>
        [DataMember()]
        public string ApplicationId { get; set; }

        /// <summary>
        /// The status of the Veracode scan
        /// </summary>
        [DataMember()]
        public string Status { get; set; }

        /// <summary>
        /// The total number of vulnerabilities discovered by Veracode
        /// </summary>
        [DataMember()]
        public int TotalFlaws { get; set; }

        /// <summary>
        /// The total number of vulnerabilities discovered by Veracode that do not have an approved mitigation
        /// </summary>
        [DataMember()]
        public int UnmitigatedFlaws { get; set; }

        /// <summary>
        /// Constructs a ScanReport object where there is no report available
        /// </summary>
        public ScanReport()
        {
            Id = NO_REPORT_AVAILABLE;
            Name = NO_REPORT_AVAILABLE;
            ApplicationName = NO_REPORT_AVAILABLE;
            Status = NO_REPORT_AVAILABLE;
            TotalFlaws = Int32.MaxValue;
            UnmitigatedFlaws = Int32.MaxValue;
            ScanDate = DateTime.Now;
        }

        /// <summary>
        /// Constructs a ScanReport object
        /// </summary>
        /// <param name="build_id"></param>
        /// <param name="version"></param>
        /// <param name="submitter"></param>
        /// <param name="app_name"></param>
        /// <param name="app_id"></param>
        /// <param name="policy_compliance_status"></param>
        /// <param name="total_flaws"></param>
        /// <param name="flaws_not_mitigated"></param>
        /// <param name="submitted_date"></param>
        public ScanReport(string build_id, 
            string version, 
            string submitter, 
            string app_name, 
            string app_id,
            string policy_compliance_status, 
            string total_flaws,
            string flaws_not_mitigated,
            string submitted_date)
        {
            Id = build_id;
            Name = version;
            ApplicationId = app_id;
            ApplicationName = app_name;
            Status = policy_compliance_status;
            SubmittedBy = submitter;
            TotalFlaws = Int32.Parse(total_flaws);
            UnmitigatedFlaws = Int32.Parse(flaws_not_mitigated);

            // parse the scan date and set the property
            var culture = CultureInfo.CreateSpecificCulture("en-US");
            var styles = DateTimeStyles.AssumeUniversal;
            var format = "yyyy-MM-dd HH:mm:ss UTC"; // 2016-11-08 10:18:15 UTC
            ScanDate = DateTime.ParseExact(submitted_date, format, culture, styles);
        }

        public override string ToString()
        {
            return Id == NO_REPORT_AVAILABLE ? NO_REPORT_AVAILABLE : $"{Name} scanned on {ScanDate.ToLongDateString()} at {ScanDate.ToLongTimeString()} had {UnmitigatedFlaws} unmitigated flaws out of a total of {TotalFlaws}.";
        }
    }
}
