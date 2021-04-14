using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	public enum TenantRelocationStatus : byte
	{
		NotStarted = 200,
		Synchronization,
		Lockdown,
		Retired,
		Arriving,
		Active
	}
}
