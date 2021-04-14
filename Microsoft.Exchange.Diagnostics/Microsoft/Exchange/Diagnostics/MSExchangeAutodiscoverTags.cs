using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct MSExchangeAutodiscoverTags
	{
		public const int Framework = 0;

		public const int OutlookProvider = 1;

		public const int MobileSyncProvider = 2;

		public const int FaultInjection = 3;

		public const int AuthMetadata = 4;

		public static Guid guid = new Guid("B3E33516-3A9E-4fba-8469-A88ECCCCDCD1");
	}
}
