using System;
using System.Diagnostics;
using System.Threading;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;

namespace Microsoft.Exchange.Cluster.Replay
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class ReplayQueuedItemBase : IQueuedCallback
	{
		private QueuedItemStatus Status { get; set; }

		private bool IsCancelRequested { get; set; }

		private bool IsStarted { get; set; }

		public bool IsDuplicateAllowed { get; set; }

		public Exception LastException { get; private set; }

		public bool IsComplete
		{
			get
			{
				return this.Status == QueuedItemStatus.Cancelled || this.Status == QueuedItemStatus.Completed || this.Status == QueuedItemStatus.Failed;
			}
		}

		public bool IsCancelled
		{
			get
			{
				return (!this.IsStarted && this.IsCancelRequested) || (this.IsStarted && this.Status == QueuedItemStatus.Cancelled);
			}
		}

		public virtual string Name
		{
			get
			{
				return this.m_operationName;
			}
		}

		public ReplayQueuedItemBase()
		{
			this.CreateTimeUtc = DateTime.UtcNow;
			this.IsDuplicateAllowed = true;
			this.Status = QueuedItemStatus.Unknown;
			this.m_operationName = base.GetType().Name;
		}

		public DateTime CreateTimeUtc { get; private set; }

		public DateTime StartTimeUtc { private get; set; }

		public DateTime EndTimeUtc { private get; set; }

		public TimeSpan StartTimeDuration
		{
			get
			{
				if (this.IsStarted)
				{
					return this.StartTimeUtc.Subtract(this.CreateTimeUtc);
				}
				return TimeSpan.Zero;
			}
		}

		public TimeSpan ExecutionDuration
		{
			get
			{
				if (this.IsComplete && !this.IsCancelled)
				{
					return this.EndTimeUtc.Subtract(this.StartTimeUtc);
				}
				return TimeSpan.Zero;
			}
		}

		public virtual bool IsEquivalentOrSuperset(IQueuedCallback otherCallback)
		{
			return object.ReferenceEquals(this, otherCallback);
		}

		public void ReportStatus(QueuedItemStatus status)
		{
			lock (this)
			{
				if (this.IsCancelRequested && status == QueuedItemStatus.Started)
				{
					ExTraceGlobals.ReplayManagerTracer.TraceError<string, QueuedItemStatus>((long)this.GetHashCode(), "ReplayQueuedItemBase: ReportStatus for queued item '{0}' and status '{1}' is throwing OperationAbortedException because Cancel() has already been called.", base.GetType().Name, status);
					throw new OperationAbortedException();
				}
				ExTraceGlobals.ReplayManagerTracer.TraceDebug<string, QueuedItemStatus, QueuedItemStatus>((long)this.GetHashCode(), "ReplayQueuedItemBase: ReportStatus for queued item '{0}'. Status changed from {1} to {2}.", base.GetType().Name, this.Status, status);
				this.Status = status;
				if (status == QueuedItemStatus.Started)
				{
					this.IsStarted = true;
				}
			}
			if (status == QueuedItemStatus.Cancelled)
			{
				this.Cancel();
			}
		}

		public void Cancel()
		{
			lock (this)
			{
				if (!this.IsCancelRequested)
				{
					ExTraceGlobals.ReplayManagerTracer.TraceDebug<string>((long)this.GetHashCode(), "ReplayQueuedItemBase: Queued item for '{0}' has been cancelled.", base.GetType().Name);
					this.IsCancelRequested = true;
					this.LastException = this.GetOperationCancelledException();
				}
			}
		}

		public void Execute()
		{
			Exception lastException = ReplayConfigurationHelper.HandleKnownExceptions(delegate(object param0, EventArgs param1)
			{
				this.ExecuteInternal();
			});
			this.LastException = lastException;
		}

		public ReplayQueuedItemCompletionReason Wait()
		{
			return this.Wait(-1);
		}

		public ReplayQueuedItemCompletionReason Wait(int timeoutMs)
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			ReplayQueuedItemCompletionReason replayQueuedItemCompletionReason;
			while (!this.IsCancelled)
			{
				if (timeoutMs != -1 && stopwatch.ElapsedMilliseconds > (long)timeoutMs)
				{
					replayQueuedItemCompletionReason = ReplayQueuedItemCompletionReason.Timedout;
				}
				else
				{
					if (!this.IsComplete)
					{
						Thread.Sleep(50);
						continue;
					}
					replayQueuedItemCompletionReason = ReplayQueuedItemCompletionReason.Finished;
				}
				IL_41:
				Exception ex = this.LastException;
				if (ex == null && replayQueuedItemCompletionReason == ReplayQueuedItemCompletionReason.Timedout)
				{
					ex = this.GetOperationTimedoutException(TimeSpan.FromMilliseconds((double)timeoutMs));
				}
				if (ex != null)
				{
					if (ex is OperationAbortedException)
					{
						ex = this.GetOperationCancelledException();
					}
					throw ex;
				}
				return replayQueuedItemCompletionReason;
			}
			replayQueuedItemCompletionReason = ReplayQueuedItemCompletionReason.Cancelled;
			goto IL_41;
		}

		protected abstract void ExecuteInternal();

		protected abstract Exception GetOperationCancelledException();

		protected abstract Exception GetOperationTimedoutException(TimeSpan timeout);

		private string m_operationName;
	}
}
