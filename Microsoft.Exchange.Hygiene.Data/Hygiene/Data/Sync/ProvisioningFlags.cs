using System;

namespace Microsoft.Exchange.Hygiene.Data.Sync
{
	[Flags]
	public enum ProvisioningFlags
	{
		Default = 0,
		DisableMxRecordProvisioning = 1,
		ExchangeOnlineProtection = 2,
		SyncOnly = 4,
		SynchronousProvisioningEnabled = 8
	}
}
