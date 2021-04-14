using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct LiveIdRedirectionTags
	{
		public const int Redirection = 0;

		public const int ErrorReporting = 1;

		public const int FaultInjection = 2;

		public const int TenantMonitoring = 3;

		public static Guid guid = new Guid("62a46310-1793-40b2-9f61-74bf8637fce6");
	}
}
