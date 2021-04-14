using System;
using System.IO;
using Microsoft.SharePoint.Client;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core.OneDrive
{
	public class MockFolder : MockClientObject<Folder>, IFolder, IClientObject<Folder>
	{
		public int ItemCount { get; private set; }

		public string ServerRelativeUrl { get; private set; }

		public IListItem ListItemAllFields
		{
			get
			{
				if (this.listItem == null)
				{
					string path = Path.Combine(MockClientContext.MockAttachmentDataProviderFilePath, this.ServerRelativeUrl);
					DirectoryInfo dirInfo = new DirectoryInfo(path);
					this.listItem = new MockListItem(dirInfo, new DirectoryInfo(path).Parent.FullName, this.context);
				}
				return this.listItem;
			}
			private set
			{
				this.listItem = value;
			}
		}

		public IFolderCollection Folders
		{
			get
			{
				MockFolderCollection result;
				if ((result = this.folders) == null)
				{
					result = (this.folders = new MockFolderCollection(this.ServerRelativeUrl, this.context));
				}
				return result;
			}
		}

		public IFileCollection Files
		{
			get
			{
				MockFileCollection result;
				if ((result = this.files) == null)
				{
					result = (this.files = new MockFileCollection(this.ServerRelativeUrl, this.context));
				}
				return result;
			}
		}

		public void DeleteObject()
		{
			string path = Path.Combine(MockClientContext.MockAttachmentDataProviderFilePath, this.ServerRelativeUrl);
			DirectoryInfo directoryInfo = new DirectoryInfo(path);
			if (directoryInfo.Exists)
			{
				directoryInfo.Delete();
			}
		}

		public MockFolder(string serverRelativeUrl, MockClientContext context)
		{
			this.context = context;
			this.ServerRelativeUrl = serverRelativeUrl;
		}

		public MockFolder(MockListItem mockListItem, MockClientContext context)
		{
			this.ListItemAllFields = mockListItem;
			this.ServerRelativeUrl = (string)mockListItem["FileRef"];
			this.context = context;
		}

		public override void LoadMockData()
		{
			string path = Path.Combine(MockClientContext.MockAttachmentDataProviderFilePath, this.ServerRelativeUrl);
			DirectoryInfo directoryInfo = new DirectoryInfo(path);
			if (!directoryInfo.Exists)
			{
				directoryInfo.Create();
			}
			this.ItemCount = directoryInfo.GetDirectories().Length + directoryInfo.GetFiles().Length;
		}

		private MockClientContext context;

		private MockFolderCollection folders;

		private MockFileCollection files;

		private IListItem listItem;
	}
}
