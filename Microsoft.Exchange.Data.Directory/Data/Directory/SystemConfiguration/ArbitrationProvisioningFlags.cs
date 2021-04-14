using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Flags]
	public enum ArbitrationProvisioningFlags
	{
		None = 0,
		Enabled = 1,
		Halted = 2,
		HaltRecoveryDisabled = 4
	}
}
