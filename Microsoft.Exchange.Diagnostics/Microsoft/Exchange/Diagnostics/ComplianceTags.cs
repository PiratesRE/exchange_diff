using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct ComplianceTags
	{
		public const int General = 0;

		public const int Configuration = 1;

		public const int ViewProvider = 2;

		public const int DataProvider = 3;

		public const int View = 4;

		public const int FaultInjection = 5;

		public const int ComplianceService = 6;

		public const int TaskDistributionSystem = 7;

		public static Guid guid = new Guid("3719A9EF-E0BD-45DF-9B58-B36C0C2ECF0E");
	}
}
