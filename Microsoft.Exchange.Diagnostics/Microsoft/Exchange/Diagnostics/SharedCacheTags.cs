using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct SharedCacheTags
	{
		public const int Cache = 0;

		public const int Server = 1;

		public const int Client = 2;

		public static Guid guid = new Guid("E71C276F-E35F-40CB-BC7E-559CE4A9B4B3");
	}
}
