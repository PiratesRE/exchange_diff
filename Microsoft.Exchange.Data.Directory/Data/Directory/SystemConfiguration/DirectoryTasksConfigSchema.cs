using System;
using System.Configuration;
using Microsoft.Exchange.Data.ConfigurationSettings;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal class DirectoryTasksConfigSchema : ConfigSchemaBase
	{
		public override string Name
		{
			get
			{
				return "DirectoryTasks";
			}
		}

		public override string SectionName
		{
			get
			{
				return "DirectoryTasksConfiguration";
			}
		}

		[ConfigurationProperty("IsDirectoryTaskProcessingEnabled", DefaultValue = true)]
		public bool IsDirectoryTaskProcessingEnabled
		{
			get
			{
				return (bool)base["IsDirectoryTaskProcessingEnabled"];
			}
			set
			{
				base["IsDirectoryTaskProcessingEnabled"] = value;
			}
		}

		[ConfigurationProperty("MaxConcurrentNonRecurringTasks", DefaultValue = "1")]
		public uint MaxConcurrentNonRecurringTasks
		{
			get
			{
				return (uint)base["MaxConcurrentNonRecurringTasks"];
			}
			set
			{
				base["MaxConcurrentNonRecurringTasks"] = value;
			}
		}

		[ConfigurationProperty("OffersRequiringSCT", DefaultValue = new string[]
		{
			"BPOS_L",
			"BPOS_M",
			"BPOS_S",
			"BPOS_Basic_CustomDomain"
		})]
		public string[] OffersRequiringSCT
		{
			get
			{
				return (string[])base["OffersRequiringSCT"];
			}
			set
			{
				base["OffersRequiringSCT"] = value;
			}
		}

		[ConfigurationProperty("DelayBetweenSCTChecksInMinutes", DefaultValue = "60")]
		public uint DelayBetweenSCTChecksInMinutes
		{
			get
			{
				return (uint)base["DelayBetweenSCTChecksInMinutes"];
			}
			set
			{
				base["DelayBetweenSCTChecksInMinutes"] = value;
			}
		}

		[ConfigurationProperty("SCTTaskMaxRandomStartDelayInMinutes", DefaultValue = "15")]
		public uint SCTTaskMaxStartDelayInMinutes
		{
			get
			{
				return (uint)base["SCTTaskMaxRandomStartDelayInMinutes"];
			}
			set
			{
				base["SCTTaskMaxRandomStartDelayInMinutes"] = value;
			}
		}

		[ConfigurationProperty("SCTCreateNumberOfRetries", DefaultValue = "3")]
		public uint SCTCreateNumberOfRetries
		{
			get
			{
				return (uint)base["SCTCreateNumberOfRetries"];
			}
			set
			{
				base["SCTCreateNumberOfRetries"] = value;
			}
		}

		[ConfigurationProperty("SCTCreateDelayBetweenRetriesInSeconds", DefaultValue = "10")]
		public uint SCTCreateDelayBetweenRetriesInSeconds
		{
			get
			{
				return (uint)base["SCTCreateDelayBetweenRetriesInSeconds"];
			}
			set
			{
				base["SCTCreateDelayBetweenRetriesInSeconds"] = value;
			}
		}

		[ConfigurationProperty("SCTCreateUseADHealthMonitor", DefaultValue = false)]
		public bool SCTCreateUseADHealthMonitor
		{
			get
			{
				return (bool)base["SCTCreateUseADHealthMonitor"];
			}
			set
			{
				base["SCTCreateUseADHealthMonitor"] = value;
			}
		}

		protected override bool OnDeserializeUnrecognizedAttribute(string name, string value)
		{
			ExTraceGlobals.DirectoryTasksTracer.TraceDebug<string, string>(0L, "Unrecognized configuration attribute {0}={1}", name, value);
			return base.OnDeserializeUnrecognizedAttribute(name, value);
		}

		public static class Setting
		{
			public const string IsDirectoryTaskProcessingEnabled = "IsDirectoryTaskProcessingEnabled";

			public const string MaxConcurrentNonRecurringTasks = "MaxConcurrentNonRecurringTasks";

			public const string OffersRequiringSCT = "OffersRequiringSCT";

			public const string DelayBetweenSCTChecksInMinutes = "DelayBetweenSCTChecksInMinutes";

			public const string SCTTaskMaxStartDelayInMinutes = "SCTTaskMaxRandomStartDelayInMinutes";

			public const string SCTCreateNumberOfRetries = "SCTCreateNumberOfRetries";

			public const string SCTCreateDelayBetweenRetriesInSeconds = "SCTCreateDelayBetweenRetriesInSeconds";

			public const string SCTCreateUseADHealthMonitor = "SCTCreateUseADHealthMonitor";
		}
	}
}
