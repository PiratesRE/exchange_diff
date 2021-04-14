using System;
using System.Collections;
using System.ComponentModel;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.SettingsResponse
{
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[Serializable]
	public class ListsGetResponseTypeListCollection : ArrayList
	{
		public ListsGetResponseTypeList Add(ListsGetResponseTypeList obj)
		{
			base.Add(obj);
			return obj;
		}

		public ListsGetResponseTypeList Add()
		{
			return this.Add(new ListsGetResponseTypeList());
		}

		public void Insert(int index, ListsGetResponseTypeList obj)
		{
			base.Insert(index, obj);
		}

		public void Remove(ListsGetResponseTypeList obj)
		{
			base.Remove(obj);
		}

		public ListsGetResponseTypeList this[int index]
		{
			get
			{
				return (ListsGetResponseTypeList)base[index];
			}
			set
			{
				base[index] = value;
			}
		}
	}
}
