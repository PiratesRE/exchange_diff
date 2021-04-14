using System;

namespace Microsoft.Exchange.Data.Directory.Recipient
{
	public enum RecipientDisplayType
	{
		MailboxUser,
		DistributionGroup,
		PublicFolder,
		DynamicDistributionGroup,
		RemoteMailUser = 6,
		ConferenceRoomMailbox,
		EquipmentMailbox,
		SecurityDistributionGroup = 1073741833,
		ACLableMailboxUser = 1073741824,
		ACLableRemoteMailUser = 1073741830,
		Organization = 4,
		PrivateDistributionList,
		ArbitrationMailbox = 10,
		MailboxPlan,
		LinkedUser,
		RoomList = 15,
		TeamMailboxUser,
		GroupMailboxUser,
		ACLableTeamMailboxUser = 1073741840,
		SyncedUDGasContact = -2147483386,
		SyncedUDGasUDG = -2147483391,
		SyncedUSGasUDG = -2147481343,
		SyncedUSGasUSG = -1073739511,
		SyncedUSGasContact = -2147481338,
		ACLableSyncedUSGasContact = -1073739514,
		SyncedDynamicDistributionGroup = -2147482874,
		ACLableSyncedMailboxUser = -1073741818,
		SyncedMailboxUser = -2147483642,
		SyncedConferenceRoomMailbox = -2147481850,
		SyncedEquipmentMailbox = -2147481594,
		SyncedTeamMailboxUser = -2147479546,
		ACLableSyncedTeamMailboxUser = -1073737722,
		SyncedRemoteMailUser = -2147482106,
		ACLableSyncedRemoteMailUser = -1073740282,
		SyncedPublicFolder = -2147483130
	}
}
