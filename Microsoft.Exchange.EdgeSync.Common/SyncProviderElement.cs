using System;
using System.Configuration;

namespace Microsoft.Exchange.EdgeSync
{
	internal class SyncProviderElement : ConfigurationElement
	{
		static SyncProviderElement()
		{
			SyncProviderElement.properties.Add(SyncProviderElement.propName);
			SyncProviderElement.properties.Add(SyncProviderElement.propSynchronizationProvider);
			SyncProviderElement.properties.Add(SyncProviderElement.propAssemblyPath);
			SyncProviderElement.properties.Add(SyncProviderElement.propEnabled);
		}

		[ConfigurationProperty("name", IsRequired = true)]
		public string Name
		{
			get
			{
				return (string)base[SyncProviderElement.propName];
			}
			set
			{
				base[SyncProviderElement.propName] = value;
			}
		}

		[ConfigurationProperty("synchronizationProvider")]
		public string SynchronizationProvider
		{
			get
			{
				return (string)base[SyncProviderElement.propSynchronizationProvider];
			}
			set
			{
				base[SyncProviderElement.propSynchronizationProvider] = value;
			}
		}

		[ConfigurationProperty("assemblyPath")]
		public string AssemblyPath
		{
			get
			{
				return (string)base[SyncProviderElement.propAssemblyPath];
			}
			set
			{
				base[SyncProviderElement.propAssemblyPath] = value;
			}
		}

		[ConfigurationProperty("enabled")]
		public bool Enabled
		{
			get
			{
				return (bool)base[SyncProviderElement.propEnabled];
			}
			set
			{
				base[SyncProviderElement.propEnabled] = value;
			}
		}

		protected override ConfigurationPropertyCollection Properties
		{
			get
			{
				return SyncProviderElement.properties;
			}
		}

		private static readonly ConfigurationProperty propName = new ConfigurationProperty("name", typeof(string), null, ConfigurationPropertyOptions.IsRequired);

		private static readonly ConfigurationProperty propSynchronizationProvider = new ConfigurationProperty("synchronizationProvider", typeof(string), null, ConfigurationPropertyOptions.None);

		private static readonly ConfigurationProperty propAssemblyPath = new ConfigurationProperty("assemblyPath", typeof(string), null, ConfigurationPropertyOptions.None);

		private static readonly ConfigurationProperty propEnabled = new ConfigurationProperty("enabled", typeof(bool), false, ConfigurationPropertyOptions.None);

		private static readonly ConfigurationPropertyCollection properties = new ConfigurationPropertyCollection();
	}
}
