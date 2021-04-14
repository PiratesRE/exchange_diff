using System;
using System.Configuration;

namespace Microsoft.Exchange.Diagnostics.Service.Common
{
	public abstract class ConfigurationElementCollection<T> : ConfigurationElementCollection where T : ConfigurationElement, new()
	{
		public ConfigurationElementCollection()
		{
		}

		public abstract override ConfigurationElementCollectionType CollectionType { get; }

		protected abstract override string ElementName { get; }

		public T this[int index]
		{
			get
			{
				return (T)((object)base.BaseGet(index));
			}
		}

		protected override ConfigurationElement CreateNewElement()
		{
			return Activator.CreateInstance<T>();
		}

		protected override object GetElementKey(ConfigurationElement element)
		{
			return this.GetElementKey((T)((object)element));
		}
	}
}
