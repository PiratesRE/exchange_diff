using System;
using System.Configuration;

namespace Microsoft.Exchange.Diagnostics.Service.Common
{
	public class ServiceConfiguration : ConfigurationSection
	{
		static ServiceConfiguration()
		{
			ServiceConfiguration.properties.Add(ServiceConfiguration.directories);
		}

		public ServiceConfiguration.DirectoryConfigurationCollection Directories
		{
			get
			{
				return (ServiceConfiguration.DirectoryConfigurationCollection)base[ServiceConfiguration.directories];
			}
		}

		protected override ConfigurationPropertyCollection Properties
		{
			get
			{
				return ServiceConfiguration.properties;
			}
		}

		private static ConfigurationPropertyCollection properties = new ConfigurationPropertyCollection();

		private static ConfigurationProperty directories = new ConfigurationProperty("Directories", typeof(ServiceConfiguration.DirectoryConfigurationCollection), null, ConfigurationPropertyOptions.IsRequired);

		public class DirectoryConfigurationCollection : ConfigurationElementCollection<ServiceConfiguration.DirectoryConfigurationElement>
		{
			public override ConfigurationElementCollectionType CollectionType
			{
				get
				{
					return ConfigurationElementCollectionType.AddRemoveClearMap;
				}
			}

			protected override string ElementName
			{
				get
				{
					return "name";
				}
			}

			protected override object GetElementKey(ConfigurationElement element)
			{
				if (element != null)
				{
					return ((ServiceConfiguration.DirectoryConfigurationElement)element).Name;
				}
				return null;
			}
		}

		public class DirectoryConfigurationElement : ConfigurationElement
		{
			static DirectoryConfigurationElement()
			{
				ServiceConfiguration.DirectoryConfigurationElement.properties.Add(ServiceConfiguration.DirectoryConfigurationElement.name);
				ServiceConfiguration.DirectoryConfigurationElement.properties.Add(ServiceConfiguration.DirectoryConfigurationElement.agent);
				ServiceConfiguration.DirectoryConfigurationElement.properties.Add(ServiceConfiguration.DirectoryConfigurationElement.logDataLoss);
				ServiceConfiguration.DirectoryConfigurationElement.properties.Add(ServiceConfiguration.DirectoryConfigurationElement.maxSize);
				ServiceConfiguration.DirectoryConfigurationElement.properties.Add(ServiceConfiguration.DirectoryConfigurationElement.maxSizeDatacenter);
				ServiceConfiguration.DirectoryConfigurationElement.properties.Add(ServiceConfiguration.DirectoryConfigurationElement.maxAge);
				ServiceConfiguration.DirectoryConfigurationElement.properties.Add(ServiceConfiguration.DirectoryConfigurationElement.checkInterval);
			}

			public string Name
			{
				get
				{
					return (string)base[ServiceConfiguration.DirectoryConfigurationElement.name];
				}
			}

			public string Agent
			{
				get
				{
					return (string)base[ServiceConfiguration.DirectoryConfigurationElement.agent];
				}
			}

			public int MaxSize
			{
				get
				{
					return (int)base[ServiceConfiguration.DirectoryConfigurationElement.maxSize];
				}
			}

			public int MaxSizeDatacenter
			{
				get
				{
					return (int)base[ServiceConfiguration.DirectoryConfigurationElement.maxSizeDatacenter];
				}
			}

			public bool LogDataLoss
			{
				get
				{
					return (bool)base[ServiceConfiguration.DirectoryConfigurationElement.logDataLoss];
				}
			}

			public TimeSpan MaxAge
			{
				get
				{
					return (TimeSpan)base[ServiceConfiguration.DirectoryConfigurationElement.maxAge];
				}
			}

			public TimeSpan CheckInterval
			{
				get
				{
					return (TimeSpan)base[ServiceConfiguration.DirectoryConfigurationElement.checkInterval];
				}
			}

			protected override ConfigurationPropertyCollection Properties
			{
				get
				{
					return ServiceConfiguration.DirectoryConfigurationElement.properties;
				}
			}

			private static ConfigurationProperty name = new ConfigurationProperty("Name", typeof(string), null, ConfigurationPropertyOptions.IsRequired);

			private static ConfigurationProperty agent = new ConfigurationProperty("Agent", typeof(string), null, ConfigurationPropertyOptions.None);

			private static ConfigurationProperty logDataLoss = new ConfigurationProperty("LogDataLoss", typeof(bool), null, ConfigurationPropertyOptions.IsRequired);

			private static ConfigurationProperty maxSize = new ConfigurationProperty("MaxSize", typeof(int), 0, ConfigurationPropertyOptions.IsRequired);

			private static ConfigurationProperty maxSizeDatacenter = new ConfigurationProperty("MaxSizeDatacenter", typeof(int), 0, ConfigurationPropertyOptions.None);

			private static ConfigurationProperty maxAge = new ConfigurationProperty("MaxAge", typeof(TimeSpan), TimeSpan.Zero, ConfigurationPropertyOptions.IsRequired);

			private static ConfigurationProperty checkInterval = new ConfigurationProperty("CheckInterval", typeof(TimeSpan), TimeSpan.Zero, ConfigurationPropertyOptions.IsRequired);

			private static ConfigurationPropertyCollection properties = new ConfigurationPropertyCollection();
		}
	}
}
