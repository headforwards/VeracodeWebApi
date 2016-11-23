using Headforwards.Axa.Ppp.WebApi.Filters;
using Headforwards.Veracode.Api;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Headforwards.Axa.Ppp.WebApi.Controllers
{
    [EnableCors("*", "*", "GET")]
    public class LatestVeracodeScanController : ApiController
    {
        private ApiManager _workflowAppApiManager;

        // GET api/220896/scans/latest
        [VeracodeAuthorize()]
        public ScanReport Get(string applicationId)
        {
            var username = Request.Properties["username"].ToString();
            var password = Request.Properties["password"].ToString();

            _workflowAppApiManager = new ApiManager(username, password, applicationId);
            
            return _workflowAppApiManager.GetLatestScanReport();
        }
    }
}
