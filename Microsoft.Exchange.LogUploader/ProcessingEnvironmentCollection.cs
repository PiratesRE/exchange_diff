using System;
using System.Configuration;

namespace Microsoft.Exchange.LogUploader
{
	[ConfigurationCollection(typeof(ProcessingEnvironment), AddItemName = "Environment")]
	internal class ProcessingEnvironmentCollection : ConfigurationElementCollection
	{
		public ProcessingEnvironmentCollection() : base(StringComparer.InvariantCultureIgnoreCase)
		{
		}

		public void Add(ConfigurationElement element)
		{
			this.BaseAdd(element);
		}

		public ProcessingEnvironment Get(object key)
		{
			return (ProcessingEnvironment)base.BaseGet(key);
		}

		protected override ConfigurationElement CreateNewElement()
		{
			return new ProcessingEnvironment();
		}

		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((ProcessingEnvironment)element).Name;
		}
	}
}
