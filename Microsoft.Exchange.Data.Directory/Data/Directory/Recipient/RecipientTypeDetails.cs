using System;

namespace Microsoft.Exchange.Data.Directory.Recipient
{
	[Flags]
	public enum RecipientTypeDetails : long
	{
		[LocDescription(DirectoryStrings.IDs.UndefinedRecipientTypeDetails)]
		None = 0L,
		[LocDescription(DirectoryStrings.IDs.MailboxUserRecipientTypeDetails)]
		UserMailbox = 1L,
		[LocDescription(DirectoryStrings.IDs.LinkedMailboxRecipientTypeDetails)]
		LinkedMailbox = 2L,
		[LocDescription(DirectoryStrings.IDs.SharedMailboxRecipientTypeDetails)]
		SharedMailbox = 4L,
		[LocDescription(DirectoryStrings.IDs.LegacyMailboxRecipientTypeDetails)]
		LegacyMailbox = 8L,
		[LocDescription(DirectoryStrings.IDs.ConferenceRoomMailboxRecipientTypeDetails)]
		RoomMailbox = 16L,
		[LocDescription(DirectoryStrings.IDs.EquipmentMailboxRecipientTypeDetails)]
		EquipmentMailbox = 32L,
		[LocDescription(DirectoryStrings.IDs.MailEnabledContactRecipientTypeDetails)]
		MailContact = 64L,
		[LocDescription(DirectoryStrings.IDs.MailEnabledUserRecipientTypeDetails)]
		MailUser = 128L,
		[LocDescription(DirectoryStrings.IDs.MailEnabledUniversalDistributionGroupRecipientTypeDetails)]
		MailUniversalDistributionGroup = 256L,
		[LocDescription(DirectoryStrings.IDs.MailEnabledNonUniversalGroupRecipientTypeDetails)]
		MailNonUniversalGroup = 512L,
		[LocDescription(DirectoryStrings.IDs.MailEnabledUniversalSecurityGroupRecipientTypeDetails)]
		MailUniversalSecurityGroup = 1024L,
		[LocDescription(DirectoryStrings.IDs.MailEnabledDynamicDistributionGroupRecipientTypeDetails)]
		DynamicDistributionGroup = 2048L,
		[LocDescription(DirectoryStrings.IDs.PublicFolderRecipientTypeDetails)]
		PublicFolder = 4096L,
		[LocDescription(DirectoryStrings.IDs.SystemAttendantMailboxRecipientTypeDetails)]
		SystemAttendantMailbox = 8192L,
		[LocDescription(DirectoryStrings.IDs.SystemMailboxRecipientTypeDetails)]
		SystemMailbox = 16384L,
		[LocDescription(DirectoryStrings.IDs.MailEnabledForestContactRecipientTypeDetails)]
		MailForestContact = 32768L,
		[LocDescription(DirectoryStrings.IDs.UserRecipientTypeDetails)]
		User = 65536L,
		[LocDescription(DirectoryStrings.IDs.ContactRecipientTypeDetails)]
		Contact = 131072L,
		[LocDescription(DirectoryStrings.IDs.UniversalDistributionGroupRecipientTypeDetails)]
		UniversalDistributionGroup = 262144L,
		[LocDescription(DirectoryStrings.IDs.UniversalSecurityGroupRecipientTypeDetails)]
		UniversalSecurityGroup = 524288L,
		[LocDescription(DirectoryStrings.IDs.NonUniversalGroupRecipientTypeDetails)]
		NonUniversalGroup = 1048576L,
		[LocDescription(DirectoryStrings.IDs.DisabledUserRecipientTypeDetails)]
		DisabledUser = 2097152L,
		[LocDescription(DirectoryStrings.IDs.MicrosoftExchangeRecipientTypeDetails)]
		MicrosoftExchange = 4194304L,
		[LocDescription(DirectoryStrings.IDs.ArbitrationMailboxTypeDetails)]
		ArbitrationMailbox = 8388608L,
		[LocDescription(DirectoryStrings.IDs.MailboxPlanTypeDetails)]
		MailboxPlan = 16777216L,
		[LocDescription(DirectoryStrings.IDs.LinkedUserTypeDetails)]
		LinkedUser = 33554432L,
		[LocDescription(DirectoryStrings.IDs.RoomListGroupTypeDetails)]
		RoomList = 268435456L,
		[LocDescription(DirectoryStrings.IDs.DiscoveryMailboxTypeDetails)]
		DiscoveryMailbox = 536870912L,
		[LocDescription(DirectoryStrings.IDs.RoleGroupTypeDetails)]
		RoleGroup = 1073741824L,
		[LocDescription(DirectoryStrings.IDs.RemoteUserMailboxTypeDetails)]
		RemoteUserMailbox = 2147483648L,
		[LocDescription(DirectoryStrings.IDs.ComputerRecipientTypeDetails)]
		Computer = 4294967296L,
		[LocDescription(DirectoryStrings.IDs.RemoteRoomMailboxTypeDetails)]
		RemoteRoomMailbox = 8589934592L,
		[LocDescription(DirectoryStrings.IDs.RemoteEquipmentMailboxTypeDetails)]
		RemoteEquipmentMailbox = 17179869184L,
		[LocDescription(DirectoryStrings.IDs.RemoteSharedMailboxTypeDetails)]
		RemoteSharedMailbox = 34359738368L,
		[LocDescription(DirectoryStrings.IDs.PublicFolderMailboxRecipientTypeDetails)]
		PublicFolderMailbox = 68719476736L,
		[LocDescription(DirectoryStrings.IDs.TeamMailboxRecipientTypeDetails)]
		TeamMailbox = 137438953472L,
		[LocDescription(DirectoryStrings.IDs.RemoteTeamMailboxRecipientTypeDetails)]
		RemoteTeamMailbox = 274877906944L,
		[LocDescription(DirectoryStrings.IDs.MonitoringMailboxRecipientTypeDetails)]
		MonitoringMailbox = 549755813888L,
		[LocDescription(DirectoryStrings.IDs.GroupMailboxRecipientTypeDetails)]
		GroupMailbox = 1099511627776L,
		[LocDescription(DirectoryStrings.IDs.LinkedRoomMailboxRecipientTypeDetails)]
		LinkedRoomMailbox = 2199023255552L,
		[LocDescription(DirectoryStrings.IDs.AuditLogMailboxRecipientTypeDetails)]
		AuditLogMailbox = 4398046511104L,
		[LocDescription(DirectoryStrings.IDs.RemoteGroupMailboxRecipientTypeDetails)]
		RemoteGroupMailbox = 8796093022208L,
		AllUniqueRecipientTypes = 17592186044415L
	}
}
