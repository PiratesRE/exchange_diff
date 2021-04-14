using System;
using System.Configuration;
using Microsoft.Exchange.Data.ConfigurationSettings;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal sealed class ComplianceConfigSchema : ConfigSchemaBase
	{
		public override string Name
		{
			get
			{
				return "Compliance";
			}
		}

		public override string SectionName
		{
			get
			{
				return "ComplianceConfiguration";
			}
		}

		[ConfigurationProperty("JournalArchivingHardeningEnabled", DefaultValue = true)]
		public bool JournalArchivingHardeningEnabled
		{
			get
			{
				return (bool)base["JournalArchivingHardeningEnabled"];
			}
			set
			{
				base["JournalArchivingHardeningEnabled"] = value;
			}
		}

		[ConfigurationProperty("ArchivePropertiesHardeningEnabled", DefaultValue = true)]
		public bool ArchivePropertiesHardeningEnabled
		{
			get
			{
				return (bool)base["ArchivePropertiesHardeningEnabled"];
			}
			set
			{
				base["ArchivePropertiesHardeningEnabled"] = value;
			}
		}

		protected override bool OnDeserializeUnrecognizedAttribute(string name, string value)
		{
			ExTraceGlobals.ComplianceTracer.TraceDebug<string, string>(0L, "Unrecognized configuration attribute {0}={1}", name, value);
			return base.OnDeserializeUnrecognizedAttribute(name, value);
		}

		internal static class Setting
		{
			public const string JournalArchivingHardeningEnabled = "JournalArchivingHardeningEnabled";

			public const string ArchivePropertiesHardeningEnabled = "ArchivePropertiesHardeningEnabled";
		}
	}
}
