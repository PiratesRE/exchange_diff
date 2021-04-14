using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Net.JobQueues
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class JobQueue
	{
		public ManualResetEvent ShutdownEvent { get; private set; }

		public QueueType Type { get; private set; }

		public int PendingJobCount
		{
			get
			{
				return this.pendingJobCount;
			}
		}

		public bool IsDipatcherActive { get; private set; }

		public int Length
		{
			get
			{
				int count;
				lock (this.syncObject)
				{
					count = this.queue.Count;
				}
				return count;
			}
		}

		public Configuration Configuration
		{
			get
			{
				return this.config;
			}
		}

		public bool IsShuttingdown
		{
			get
			{
				return this.shuttingdown;
			}
		}

		public void SignalShutdown()
		{
			lock (this.syncObject)
			{
				this.shuttingdown = true;
				if (this.dispatchTimer != null)
				{
					this.dispatchTimer.Dispose();
					this.dispatchTimer = null;
				}
			}
			this.SignalShutdownEventIfNecessary();
		}

		public virtual void Cleanup()
		{
		}

		protected abstract bool TryCreateJob(byte[] data, out Job job, out EnqueueResult result);

		private void SignalShutdownEventIfNecessary()
		{
			if (this.shuttingdown && this.pendingJobCount == 0)
			{
				this.ShutdownEvent.Set();
			}
		}

		public EnqueueResult Enqueue(byte[] data)
		{
			bool flag = false;
			EnqueueResult result2;
			try
			{
				object obj;
				Monitor.Enter(obj = this.syncObject, ref flag);
				if (this.shuttingdown)
				{
					result2 = new EnqueueResult(EnqueueResultType.QueueServerShutDown);
				}
				else if (this.queue.Count + 1 > this.config.MaxAllowedQueueLength)
				{
					result2 = new EnqueueResult(EnqueueResultType.QueueLengthLimitReached);
				}
				else
				{
					Job job = null;
					EnqueueResult result = EnqueueResult.Success;
					bool jobCreationSuccessful = false;
					try
					{
						GrayException.MapAndReportGrayExceptions(delegate()
						{
							jobCreationSuccessful = this.TryCreateJob(data, out job, out result);
						});
					}
					catch (GrayException ex)
					{
						return new EnqueueResult(EnqueueResultType.UnknownError, ex.ToString());
					}
					if (!jobCreationSuccessful)
					{
						result2 = result;
					}
					else
					{
						this.queue.Enqueue(job);
						if (!this.IsDipatcherActive)
						{
							this.WakeUpDispatcher();
						}
						result2 = result;
					}
				}
			}
			finally
			{
				if (flag)
				{
					object obj;
					Monitor.Exit(obj);
				}
			}
			return result2;
		}

		protected JobQueue(QueueType type, Configuration config)
		{
			this.Type = type;
			this.config = config;
			this.ShutdownEvent = new ManualResetEvent(false);
			this.dispatchTimer = new Timer(new TimerCallback(this.OnDispatch), null, TimeSpan.FromMilliseconds(-1.0), TimeSpan.FromMilliseconds(-1.0));
		}

		public virtual void OnJobCompletion(Job job)
		{
			Interlocked.Decrement(ref this.pendingJobCount);
			this.SignalShutdownEventIfNecessary();
		}

		private void OnDispatch(object state)
		{
			lock (this.syncObject)
			{
				if (!this.shuttingdown)
				{
					int num = this.config.MaxAllowedPendingJobCount - this.PendingJobCount;
					int num2 = 0;
					while (num2 < num && this.queue.Count != 0)
					{
						Job job = this.queue.Dequeue();
						if (ThreadPool.QueueUserWorkItem(new WaitCallback(job.Begin), null))
						{
							Interlocked.Increment(ref this.pendingJobCount);
						}
						else
						{
							this.queue.Enqueue(job);
						}
						num2++;
					}
					if (this.queue.Count == 0)
					{
						this.DormantDispatcher();
					}
					else
					{
						this.WakeUpDispatcher();
					}
				}
			}
		}

		private void WakeUpDispatcher()
		{
			this.IsDipatcherActive = true;
			this.dispatchTimer.Change(this.config.DispatcherWakeUpInterval, TimeSpan.FromMilliseconds(-1.0));
		}

		private void DormantDispatcher()
		{
			this.dispatchTimer.Change(-1, -1);
			this.IsDipatcherActive = false;
		}

		protected readonly object syncObject = new object();

		protected readonly Configuration config;

		private readonly Queue<Job> queue = new Queue<Job>();

		private Timer dispatchTimer;

		private int pendingJobCount;

		protected volatile bool shuttingdown;
	}
}
