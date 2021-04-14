using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct ManagedStore_StoreIntegrityCheckTags
	{
		public const int StartupShutdown = 0;

		public const int OnlineIsinteg = 1;

		public const int FaultInjection = 20;

		public static Guid guid = new Guid("856DA9F3-E7F6-4565-84F6-71A96AF18D92");
	}
}
