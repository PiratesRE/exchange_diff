using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal interface IHierarchyChangesFetcher : IDisposable
	{
		MailboxChangesManifest EnumerateHierarchyChanges(SyncHierarchyManifestState hierState, EnumerateHierarchyChangesFlags flags, int maxChanges);
	}
}
