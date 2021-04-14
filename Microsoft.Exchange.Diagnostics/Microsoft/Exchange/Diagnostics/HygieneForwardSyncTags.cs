using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct HygieneForwardSyncTags
	{
		public const int ServiceInstanceSync = 0;

		public const int FullTenantSync = 1;

		public const int Persistence = 2;

		public const int Provisioning = 3;

		public const int MsoServices = 4;

		public const int GlsServices = 5;

		public const int DNSServices = 6;

		public static Guid guid = new Guid("952887AB-4E9A-4CF8-867F-3C5BD5BB67A3");
	}
}
