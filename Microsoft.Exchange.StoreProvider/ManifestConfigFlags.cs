using System;

namespace Microsoft.Mapi
{
	[Flags]
	internal enum ManifestConfigFlags
	{
		NoChanges = 1,
		NoDeletions = 2,
		NoSoftDeletions = 4,
		NoReadUnread = 8,
		Associated = 16,
		Normal = 32,
		OrderByDeliveryTime = 64,
		ReevaluateOnRestrictionChange = 128,
		Catchup = 256,
		Conversations = 512
	}
}
