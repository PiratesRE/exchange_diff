using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Search;
using Microsoft.Exchange.Rpc.Search;

namespace Microsoft.Exchange.Search.Core.RpcEndpoint
{
	internal sealed class SearchServiceRpcClient : SearchRpcClient, IDisposeTrackable, IDisposable
	{
		internal SearchServiceRpcClient(string server) : base(server)
		{
			this.tracingContext = this.GetHashCode();
			this.disposeTracker = this.GetDisposeTracker();
		}

		public DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<SearchServiceRpcClient>(this);
		}

		public void SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
			}
		}

		internal new void RecordDocumentProcessing(Guid mdbGuid, Guid flowInstance, Guid correlationId, long docId)
		{
			SearchServiceRpcClient.tracer.TraceDebug<Guid>((long)this.tracingContext, "Executing RPC - RecordDocumentProcessing for CorrelationId: {0}", correlationId);
			try
			{
				base.RecordDocumentProcessing(mdbGuid, flowInstance, correlationId, docId);
			}
			finally
			{
				SearchServiceRpcClient.tracer.TraceDebug<Guid>((long)this.tracingContext, "Completed RPC - RecordDocumentProcessing for CorrelationId: {0}", correlationId);
			}
		}

		internal new void RecordDocumentFailure(Guid mdbGuid, Guid correlationId, long docId, string errorMessage)
		{
			SearchServiceRpcClient.tracer.TraceDebug<Guid>((long)this.tracingContext, "Executing RPC - RecordDocumentFailure for CorrelationId: {0}", correlationId);
			try
			{
				base.RecordDocumentFailure(mdbGuid, correlationId, docId, errorMessage);
			}
			finally
			{
				SearchServiceRpcClient.tracer.TraceDebug<Guid>((long)this.tracingContext, "Completed RPC - RecordDocumentFailure for CorrelationId: {0}", correlationId);
			}
		}

		internal new void UpdateIndexSystems()
		{
			SearchServiceRpcClient.tracer.TraceDebug((long)this.tracingContext, "Executing RPC - UpdateIndexSystems");
			try
			{
				base.UpdateIndexSystems();
			}
			finally
			{
				SearchServiceRpcClient.tracer.TraceDebug((long)this.tracingContext, "Completed RPC - UpdateIndexSystems");
			}
		}

		internal new void ResumeIndexing(Guid databaseGuid)
		{
			SearchServiceRpcClient.tracer.TraceDebug<Guid>((long)this.tracingContext, "Executing RPC - ResumeIndexing for database: {0}", databaseGuid);
			try
			{
				base.ResumeIndexing(databaseGuid);
			}
			finally
			{
				SearchServiceRpcClient.tracer.TraceDebug<Guid>((long)this.tracingContext, "Completed RPC - ResumeIndexing for database: {0}", databaseGuid);
			}
		}

		internal new void RebuildIndexSystem(Guid databaseGuid)
		{
			SearchServiceRpcClient.tracer.TraceDebug<Guid>((long)this.tracingContext, "Executing RPC - RebuildIndexSystem for database: {0}", databaseGuid);
			try
			{
				base.RebuildIndexSystem(databaseGuid);
			}
			finally
			{
				SearchServiceRpcClient.tracer.TraceDebug<Guid>((long)this.tracingContext, "Completed RPC - RebuildIndexSystem for database: {0}", databaseGuid);
			}
		}

		protected override void Dispose(bool calledFromDispose)
		{
			if (calledFromDispose && this.disposeTracker != null)
			{
				this.disposeTracker.Dispose();
				this.disposeTracker = null;
			}
			base.Dispose(calledFromDispose);
		}

		private static readonly Trace tracer = ExTraceGlobals.SearchRpcClientTracer;

		private readonly int tracingContext;

		private DisposeTracker disposeTracker;
	}
}
