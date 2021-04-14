using System;
using System.Web;

namespace Microsoft.Exchange.Security.Authorization
{
	internal static class CommonAccessTokenExtensions
	{
		public static string GetMemberName(this HttpContext httpContext)
		{
			if (httpContext != null && httpContext.Items != null)
			{
				CommonAccessToken commonAccessToken = httpContext.Items["Item-CommonAccessToken"] as CommonAccessToken;
				if (commonAccessToken != null && commonAccessToken.ExtensionData.ContainsKey("MemberName"))
				{
					return commonAccessToken.ExtensionData["MemberName"];
				}
			}
			return null;
		}
	}
}
