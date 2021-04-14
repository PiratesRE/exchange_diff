using System;

namespace Microsoft.Exchange.InfoWorker.Common.ELC
{
	[Flags]
	internal enum UpgradeStatus
	{
		None = 0,
		AppliedFolderTag = 1
	}
}
