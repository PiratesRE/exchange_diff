using System;

namespace Microsoft.Exchange.Data.Storage
{
	internal enum SharingPermissionLevel
	{
		FreeBusy,
		LimitedDetails = 10,
		Read = 20,
		ReadWrite = 30,
		CoOwner = 40,
		Owner = 50
	}
}
