using System;
using Microsoft.SharePoint.Client;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core.OneDrive
{
	public class MockListCollection : MockClientObject<ListCollection>, IListCollection, IClientObject<ListCollection>
	{
		public MockListCollection(MockClientContext context)
		{
			this.context = context;
		}

		public override void LoadMockData()
		{
		}

		public IList GetByTitle(string title)
		{
			return new MockList(this.context, title);
		}

		public IList GetById(Guid guid)
		{
			return new MockList(this.context);
		}

		private MockClientContext context;
	}
}
