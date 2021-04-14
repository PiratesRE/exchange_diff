using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace Microsoft.Office.Datacenter.WorkerTaskFramework
{
	public abstract class WorkBroker : MarshalByRefObject
	{
		internal Action<RestartRequest> RestartRequestEvent { get; set; }

		private RestartRequest RestartRequest { get; set; }

		public abstract IDataAccessQuery<TEntity> AsDataAccessQuery<TEntity>(IEnumerable<TEntity> query);

		internal abstract void PublishResult(WorkItemResult result, TracingContext traceContext);

		internal abstract BlockingCollection<WorkItem> AsyncGetWork(int maxWorkitemCount, CancellationToken cancellationToken);

		internal abstract void Reject(WorkItem workItem);

		internal abstract void Abandon(WorkItem workItem);

		internal abstract BlockingCollection<string> AsyncGetWorkItemPackages(CancellationToken cancellationToken);

		internal void RequestRestart(RestartRequest reason)
		{
			this.RestartRequest = reason;
			this.RestartRequestEvent(reason);
			WTFDiagnostics.TraceError<string>(WTFLog.Core, TracingContext.Default, "[WorkBroker:RequestRestart]: Reason {0}", this.RestartRequest.ToString(), null, "RequestRestart", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\Core\\WorkBroker.cs", 113);
		}
	}
}
