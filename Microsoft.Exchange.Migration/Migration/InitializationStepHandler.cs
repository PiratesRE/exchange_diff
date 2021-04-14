using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class InitializationStepHandler : IStepHandler
	{
		public InitializationStepHandler(IMigrationDataProvider dataProvider)
		{
			this.DataProvider = dataProvider;
		}

		public bool ExpectMailboxData
		{
			get
			{
				return false;
			}
		}

		private protected IMigrationDataProvider DataProvider { protected get; private set; }

		public IStepSettings Discover(MigrationJobItem jobItem, MailboxData localMailbox)
		{
			return null;
		}

		public void Validate(MigrationJobItem jobItem)
		{
		}

		public IStepSnapshot Inject(MigrationJobItem jobItem)
		{
			return null;
		}

		public IStepSnapshot Process(ISnapshotId id, MigrationJobItem jobItem, out bool updated)
		{
			updated = false;
			string identifier = jobItem.Identifier;
			List<MigrationStep> remainingSteps = jobItem.MigrationJob.Workflow.GetRemainingSteps(jobItem.WorkflowPosition);
			MigrationJobObjectCache migrationJobObjectCache = new MigrationJobObjectCache(this.DataProvider);
			migrationJobObjectCache.PreSeed(jobItem.MigrationJob);
			foreach (MigrationJobItem migrationJobItem in MigrationJobItem.GetByIdentifier(this.DataProvider, null, identifier, migrationJobObjectCache))
			{
				if (!(migrationJobItem.JobItemGuid == jobItem.JobItemGuid) && migrationJobItem.State != MigrationState.Disabled)
				{
					if (jobItem.MigrationJobId.Equals(migrationJobItem.MigrationJobId))
					{
						throw new UserDuplicateInCSVException(identifier);
					}
					if (migrationJobItem.MigrationType == jobItem.MigrationType && migrationJobItem.MigrationJob != null && migrationJobItem.MigrationJob.JobDirection == jobItem.MigrationJob.JobDirection && migrationJobItem.State != MigrationState.Completed && remainingSteps.Intersect(migrationJobItem.MigrationJob.Workflow.GetRemainingSteps(migrationJobItem.WorkflowPosition)).Any<MigrationStep>())
					{
						throw new UserDuplicateInOtherBatchException(identifier, migrationJobItem.MigrationJob.JobName);
					}
				}
			}
			return null;
		}

		public void Start(ISnapshotId id)
		{
		}

		public IStepSnapshot Stop(ISnapshotId id)
		{
			return null;
		}

		public void Delete(ISnapshotId id)
		{
		}

		public bool CanProcess(MigrationJobItem jobItem)
		{
			return true;
		}

		public MigrationUserStatus ResolvePresentationStatus(MigrationFlags flags, IStepSnapshot stepSnapshot = null)
		{
			MigrationUserStatus? migrationUserStatus = MigrationJobItem.ResolveFlagStatus(flags);
			if (migrationUserStatus != null)
			{
				return migrationUserStatus.Value;
			}
			return MigrationUserStatus.Validating;
		}

		public static readonly MigrationStage[] AllowedStages = new MigrationStage[]
		{
			MigrationStage.Processing
		};
	}
}
