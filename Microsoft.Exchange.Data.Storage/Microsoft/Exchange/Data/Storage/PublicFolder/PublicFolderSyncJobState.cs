using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.PublicFolder
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class PublicFolderSyncJobState
	{
		public PublicFolderSyncJobState(PublicFolderSyncJobState.Status status, LocalizedException lastError)
		{
			this.JobStatus = status;
			this.LastError = lastError;
		}

		public PublicFolderSyncJobState.Status JobStatus;

		public LocalizedException LastError;

		public enum Status
		{
			None,
			Queued,
			Completed
		}
	}
}
