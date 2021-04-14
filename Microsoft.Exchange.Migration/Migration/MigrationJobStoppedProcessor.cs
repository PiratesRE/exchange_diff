using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class MigrationJobStoppedProcessor : JobProcessor
	{
		internal static MigrationJobStoppedProcessor CreateProcessor(MigrationType type)
		{
			return new MigrationJobStoppedProcessor();
		}

		internal override bool Validate()
		{
			return base.Job.Status == MigrationJobStatus.Stopped;
		}

		internal override MigrationJobStatus GetNextStageStatus()
		{
			return MigrationJobStatus.Removing;
		}

		protected override LegacyMigrationJobProcessorResponse Process(bool scheduleNewWork)
		{
			ExDateTime utcNow = ExDateTime.UtcNow;
			if (utcNow < base.Job.StateLastUpdated)
			{
				throw new MigrationItemLastUpdatedInTheFutureTransientException(base.Job.StateLastUpdated.Value.ToLongDateString());
			}
			TimeSpan config = ConfigBase<MigrationServiceConfigSchema>.GetConfig<TimeSpan>("MigrationJobStoppedThreshold");
			if (utcNow - base.Job.StateLastUpdated > config)
			{
				return LegacyMigrationJobProcessorResponse.Create(MigrationProcessorResult.Completed, null);
			}
			return LegacyMigrationJobProcessorResponse.Create(MigrationProcessorResult.Waiting, config - (utcNow - base.Job.StateLastUpdated));
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<MigrationJobStoppedProcessor>(this);
		}
	}
}
