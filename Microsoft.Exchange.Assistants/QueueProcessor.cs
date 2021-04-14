using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.Assistants;

namespace Microsoft.Exchange.Assistants
{
	internal abstract class QueueProcessor<T> : Base, IDisposable
	{
		internal static void SetTestHookBeforeOnTransientFailure(Action testhook)
		{
			QueueProcessor<T>.syncWithTestCodeBeforeOnTransientFailure = testhook;
		}

		internal static void SetTestHookAfterOnCompletedItem(Action testhook)
		{
			QueueProcessor<T>.syncWithTestCodeAfterOnCompletedItem = testhook;
		}

		protected QueueProcessor(ThrottleGovernor governor)
		{
			this.governor = governor;
		}

		protected ThrottleGovernor Governor
		{
			get
			{
				return this.governor;
			}
		}

		protected Throttle Throttle
		{
			get
			{
				return this.governor.Throttle;
			}
		}

		protected object Locker
		{
			get
			{
				return this;
			}
		}

		protected List<T> PendingQueue
		{
			get
			{
				return this.pendingQueue;
			}
		}

		protected List<T> ActiveQueue
		{
			get
			{
				return this.activeQueue;
			}
		}

		protected int PendingWorkers
		{
			get
			{
				return this.pendingWorkers;
			}
		}

		protected int ActiveWorkers
		{
			get
			{
				return this.activeWorkers;
			}
		}

		protected abstract bool Shutdown { get; }

		protected abstract PerformanceCountersPerDatabaseInstance PerformanceCounters { get; }

		public void WaitForShutdown()
		{
			lock (this.Locker)
			{
				if (this.activeWorkers == 0)
				{
					ExTraceGlobals.QueueProcessorTracer.TraceDebug<QueueProcessor<T>>((long)this.GetHashCode(), "{0}: Clear for shutdown", this);
					this.shutdownEvent.Set();
				}
				else
				{
					ExTraceGlobals.QueueProcessorTracer.TraceDebug<QueueProcessor<T>, int>((long)this.GetHashCode(), "{0}: Waiting for {1} workers to exit...", this, this.activeWorkers);
				}
			}
			this.shutdownEvent.WaitOne();
			base.TracePfd("PFD AIS {0} {1}: finished shutting down", new object[]
			{
				22615,
				this
			});
			ExTraceGlobals.QueueProcessorTracer.TraceDebug<QueueProcessor<T>>((long)this.GetHashCode(), "{0}: Workers have exited.", this);
		}

		public void EnqueueItem(T item)
		{
			lock (this.Locker)
			{
				ExTraceGlobals.QueueProcessorTracer.TraceDebug<QueueProcessor<T>, T>((long)this.GetHashCode(), "{0}: Enqueing item: {1}", this, item);
				this.OnEnqueueItem(item);
				this.pendingQueue.Add(item);
				this.QueueWorkersIfNecessary();
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
		}

		protected abstract AIException ProcessItem(T item);

		protected virtual void OnEnqueueItem(T item)
		{
		}

		protected virtual void OnCompletedItem(T item, AIException exception)
		{
		}

		protected virtual void OnTransientFailure(T item, AIException exception)
		{
		}

		protected virtual void OnWorkersStarted()
		{
		}

		protected virtual void OnWorkersClear()
		{
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.governor.Dispose();
			}
		}

		private static void Worker(object state)
		{
			((QueueProcessor<T>)state).Worker();
		}

		private void QueueWorkersIfNecessary()
		{
			int num = this.Shutdown ? 0 : Math.Min(this.Throttle.OpenThrottleValue, this.pendingQueue.Count + this.activeQueue.Count);
			ExTraceGlobals.QueueProcessorTracer.TraceDebug((long)this.GetHashCode(), "{0}: desiredWorkers: {1}, pending: {2}, active: {3}", new object[]
			{
				this,
				num,
				this.pendingWorkers,
				this.activeWorkers
			});
			while (this.pendingWorkers + this.activeWorkers < num)
			{
				this.Throttle.QueueUserWorkItem(QueueProcessor<T>.workerCallback, this);
				this.pendingWorkers++;
				ExTraceGlobals.QueueProcessorTracer.TraceDebug((long)this.GetHashCode(), "{0}: Queued worker. desired: {1}, pending: {2}, active: {3}", new object[]
				{
					this,
					num,
					this.pendingWorkers,
					this.activeWorkers
				});
			}
		}

