using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.Directory.Management
{
	[Serializable]
	public sealed class SystemAddressList : AddressListBase
	{
		internal override ADPresentationSchema PresentationSchema
		{
			get
			{
				return SystemAddressList.schema;
			}
		}

		public SystemAddressList()
		{
		}

		public SystemAddressList(AddressBookBase dataObject) : base(dataObject)
		{
		}

		public static string[] AddressLists
		{
			get
			{
				return new string[]
				{
					"All Recipients(VLV)",
					"All Mailboxes(VLV)",
					"All Mail Users(VLV)",
					"All Contacts(VLV)",
					"All Groups(VLV)",
					"Mailboxes(VLV)",
					"Groups(VLV)",
					"TeamMailboxes(VLV)",
					"GroupMailboxes(VLV)",
					"PublicFolderMailboxes(VLV)",
					"MailPublicFolders(VLV)"
				};
			}
		}

		public const string AllRecipients = "All Recipients(VLV)";

		public const string AllMailboxes = "All Mailboxes(VLV)";

		public const string AllMailUsers = "All Mail Users(VLV)";

		public const string AllContacts = "All Contacts(VLV)";

		public const string AllGroups = "All Groups(VLV)";

		public const string Mailboxes = "Mailboxes(VLV)";

		public const string Groups = "Groups(VLV)";

		public const string TeamMailboxes = "TeamMailboxes(VLV)";

		public const string PublicFolderMailboxes = "PublicFolderMailboxes(VLV)";

		public const string MailPublicFolders = "MailPublicFolders(VLV)";

		public const string GroupMailboxes = "GroupMailboxes(VLV)";

		private static SystemAddressListSchema schema = ObjectSchema.GetInstance<SystemAddressListSchema>();

		public static readonly ADObjectId RdnSystemAddressListContainerToOrganization = new ADObjectId("CN=All System Address Lists,CN=Address Lists Container");
	}
}
