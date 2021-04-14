using System;

namespace Microsoft.Exchange.Data.Storage
{
	[Flags]
	internal enum ManifestConfigFlags
	{
		None = 0,
		NoDeletions = 2,
		NoSoftDeletions = 4,
		ReadState = 8,
		Associated = 16,
		Normal = 32,
		Catchup = 64,
		NoChanges = 128,
		OrderByDeliveryTime = 1048576
	}
}
