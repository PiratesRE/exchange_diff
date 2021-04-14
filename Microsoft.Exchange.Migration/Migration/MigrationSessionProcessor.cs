using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Migration
{
	internal class MigrationSessionProcessor : MigrationHierarchyProcessorBase<MigrationSession, MigrationJob, StoreObjectId, MigrationProcessorResponse>
	{
		public MigrationSessionProcessor(MigrationSession migrationObject, IMigrationDataProvider dataProvider) : base(migrationObject, dataProvider)
		{
		}

		protected override MigrationProcessorResponse DefaultCorruptedChildResponse
		{
			get
			{
				return MigrationProcessorResponse.Create(MigrationProcessorResult.Failed, null, null);
			}
		}

		protected override Func<int?, IEnumerable<StoreObjectId>>[] ProcessableChildObjectQueries
		{
			get
			{
				return this.GetChildObjectQueries(true);
			}
		}

		protected override int? MaxChildObjectsToProcessCount
		{
			get
			{
				return new int?(ConfigBase<MigrationServiceConfigSchema>.GetConfig<int>("ProcessingSessionSize"));
			}
		}

		protected override MigrationProcessorResponse PerformPoisonDetection()
		{
			return MigrationProcessorResponse.Create(MigrationProcessorResult.Working, null, null);
		}

		protected override bool TryLoad(StoreObjectId childId, out MigrationJob child)
		{
			return MigrationJob.TryLoad(this.DataProvider, childId, out child);
		}

		protected override MigrationProcessorResponse ProcessChild(MigrationJob child)
		{
			MigrationJobProcessorBase migrationJobProcessorBase = null;
			if (child.Flags.HasFlag(MigrationFlags.Remove))
			{
				migrationJobProcessorBase = new RemoveMigrationJobProcessor(child, this.DataProvider);
			}
			else if (child.Flags.HasFlag(MigrationFlags.Stop))
			{
				migrationJobProcessorBase = new StopMigrationJobProcessor(child, this.DataProvider);
			}
			else if (child.Flags.HasFlag(MigrationFlags.Start))
			{
				migrationJobProcessorBase = new StartMigrationJobProcessor(child, this.DataProvider);
			}
			else if (child.ShouldReport)
			{
				migrationJobProcessorBase = new ReportMigrationJobProcessor(child, this.DataProvider);
			}
			else if (child.Flags == MigrationFlags.None && (child.State == MigrationState.Active || (child.State == MigrationState.Waiting && child.NextProcessTime <= ExDateTime.UtcNow)))
			{
				migrationJobProcessorBase = new ActiveMigrationJobProcessor(child, this.DataProvider);
			}
			if (migrationJobProcessorBase != null)
			{
				return migrationJobProcessorBase.Process();
			}
			return MigrationProcessorResponse.CreateWaitingMax();
		}

		protected override void SetContext()
		{
		}

		protected override void RestoreContext()
		{
		}

		protected override MigrationProcessorResponse ProcessObject()
		{
			IUpgradeConstraintAdapter upgradeConstraintAdapter = MigrationServiceFactory.Instance.GetUpgradeConstraintAdapter(this.MigrationObject);
			upgradeConstraintAdapter.AddUpgradeConstraintIfNeeded(this.DataProvider, this.MigrationObject);
			return MigrationProcessorResponse.Create(MigrationProcessorResult.Deleted, null, null);
		}

		protected override MigrationProcessorResponse ApplyResponse(MigrationProcessorResponse response)
		{
			if (response.Result == MigrationProcessorResult.Completed)
			{
				response.Result = MigrationProcessorResult.Working;
			}
			if (response.Result == MigrationProcessorResult.Deleted && this.GetChildObjectIds(this.GetChildObjectQueries(false), new int?(1)).Any<StoreObjectId>())
			{
				response.Result = MigrationProcessorResult.Waiting;
			}
			return response;
		}

		protected override MigrationProcessorResponse HandlePermanentException(LocalizedException ex)
		{
			return MigrationProcessorResponse.Create(MigrationProcessorResult.Failed, null, ex);
		}

		protected override MigrationProcessorResponse HandleTransientException(LocalizedException ex)
		{
			return MigrationProcessorResponse.Create(MigrationProcessorResult.Failed, null, ex);
		}

		private Func<int?, IEnumerable<StoreObjectId>>[] GetChildObjectQueries(bool processable)
		{
			ExDateTime? nextProcessTime = processable ? new ExDateTime?(ExDateTime.UtcNow) : null;
			return new Func<int?, IEnumerable<StoreObjectId>>[]
			{
				(int? maxCount) => MigrationJob.GetIdsWithFlagPresence(this.DataProvider, true, maxCount),
				(int? maxCount) => MigrationJob.GetIdsByState(this.DataProvider, MigrationState.Active, null, maxCount),
				(int? maxCount) => MigrationJob.GetIdsByState(this.DataProvider, MigrationState.Waiting, nextProcessTime, maxCount)
			};
		}
	}
}
