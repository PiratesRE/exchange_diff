using System;
using System.Collections.Specialized;
using System.Web;

namespace Microsoft.Exchange.Configuration.DiagnosticsModules
{
	internal class DiagnosticsHelper
	{
		internal static NameValueCollection GetUrlProperties(Uri uri)
		{
			if (uri == null)
			{
				throw new ArgumentNullException("uri");
			}
			return HttpUtility.ParseQueryString(uri.Query.Replace(';', '&'));
		}
	}
}
