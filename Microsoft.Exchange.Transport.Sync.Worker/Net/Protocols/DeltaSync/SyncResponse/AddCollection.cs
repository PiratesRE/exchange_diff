using System;
using System.Collections;
using System.ComponentModel;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.SyncResponse
{
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[Serializable]
	public class AddCollection : ArrayList
	{
		public Add Add(Add obj)
		{
			base.Add(obj);
			return obj;
		}

		public Add Add()
		{
			return this.Add(new Add());
		}

		public void Insert(int index, Add obj)
		{
			base.Insert(index, obj);
		}

		public void Remove(Add obj)
		{
			base.Remove(obj);
		}

		public Add this[int index]
		{
			get
			{
				return (Add)base[index];
			}
			set
			{
				base[index] = value;
			}
		}
	}
}
