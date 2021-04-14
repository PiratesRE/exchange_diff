using System;

namespace Microsoft.Exchange.Diagnostics
{
	internal static class SystemProbeConstants
	{
		public static Guid TenantID
		{
			get
			{
				return new Guid("FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF");
			}
		}

		public const string SysProbeRecipientDomain = "contoso.com";

		public const string SysProbeRecipientPrefix = "sysprb";
	}
}
