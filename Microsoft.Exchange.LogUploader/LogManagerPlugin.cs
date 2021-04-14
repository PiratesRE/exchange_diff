using System;
using System.Configuration;

namespace Microsoft.Exchange.LogUploader
{
	internal class LogManagerPlugin : ConfigurationElement
	{
		[ConfigurationProperty("Name", IsRequired = true)]
		public string Name
		{
			get
			{
				return (string)base["Name"];
			}
		}

		[ConfigurationProperty("MonitorCreator", IsRequired = true)]
		public string MonitorCreatorClassName
		{
			get
			{
				return (string)base["MonitorCreator"];
			}
		}

		[ConfigurationProperty("Assembly", IsRequired = true)]
		public string AssemblyName
		{
			get
			{
				return (string)base["Assembly"];
			}
		}
	}
}
