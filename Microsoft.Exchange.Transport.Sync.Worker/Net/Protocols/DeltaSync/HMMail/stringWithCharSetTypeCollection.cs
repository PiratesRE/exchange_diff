using System;
using System.Collections;
using System.ComponentModel;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.HMMail
{
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[Serializable]
	public class stringWithCharSetTypeCollection : ArrayList
	{
		public stringWithCharSetType Add(stringWithCharSetType obj)
		{
			base.Add(obj);
			return obj;
		}

		public stringWithCharSetType Add()
		{
			return this.Add(new stringWithCharSetType());
		}

		public void Insert(int index, stringWithCharSetType obj)
		{
			base.Insert(index, obj);
		}

		public void Remove(stringWithCharSetType obj)
		{
			base.Remove(obj);
		}

		public stringWithCharSetType this[int index]
		{
			get
			{
				return (stringWithCharSetType)base[index];
			}
			set
			{
				base[index] = value;
			}
		}
	}
}
