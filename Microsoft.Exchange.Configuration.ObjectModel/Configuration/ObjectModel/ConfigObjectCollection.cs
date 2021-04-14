using System;
using System.ComponentModel;

namespace Microsoft.Exchange.Configuration.ObjectModel
{
	[Serializable]
	internal class ConfigObjectCollection : BaseConfigCollection, ICloneable
	{
		public ConfigObjectCollection()
		{
		}

		public ConfigObjectCollection(ConfigObject[] configObjectArray) : base(configObjectArray)
		{
		}

		public ConfigObject this[int index]
		{
			get
			{
				return (ConfigObject)base.List[index];
			}
			set
			{
				base.Replace(index, value);
			}
		}

		public ConfigObject this[string identity]
		{
			get
			{
				int num = base.IndexOfIdentity(identity);
				if (-1 != num)
				{
					return this[num];
				}
				return null;
			}
			set
			{
				base.Replace(base.IndexOfIdentity(identity), value);
			}
		}

		public int TotalCount
		{
			get
			{
				return this.totalCount;
			}
			set
			{
				this.totalCount = value;
			}
		}

		public int PageOffset
		{
			get
			{
				return this.pageOffset;
			}
			set
			{
				this.pageOffset = value;
			}
		}

		public object Clone()
		{
			ConfigObjectCollection configObjectCollection = new ConfigObjectCollection();
			foreach (object obj in base.List)
			{
				ConfigObject value = (ConfigObject)obj;
				configObjectCollection.List.Add(value);
			}
			configObjectCollection.IsReadOnly = this.IsReadOnly;
			configObjectCollection.TotalCount = this.TotalCount;
			configObjectCollection.PageOffset = this.PageOffset;
			return configObjectCollection;
		}

		public virtual void Sort(string sortProperty, ListSortDirection direction)
		{
			this.SortWithComparer(new ConfigObjectComparer(sortProperty, direction));
		}

		public virtual void Sort(ListSortDescription sort)
		{
			this.SortWithComparer(new ConfigObjectComparer(sort));
		}

		public virtual void Sort(ListSortDescriptionCollection sorts)
		{
			this.SortWithComparer(new ConfigObjectComparer(sorts));
		}

		private void SortWithComparer(ConfigObjectComparer comparer)
		{
			ConfigObject[] array = new ConfigObject[base.Count];
			this.CopyTo(array, 0);
			Array.Sort(array, comparer);
			base.Clear();
			this.AddRange(array);
		}

		private int totalCount;

		private int pageOffset;
	}
}
