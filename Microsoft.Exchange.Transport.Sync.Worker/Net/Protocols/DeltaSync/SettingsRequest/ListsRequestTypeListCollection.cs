using System;
using System.Collections;
using System.ComponentModel;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.SettingsRequest
{
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[Serializable]
	public class ListsRequestTypeListCollection : ArrayList
	{
		public ListsRequestTypeList Add(ListsRequestTypeList obj)
		{
			base.Add(obj);
			return obj;
		}

		public ListsRequestTypeList Add()
		{
			return this.Add(new ListsRequestTypeList());
		}

		public void Insert(int index, ListsRequestTypeList obj)
		{
			base.Insert(index, obj);
		}

		public void Remove(ListsRequestTypeList obj)
		{
			base.Remove(obj);
		}

		public ListsRequestTypeList this[int index]
		{
			get
			{
				return (ListsRequestTypeList)base[index];
			}
			set
			{
				base[index] = value;
			}
		}
	}
}
