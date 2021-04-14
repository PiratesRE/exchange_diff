using System;
using System.Collections;
using System.ComponentModel;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.HMSync
{
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[Serializable]
	public class NameCollection : ArrayList
	{
		public string Add(string obj)
		{
			base.Add(obj);
			return obj;
		}

		public void Insert(int index, string obj)
		{
			base.Insert(index, obj);
		}

		public void Remove(string obj)
		{
			base.Remove(obj);
		}

		public string this[int index]
		{
			get
			{
				return (string)base[index];
			}
			set
			{
				base[index] = value;
			}
		}
	}
}
