using System;
using Microsoft.SharePoint.Client;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core.OneDrive
{
	public class ListWrapper : ClientObjectWrapper<List>, IList, IClientObject<List>
	{
		public IFolder RootFolder
		{
			get
			{
				return new FolderWrapper(this.backingList.RootFolder);
			}
		}

		public ListWrapper(List list) : base(list)
		{
			this.backingList = list;
		}

		public IListItemCollection GetItems(CamlQuery query)
		{
			return new ListItemCollectionWrapper(this.backingList.GetItems(query));
		}

		public IListItem GetItemById(string id)
		{
			return new ListItemWrapper(this.backingList.GetItemById(id));
		}

		private List backingList;
	}
}
