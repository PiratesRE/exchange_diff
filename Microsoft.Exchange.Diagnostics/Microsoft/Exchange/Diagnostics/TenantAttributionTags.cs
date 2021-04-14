using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct TenantAttributionTags
	{
		public const int TenantAttributionInbound = 0;

		public const int TenantAttributionOutbound = 1;

		public static Guid guid = new Guid("97680724-6FF7-4C3A-BD8F-6E329E54AF3A");
	}
}
