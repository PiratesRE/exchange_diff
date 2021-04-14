using System;
using System.Configuration;

namespace Microsoft.Exchange.Diagnostics.Service.Common
{
	public class AnalyzerConfigurationElement : ConfigurationElement
	{
		static AnalyzerConfigurationElement()
		{
			AnalyzerConfigurationElement.properties.Add(AnalyzerConfigurationElement.name);
			AnalyzerConfigurationElement.properties.Add(AnalyzerConfigurationElement.role);
			AnalyzerConfigurationElement.properties.Add(AnalyzerConfigurationElement.datacenter);
			AnalyzerConfigurationElement.properties.Add(AnalyzerConfigurationElement.enabled);
			AnalyzerConfigurationElement.properties.Add(AnalyzerConfigurationElement.assembly);
			AnalyzerConfigurationElement.properties.Add(AnalyzerConfigurationElement.outputFormat);
		}

		public string Name
		{
			get
			{
				return (string)base[AnalyzerConfigurationElement.name];
			}
		}

		public string Role
		{
			get
			{
				return (string)base[AnalyzerConfigurationElement.role];
			}
		}

		public bool Datacenter
		{
			get
			{
				return (bool)base[AnalyzerConfigurationElement.datacenter];
			}
		}

		public bool Enabled
		{
			get
			{
				return (bool)base[AnalyzerConfigurationElement.enabled];
			}
		}

		public string Assembly
		{
			get
			{
				return (string)base[AnalyzerConfigurationElement.assembly];
			}
		}

		public string OutputFormat
		{
			get
			{
				return (string)base[AnalyzerConfigurationElement.outputFormat];
			}
		}

		protected override ConfigurationPropertyCollection Properties
		{
			get
			{
				return AnalyzerConfigurationElement.properties;
			}
		}

		private static ConfigurationProperty name = new ConfigurationProperty("Name", typeof(string), null, ConfigurationPropertyOptions.IsRequired);

		private static ConfigurationProperty role = new ConfigurationProperty("Role", typeof(string), null, ConfigurationPropertyOptions.IsRequired);

		private static ConfigurationProperty datacenter = new ConfigurationProperty("Datacenter", typeof(bool), false, ConfigurationPropertyOptions.None);

		private static ConfigurationProperty enabled = new ConfigurationProperty("Enabled", typeof(bool), true, ConfigurationPropertyOptions.IsRequired);

		private static ConfigurationProperty assembly = new ConfigurationProperty("Assembly", typeof(string), null, ConfigurationPropertyOptions.IsRequired);

		private static ConfigurationProperty outputFormat = new ConfigurationProperty("OutputFormat", typeof(string), null, ConfigurationPropertyOptions.None);

		private static ConfigurationPropertyCollection properties = new ConfigurationPropertyCollection();
	}
}
