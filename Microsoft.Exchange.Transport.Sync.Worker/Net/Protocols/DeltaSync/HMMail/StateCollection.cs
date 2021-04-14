using System;
using System.Collections;
using System.ComponentModel;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.HMMail
{
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[Serializable]
	public class StateCollection : ArrayList
	{
		public byte Add(byte obj)
		{
			base.Add(obj);
			return obj;
		}

		public byte Add()
		{
			return this.Add(0);
		}

		public void Insert(int index, byte obj)
		{
			base.Insert(index, obj);
		}

		public void Remove(byte obj)
		{
			base.Remove(obj);
		}

		public byte this[int index]
		{
			get
			{
				return (byte)base[index];
			}
			set
			{
				base[index] = value;
			}
		}
	}
}
