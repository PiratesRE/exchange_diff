using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct DirectoryCacheTags
	{
		public const int Session = 0;

		public const int CacheSession = 1;

		public const int WCFServiceEndpoint = 2;

		public const int WCFClientEndpoint = 3;

		public static Guid guid = new Guid("2550C2A5-C4F4-4358-83E4-894A370B5A20");
	}
}
