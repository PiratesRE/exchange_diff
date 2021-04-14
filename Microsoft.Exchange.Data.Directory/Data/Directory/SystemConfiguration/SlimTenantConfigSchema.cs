using System;
using System.Configuration;
using Microsoft.Exchange.Data.ConfigurationSettings;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal class SlimTenantConfigSchema : ConfigSchemaBase
	{
		public override string Name
		{
			get
			{
				return "SlimTenant";
			}
		}

		public override string SectionName
		{
			get
			{
				return "SlimTenantConfiguration";
			}
		}

		[ConfigurationProperty("IsHydrationAllowed", DefaultValue = true)]
		public bool IsHydrationAllowed
		{
			get
			{
				return (bool)base["IsHydrationAllowed"];
			}
			set
			{
				base["IsHydrationAllowed"] = value;
			}
		}

		protected override bool OnDeserializeUnrecognizedAttribute(string name, string value)
		{
			ExTraceGlobals.SlimTenantTracer.TraceDebug<string, string>(0L, "Unrecognized configuration attribute {0}={1}", name, value);
			return base.OnDeserializeUnrecognizedAttribute(name, value);
		}

		public static class Setting
		{
			public const string IsHydrationAllowed = "IsHydrationAllowed";
		}
	}
}
