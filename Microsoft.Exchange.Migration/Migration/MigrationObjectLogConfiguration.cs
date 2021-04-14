using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Migration
{
	internal abstract class MigrationObjectLogConfiguration : ObjectLogConfiguration
	{
		public override bool IsEnabled
		{
			get
			{
				return ConfigBase<MigrationServiceConfigSchema>.GetConfig<bool>("MigrationReportingLoggingEnabled");
			}
		}

		public override TimeSpan MaxLogAge
		{
			get
			{
				return ConfigBase<MigrationServiceConfigSchema>.GetConfig<TimeSpan>("MigrationReportingMaxLogAge");
			}
		}

		public override string LoggingFolder
		{
			get
			{
				return ConfigBase<MigrationServiceConfigSchema>.GetConfig<string>("MigrationReportingLoggingFolder");
			}
		}
	}
}
