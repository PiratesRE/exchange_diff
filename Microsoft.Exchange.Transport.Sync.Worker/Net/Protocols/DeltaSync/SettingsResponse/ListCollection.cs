using System;
using System.Collections;
using System.ComponentModel;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.SettingsResponse
{
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[Serializable]
	public class ListCollection : ArrayList
	{
		public List Add(List obj)
		{
			base.Add(obj);
			return obj;
		}

		public List Add()
		{
			return this.Add(new List());
		}

		public void Insert(int index, List obj)
		{
			base.Insert(index, obj);
		}

		public void Remove(List obj)
		{
			base.Remove(obj);
		}

		public List this[int index]
		{
			get
			{
				return (List)base[index];
			}
			set
			{
				base[index] = value;
			}
		}
	}
}
