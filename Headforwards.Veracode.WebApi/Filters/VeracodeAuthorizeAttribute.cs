using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Principal;
using System.Text;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Headforwards.Axa.Ppp.WebApi.Filters
{
    /// <summary>
    /// Custom authorisation attribute to authenticate users with the Veracode API
    /// Can be used by adding the [VeracodeAuthorize()] attribute to all methods that 
    /// need to be authorised by Veracode
    /// </summary>
    public class VeracodeAuthorizeAttribute : AuthorizationFilterAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            var authHeader = actionContext.Request.Headers.Authorization;

            if (authHeader != null)
            {
                if (authHeader.Scheme.Equals("basic", StringComparison.OrdinalIgnoreCase) && 
                    !string.IsNullOrWhiteSpace(authHeader.Parameter))
                {
                    var rawCredentials = authHeader.Parameter;
                    var encoding = Encoding.GetEncoding("iso-8859-1");
                    var credentials = encoding.GetString(Convert.FromBase64String(rawCredentials));

                    var split = credentials.Split(':');
                    var username = split[0];
                    var password = split[1];

                    if (ValidateVeracodeUser(username, password))
                    {
                        actionContext.Request.Properties.Add("username", username);
                        actionContext.Request.Properties.Add("password", password);
                        return;
                    }
                }
            }
            // if we have got here then we have failed authorisation
            HandleUnauthorised(actionContext);
        }

        private bool ValidateVeracodeUser(string username, string password)
        {
            // TODO validate username and password?
            // at the moment we don't need to validate because the API call itself will validate
            // the Veracode API does not have an easy way to validate credentials other than calling the API
            // Instead we will just store the username and password from the header in the request properties
            // so that the API calls can easily extract them rather than needing to parse the header again
            return true;
        }

        private void HandleUnauthorised(HttpActionContext actionContext)
        {
            actionContext.Response = actionContext.Request.CreateResponse(System.Net.HttpStatusCode.Unauthorized);
        }
    }
}