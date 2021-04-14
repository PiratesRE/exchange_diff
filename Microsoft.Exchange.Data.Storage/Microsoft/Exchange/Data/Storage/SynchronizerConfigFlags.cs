using System;

namespace Microsoft.Exchange.Data.Storage
{
	[Flags]
	internal enum SynchronizerConfigFlags
	{
		None = 0,
		Unicode = 1,
		NoDeletions = 2,
		NoSoftDeletions = 4,
		ReadState = 8,
		Associated = 16,
		Normal = 32,
		NoConflicts = 64,
		OnlySpecifiedProps = 128,
		NoForeignKeys = 256,
		Catchup = 1024,
		BestBody = 8192,
		IgnoreSpecifiedOnAssociated = 16384,
		ProgressMode = 32768,
		FXRecoverMode = 65536,
		ForceUnicode = 262144,
		NoChanges = 524288,
		OrderByDeliveryTime = 1048576,
		PartialItem = 8388608,
		UseCpId = 16777216,
		SendPropErrors = 33554432,
		ManifestMode = 67108864,
		CatchupFull = 134217728
	}
}
