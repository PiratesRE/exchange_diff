using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net.JobQueues;

namespace Microsoft.Exchange.Data.Storage.LinkedFolder
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class DocumentSyncJobQueue : TeamMailboxSyncJobQueue
	{
		public DocumentSyncJobQueue(TeamMailboxSyncConfiguration config, IResourceMonitorFactory resourceMonitorFactory, IOAuthCredentialFactory oauthCredentialFactory, bool createTeamMailboxSyncInfoCache = true) : base(QueueType.TeamMailboxDocumentSync, "TeamMailboxDocumentLastSyncCycleLog", config, resourceMonitorFactory, oauthCredentialFactory, createTeamMailboxSyncInfoCache)
		{
		}

		protected override Job InternalCreateJob(TeamMailboxSyncInfo info, string clientString, SyncOption syncOption)
		{
			return new DocumentSyncJob(this, this.config, info, clientString, syncOption);
		}
	}
}
