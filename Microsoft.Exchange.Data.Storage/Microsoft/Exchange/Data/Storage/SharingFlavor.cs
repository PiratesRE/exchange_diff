using System;

namespace Microsoft.Exchange.Data.Storage
{
	[Flags]
	internal enum SharingFlavor
	{
		None = 0,
		SharingIn = 1,
		BindingIn = 2,
		IndexIn = 4,
		ExclusiveIn = 8,
		SharingOut = 16,
		BindingOut = 32,
		IndexOut = 64,
		ExclusiveOut = 128,
		SharingMessage = 256,
		SharingMessageInvitation = 512,
		SharingMessageRequest = 1024,
		SharingMessageUpdate = 2048,
		SharingMessageResponse = 4096,
		SharingMessageAccept = 8192,
		SharingMessageDeny = 16384,
		SharingMessageRevoke = 32768,
		SharingReciprocation = 65536,
		PrimaryOwnership = 131072
	}
}
