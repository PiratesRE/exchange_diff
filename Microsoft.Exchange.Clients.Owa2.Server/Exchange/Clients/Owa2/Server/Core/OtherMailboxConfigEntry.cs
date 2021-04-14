using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[SimpleConfiguration("OWA.OtherMailbox", "OtherMailbox")]
	internal class OtherMailboxConfigEntry
	{
		public OtherMailboxConfigEntry()
		{
		}

		public OtherMailboxConfigEntry(string displayName, string inboxFolderOwaStoreObjectId)
		{
			this.mailboxDisplayName = displayName;
			this.inboxFolderOwaStoreObjectId = inboxFolderOwaStoreObjectId;
		}

		[SimpleConfigurationProperty("displayName")]
		[DataMember]
		public string DisplayName
		{
			get
			{
				return this.mailboxDisplayName;
			}
			set
			{
				this.mailboxDisplayName = value;
			}
		}

		[SimpleConfigurationProperty("rootFolderId")]
		public string InboxFolderOwaStoreObjectId
		{
			get
			{
				return this.inboxFolderOwaStoreObjectId;
			}
			set
			{
				this.inboxFolderOwaStoreObjectId = value;
			}
		}

		[DataMember]
		[SimpleConfigurationProperty("principalSMTPAddress")]
		public string PrincipalSMTPAddress
		{
			get
			{
				return this.principalSMTPAddress;
			}
			set
			{
				this.principalSMTPAddress = value;
			}
		}

		private string mailboxDisplayName = string.Empty;

		private string inboxFolderOwaStoreObjectId = string.Empty;

		private string principalSMTPAddress;
	}
}
