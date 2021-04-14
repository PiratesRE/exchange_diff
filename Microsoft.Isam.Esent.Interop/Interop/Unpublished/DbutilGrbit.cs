using System;

namespace Microsoft.Isam.Esent.Interop.Unpublished
{
	[Flags]
	public enum DbutilGrbit
	{
		None = 0,
		OptionAllNodes = 1,
		OptionKeyStats = 2,
		OptionPageDump = 4,
		OptionStats = 8,
		OptionSuppressConsoleOutput = 16,
		OptionIgnoreErrors = 32,
		OptionVerify = 64,
		OptionReportErrors = 128,
		OptionDontRepair = 256,
		OptionRepairAll = 512,
		OptionRepairIndexes = 1024,
		OptionDontBuildIndexes = 2048,
		OptionSuppressLogo = 32768,
		OptionRepairCheckOnly = 65536,
		OptionDumpLVPageUsage = 131072,
		OptionDumpLogInfoCSV = 262144,
		OptionDumpLogPermitPatching = 524288,
		OptionDumpVerbose = 268435456,
		OptionCheckBTree = 536870912
	}
}
