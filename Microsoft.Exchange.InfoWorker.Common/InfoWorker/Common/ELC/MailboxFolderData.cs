using System;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.InfoWorker.Common.ELC
{
	internal class MailboxFolderData
	{
		internal VersionedId Id
		{
			get
			{
				return this.id;
			}
			set
			{
				this.id = value;
			}
		}

		internal StoreObjectId ParentId
		{
			get
			{
				return this.parentId;
			}
			set
			{
				this.parentId = value;
			}
		}

		internal string Name
		{
			get
			{
				return this.name;
			}
			set
			{
				this.name = value;
			}
		}

		internal Guid ElcFolderGuid
		{
			get
			{
				return this.adGuid;
			}
			set
			{
				this.adGuid = value;
			}
		}

		internal string Comment
		{
			get
			{
				return this.comment;
			}
			set
			{
				this.comment = value;
			}
		}

		internal ELCFolderFlags Flags
		{
			get
			{
				return this.elcFlags;
			}
			set
			{
				this.elcFlags = value;
			}
		}

		internal int FolderQuota
		{
			get
			{
				return this.folderQuota;
			}
			set
			{
				this.folderQuota = value;
			}
		}

		internal string Url
		{
			get
			{
				return this.url;
			}
			set
			{
				this.url = value;
			}
		}

		internal string LocalizedName
		{
			get
			{
				return this.localizedName;
			}
			set
			{
				this.localizedName = value;
			}
		}

		internal bool IsOrganizationalFolder()
		{
			return (this.elcFlags & ELCFolderFlags.Provisioned) == ELCFolderFlags.Provisioned;
		}

		private VersionedId id;

		private StoreObjectId parentId;

		private string name;

		private Guid adGuid;

		private string comment;

		private ELCFolderFlags elcFlags;

		private int folderQuota;

		private string url;

		private string localizedName;
	}
}
