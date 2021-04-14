using System;
using System.Collections;
using System.ComponentModel;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.SettingsRequest
{
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[Serializable]
	public class FiltersRequestTypeFilterCollection : ArrayList
	{
		public FiltersRequestTypeFilter Add(FiltersRequestTypeFilter obj)
		{
			base.Add(obj);
			return obj;
		}

		public FiltersRequestTypeFilter Add()
		{
			return this.Add(new FiltersRequestTypeFilter());
		}

		public void Insert(int index, FiltersRequestTypeFilter obj)
		{
			base.Insert(index, obj);
		}

		public void Remove(FiltersRequestTypeFilter obj)
		{
			base.Remove(obj);
		}

		public FiltersRequestTypeFilter this[int index]
		{
			get
			{
				return (FiltersRequestTypeFilter)base[index];
			}
			set
			{
				base[index] = value;
			}
		}
	}
}
