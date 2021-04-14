using System;

namespace Microsoft.Exchange.Data.Directory.Recipient
{
	internal static class RemoteRecipientTypeHelper
	{
		public const long RemoteMailboxTypeMask = 224L;

		public const long ProvisionOrDeprovisionTypeMask = 251L;

		public static readonly RemoteRecipientType[] AllowedProvisionOrDeprovisionType = new RemoteRecipientType[]
		{
			RemoteRecipientType.None,
			RemoteRecipientType.ProvisionMailbox,
			RemoteRecipientType.ProvisionMailbox | RemoteRecipientType.RoomMailbox,
			RemoteRecipientType.ProvisionMailbox | RemoteRecipientType.EquipmentMailbox,
			RemoteRecipientType.ProvisionMailbox | RemoteRecipientType.RoomMailbox | RemoteRecipientType.EquipmentMailbox,
			RemoteRecipientType.ProvisionMailbox | RemoteRecipientType.TeamMailbox,
			RemoteRecipientType.ProvisionMailbox | RemoteRecipientType.ProvisionArchive,
			RemoteRecipientType.ProvisionMailbox | RemoteRecipientType.ProvisionArchive | RemoteRecipientType.RoomMailbox,
			RemoteRecipientType.ProvisionMailbox | RemoteRecipientType.ProvisionArchive | RemoteRecipientType.EquipmentMailbox,
			RemoteRecipientType.ProvisionMailbox | RemoteRecipientType.ProvisionArchive | RemoteRecipientType.RoomMailbox | RemoteRecipientType.EquipmentMailbox,
			RemoteRecipientType.ProvisionMailbox | RemoteRecipientType.ProvisionArchive | RemoteRecipientType.TeamMailbox,
			RemoteRecipientType.ProvisionMailbox | RemoteRecipientType.DeprovisionArchive,
			RemoteRecipientType.ProvisionMailbox | RemoteRecipientType.DeprovisionArchive | RemoteRecipientType.RoomMailbox,
			RemoteRecipientType.ProvisionMailbox | RemoteRecipientType.DeprovisionArchive | RemoteRecipientType.EquipmentMailbox,
			RemoteRecipientType.ProvisionMailbox | RemoteRecipientType.DeprovisionArchive | RemoteRecipientType.RoomMailbox | RemoteRecipientType.EquipmentMailbox,
			RemoteRecipientType.ProvisionMailbox | RemoteRecipientType.DeprovisionArchive | RemoteRecipientType.TeamMailbox,
			RemoteRecipientType.DeprovisionMailbox,
			RemoteRecipientType.ProvisionArchive | RemoteRecipientType.DeprovisionMailbox,
			RemoteRecipientType.DeprovisionMailbox | RemoteRecipientType.DeprovisionArchive,
			RemoteRecipientType.ProvisionArchive,
			RemoteRecipientType.DeprovisionArchive
		};
	}
}
