using System;
using System.Collections;
using System.ComponentModel;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.SyncRequest
{
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[Serializable]
	public class DeleteCollection : ArrayList
	{
		public Delete Add(Delete obj)
		{
			base.Add(obj);
			return obj;
		}

		public Delete Add()
		{
			return this.Add(new Delete());
		}

		public void Insert(int index, Delete obj)
		{
			base.Insert(index, obj);
		}

		public void Remove(Delete obj)
		{
			base.Remove(obj);
		}

		public Delete this[int index]
		{
			get
			{
				return (Delete)base[index];
			}
			set
			{
				base[index] = value;
			}
		}
	}
}
