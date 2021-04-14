using System;
using System.Configuration;

namespace Microsoft.Exchange.EdgeSync
{
	internal class SyncProviderSection : ConfigurationSection
	{
		static SyncProviderSection()
		{
			SyncProviderSection.syncProviderElementProperty = new ConfigurationProperty("synchronizationProvider", typeof(SyncProviderElementCollection), null, ConfigurationPropertyOptions.IsRequired);
			SyncProviderSection.properties.Add(SyncProviderSection.syncProviderElementProperty);
		}

		public SyncProviderElementCollection SyncProviderElements
		{
			get
			{
				return (SyncProviderElementCollection)base[SyncProviderSection.syncProviderElementProperty];
			}
		}

		protected override ConfigurationPropertyCollection Properties
		{
			get
			{
				return SyncProviderSection.properties;
			}
		}

		private static ConfigurationProperty syncProviderElementProperty;

		private static ConfigurationPropertyCollection properties = new ConfigurationPropertyCollection();
	}
}
