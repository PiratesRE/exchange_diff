using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.SharePoint.Client;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core.OneDrive
{
	public class ListItemCollectionWrapper : ClientObjectWrapper<ListItemCollection>, IListItemCollection, IClientObjectCollection<IListItem, ListItemCollection>, IClientObject<ListItemCollection>, IEnumerable<IListItem>, IEnumerable
	{
		public IListItem this[int index]
		{
			get
			{
				return new ListItemWrapper(this.backingCollection[index]);
			}
		}

		public ListItemCollectionWrapper(ListItemCollection collection) : base(collection)
		{
			this.backingCollection = collection;
		}

		public int Count()
		{
			return this.backingCollection.Count<ListItem>();
		}

		public IEnumerator<IListItem> GetEnumerator()
		{
			foreach (ListItem item in this.backingCollection)
			{
				yield return new ListItemWrapper(item);
			}
			yield break;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		private ListItemCollection backingCollection;
	}
}
