using System;
using Microsoft.SharePoint.Client;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core.OneDrive
{
	public class ListCollectionWrapper : ClientObjectWrapper<ListCollection>, IListCollection, IClientObject<ListCollection>
	{
		public ListCollectionWrapper(ListCollection lists) : base(lists)
		{
			this.backingLists = lists;
		}

		public IList GetByTitle(string title)
		{
			return new ListWrapper(this.backingLists.GetByTitle(title));
		}

		public IList GetById(Guid guid)
		{
			return new ListWrapper(this.backingLists.GetById(guid));
		}

		private ListCollection backingLists;
	}
}
