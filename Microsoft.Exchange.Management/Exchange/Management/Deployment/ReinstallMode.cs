using System;

namespace Microsoft.Exchange.Management.Deployment
{
	[Flags]
	internal enum ReinstallMode
	{
		Repair = 1,
		FileMissing = 2,
		FileOlderVersion = 4,
		FileEqualVersion = 8,
		FileExact = 16,
		FileVerify = 32,
		FileReplace = 64,
		MachineData = 128,
		UserData = 256,
		ShortCut = 512,
		Package = 1024
	}
}
