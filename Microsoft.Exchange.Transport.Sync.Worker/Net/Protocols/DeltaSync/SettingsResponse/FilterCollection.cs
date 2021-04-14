using System;
using System.Collections;
using System.ComponentModel;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.SettingsResponse
{
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[Serializable]
	public class FilterCollection : ArrayList
	{
		public Filter Add(Filter obj)
		{
			base.Add(obj);
			return obj;
		}

		public Filter Add()
		{
			return this.Add(new Filter());
		}

		public void Insert(int index, Filter obj)
		{
			base.Insert(index, obj);
		}

		public void Remove(Filter obj)
		{
			base.Remove(obj);
		}

		public Filter this[int index]
		{
			get
			{
				return (Filter)base[index];
			}
			set
			{
				base[index] = value;
			}
		}
	}
}
