using System;
using System.Configuration;

namespace Microsoft.Exchange.EdgeSync
{
	[ConfigurationCollection(typeof(SyncProviderElement), CollectionType = ConfigurationElementCollectionType.AddRemoveClearMap)]
	internal class SyncProviderElementCollection : ConfigurationElementCollection
	{
		public override ConfigurationElementCollectionType CollectionType
		{
			get
			{
				return ConfigurationElementCollectionType.AddRemoveClearMap;
			}
		}

		protected override ConfigurationPropertyCollection Properties
		{
			get
			{
				return SyncProviderElementCollection.properties;
			}
		}

		public void Add(ConfigurationElement element)
		{
			this.BaseAdd(element);
		}

		public void Clear()
		{
			base.BaseClear();
		}

		protected override ConfigurationElement CreateNewElement()
		{
			return new SyncProviderElement();
		}

		protected override object GetElementKey(ConfigurationElement element)
		{
			return (element as SyncProviderElement).Name;
		}

		private static readonly ConfigurationPropertyCollection properties = new ConfigurationPropertyCollection();
	}
}
