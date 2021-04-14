using System;
using System.Collections;
using System.ComponentModel;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.SyncRequest
{
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[Serializable]
	public class CollectionCollection : ArrayList
	{
		public Collection Add(Collection obj)
		{
			base.Add(obj);
			return obj;
		}

		public Collection Add()
		{
			return this.Add(new Collection());
		}

		public void Insert(int index, Collection obj)
		{
			base.Insert(index, obj);
		}

		public void Remove(Collection obj)
		{
			base.Remove(obj);
		}

		public Collection this[int index]
		{
			get
			{
				return (Collection)base[index];
			}
			set
			{
				base[index] = value;
			}
		}
	}
}
