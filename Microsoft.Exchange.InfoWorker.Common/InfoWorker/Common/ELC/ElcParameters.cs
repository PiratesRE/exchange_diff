using System;

namespace Microsoft.Exchange.InfoWorker.Common.ELC
{
	[Flags]
	internal enum ElcParameters
	{
		None = 0,
		HoldCleanup = 1,
		EHAHiddenFolderCleanup = 2
	}
}
