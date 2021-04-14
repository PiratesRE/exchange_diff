using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery
{
	public class DistributedAction : DisposeTrackableBase
	{
		public DistributedAction(string[] resources, int votesRequired = -1, bool isSyncMode = false)
		{
			this.resources = resources;
			this.isSyncMode = isSyncMode;
			this.ExceptionsByServer = new ConcurrentDictionary<string, Exception>();
			if (votesRequired == -1)
			{
				this.votesRequired = this.resources.Length / 2 + 1;
				return;
			}
			this.votesRequired = votesRequired;
		}

		internal int TotalRequests { get; private set; }

		internal int SuccessCount { get; private set; }

		internal int FailedCount { get; private set; }

		internal ConcurrentDictionary<string, Exception> ExceptionsByServer { get; private set; }

		public bool Run(Action<string> actionToPerform, TimeSpan timeout)
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			this.TotalRequests = this.resources.Length;
			Task[] array = new Task[this.TotalRequests];
			for (int i = 0; i < this.TotalRequests; i++)
			{
				string resource = this.resources[i];
				Task task = Task.Factory.StartNew(delegate()
				{
					Exception ex3 = null;
					if (this.isCompleted)
					{
						WTFDiagnostics.TraceDebug<string>(ExTraceGlobals.RecoveryActionTracer, this.traceContext, "DistributedAction: Action({0}) skipped since the calculations are already concluded.", resource, null, "Run", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Recovery\\DistributedAction.cs", 140);
						return;
					}
					try
					{
						actionToPerform(resource);
						WTFDiagnostics.TraceDebug<string>(ExTraceGlobals.RecoveryActionTracer, this.traceContext, "DistributedAction: Action({0}) succeeded.", resource, null, "Run", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Recovery\\DistributedAction.cs", 153);
					}
					catch (Exception ex4)
					{
						this.ExceptionsByServer[resource] = ex4;
						ex3 = ex4;
						WTFDiagnostics.TraceError<string, string>(ExTraceGlobals.RecoveryActionTracer, this.traceContext, "DistributedAction: Action({0}) failed with error {1}.", resource, ex4.ToString(), null, "Run", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Recovery\\DistributedAction.cs", 165);
					}
					lock (this.locker)
					{
						if (!this.isCompleted)
						{
							if (ex3 == null)
							{
								this.SuccessCount++;
							}
							else
							{
								this.FailedCount++;
							}
							if (this.SuccessCount + this.FailedCount >= this.votesRequired)
							{
								this.isCompleted = true;
								if (!this.cancellationTokenSource.IsCancellationRequested)
								{
									this.cancellationTokenSource.Cancel();
								}
							}
						}
						else
						{
							WTFDiagnostics.TraceWarning<string>(ExTraceGlobals.RecoveryActionTracer, this.traceContext, "DistributedAction: Action({0}) success/failure count update skipped since it is already marked completed.", resource, null, "Run", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Recovery\\DistributedAction.cs", 200);
						}
					}
				});
				array[i] = task;
				if (this.isSyncMode)
				{
					task.Wait();
				}
			}
			Exception ex = null;
			bool flag = false;
			try
			{
				flag = !Task.WaitAll(array, (int)timeout.TotalMilliseconds, this.cancellationTokenSource.Token);
			}
			catch (Exception ex2)
			{
				if (!(ex2 is OperationCanceledException))
				{
					ex = ex2;
				}
			}
			WTFDiagnostics.TraceError(ExTraceGlobals.RecoveryActionTracer, this.traceContext, string.Format("DistributedAction: Run() Completed. Duration:{0}, IsTimedout:{1}, Total: {2}, Success: {3}, Failed: {4} (Error: {5})", new object[]
			{
				stopwatch.Elapsed,
				flag,
				this.TotalRequests,
				this.SuccessCount,
				this.FailedCount,
				(ex != null) ? ex.ToString() : "<none>"
			}), null, "Run", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Recovery\\DistributedAction.cs", 233);
			return this.isCompleted;
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<DistributedAction>(this);
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				lock (this.locker)
				{
					this.cancellationTokenSource.Dispose();
				}
			}
		}

		private readonly int votesRequired;

		private readonly bool isSyncMode;

		private object locker = new object();

		private string[] resources;

		private bool isCompleted;

		private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

		private TracingContext traceContext = TracingContext.Default;
	}
}
