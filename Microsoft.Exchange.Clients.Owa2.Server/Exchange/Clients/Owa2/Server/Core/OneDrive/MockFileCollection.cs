using System;
using Microsoft.SharePoint.Client;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core.OneDrive
{
	public class MockFileCollection : MockClientObject<FileCollection>, IFileCollection, IClientObject<FileCollection>
	{
		public MockFileCollection(string serverRelativeUrl, MockClientContext context)
		{
			this.serverRelativeUrl = serverRelativeUrl;
			this.context = context;
		}

		public override void LoadMockData()
		{
		}

		public IFile Add(FileCreationInformation parameters)
		{
			return new MockFile(parameters, this.serverRelativeUrl, this.context);
		}

		private readonly string serverRelativeUrl;

		private MockClientContext context;
	}
}
