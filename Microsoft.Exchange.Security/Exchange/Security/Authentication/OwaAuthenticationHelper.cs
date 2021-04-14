using System;
using System.Web;

namespace Microsoft.Exchange.Security.Authentication
{
	public static class OwaAuthenticationHelper
	{
		internal static bool IsOwaUserActivityRequest(HttpRequest httpRequest)
		{
			return httpRequest.Headers["X-UserActivity"] != "0" && httpRequest.QueryString["UA"] != "0" && !Utility.IsOWAPingRequest(httpRequest) && !Utility.IsResourceRequest(httpRequest.Path);
		}

		public const string UserActivity = "X-UserActivity";
	}
}
