using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Microsoft.SharePoint.Client;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core.OneDrive
{
	public class MockListItemCollection : MockClientObject<ListItemCollection>, IListItemCollection, IClientObjectCollection<IListItem, ListItemCollection>, IClientObject<ListItemCollection>, IEnumerable<IListItem>, IEnumerable
	{
		public IListItem this[int index]
		{
			get
			{
				return this.interalList[index];
			}
		}

		public MockListItemCollection(MockClientContext context, string listTitle, string relativePath, List<KeyValuePair<string, bool>> orderBy, List<string> viewFields, int rowLimit, ListItemCollectionPosition listItemCollectionPosition)
		{
			this.context = context;
			this.listTitle = listTitle;
			this.relativePath = relativePath;
			this.orderBy = orderBy;
			this.viewFields = viewFields;
			this.rowLimit = rowLimit;
			this.listItemCollectionPosition = listItemCollectionPosition;
		}

		public override void LoadMockData()
		{
			string text = string.IsNullOrEmpty(this.relativePath) ? this.listTitle : this.relativePath;
			string path = Path.Combine(MockClientContext.MockAttachmentDataProviderFilePath, text);
			DirectoryInfo directoryInfo = new DirectoryInfo(path);
			List<string> list = new List<string>(this.viewFields);
			foreach (KeyValuePair<string, bool> keyValuePair in this.orderBy)
			{
				list.Add(keyValuePair.Key);
			}
			List<MockListItem> list2 = new List<MockListItem>();
			foreach (DirectoryInfo dirInfo in directoryInfo.GetDirectories())
			{
				list2.Add(new MockListItem(dirInfo, text, this.context));
			}
			foreach (FileInfo fileInfo in directoryInfo.GetFiles())
			{
				list2.Add(new MockListItem(fileInfo, text, this.context));
			}
			if (this.orderBy != null && this.orderBy.Count > 0)
			{
				list2.Sort((MockListItem item1, MockListItem item2) => this.Compare(item1, item2, 0));
			}
			ListItemCollectionPosition listItemCollectionPosition = this.listItemCollectionPosition;
			if (this.rowLimit > 0 && this.rowLimit < list2.Count)
			{
				MockListItem[] array = new MockListItem[this.rowLimit];
				list2.CopyTo(0, array, 0, this.rowLimit);
				list2 = new List<MockListItem>(array);
			}
			this.interalList = list2;
		}

		public int Count()
		{
			return this.interalList.Count;
		}

		public IEnumerator<IListItem> GetEnumerator()
		{
			return this.interalList.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		private int Compare(MockListItem item1, MockListItem item2, int level)
		{
			if (level == this.orderBy.Count)
			{
				return 0;
			}
			string key = this.orderBy[level].Key;
			bool value = this.orderBy[level].Value;
			int num = item1[key].ToString().CompareTo(item2[key].ToString()) * (value ? 1 : -1);
			if (num != 0)
			{
				return num;
			}
			return this.Compare(item1, item2, level + 1);
		}

		private readonly ListItemCollectionPosition listItemCollectionPosition;

		private readonly string listTitle;

		private readonly string relativePath;

		private readonly MockClientContext context;

		private readonly List<KeyValuePair<string, bool>> orderBy;

		private readonly List<string> viewFields;

		private readonly int rowLimit;

		private List<MockListItem> interalList;
	}
}
