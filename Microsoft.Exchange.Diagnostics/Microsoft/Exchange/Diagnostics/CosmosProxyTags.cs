using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct CosmosProxyTags
	{
		public const int Sender = 0;

		public const int Receiver = 1;

		public const int Downloader = 2;

		public static Guid guid = new Guid("18229B60-53AF-4337-9F63-BACE4AB588AD");
	}
}
