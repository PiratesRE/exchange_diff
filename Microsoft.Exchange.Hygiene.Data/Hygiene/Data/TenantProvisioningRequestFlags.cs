using System;

namespace Microsoft.Exchange.Hygiene.Data
{
	[Flags]
	public enum TenantProvisioningRequestFlags
	{
		Default = 0,
		Reporting = 1,
		ServiceUpgrade = 2,
		GlobalLocator = 4,
		MicrosoftOnline = 8,
		PremigrationCheck = 16,
		Relocation = 32,
		ForceServiceUpgrade = 64
	}
}
