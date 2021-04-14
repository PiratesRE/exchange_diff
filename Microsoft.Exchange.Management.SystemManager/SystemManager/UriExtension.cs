using System;

namespace Microsoft.Exchange.Management.SystemManager
{
	public static class UriExtension
	{
		public static bool IsHttp(this Uri uri)
		{
			if (uri == null)
			{
				throw new ArgumentNullException("uri");
			}
			return uri.Scheme.Equals(Uri.UriSchemeHttp, StringComparison.OrdinalIgnoreCase);
		}

		public static bool IsHttps(this Uri uri)
		{
			if (uri == null)
			{
				throw new ArgumentNullException("uri");
			}
			return uri.Scheme.Equals(Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase);
		}
	}
}
