using System;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	public static class RemoteMailboxTaskHelper
	{
		public static void UpdateRemoteMailboxType(this ADUser remoteMailbox, RemoteMailboxType remoteMailboxType, bool aclableSyncedObject = false)
		{
			if (remoteMailbox == null)
			{
				throw new ArgumentNullException("remoteMailbox");
			}
			if ((remoteMailbox.RemoteRecipientType & RemoteRecipientType.ProvisionMailbox) != RemoteRecipientType.ProvisionMailbox)
			{
				throw new ArgumentException("remoteMailbox.RemoteRecipientType must include ProvisionMailbox");
			}
			remoteMailbox.RemoteRecipientType &= (remoteMailbox.RemoteRecipientType & ~(RemoteRecipientType.RoomMailbox | RemoteRecipientType.EquipmentMailbox | RemoteRecipientType.TeamMailbox));
			if (remoteMailboxType <= RemoteMailboxType.Room)
			{
				if (remoteMailboxType == (RemoteMailboxType)((ulong)-2147483648))
				{
					remoteMailbox.RecipientTypeDetails = (RecipientTypeDetails)((ulong)int.MinValue);
					remoteMailbox.RecipientDisplayType = new RecipientDisplayType?(aclableSyncedObject ? RecipientDisplayType.ACLableSyncedMailboxUser : RecipientDisplayType.SyncedMailboxUser);
					return;
				}
				if (remoteMailboxType != RemoteMailboxType.Room)
				{
					return;
				}
				remoteMailbox.RemoteRecipientType |= RemoteRecipientType.RoomMailbox;
				remoteMailbox.RecipientTypeDetails = RecipientTypeDetails.RemoteRoomMailbox;
				remoteMailbox.RecipientDisplayType = new RecipientDisplayType?(RecipientDisplayType.SyncedConferenceRoomMailbox);
				return;
			}
			else
			{
				if (remoteMailboxType == RemoteMailboxType.Equipment)
				{
					remoteMailbox.RemoteRecipientType |= RemoteRecipientType.EquipmentMailbox;
					remoteMailbox.RecipientTypeDetails = RecipientTypeDetails.RemoteEquipmentMailbox;
					remoteMailbox.RecipientDisplayType = new RecipientDisplayType?(RecipientDisplayType.SyncedEquipmentMailbox);
					return;
				}
				if (remoteMailboxType == RemoteMailboxType.Shared)
				{
					remoteMailbox.RemoteRecipientType |= RemoteRecipientType.SharedMailbox;
					remoteMailbox.RecipientTypeDetails = RecipientTypeDetails.RemoteSharedMailbox;
					remoteMailbox.RecipientDisplayType = new RecipientDisplayType?(aclableSyncedObject ? RecipientDisplayType.ACLableSyncedMailboxUser : RecipientDisplayType.SyncedMailboxUser);
					return;
				}
				if (remoteMailboxType != RemoteMailboxType.Team)
				{
					return;
				}
				remoteMailbox.RemoteRecipientType |= RemoteRecipientType.TeamMailbox;
				remoteMailbox.RecipientTypeDetails = RecipientTypeDetails.RemoteTeamMailbox;
				remoteMailbox.RecipientDisplayType = new RecipientDisplayType?(aclableSyncedObject ? RecipientDisplayType.ACLableSyncedTeamMailboxUser : RecipientDisplayType.SyncedTeamMailboxUser);
				return;
			}
		}

		public static RemoteMailboxType GetRemoteMailboxType(RemoteRecipientType remoteRecipientType)
		{
			RemoteMailboxType result = (RemoteMailboxType)((ulong)int.MinValue);
			RemoteRecipientType remoteRecipientType2 = remoteRecipientType & (RemoteRecipientType.RoomMailbox | RemoteRecipientType.EquipmentMailbox | RemoteRecipientType.TeamMailbox);
			if (remoteRecipientType2 == RemoteRecipientType.RoomMailbox)
			{
				result = RemoteMailboxType.Room;
			}
			else if (remoteRecipientType2 == RemoteRecipientType.EquipmentMailbox)
			{
				result = RemoteMailboxType.Equipment;
			}
			else if (remoteRecipientType2 == RemoteRecipientType.SharedMailbox)
			{
				result = RemoteMailboxType.Shared;
			}
			else if (remoteRecipientType2 == RemoteRecipientType.TeamMailbox)
			{
				result = RemoteMailboxType.Team;
			}
			return result;
		}
	}
}
