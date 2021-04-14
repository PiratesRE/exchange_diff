using System;
using System.Collections;
using System.ComponentModel;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.HMMail
{
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[Serializable]
	public class CompletedCollection : ArrayList
	{
		public bitType Add(bitType obj)
		{
			base.Add(obj);
			return obj;
		}

		public bitType Add()
		{
			return this.Add(bitType.zero);
		}

		public void Insert(int index, bitType obj)
		{
			base.Insert(index, obj);
		}

		public void Remove(bitType obj)
		{
			base.Remove(obj);
		}

		public bitType this[int index]
		{
			get
			{
				return (bitType)base[index];
			}
			set
			{
				base[index] = value;
			}
		}
	}
}
