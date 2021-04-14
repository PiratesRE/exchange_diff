using System;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.AirSync.SyncStateConverter
{
	internal class MailboxUtilityDeviceInfo
	{
		internal MailboxUtilityDeviceInfo(string displayName, string parentDisplayName, StoreObjectId storeObjectId, HashSet<string> folderList)
		{
			this.displayName = displayName;
			this.parentDisplayName = parentDisplayName;
			this.storeObjectId = storeObjectId;
			this.folderList = folderList;
		}

		internal string DisplayName
		{
			get
			{
				return this.displayName;
			}
			set
			{
				this.displayName = value;
			}
		}

		internal string ParentDisplayName
		{
			get
			{
				return this.parentDisplayName;
			}
			set
			{
				this.parentDisplayName = value;
			}
		}

		internal StoreObjectId StoreObjectId
		{
			get
			{
				return this.storeObjectId;
			}
			set
			{
				this.storeObjectId = value;
			}
		}

		internal HashSet<string> FolderList
		{
			get
			{
				return this.folderList;
			}
			set
			{
				this.folderList = value;
			}
		}

		private string displayName;

		private string parentDisplayName;

		private StoreObjectId storeObjectId;

		private HashSet<string> folderList;
	}
}
