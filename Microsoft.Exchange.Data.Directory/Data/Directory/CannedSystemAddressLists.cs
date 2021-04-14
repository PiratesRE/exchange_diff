using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Data.Directory
{
	internal static class CannedSystemAddressLists
	{
		public static Dictionary<string, QueryFilter> RecipientFilters { get; private set; } = new Dictionary<string, QueryFilter>();

		public static Dictionary<string, bool> SystemFlags { get; private set; } = new Dictionary<string, bool>();

		static CannedSystemAddressLists()
		{
			CannedSystemAddressLists.RecipientFilters.Add("All Recipients(VLV)", CannedSystemAddressLists.GetOrFilter(CannedSystemAddressLists.RecipientTypeDetailsForAllRecipientsAL));
			CannedSystemAddressLists.SystemFlags.Add("All Recipients(VLV)", true);
			CannedSystemAddressLists.RecipientFilters.Add("All Mailboxes(VLV)", CannedSystemAddressLists.GetOrFilter(CannedSystemAddressLists.RecipientTypeDetailsForAllMailboxesAL));
			CannedSystemAddressLists.SystemFlags.Add("All Mailboxes(VLV)", true);
			CannedSystemAddressLists.RecipientFilters.Add("All Groups(VLV)", CannedSystemAddressLists.GetOrFilter(CannedSystemAddressLists.RecipientTypeDetailsForAllGroupsAL));
			CannedSystemAddressLists.SystemFlags.Add("All Groups(VLV)", true);
			CannedSystemAddressLists.RecipientFilters.Add("All Mail Users(VLV)", CannedSystemAddressLists.GetOrFilter(CannedSystemAddressLists.RecipientTypeDetailsForAllMailUsersAL));
			CannedSystemAddressLists.SystemFlags.Add("All Mail Users(VLV)", true);
			CannedSystemAddressLists.RecipientFilters.Add("All Contacts(VLV)", CannedSystemAddressLists.GetOrFilter(CannedSystemAddressLists.RecipientTypeDetailsForAllContactsAL));
			CannedSystemAddressLists.SystemFlags.Add("All Contacts(VLV)", true);
			CannedSystemAddressLists.RecipientFilters.Add("Groups(VLV)", CannedSystemAddressLists.RecipientFilters["All Groups(VLV)"]);
			CannedSystemAddressLists.SystemFlags.Add("Groups(VLV)", false);
			CannedSystemAddressLists.RecipientFilters.Add("Mailboxes(VLV)", CannedSystemAddressLists.RecipientFilters["All Mailboxes(VLV)"]);
			CannedSystemAddressLists.SystemFlags.Add("Mailboxes(VLV)", false);
			CannedSystemAddressLists.RecipientFilters.Add("TeamMailboxes(VLV)", CannedSystemAddressLists.GetOrFilter(CannedSystemAddressLists.RecipientTypeDetailsForAllTeamMailboxesAL));
			CannedSystemAddressLists.SystemFlags.Add("TeamMailboxes(VLV)", true);
			CannedSystemAddressLists.RecipientFilters.Add("PublicFolderMailboxes(VLV)", CannedSystemAddressLists.GetOrFilter(CannedSystemAddressLists.RecipientTypeDetailsForAllPublicFolderMailboxesAL));
			CannedSystemAddressLists.SystemFlags.Add("PublicFolderMailboxes(VLV)", true);
			CannedSystemAddressLists.RecipientFilters.Add("MailPublicFolders(VLV)", CannedSystemAddressLists.GetOrFilter(CannedSystemAddressLists.RecipientTypeDetailsForMailPublicFoldersAL));
			CannedSystemAddressLists.SystemFlags.Add("MailPublicFolders(VLV)", true);
			CannedSystemAddressLists.RecipientFilters.Add("GroupMailboxes(VLV)", CannedSystemAddressLists.GetOrFilter(CannedSystemAddressLists.RecipientTypeDetailsForGroupMailboxesAL));
			CannedSystemAddressLists.SystemFlags.Add("GroupMailboxes(VLV)", true);
		}

		public static bool GetFilterByAddressList(string addressList, out QueryFilter queryFilter)
		{
			return CannedSystemAddressLists.RecipientFilters.TryGetValue(addressList, out queryFilter);
		}

		private static QueryFilter GetOrFilter(RecipientTypeDetails[] recipientTypeDetails)
		{
			List<QueryFilter> list = new List<QueryFilter>(recipientTypeDetails.Length);
			foreach (RecipientTypeDetails recipientTypeDetails2 in recipientTypeDetails)
			{
				list.Add(new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.RecipientTypeDetails, recipientTypeDetails2));
			}
			return new OrFilter(list.ToArray());
		}

		public static readonly RecipientTypeDetails[] RecipientTypeDetailsForAllRecipientsAL = new RecipientTypeDetails[]
		{
			RecipientTypeDetails.RoomMailbox,
			RecipientTypeDetails.LinkedRoomMailbox,
			RecipientTypeDetails.EquipmentMailbox,
			RecipientTypeDetails.LegacyMailbox,
			RecipientTypeDetails.LinkedMailbox,
			RecipientTypeDetails.UserMailbox,
			RecipientTypeDetails.MailContact,
			RecipientTypeDetails.DynamicDistributionGroup,
			RecipientTypeDetails.MailForestContact,
			RecipientTypeDetails.MailNonUniversalGroup,
			RecipientTypeDetails.MailUniversalDistributionGroup,
			RecipientTypeDetails.MailUniversalSecurityGroup,
			RecipientTypeDetails.RoomList,
			RecipientTypeDetails.MailUser,
			RecipientTypeDetails.DiscoveryMailbox,
			RecipientTypeDetails.PublicFolder,
			RecipientTypeDetails.TeamMailbox,
			RecipientTypeDetails.SharedMailbox,
			(RecipientTypeDetails)((ulong)int.MinValue),
			RecipientTypeDetails.RemoteRoomMailbox,
			RecipientTypeDetails.RemoteEquipmentMailbox,
			RecipientTypeDetails.RemoteTeamMailbox,
			RecipientTypeDetails.RemoteSharedMailbox
		};

		public static readonly RecipientTypeDetails[] RecipientTypeDetailsForAllMailboxesAL = new RecipientTypeDetails[]
		{
			RecipientTypeDetails.RoomMailbox,
			RecipientTypeDetails.LinkedRoomMailbox,
			RecipientTypeDetails.EquipmentMailbox,
			RecipientTypeDetails.LegacyMailbox,
			RecipientTypeDetails.LinkedMailbox,
			RecipientTypeDetails.UserMailbox,
			RecipientTypeDetails.DiscoveryMailbox,
			RecipientTypeDetails.SharedMailbox
		};

		public static readonly RecipientTypeDetails[] RecipientTypeDetailsForAllGroupsAL = new RecipientTypeDetails[]
		{
			RecipientTypeDetails.MailNonUniversalGroup,
			RecipientTypeDetails.MailUniversalDistributionGroup,
			RecipientTypeDetails.MailUniversalSecurityGroup,
			RecipientTypeDetails.RoomList
		};

		public static readonly RecipientTypeDetails[] RecipientTypeDetailsForAllMailUsersAL = new RecipientTypeDetails[]
		{
			RecipientTypeDetails.MailUser
		};

		public static readonly RecipientTypeDetails[] RecipientTypeDetailsForAllContactsAL = new RecipientTypeDetails[]
		{
			RecipientTypeDetails.MailContact,
			RecipientTypeDetails.MailForestContact
		};

		public static readonly RecipientTypeDetails[] RecipientTypeDetailsForAllTeamMailboxesAL = new RecipientTypeDetails[]
		{
			RecipientTypeDetails.TeamMailbox
		};

		public static readonly RecipientTypeDetails[] RecipientTypeDetailsForAllPublicFolderMailboxesAL = new RecipientTypeDetails[]
		{
			RecipientTypeDetails.PublicFolderMailbox
		};

		public static readonly RecipientTypeDetails[] RecipientTypeDetailsForMailPublicFoldersAL = new RecipientTypeDetails[]
		{
			RecipientTypeDetails.PublicFolder
		};

		public static readonly RecipientTypeDetails[] RecipientTypeDetailsForGroupMailboxesAL = new RecipientTypeDetails[]
		{
			RecipientTypeDetails.GroupMailbox
		};
	}
}
