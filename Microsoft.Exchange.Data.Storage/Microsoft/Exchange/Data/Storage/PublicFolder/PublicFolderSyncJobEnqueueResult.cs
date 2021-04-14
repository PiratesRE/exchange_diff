using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net.JobQueues;

namespace Microsoft.Exchange.Data.Storage.PublicFolder
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class PublicFolderSyncJobEnqueueResult : EnqueueResult
	{
		public PublicFolderSyncJobEnqueueResult(EnqueueResultType result, PublicFolderSyncJobState syncJobState) : base(result)
		{
			this.PublicFolderSyncJobState = syncJobState;
			this.Type = QueueType.PublicFolder;
		}

		public PublicFolderSyncJobState PublicFolderSyncJobState;
	}
}
