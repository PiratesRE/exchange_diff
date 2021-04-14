using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal interface IContentChangesFetcher : IDisposable
	{
		FolderChangesManifest EnumerateContentChanges(SyncContentsManifestState syncState, EnumerateContentChangesFlags flags, int maxChanges);
	}
}
