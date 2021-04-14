using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;
using Microsoft.Exchange.Migration;
using Microsoft.Exchange.Migration.Logging;
using Microsoft.Exchange.ServiceHost;
using Microsoft.Exchange.Servicelets.Provisioning;

namespace Microsoft.Exchange.Servicelets.SyncMigration
{
	public class SyncMigrationServicelet : Servicelet
	{
		public override void Work()
		{
			MigrationLogger.Initialize();
			MigrationLogContext.Current.Source = "MigrationServicelet";
			if (!ConfigBase<MigrationServiceConfigSchema>.GetConfig<bool>("SyncMigrationIsEnabled"))
			{
				MigrationLogger.Log(MigrationEventType.Warning, "SyncMigrationServicelet is not enabled.", new object[0]);
				base.StopEvent.WaitOne();
				MigrationLogger.Close();
				return;
			}
			MigrationLogger.Log(MigrationEventType.Information, "SyncMigrationServicelet started.", new object[0]);
			MigrationApplication migrationApplication = new MigrationApplication(base.StopEvent, ProvisioningServicelet.Instance);
			migrationApplication.Process();
			MigrationLogger.Log(MigrationEventType.Information, "SyncMigrationServicelet stopped.", new object[0]);
			MigrationLogger.Close();
		}

		private const string LogSource = "MigrationServicelet";
	}
}
