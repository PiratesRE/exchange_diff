using System;
using Microsoft.SharePoint.Client;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core.OneDrive
{
	public class FolderWrapper : ClientObjectWrapper<Folder>, IFolder, IClientObject<Folder>
	{
		public int ItemCount
		{
			get
			{
				return this.backingFolder.ItemCount;
			}
		}

		public string ServerRelativeUrl
		{
			get
			{
				return this.backingFolder.ServerRelativeUrl;
			}
		}

		public IListItem ListItemAllFields
		{
			get
			{
				IListItem result;
				if ((result = this.listItem) == null)
				{
					result = (this.listItem = new ListItemWrapper(this.backingFolder.ListItemAllFields));
				}
				return result;
			}
		}

		public IFolderCollection Folders
		{
			get
			{
				FolderCollectionWrapper result;
				if ((result = this.folders) == null)
				{
					result = (this.folders = new FolderCollectionWrapper(this.backingFolder.Folders));
				}
				return result;
			}
		}

		public IFileCollection Files
		{
			get
			{
				FileCollectionWrapper result;
				if ((result = this.files) == null)
				{
					result = (this.files = new FileCollectionWrapper(this.backingFolder.Files));
				}
				return result;
			}
		}

		public void DeleteObject()
		{
			this.backingFolder.DeleteObject();
		}

		public FolderWrapper(Folder folder) : base(folder)
		{
			this.backingFolder = folder;
		}

		private Folder backingFolder;

		private FolderCollectionWrapper folders;

		private FileCollectionWrapper files;

		private IListItem listItem;
	}
}
