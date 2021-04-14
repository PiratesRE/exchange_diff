using System;
using Microsoft.Exchange.Rpc.Cluster;
using Microsoft.Exchange.Search.OperatorSchema;

namespace Microsoft.Exchange.Search.Core.Abstraction
{
	internal interface IIndexStatusStore
	{
		IndexStatus SetIndexStatus(Guid databaseGuid, int mailboxToCrawl, VersionInfo version);

		IndexStatus SetIndexStatus(Guid databaseGuid, ContentIndexStatusType indexingState, IndexStatusErrorCode errorCode, VersionInfo version, string seedingSource);

		IndexStatus GetIndexStatus(Guid databaseGuid);

		void UpdateIndexStatus(Guid databaseGuid, IndexStatusIndex indexStatusIndex, long value);
	}
}
