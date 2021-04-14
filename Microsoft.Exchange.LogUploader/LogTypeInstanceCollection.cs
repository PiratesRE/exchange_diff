using System;
using System.Configuration;

namespace Microsoft.Exchange.LogUploader
{
	[ConfigurationCollection(typeof(LogTypeInstance), AddItemName = "Log")]
	internal class LogTypeInstanceCollection : ConfigurationElementCollection
	{
		public void Add(ConfigurationElement element)
		{
			this.BaseAdd(element);
		}

		protected override ConfigurationElement CreateNewElement()
		{
			return new LogTypeInstance();
		}

		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((LogTypeInstance)element).Instance;
		}
	}
}
