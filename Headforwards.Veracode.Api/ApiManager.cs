using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml;

namespace Headforwards.Veracode.Api
{
    /// <summary>
    /// Wraps the main Veracode Api and returns strongly typed objects rather than XML
    /// </summary>
    public class ApiManager
    {
        static class VeracodeApiEndpoints
        {
            public static string GetScans => "https://analysiscenter.veracode.com/api/5.0/getbuildlist.do";

            public static string GetScanSummaryReport => "https://analysiscenter.veracode.com/api/4.0/summaryreport.do";
        }

        /// <summary>
        /// THe user to connect to the Veracode Api as
        /// </summary>
        public string ApiUserName { get; set; }

        /// <summary>
        /// The password for the Veracode Api user
        /// </summary>
        public string ApiPassword { get; set; }

        /// <summary>
        /// The id of the application to retrieve information from Veracode about
        /// </summary>
        public string ApplicationId { get; set; }

        /// <summary>
        /// Initialises the VeracodeApi object with the Veracode credentials and application identifier
        /// </summary>
        /// <param name="apiUserName">The user to connect to the Api as</param>
        /// <param name="apiPassword">The password for the Veracode user</param>
        /// <param name="applicationId">The Id of the application to retrieve details for</param>
        public ApiManager(string apiUserName, string apiPassword, string applicationId)
        {
            ApiPassword = apiPassword;
            ApiUserName = apiUserName;
            ApplicationId = applicationId;
        }

        /// <summary>
        /// Retrives a list of all the Veracode scans executed for the current application
        /// </summary>
        /// <returns>A list of populated VeracodeScan objects</returns>
        public List<ScanSummary> GetScans()
        {
            var returnData = new List<ScanSummary>();
            try
            {
                // set up the web request to the Veracode API
                var nvc = new NameValueCollection {{"app_id", ApplicationId}};
                var client = new WebClient
                {
                    Credentials = new NetworkCredential(ApiUserName, ApiPassword),
                    QueryString = nvc
                };

                // execute the API call and parse the response into an XML document
                var data = client.DownloadData(VeracodeApiEndpoints.GetScans);
                var doc = new XmlDocument();
                var ms = new MemoryStream(data);
                doc.Load(ms);

                if (doc.InnerText.Contains("Access denied"))
                {
                    // the API call has failed so throw an exception
                    throw new UnauthorizedAccessException("The Veracode requested failed: Access Denied");
                }

                // parse the XML and create the VeracodeScan objects
                // XML format is:
                // <build build_id="1005358" version="Sprint 13.3 Scan 1" policy_updated_date="2016-10-12T11:33:27-04:00"/>
                if ((doc.DocumentElement == null) || (!doc.DocumentElement.HasChildNodes)) return returnData;

                returnData.AddRange(from XmlNode node in doc.DocumentElement.ChildNodes
                    select new ScanSummary(
                        node.Attributes?["build_id"].Value,
                        node.Attributes?["version"].Value,
                        node.Attributes?["policy_updated_date"].Value)
                    );

                // return the collection of builds
                return returnData;
            }
            catch (WebException we)
            {
                returnData.Add(new ScanSummary(we.Message, "", "1900-01-01 00:00:00 UTC"));
                return returnData;
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception in GetScans: " + e);
                throw;
            }
        }

        /// <summary>
        /// Retrieves the summary details of a Veracode Scan for the current application given a scan identifier
        /// </summary>
        /// <param name="scanId">The build_id of the Veracode scan to return</param>
        /// <returns>A populated VeracodeScanReport object</returns>
        public ScanReport GetSummaryReportForScan(string scanId)
        {
            try
            {
                // set up the web request to the Veracode API
                var nvc = new NameValueCollection {{"build_id", scanId}};
                var client = new WebClient
                {
                    Credentials = new NetworkCredential(ApiUserName, ApiPassword),
                    QueryString = nvc
                };

                // execute the API call and parse the response into an XML document
                var data = client.DownloadData(VeracodeApiEndpoints.GetScanSummaryReport);
                var doc = new XmlDocument();
                var ms = new MemoryStream(data);
                doc.Load(ms);

                // parse the XML and create the VeracodeScanReport object
                if ((doc.DocumentElement == null) || (!doc.DocumentElement.HasChildNodes)) return null;

                ScanReport returnData;
                if (doc.DocumentElement.InnerText == "No report available.")
                {
                    // there is no report available so return a blank report
                    returnData = new ScanReport();
                }
                else
                {
                    var scanDate = "";
                    if ((doc.DocumentElement.GetElementsByTagName("static-analysis").Item(0) != null) && 
                        (doc.DocumentElement.GetElementsByTagName("static-analysis").Item(0)?.Attributes?["submitted_date"] != null))
                    {
                        scanDate = doc.DocumentElement.GetElementsByTagName("static-analysis").Item(0)?.Attributes?["published_date"].Value;
                    }
                    returnData = new ScanReport(
                        doc.DocumentElement.Attributes["build_id"].Value,
                        doc.DocumentElement.Attributes["version"].Value,
                        doc.DocumentElement.Attributes["submitter"].Value,
                        doc.DocumentElement.Attributes["app_name"].Value,
                        doc.DocumentElement.Attributes["app_id"].Value,
                        doc.DocumentElement.Attributes["policy_compliance_status"].Value,
                        doc.DocumentElement.Attributes["total_flaws"].Value,
                        doc.DocumentElement.Attributes["flaws_not_mitigated"].Value,
                        scanDate);
                }

                // return the collection of builds
                return returnData;
            }
            catch (WebException we)
            {
                var rep = new ScanReport();
                rep.Id = we.Message;
                return rep;
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception in GetSummaryReportForScan: " + e);
                throw;
            }
        }

        /// <summary>
        /// Retrieves the summary details of the latest Veracode Scan for the current application
        /// </summary>
        /// <returns>A populated VeracodeScanReport object</returns>
        public ScanReport GetLatestScanReport()
        {
            var latestScanId = (from scan in GetScans()
                                   orderby scan.ScanDate descending
                                   select scan.Id).FirstOrDefault();

            return GetSummaryReportForScan(latestScanId);
        }
    }
}