		private void Worker()
		{
			lock (this.Locker)
			{
				this.pendingWorkers--;
				if (this.Shutdown)
				{
					ExTraceGlobals.QueueProcessorTracer.TraceDebug<QueueProcessor<T>>((long)this.GetHashCode(), "{0}: Shutdown before Worker could start.", this);
					return;
				}
				this.PerformanceCounters.NumberOfThreadsUsed.Increment();
				if (++this.activeWorkers == 1)
				{
					ExTraceGlobals.QueueProcessorTracer.TraceDebug<QueueProcessor<T>>((long)this.GetHashCode(), "{0}: First worker entering", this);
					this.OnWorkersStarted();
				}
				ExTraceGlobals.QueueProcessorTracer.TraceDebug<QueueProcessor<T>, int>((long)this.GetHashCode(), "{0}: New active worker #{1}", this, this.activeWorkers);
				int num = 0;
				while (!this.Shutdown && !this.Throttle.IsOverThrottle && this.pendingQueue.Count > 0 && num < 128)
				{
					this.ProcessOneItem();
					num++;
				}
				ExTraceGlobals.QueueProcessorTracer.TraceDebug<QueueProcessor<T>>((long)this.GetHashCode(), "{0}: Out of worker loop", this);
				if (--this.activeWorkers == 0)
				{
					ExTraceGlobals.QueueProcessorTracer.TraceDebug<QueueProcessor<T>>((long)this.GetHashCode(), "{0}: Last worker exiting", this);
					this.OnWorkersClear();
					if (this.Shutdown)
					{
						ExTraceGlobals.QueueProcessorTracer.TraceDebug<QueueProcessor<T>>((long)this.GetHashCode(), "{0}: Last worker setting shutdownEvent", this);
						this.shutdownEvent.Set();
					}
				}
				this.QueueWorkersIfNecessary();
			}
			this.PerformanceCounters.NumberOfThreadsUsed.Decrement();
		}

		private void ProcessOneItem()
		{
			bool flag = false;
			T t = this.pendingQueue[0];
			this.pendingQueue.RemoveAt(0);
			this.activeQueue.Add(t);
			ExTraceGlobals.QueueProcessorTracer.TraceDebug<QueueProcessor<T>, T>((long)this.GetHashCode(), "{0}: ProcessOneItem releasing lock for item: {1}", this, t);
			Monitor.Exit(this.Locker);
			try
			{
				AIException ex = this.ProcessItem(t);
				ExTraceGlobals.QueueProcessorTracer.TraceDebug<QueueProcessor<T>, T, AIException>((long)this.GetHashCode(), "{0}: Item: {1}, Result: {2}", this, t, ex);
				flag = this.governor.ReportResult(ex);
				if (flag)
				{
					ExTraceGlobals.QueueProcessorTracer.TraceDebug<QueueProcessor<T>, T, AIException>((long)this.GetHashCode(), "{0}: OpComplete, Item: {1}, Result: {2}", this, t, ex);
					this.OnCompletedItem(t, ex);
					if (QueueProcessor<T>.syncWithTestCodeAfterOnCompletedItem != null)
					{
						QueueProcessor<T>.syncWithTestCodeAfterOnCompletedItem();
					}
				}
				else
				{
					ExTraceGlobals.QueueProcessorTracer.TraceDebug<QueueProcessor<T>, T, AIException>((long)this.GetHashCode(), "{0}: WillRetry, Item: {1}, Result: {2}", this, t, ex);
					if (QueueProcessor<T>.syncWithTestCodeBeforeOnTransientFailure != null)
					{
						QueueProcessor<T>.syncWithTestCodeBeforeOnTransientFailure();
					}
					this.OnTransientFailure(t, ex);
				}
			}
			finally
			{
				Monitor.Enter(this.Locker);
				ExTraceGlobals.QueueProcessorTracer.TraceDebug<QueueProcessor<T>, T>((long)this.GetHashCode(), "{0}: ProcessOneItem reacquire lock for item: {1}", this, t);
				this.activeQueue.Remove(t);
				if (!flag)
				{
					ExTraceGlobals.QueueProcessorTracer.TraceDebug<QueueProcessor<T>, T>((long)this.GetHashCode(), "{0}: Requeing item for retry: {1}", this, t);
					this.pendingQueue.Insert(0, t);
					this.QueueWorkersIfNecessary();
				}
			}
		}

		private const int MaximumEventsProcessedPerThread = 128;

		private static WaitCallback workerCallback = new WaitCallback(QueueProcessor<T>.Worker);

		private static Action syncWithTestCodeBeforeOnTransientFailure = null;

		private static Action syncWithTestCodeAfterOnCompletedItem = null;

		private ThrottleGovernor governor;

		private List<T> pendingQueue = new List<T>();

		private List<T> activeQueue = new List<T>();

		private FastManualResetEvent shutdownEvent = new FastManualResetEvent(false);

		private int pendingWorkers;

		private int activeWorkers;
	}
}
