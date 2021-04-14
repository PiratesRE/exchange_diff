using System;
using Microsoft.Exchange.Diagnostics.Components.Search;
using Microsoft.Exchange.Rpc.Cluster;
using Microsoft.Exchange.Search.Core.Abstraction;
using Microsoft.Exchange.Search.Core.Common;
using Microsoft.Exchange.Search.Core.Diagnostics;

namespace Microsoft.Exchange.Search.Fast
{
	internal sealed class FastIndexStatus
	{
		internal FastIndexStatus(string indexName, IIndexManager indexManager)
		{
			Util.ThrowOnNullOrEmptyArgument(indexName, "indexName");
			Util.ThrowOnNullArgument(indexManager, "indexManager");
			this.diagnosticsSession = DiagnosticsSession.CreateComponentDiagnosticsSession("FastIndexStatus", ComponentInstance.Globals.Search.ServiceName, ExTraceGlobals.IndexStatusProviderTracer, (long)this.GetHashCode());
			this.indexName = indexName;
			this.indexManager = indexManager;
		}

		internal ContentIndexStatusType GetIndexStatus(out IndexStatusErrorCode errorCode, out string seedingSource, out int? failureCode, out string failureReason)
		{
			errorCode = IndexStatusErrorCode.Unknown;
			ContentIndexStatusType contentIndexStatusType = ContentIndexStatusType.Unknown;
			seedingSource = null;
			failureCode = null;
			failureReason = null;
			try
			{
				CatalogState catalogState = this.indexManager.GetCatalogState(this.indexName, out seedingSource, out failureCode, out failureReason);
				this.diagnosticsSession.TraceDebug<string, CatalogState>("Got state for catalog {0}: {1}", this.indexName, catalogState);
				switch (catalogState)
				{
				case CatalogState.Healthy:
					contentIndexStatusType = ContentIndexStatusType.Healthy;
					errorCode = IndexStatusErrorCode.Success;
					break;
				case CatalogState.Seeding:
					contentIndexStatusType = ContentIndexStatusType.Seeding;
					errorCode = IndexStatusErrorCode.SeedingCatalog;
					this.diagnosticsSession.Assert(!string.IsNullOrEmpty(seedingSource), "Seeding source must be present", new object[0]);
					break;
				case CatalogState.Suspended:
					contentIndexStatusType = ContentIndexStatusType.Suspended;
					errorCode = IndexStatusErrorCode.CatalogSuspended;
					break;
				case CatalogState.Failed:
					contentIndexStatusType = ContentIndexStatusType.Failed;
					errorCode = IndexStatusErrorCode.CatalogCorruption;
					break;
				}
			}
			catch (PerformingFastOperationException ex)
			{
				contentIndexStatusType = ContentIndexStatusType.Failed;
				errorCode = IndexStatusErrorCode.FastError;
				this.diagnosticsSession.LogDiagnosticsInfo(DiagnosticsLoggingTag.Failures, "Failed to get state for catalog {0}. ErrorCode: {1}. Assuming state: {2}. Exception: {3}", new object[]
				{
					this.indexName,
					errorCode,
					contentIndexStatusType,
					ex
				});
			}
			this.diagnosticsSession.TraceDebug("Return index status for catalog {0}: State={1}, ErrorCode={2}, SeedingSource={3}", new object[]
			{
				this.indexName,
				contentIndexStatusType,
				errorCode,
				seedingSource ?? "(n/a)"
			});
			return contentIndexStatusType;
		}

		private readonly IDiagnosticsSession diagnosticsSession;

		private readonly string indexName;

		private readonly IIndexManager indexManager;
	}
}
