using System;
using System.Collections;
using System.ComponentModel;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.SyncRequest
{
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[Serializable]
	public class ChangeCollection : ArrayList
	{
		public Change Add(Change obj)
		{
			base.Add(obj);
			return obj;
		}

		public Change Add()
		{
			return this.Add(new Change());
		}

		public void Insert(int index, Change obj)
		{
			base.Insert(index, obj);
		}

		public void Remove(Change obj)
		{
			base.Remove(obj);
		}

		public Change this[int index]
		{
			get
			{
				return (Change)base[index];
			}
			set
			{
				base[index] = value;
			}
		}
	}
}
