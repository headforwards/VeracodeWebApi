using System.Collections.Generic;
using System.Web.Http;
using Headforwards.Veracode.Api;
using System.Web.Http.Cors;
using System.Configuration;
using Headforwards.Axa.Ppp.WebApi.Filters;

namespace Headforwards.Axa.Ppp.WebApi.Controllers
{
    [VeracodeAuthorize()]
    public class VeracodeScansController : ApiController
    {
        private ApiManager _workflowAppApiManager;

        // GET api/220896/scans
        public IEnumerable<ScanSummary> Get(string applicationId)
        {
            var username = Request.Properties["username"].ToString();
            var password = Request.Properties["password"].ToString();

            _workflowAppApiManager = new ApiManager(username, password, applicationId);

            return _workflowAppApiManager.GetScans();
        }

        // GET api/220896/scans/1048864
        public ScanReport Get(string scanId, string applicationId)
        {
            var username = Request.Properties["username"].ToString();
            var password = Request.Properties["password"].ToString();

            _workflowAppApiManager = new ApiManager(username, password, applicationId);

            return _workflowAppApiManager.GetSummaryReportForScan(scanId);
        }
    }
}
