using System;

namespace Microsoft.Isam.Esent.Interop
{
	public enum JET_SNP
	{
		Repair = 2,
		Compact = 4,
		Restore = 8,
		Backup,
		Scrub = 11,
		UpgradeRecordFormat
	}
}
