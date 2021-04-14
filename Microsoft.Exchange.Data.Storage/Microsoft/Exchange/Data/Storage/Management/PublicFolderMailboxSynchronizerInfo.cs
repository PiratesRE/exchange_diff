using System;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[Serializable]
	public class PublicFolderMailboxSynchronizerInfo : PublicFolderMailboxMonitoringInfo
	{
		public int? NumberOfBatchesExecuted { get; set; }

		public int? NumberOfFoldersToBeSynced { get; set; }

		public int? BatchSize { get; set; }

		public int? NumberOfFoldersSynced { get; set; }

		internal const string LastSyncCycleLogConfigurationName = "PublicFolderLastSyncCylceLog";

		internal const string SyncInfoConfigurationName = "PublicFolderSyncInfo";
	}
}
