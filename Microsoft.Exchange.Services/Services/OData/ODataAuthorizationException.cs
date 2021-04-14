using System;
using System.Net;
using System.Web;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Security.OAuth;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.OData
{
	internal class ODataAuthorizationException : ODataResponseException
	{
		public ODataAuthorizationException(Exception innerException) : base(HttpStatusCode.Forbidden, ResponseCodeType.ErrorAccessDenied, CoreResources.ErrorAccessDenied, innerException)
		{
		}

		public ODataAuthorizationException(LocalizedString errorMessage) : base(HttpStatusCode.Forbidden, ResponseCodeType.ErrorAccessDenied, errorMessage, null)
		{
		}

		public override void AppendResponseHeader(HttpContext httpContext)
		{
			if (base.InnerException != null)
			{
				InvalidOAuthTokenException ex = base.InnerException as InvalidOAuthTokenException;
				if (ex != null)
				{
					MSDiagnosticsHeader.AppendInvalidOAuthTokenExceptionToBackendResponse(httpContext, ex);
				}
			}
		}
	}
}
