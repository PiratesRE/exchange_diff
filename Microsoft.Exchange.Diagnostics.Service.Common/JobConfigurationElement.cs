using System;
using System.Configuration;

namespace Microsoft.Exchange.Diagnostics.Service.Common
{
	public class JobConfigurationElement : ConfigurationElement
	{
		static JobConfigurationElement()
		{
			JobConfigurationElement.enabled = new ConfigurationProperty("Enabled", typeof(bool), true, ConfigurationPropertyOptions.IsRequired);
			JobConfigurationElement.className = new ConfigurationProperty("Class", typeof(string), null, ConfigurationPropertyOptions.IsRequired);
			JobConfigurationElement.role = new ConfigurationProperty("Role", typeof(string), null, ConfigurationPropertyOptions.IsRequired);
			JobConfigurationElement.datacenter = new ConfigurationProperty("Datacenter", typeof(bool), false, ConfigurationPropertyOptions.None);
			JobConfigurationElement.outputStream = new ConfigurationProperty("OutputStream", typeof(string), null, ConfigurationPropertyOptions.IsRequired);
			JobConfigurationElement.analyzers = new ConfigurationProperty("Analyzers", typeof(AnalyzerConfigurationCollection), null, ConfigurationPropertyOptions.None);
			JobConfigurationElement.properties = new ConfigurationPropertyCollection();
			JobConfigurationElement.properties.Add(JobConfigurationElement.name);
			JobConfigurationElement.properties.Add(JobConfigurationElement.assembly);
			JobConfigurationElement.properties.Add(JobConfigurationElement.method);
			JobConfigurationElement.properties.Add(JobConfigurationElement.enabled);
			JobConfigurationElement.properties.Add(JobConfigurationElement.className);
			JobConfigurationElement.properties.Add(JobConfigurationElement.role);
			JobConfigurationElement.properties.Add(JobConfigurationElement.datacenter);
			JobConfigurationElement.properties.Add(JobConfigurationElement.outputStream);
			JobConfigurationElement.properties.Add(JobConfigurationElement.analyzers);
		}

		public string Name
		{
			get
			{
				return (string)base[JobConfigurationElement.name];
			}
		}

		public string Assembly
		{
			get
			{
				return (string)base[JobConfigurationElement.assembly];
			}
		}

		public string Method
		{
			get
			{
				return (string)base[JobConfigurationElement.method];
			}
		}

		public bool Enabled
		{
			get
			{
				return (bool)base[JobConfigurationElement.enabled];
			}
		}

		public string Class
		{
			get
			{
				return (string)base[JobConfigurationElement.className];
			}
		}

		public string Role
		{
			get
			{
				return (string)base[JobConfigurationElement.role];
			}
		}

		public bool Datacenter
		{
			get
			{
				return (bool)base[JobConfigurationElement.datacenter];
			}
		}

		public string OutputStream
		{
			get
			{
				return (string)base[JobConfigurationElement.outputStream];
			}
		}

		public AnalyzerConfigurationCollection Analyzers
		{
			get
			{
				return (AnalyzerConfigurationCollection)base[JobConfigurationElement.analyzers];
			}
		}

		protected override ConfigurationPropertyCollection Properties
		{
			get
			{
				return JobConfigurationElement.properties;
			}
		}

		private static ConfigurationProperty name = new ConfigurationProperty("Name", typeof(string), null, ConfigurationPropertyOptions.IsRequired);

		private static ConfigurationProperty assembly = new ConfigurationProperty("Assembly", typeof(string), null, ConfigurationPropertyOptions.IsRequired);

		private static ConfigurationProperty enabled;

		private static ConfigurationProperty className;

		private static ConfigurationProperty method = new ConfigurationProperty("Method", typeof(string), null, ConfigurationPropertyOptions.IsRequired);

		private static ConfigurationProperty role;

		private static ConfigurationProperty datacenter;

		private static ConfigurationProperty outputStream;

		private static ConfigurationPropertyCollection properties;

		private static ConfigurationProperty analyzers;
	}
}
