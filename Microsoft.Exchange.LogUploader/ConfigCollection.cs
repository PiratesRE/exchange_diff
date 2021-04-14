using System;
using System.Configuration;

namespace Microsoft.Exchange.LogUploader
{
	[ConfigurationCollection(typeof(ConfigInstance), AddItemName = "Config")]
	internal class ConfigCollection : ConfigurationElementCollection
	{
		public ConfigCollection() : base(StringComparer.InvariantCultureIgnoreCase)
		{
		}

		public void Add(ConfigurationElement element)
		{
			this.BaseAdd(element);
		}

		public ConfigInstance Get(object key)
		{
			return (ConfigInstance)base.BaseGet(key);
		}

		protected override ConfigurationElement CreateNewElement()
		{
			return new ConfigInstance();
		}

		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((ConfigInstance)element).Name;
		}
	}
}
