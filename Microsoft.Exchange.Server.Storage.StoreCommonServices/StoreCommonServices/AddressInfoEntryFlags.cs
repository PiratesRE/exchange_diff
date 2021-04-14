using System;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	[Flags]
	public enum AddressInfoEntryFlags
	{
		EntryId = 1,
		SearchKey = 2,
		AddressType = 4,
		EmailAddress = 8,
		DisplayName = 16,
		SimpleDisplayName = 32,
		Flags = 64,
		OriginalAddressType = 128,
		OriginalEmailAddress = 256,
		Sid = 512,
		Guid = 1024
	}
}
