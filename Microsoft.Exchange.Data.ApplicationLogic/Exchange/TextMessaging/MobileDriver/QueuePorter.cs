using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.MobileTransport;

namespace Microsoft.Exchange.TextMessaging.MobileDriver
{
	internal class QueuePorter<T>
	{
		public QueuePorter(IList<QueuePorterWorkingContext<T>> contexts, bool allowToHandleErrors)
		{
			if (contexts == null)
			{
				throw new ArgumentNullException("contexts");
			}
			this.AllowToHandleErrors = allowToHandleErrors;
			this.Contexts = (contexts.IsReadOnly ? contexts : new ReadOnlyCollection<QueuePorterWorkingContext<T>>(contexts));
			this.StopRequested = true;
			this.Watcher = new Thread(new ThreadStart(this.Processor));
			this.Watcher.IsBackground = true;
		}

		public QueuePorter(ThreadSafeQueue<T> queue, QueueDataAvailableEventHandler<T> dataAvailableEventHandler, bool allowToHandleErrors) : this(new QueuePorterWorkingContext<T>[]
		{
			new QueuePorterWorkingContext<T>(queue, dataAvailableEventHandler, 1)
		}, allowToHandleErrors)
		{
		}

		private IList<QueuePorterWorkingContext<T>> Contexts { get; set; }

		private Thread Watcher { get; set; }

		private bool AllowToHandleErrors { get; set; }

		private bool StopRequested
		{
			get
			{
				return this.stopRequested;
			}
			set
			{
				this.stopRequested = value;
				if (value)
				{
					this.StopRequestedWaitHandle.Set();
					return;
				}
				this.StopRequestedWaitHandle.Reset();
			}
		}

		private ManualResetEvent StopRequestedWaitHandle
		{
			get
			{
				if (this.stopRequestedWaitHandle == null)
				{
					this.stopRequestedWaitHandle = new ManualResetEvent(this.StopRequested);
				}
				return this.stopRequestedWaitHandle;
			}
		}

		public void Start()
		{
			lock (this)
			{
				if (this.StopRequested)
				{
					while (ThreadState.Stopped != (ThreadState.Stopped & this.Watcher.ThreadState) && ThreadState.Unstarted != (ThreadState.Unstarted & this.Watcher.ThreadState))
					{
						Thread.SpinWait(100);
					}
					this.StopRequested = false;
					this.Watcher.Start();
				}
			}
		}

		public void Stop()
		{
			lock (this)
			{
				this.StopRequested = true;
			}
		}

		public void Processor()
		{
			List<WaitHandle> list = new List<WaitHandle>(1 + this.Contexts.Count);
			list.Add(this.StopRequestedWaitHandle);
			foreach (QueuePorterWorkingContext<T> queuePorterWorkingContext in this.Contexts)
			{
				list.Add(queuePorterWorkingContext.Queue.DataAvailable);
			}
			WaitHandle[] waitHandles = list.ToArray();
			int num;
			while (!this.StopRequested && 0 < (num = WaitHandle.WaitAny(waitHandles)))
			{
				int index = num - 1;
				T item;
				while (!this.StopRequested && this.Contexts[index].Queue.Dequeue(out item))
				{
					ThreadPool.QueueUserWorkItem(new WaitCallback(this.HandleDataAvailableEvent), new QueuePorter<T>.DequeuedItem(index, item));
				}
			}
		}

		private void HandleDataAvailableEvent(object state)
		{
			QueuePorter<T>.DequeuedItem dequeuedItem = (QueuePorter<T>.DequeuedItem)state;
			try
			{
				this.Contexts[dequeuedItem.ContextIndex].OnDataAvailable(new QueueDataAvailableEventSource<T>(this.Contexts[dequeuedItem.ContextIndex].Queue), new QueueDataAvailableEventArgs<T>(dequeuedItem.Item));
			}
			catch (Exception ex)
			{
				ExTraceGlobals.CoreTracer.TraceError<Exception>((long)this.GetHashCode(), "OnDataAvailable raises Exception: {0}", ex);
				if (!this.AllowToHandleErrors || !GrayException.IsGrayException(ex))
				{
					throw;
				}
				ExWatson.SendReport(ex, ReportOptions.None, null);
			}
		}

		private const int SpinWaitInterval = 100;

		private bool stopRequested;

		private ManualResetEvent stopRequestedWaitHandle;

		private struct DequeuedItem
		{
			public DequeuedItem(int index, T item)
			{
				this.contextIndex = index;
				this.item = item;
			}

			public int ContextIndex
			{
				get
				{
					return this.contextIndex;
				}
			}

			public T Item
			{
				get
				{
					return this.item;
				}
			}

			private int contextIndex;

			private T item;
		}
	}
}
