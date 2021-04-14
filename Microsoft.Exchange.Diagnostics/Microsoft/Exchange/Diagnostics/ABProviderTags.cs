using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct ABProviderTags
	{
		public const int Framework = 0;

		public const int ActiveDirectory = 1;

		public const int ExchangeWebServices = 2;

		public const int OwaUrls = 3;

		public static Guid guid = new Guid("9E009811-D5D4-434b-B1BC-85C64CE57046");
	}
}
