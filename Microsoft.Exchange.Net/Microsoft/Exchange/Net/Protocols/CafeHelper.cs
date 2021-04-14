using System;
using System.Web;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Net.Protocols
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class CafeHelper
	{
		public static string GetSourceCafeServer(HttpRequest httpRequest)
		{
			if (httpRequest == null)
			{
				throw new ArgumentNullException("httpRequest");
			}
			return httpRequest.Headers[WellKnownHeader.XSourceCafeServer];
		}

		public static string GetSourceCafeArrayUrl(HttpRequest httpRequest)
		{
			if (httpRequest == null)
			{
				throw new ArgumentNullException("httpRequest");
			}
			throw new NotImplementedException("OM: 1361029");
		}

		public static bool IsFromNativeProxy(HttpRequest httpRequest)
		{
			if (httpRequest == null)
			{
				throw new ArgumentNullException("httpRequest");
			}
			string b = httpRequest.Headers[CafeHelper.CafeProxyHandler];
			return string.Equals(CafeHelper.NativeHttpProxy, b, StringComparison.OrdinalIgnoreCase);
		}

		public static readonly string CafeProxyHandler = "X-CafeProxyHandler";

		public static readonly string NativeHttpProxy = "NativeHttpProxy";
	}
}
