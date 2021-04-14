using System;
using System.Collections.Specialized;
using System.Web;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Configuration.Core
{
	internal static class Extensions
	{
		public static string GetFriendlyName(this OrganizationId orgId)
		{
			if (orgId != null && orgId.OrganizationalUnit != null)
			{
				return orgId.OrganizationalUnit.Name;
			}
			return null;
		}

		internal static NameValueCollection GetUrlProperties(this Uri uri)
		{
			if (uri == null)
			{
				return null;
			}
			UriBuilder uriBuilder = new UriBuilder(uri);
			return HttpUtility.ParseQueryString(uriBuilder.Query.Replace(';', '&'));
		}
	}
}
