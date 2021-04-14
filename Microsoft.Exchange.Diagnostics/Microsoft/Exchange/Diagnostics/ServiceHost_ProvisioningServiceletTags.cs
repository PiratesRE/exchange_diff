using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct ServiceHost_ProvisioningServiceletTags
	{
		public const int FaultInjection = 0;

		public const int Servicelet = 1;

		public const int Worker = 2;

		public static Guid guid = new Guid("9132698f-5149-4949-a24f-1bb1928f9692");
	}
}
