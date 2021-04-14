using System;

namespace Microsoft.Exchange.Security.Authentication
{
	[Flags]
	public enum AuthOptions
	{
		None = 0,
		SyncAD = 1,
		BypassCache = 2,
		SyncADBackEndOnly = 4,
		PasswordAndHRDSync = 8,
		CompactToken = 16,
		ReturnWindowsIdentity = 32,
		BypassPositiveCache = 64,
		SyncUPN = 128,
		AllowOfflineOrgIdAsPrimeAuth = 256,
		LiveIdXmlAuth = 512
	}
}
