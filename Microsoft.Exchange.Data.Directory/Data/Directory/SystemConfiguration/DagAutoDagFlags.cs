using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Flags]
	internal enum DagAutoDagFlags
	{
		None = 0,
		AutoDagEnabled = 1,
		DatabaseCopyLocationAgilityDisabled = 2,
		AutoReseedDisabled = 4,
		AllServersInstalled = 8,
		ReplayLagManagerEnabled = 16,
		DiskReclaimerDisabled = 32,
		BitlockerEnabled = 64,
		FIPSCompliant = 128
	}
}
