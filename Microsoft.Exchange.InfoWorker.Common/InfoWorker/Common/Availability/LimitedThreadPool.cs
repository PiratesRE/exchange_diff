using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal sealed class LimitedThreadPool
	{
		public static int MaximumThreads
		{
			get
			{
				int result;
				int num;
				ThreadPool.GetAvailableThreads(out result, out num);
				return result;
			}
		}

		public LimitedThreadPool(int maximumThreads, WaitCallback callback)
		{
			this.maximumThreads = maximumThreads;
			this.callback = callback;
		}

		public void Add(object state)
		{
			this.queue.Enqueue(state);
		}

		public void Start()
		{
			lock (this.locker)
			{
				this.Dispatch();
			}
		}

		public void Cancel()
		{
			lock (this.locker)
			{
				this.cancelled = true;
				this.queue.Clear();
			}
		}

		private void Dispatch()
		{
			while (this.threadsInUse < this.maximumThreads)
			{
				if (this.queue.Count == 0)
				{
					return;
				}
				object state = this.queue.Dequeue();
				using (ActivityContext.SuppressThreadScope())
				{
					ThreadPool.QueueUserWorkItem(new WaitCallback(this.Worker), state);
				}
				this.threadsInUse++;
			}
		}

		private void Worker(object state)
		{
			if (!this.cancelled)
			{
				try
				{
					this.callback(state);
				}
				finally
				{
					if (!this.cancelled)
					{
						lock (this.locker)
						{
							this.threadsInUse--;
							this.Dispatch();
						}
					}
				}
			}
		}

		private WaitCallback callback;

		private Queue<object> queue = new Queue<object>();

		private int threadsInUse;

		private int maximumThreads;

		private bool cancelled;

		private object locker = new object();
	}
}
