using System;
using System.IO;
using Microsoft.SharePoint.Client;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core.OneDrive
{
	public class MockFolderCollection : MockClientObject<FolderCollection>, IFolderCollection, IClientObject<FolderCollection>
	{
		public MockFolderCollection(string serverRelativeUrl, MockClientContext context)
		{
			this.serverRelativeUrl = serverRelativeUrl;
			this.context = context;
		}

		public override void LoadMockData()
		{
		}

		public IFolder Add(string url)
		{
			return new MockFolder(Path.Combine(this.serverRelativeUrl, url), this.context);
		}

		public IFolder GetByUrl(string url)
		{
			string path = Path.Combine(MockClientContext.MockAttachmentDataProviderFilePath, Path.Combine(this.serverRelativeUrl, url));
			DirectoryInfo directoryInfo = new DirectoryInfo(path);
			if (!directoryInfo.Exists)
			{
				throw new MockServerException();
			}
			return new MockFolder(Path.Combine(this.serverRelativeUrl, url), this.context);
		}

		private readonly string serverRelativeUrl;

		private MockClientContext context;
	}
}
