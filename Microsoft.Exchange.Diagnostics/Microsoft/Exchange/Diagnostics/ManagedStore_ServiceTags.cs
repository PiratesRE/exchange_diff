using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct ManagedStore_ServiceTags
	{
		public const int StartupShutdown = 0;

		public const int FaultInjection = 20;

		public static Guid guid = new Guid("2e177940-9c28-43b0-9f7a-b92bf03227a6");
	}
}
