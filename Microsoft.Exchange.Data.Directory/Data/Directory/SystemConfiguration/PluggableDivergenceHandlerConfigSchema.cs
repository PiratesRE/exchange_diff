using System;
using System.Configuration;
using Microsoft.Exchange.Data.ConfigurationSettings;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal sealed class PluggableDivergenceHandlerConfigSchema : ConfigSchemaBase
	{
		public override string Name
		{
			get
			{
				return "ProvisioningDivergenceHandler";
			}
		}

		public override string SectionName
		{
			get
			{
				return "ProvisioningDivergenceHandlerConfiguration";
			}
		}

		[ConfigurationProperty("ProvisioningDivergenceHandlerConfig", DefaultValue = "")]
		public string PluggableDivergenceHandlerConfig
		{
			get
			{
				return (string)base["ProvisioningDivergenceHandlerConfig"];
			}
			set
			{
				base["ProvisioningDivergenceHandlerConfig"] = value;
			}
		}

		protected override bool OnDeserializeUnrecognizedAttribute(string name, string value)
		{
			ExTraceGlobals.DirectoryTasksTracer.TraceDebug<string, string>(0L, "Unrecognized configuration attribute {0}={1}", name, value);
			return base.OnDeserializeUnrecognizedAttribute(name, value);
		}

		internal static class Setting
		{
			public const string PluggableDivergenceHandlerConfig = "ProvisioningDivergenceHandlerConfig";
		}
	}
}
