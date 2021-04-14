using System;
using System.Collections;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public sealed class DataTreeListViewItemCollection : CollectionBase
	{
		public DataTreeListViewItemCollection(DataTreeListViewItem owner)
		{
			if (owner == null)
			{
				throw new NotSupportedException();
			}
			this.owner = owner;
			this.listView = owner.ListView;
		}

		public DataTreeListViewItemCollection(DataTreeListView listView)
		{
			if (listView == null)
			{
				throw new NotSupportedException();
			}
			this.listView = listView;
		}

		protected override void OnClear()
		{
			foreach (object obj in this)
			{
				DataTreeListViewItem dataTreeListViewItem = (DataTreeListViewItem)obj;
				dataTreeListViewItem.RemoveFromListView();
				dataTreeListViewItem.ClearChildrenDataSources();
			}
		}

		protected override void OnRemove(int index, object value)
		{
			this[index].ClearChildrenDataSources();
			this[index].RemoveFromListView();
		}

		public void Add(DataTreeListViewItem item)
		{
			this.Insert(base.Count, item);
		}

		public void Insert(int index, DataTreeListViewItem item)
		{
			if (index < 0 && index > base.Count)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			if (!this.Contains(item))
			{
				base.List.Insert(index, item);
				item.Parent = this.owner;
				if (item.Parent != null)
				{
					this.owner.IsLeaf = false;
					item.IndentCount = this.owner.IndentCount + 1;
					if (item.Parent.IsExpanded && item.Parent.IsInListView)
					{
						item.AddToListView(this.ListView);
						return;
					}
				}
				else
				{
					item.IndentCount = 1;
					item.AddToListView(this.ListView);
				}
			}
		}

		public void Remove(DataTreeListViewItem item)
		{
			int num = this.IndexOf(item);
			if (num >= 0)
			{
				base.RemoveAt(num);
			}
		}

		public DataTreeListViewItem this[int index]
		{
			get
			{
				return (DataTreeListViewItem)base.List[index];
			}
		}

		public int IndexOf(object value)
		{
			return base.List.IndexOf(value);
		}

		public bool Contains(object value)
		{
			return base.List.Contains(value);
		}

		public DataTreeListView ListView
		{
			get
			{
				if (this.owner != null && this.owner.ListView != this.listView)
				{
					this.listView = this.owner.ListView;
				}
				return this.listView;
			}
		}

		private DataTreeListViewItem owner;

		private DataTreeListView listView;
	}
}
