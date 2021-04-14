using System;
using System.Configuration;

namespace Microsoft.Exchange.LogUploader
{
	[ConfigurationCollection(typeof(LogManagerPlugin), AddItemName = "Plugin")]
	internal class LogManagerPluginCollection : ConfigurationElementCollection
	{
		public LogManagerPluginCollection() : base(StringComparer.InvariantCultureIgnoreCase)
		{
		}

		public LogManagerPlugin Get(object key)
		{
			return (LogManagerPlugin)base.BaseGet(key);
		}

		protected override ConfigurationElement CreateNewElement()
		{
			return new LogManagerPlugin();
		}

		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((LogManagerPlugin)element).Name;
		}
	}
}
