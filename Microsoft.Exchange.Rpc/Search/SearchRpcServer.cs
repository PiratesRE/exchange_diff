using System;

namespace Microsoft.Exchange.Rpc.Search
{
	internal abstract class SearchRpcServer : RpcServerBase
	{
		public abstract void RecordDocumentProcessing(Guid mdbGuid, Guid flowInstance, Guid correlationId, long docId);

		public abstract void RecordDocumentFailure(Guid mdbGuid, Guid correlationId, long docId, string errorMessage);

		public abstract void UpdateIndexSystems();

		public abstract void ResumeIndexing(Guid databaseGuid);

		public abstract void RebuildIndexSystem(Guid databaseGuid);

		public SearchRpcServer()
		{
		}

		public static IntPtr RpcIntfHandle = (IntPtr)<Module>.ISearchServiceRpc_v4_0_s_ifspec;
	}
}
