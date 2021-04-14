using System;
using Microsoft.SharePoint.Client;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core.OneDrive
{
	public class MockWeb : MockClientObject<Web>, IWeb, IClientObject<Web>
	{
		public IListCollection Lists
		{
			get
			{
				MockListCollection result;
				if ((result = this.lists) == null)
				{
					result = (this.lists = new MockListCollection(this.context));
				}
				return result;
			}
		}

		public MockWeb(MockClientContext context)
		{
			this.context = context;
		}

		public IFolder GetFolderByServerRelativeUrl(string serverRelativeUrl)
		{
			return new MockFolder(serverRelativeUrl, this.context);
		}

		public IFile GetFileByServerRelativeUrl(string relativeLocation)
		{
			return new MockFile(relativeLocation, this.context);
		}

		public IList GetList(string url)
		{
			return new MockList(this.context);
		}

		public override void LoadMockData()
		{
		}

		private MockClientContext context;

		private MockListCollection lists;
	}
}
