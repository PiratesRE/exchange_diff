using System;

namespace Microsoft.Exchange.Cluster.Replay.MountPoint
{
	internal enum VolumeSpareStatus
	{
		Unknown,
		UnEncryptedEmptySpare,
		EncryptingEmptySpare,
		EncryptedEmptySpare,
		Quarantined,
		NotUsableAsSpare,
		Error,
		LastIndex
	}
}
