using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct ThrottlingServiceTags
	{
		public const int ThrottlingService = 0;

		public const int ThrottlingClient = 1;

		public const int Export = 2;

		public static Guid guid = new Guid("2e888ec1-6dd9-48cb-aa14-5bf7cad71a88");
	}
}
