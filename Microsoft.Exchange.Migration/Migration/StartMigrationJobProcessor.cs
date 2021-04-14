using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;

namespace Microsoft.Exchange.Migration
{
	internal class StartMigrationJobProcessor : MigrationJobProcessorBase
	{
		public StartMigrationJobProcessor(MigrationJob migrationObject, IMigrationDataProvider dataProvider) : base(migrationObject, dataProvider)
		{
		}

		protected override Func<int?, IEnumerable<StoreObjectId>>[] ProcessableChildObjectQueries
		{
			get
			{
				return new Func<int?, IEnumerable<StoreObjectId>>[]
				{
					(int? maxCount) => MigrationJobItem.GetIdsByFlag(this.DataProvider, this.MigrationObject, MigrationFlags.Remove, null, maxCount),
					(int? maxCount) => MigrationJobItem.GetIdsByState(this.DataProvider, this.MigrationObject, MigrationState.Failed, null, maxCount),
					(int? maxCount) => MigrationJobItem.GetIdsByState(this.DataProvider, this.MigrationObject, MigrationState.Stopped, null, maxCount),
					(int? maxCount) => MigrationJobItem.GetIdsByFlag(this.DataProvider, this.MigrationObject, MigrationFlags.Stop, null, maxCount),
					(int? maxCount) => MigrationJobItem.GetIdsByFlag(this.DataProvider, this.MigrationObject, MigrationFlags.Remove, new MigrationState?(MigrationState.Disabled), maxCount)
				};
			}
		}

		protected override MigrationProcessorResponse ProcessChild(MigrationJobItem child)
		{
			MigrationJobItemProcessorBase migrationJobItemProcessorBase = null;
			if (child.Flags.HasFlag(MigrationFlags.Remove))
			{
				migrationJobItemProcessorBase = new RemoveMigrationJobItemProcessor(child, this.DataProvider);
			}
			if (migrationJobItemProcessorBase != null)
			{
				return migrationJobItemProcessorBase.Process();
			}
			return MigrationJobItemProcessorBase.SetFlags(this.DataProvider, child, (child.Flags | MigrationFlags.Start) & ~MigrationFlags.Stop, MigrationProcessorResult.Completed);
		}

		protected override MigrationJobProcessorResponse ProcessObject()
		{
			return MigrationJobProcessorResponse.Create(MigrationProcessorResult.Completed, null, null, null, null, null);
		}

		protected override MigrationJobProcessorResponse ApplyResponse(MigrationJobProcessorResponse response)
		{
			if (response.Result == MigrationProcessorResult.Completed)
			{
				MigrationJob migrationObject = this.MigrationObject;
				IMigrationDataProvider dataProvider = this.DataProvider;
				MigrationJobStatus status = MigrationJobStatus.SyncStarting;
				MigrationState state = MigrationState.Active;
				MigrationStage? stage = new MigrationStage?(MigrationStage.Discovery);
				migrationObject.SetStatus(dataProvider, status, state, new MigrationFlags?(this.MigrationObject.Flags & ~MigrationFlags.Start), stage, null, null, null, null, response.ChildStatusChanges, true, null, response.ProcessingDuration);
				return MigrationJobProcessorResponse.Create(MigrationProcessorResult.Working, null, null, null, null, null);
			}
			return base.ApplyResponse(response);
		}
	}
}
