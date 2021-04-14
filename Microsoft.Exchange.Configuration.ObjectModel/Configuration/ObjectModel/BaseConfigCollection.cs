using System;
using System.Collections;
using System.Data;

namespace Microsoft.Exchange.Configuration.ObjectModel
{
	internal abstract class BaseConfigCollection : CollectionBase
	{
		public BaseConfigCollection()
		{
		}

		public BaseConfigCollection(ConfigObject[] configObjectArray)
		{
			if (configObjectArray != null)
			{
				this.AddRange(configObjectArray);
			}
		}

		public virtual bool IsReadOnly
		{
			get
			{
				return this.isReadOnly;
			}
			set
			{
				this.isReadOnly = value;
			}
		}

		public virtual int Add(ConfigObject configObject)
		{
			if (this.isReadOnly)
			{
				throw new ReadOnlyException();
			}
			return base.List.Add(configObject);
		}

		public virtual void AddRange(ConfigObject[] configObjectArray)
		{
			if (this.isReadOnly)
			{
				throw new ReadOnlyException();
			}
			base.InnerList.AddRange(configObjectArray);
		}

		public virtual void Insert(int index, ConfigObject configObject)
		{
			if (this.isReadOnly)
			{
				throw new ReadOnlyException();
			}
			base.List.Insert(index, configObject);
		}

		public virtual void Replace(int index, ConfigObject configObject)
		{
			if (this.isReadOnly)
			{
				throw new ReadOnlyException();
			}
			base.List[index] = configObject;
		}

		public virtual void Remove(ConfigObject configObject)
		{
			if (this.isReadOnly)
			{
				throw new ReadOnlyException();
			}
			base.List.Remove(configObject);
		}

		public virtual void RemoveRange(int index, int count)
		{
			if (this.isReadOnly)
			{
				throw new ReadOnlyException();
			}
			base.InnerList.RemoveRange(index, count);
		}

		public virtual void CopyTo(ConfigObject[] configObjectArray, int index)
		{
			base.List.CopyTo(configObjectArray, index);
		}

		public virtual bool Contains(ConfigObject configObject)
		{
			return base.List.Contains(configObject);
		}

		public virtual bool ContainsIdentity(string identity)
		{
			return -1 != this.IndexOfIdentity(identity);
		}

		public virtual int IndexOf(ConfigObject configObject)
		{
			return base.List.IndexOf(configObject);
		}

		public virtual int IndexOfIdentity(string identity)
		{
			for (int i = 0; i < base.List.Count; i++)
			{
				if (((ConfigObject)base.List[i]).Identity == identity)
				{
					return i;
				}
			}
			return -1;
		}

		private bool isReadOnly;
	}
}
