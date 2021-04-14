using System;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class IsInteg
	{
		public static uint[] CorruptionTypesToFix = new uint[]
		{
			7U
		};

		public static readonly PropTag[] JobPropTags = new PropTag[]
		{
			PropTag.IsIntegJobRequestGuid,
			PropTag.IsIntegJobGuid,
			PropTag.IsIntegJobMailboxGuid,
			PropTag.IsIntegJobFlags,
			PropTag.IsIntegJobTask,
			PropTag.IsIntegJobCreationTime,
			PropTag.IsIntegJobState,
			PropTag.IsIntegJobProgress,
			PropTag.IsIntegJobSource,
			PropTag.IsIntegJobPriority,
			PropTag.IsIntegJobTimeInServer,
			PropTag.IsIntegJobFinishTime,
			PropTag.IsIntegJobLastExecutionTime,
			PropTag.IsIntegJobCorruptionsDetected,
			PropTag.IsIntegJobCorruptionsFixed,
			PropTag.IsIntegJobCorruptions,
			PropTag.RtfSyncTrailingCount
		};

		public enum StoreIntegrityCheckOperation
		{
			NewJob = 1,
			GetJob,
			RemoveJob,
			GetJobDetails
		}

		[Flags]
		public enum StoreIntegrityCheckRequestFlags : uint
		{
			None = 0U,
			DetectOnly = 1U,
			Force = 2U,
			SystemJob = 4U,
			Verbose = 2147483648U
		}

		[Flags]
		public enum IntegrityCheckQueryFlags : uint
		{
			None = 0U,
			QueryJob = 4U
		}

		public enum MailboxCorruptionType
		{
			None,
			SearchFolder,
			FolderView,
			AggregateCounts,
			ProvisionedFolder,
			ReplState,
			MessagePtagCn,
			MessageId,
			RuleMessageClass = 100,
			RestrictionFolder,
			FolderACL,
			UniqueMidIndex,
			CorruptJunkRule,
			MissingSpecialFolders,
			DropAllLazyIndexes,
			LockedMoveTarget = 4096,
			Extension1 = 32768,
			Extension2,
			Extension3,
			Extension4,
			Extension5
		}

		public enum JobSource : short
		{
			OnDemand,
			Maintenance
		}

		public enum JobPriority : short
		{
			High,
			Normal,
			Low
		}

		public enum JobState
		{
			Initializing,
			Queued,
			Running,
			Succeeded,
			Failed
		}

		[Flags]
		public enum JobFlags : uint
		{
			None = 0U,
			DetectOnly = 1U,
			Background = 2U,
			OnDemand = 4U,
			System = 8U,
			Force = 16U,
			Verbose = 2147483648U
		}

		public enum IsIntegOps
		{
			None,
			CreateTask,
			QueryTask
		}
	}
}
