using System;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	[SimpleConfiguration("OWA.OtherMailbox", "OtherMailbox")]
	internal class OtherMailboxConfigEntry
	{
		public OtherMailboxConfigEntry()
		{
		}

		public OtherMailboxConfigEntry(string displayName, OwaStoreObjectId rootFolderId)
		{
			this.mailboxDisplayName = displayName;
			this.mailboxRootFolderId = rootFolderId.ToString();
		}

		[SimpleConfigurationProperty("displayName")]
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
		public string RootFolderId
		{
			get
			{
				return this.mailboxRootFolderId;
			}
			set
			{
				this.mailboxRootFolderId = value;
			}
		}

		private string mailboxDisplayName;

		private string mailboxRootFolderId;
	}
}
