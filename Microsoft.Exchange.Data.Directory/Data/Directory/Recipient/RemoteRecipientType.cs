using System;

namespace Microsoft.Exchange.Data.Directory.Recipient
{
	[Flags]
	public enum RemoteRecipientType : long
	{
		None = 0L,
		ProvisionMailbox = 1L,
		ProvisionArchive = 2L,
		Migrated = 4L,
		DeprovisionMailbox = 8L,
		DeprovisionArchive = 16L,
		RoomMailbox = 32L,
		EquipmentMailbox = 64L,
		SharedMailbox = 96L,
		TeamMailbox = 128L
	}
}
