using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MigrationJobStage
	{
		static MigrationJobStage()
		{
			int length = Enum.GetValues(typeof(MigrationJobStatus)).Length;
			MigrationJobStage.stageMap = new Dictionary<MigrationJobStatus, MigrationJobStage>();
			MigrationJobStage[] array = new MigrationJobStage[]
			{
				MigrationJobStage.Sync,
				MigrationJobStage.Incremental,
				MigrationJobStage.Completion,
				MigrationJobStage.Completed,
				MigrationJobStage.Dormant,
				MigrationJobStage.Corrupted
			};
			foreach (MigrationJobStage migrationJobStage in array)
			{
				foreach (MigrationJobStatus migrationJobStatus in migrationJobStage.SupportedStatuses)
				{
					if (MigrationJobStage.stageMap.ContainsKey(migrationJobStatus))
					{
						throw new InvalidOperationException("expect each job status to have 1 and exactly 1 stage, but found 2 status:" + migrationJobStatus);
					}
					MigrationJobStage.stageMap[migrationJobStatus] = migrationJobStage;
				}
			}
			if (MigrationJobStage.stageMap.Count != length)
			{
				throw new InvalidOperationException("expect every job status to be accounted for, but missing " + (length - MigrationJobStage.stageMap.Count));
			}
		}

		private MigrationJobStage(string name, MigrationJobStatus[] supportedStatuses)
		{
			this.Name = name;
			this.SupportedStatuses = supportedStatuses;
		}

		public string Name { get; private set; }

		public MigrationJobStatus[] SupportedStatuses { get; private set; }

		public static MigrationJobStage GetStage(MigrationJobStatus status)
		{
			return MigrationJobStage.stageMap[status];
		}

		public bool IsStatusSupported(MigrationJobStatus status)
		{
			foreach (MigrationJobStatus migrationJobStatus in this.SupportedStatuses)
			{
				if (status == migrationJobStatus)
				{
					return true;
				}
			}
			return false;
		}

		public override string ToString()
		{
			return " states:" + string.Join<MigrationJobStatus>(" ", this.SupportedStatuses);
		}

		public static readonly MigrationJobStage Sync = new MigrationJobStage("Sync", new MigrationJobStatus[]
		{
			MigrationJobStatus.SyncInitializing,
			MigrationJobStatus.Validating,
			MigrationJobStatus.ProvisionStarting,
			MigrationJobStatus.SyncStarting,
			MigrationJobStatus.SyncCompleting
		});

		public static readonly MigrationJobStage Incremental = new MigrationJobStage("Incremental", new MigrationJobStatus[]
		{
			MigrationJobStatus.SyncCompleted
		});

		public static readonly MigrationJobStage Completion = new MigrationJobStage("Completion", new MigrationJobStatus[]
		{
			MigrationJobStatus.CompletionInitializing,
			MigrationJobStatus.CompletionStarting,
			MigrationJobStatus.Completing
		});

		public static readonly MigrationJobStage Completed = new MigrationJobStage("Completed", new MigrationJobStatus[]
		{
			MigrationJobStatus.Completed,
			MigrationJobStatus.Removing
		});

		public static readonly MigrationJobStage Dormant = new MigrationJobStage("Dormant", new MigrationJobStatus[]
		{
			MigrationJobStatus.Created,
			MigrationJobStatus.Stopped,
			MigrationJobStatus.Failed,
			MigrationJobStatus.Removed
		});

		public static readonly MigrationJobStage Corrupted = new MigrationJobStage("Corrupted", new MigrationJobStatus[]
		{
			MigrationJobStatus.Corrupted
		});

		private static readonly Dictionary<MigrationJobStatus, MigrationJobStage> stageMap;
	}
}
