using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;

namespace Microsoft.Exchange.Cluster.Replay
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class PrioritizedQueue<TCallback> where TCallback : IQueuedCallback
	{
		internal bool IsEnabled { get; set; }

		internal bool IsIdle
		{
			get
			{
				bool result;
				lock (this.m_locker)
				{
					result = (this.m_highPriorityQueue.Count == 0 && this.m_queue.Count == 0 && !this.m_fInProcessing);
				}
				return result;
			}
		}

		internal TCallback ItemInProcessing
		{
			get
			{
				TCallback result;
				lock (this.m_locker)
				{
					result = ((this.m_itemInProcessing != null) ? this.m_itemInProcessing.Callback : default(TCallback));
				}
				return result;
			}
		}

		public PrioritizedQueue()
		{
			this.m_locker = new object();
			this.m_highPriorityQueue = new List<PrioritizedQueue<TCallback>.QueuedItem<TCallback>>();
			this.m_queue = new List<PrioritizedQueue<TCallback>.QueuedItem<TCallback>>();
			this.IsEnabled = true;
		}

		public void PrepareToStop()
		{
			lock (this.m_locker)
			{
				this.IsEnabled = false;
				if (this.ItemInProcessing != null)
				{
					TCallback itemInProcessing = this.ItemInProcessing;
					itemInProcessing.Cancel();
				}
				this.m_fPrepareToStopCalled = true;
			}
		}

		public void Stop()
		{
			lock (this.m_locker)
			{
				if (!this.m_fPrepareToStopCalled)
				{
					this.PrepareToStop();
				}
				goto IL_32;
			}
			IL_2B:
			Thread.Sleep(50);
			IL_32:
			if (this.IsIdle)
			{
				return;
			}
			goto IL_2B;
		}

		public bool EnqueueHighPriority(TCallback callback, EventWaitHandle waitHandle)
		{
			return this.EnqueueInternal(callback, waitHandle, true);
		}

		public bool Enqueue(TCallback callback, EventWaitHandle waitHandle)
		{
			return this.EnqueueInternal(callback, waitHandle, false);
		}

		public bool EnqueueUniqueItem(TCallback callback, EventWaitHandle waitHandle)
		{
			TCallback tcallback;
			EventWaitHandle eventWaitHandle;
			return this.EnqueueUniqueItem(callback, waitHandle, false, out tcallback, out eventWaitHandle);
		}

		public bool EnqueueUniqueItem(TCallback callback, EventWaitHandle waitHandle, bool includeInProgressItem, out TCallback existingItem, out EventWaitHandle existingWaitHandle)
		{
			bool result = false;
			existingItem = default(TCallback);
			existingWaitHandle = null;
			lock (this.m_locker)
			{
				PrioritizedQueue<TCallback>.QueuedItem<TCallback> queuedItem = null;
				if (includeInProgressItem && this.m_itemInProcessing != null)
				{
					TCallback callback2 = this.m_itemInProcessing.Callback;
					if (callback2.IsEquivalentOrSuperset(callback))
					{
						queuedItem = this.m_itemInProcessing;
					}
				}
				if (queuedItem == null)
				{
					queuedItem = this.m_highPriorityQueue.Find(delegate(PrioritizedQueue<TCallback>.QueuedItem<TCallback> item)
					{
						TCallback callback3 = item.Callback;
						return callback3.IsEquivalentOrSuperset(callback);
					});
					if (queuedItem == null)
					{
						queuedItem = this.m_queue.Find(delegate(PrioritizedQueue<TCallback>.QueuedItem<TCallback> item)
						{
							TCallback callback3 = item.Callback;
							return callback3.IsEquivalentOrSuperset(callback);
						});
					}
				}
				if (queuedItem == null)
				{
					ExTraceGlobals.ReplayManagerTracer.TraceDebug<string>((long)this.GetHashCode(), "EnqueueUniqueItem: Enqueueing item of type '{0}' because it isn't already in the queue.", callback.GetType().Name);
					result = this.Enqueue(callback, waitHandle);
				}
				if (queuedItem != null)
				{
					ExTraceGlobals.ReplayManagerTracer.TraceDebug<string>((long)this.GetHashCode(), "EnqueueUniqueItem: Item of type '{0}' was not enqueued because another equivalent one is already in the queue. Returning that one to wait on.", callback.GetType().Name);
					callback.Cancel();
					existingItem = queuedItem.Callback;
					existingWaitHandle = queuedItem.CompletedEvent;
				}
			}
			return result;
		}

		private bool EnqueueInternal(TCallback callback, EventWaitHandle waitHandle, bool isHighPriority)
		{
			if (callback == null)
			{
				throw new ArgumentNullException("callback", "Cannot enqueue a null callback.");
			}
			bool result = false;
			lock (this.m_locker)
			{
				if (!this.IsEnabled)
				{
					ExTraceGlobals.ReplayManagerTracer.TraceDebug<string>((long)this.GetHashCode(), "EnqueueInternal: Item of type '{0}' not enqueued because the queue ha been disabled. Cancelling it now.", callback.GetType().Name);
					callback.Cancel();
					return false;
				}
				PrioritizedQueue<TCallback>.QueuedItem<TCallback> item = new PrioritizedQueue<TCallback>.QueuedItem<TCallback>(callback, waitHandle);
				result = true;
				if (isHighPriority)
				{
					this.m_highPriorityQueue.Add(item);
					ExTraceGlobals.ReplayManagerTracer.TraceDebug<string>((long)this.GetHashCode(), "EnqueueInternal: Item of type '{0}' enqueued into the high-priority queue.", callback.GetType().Name);
				}
				else
				{
					this.m_queue.Add(item);
					ExTraceGlobals.ReplayManagerTracer.TraceDebug<string>((long)this.GetHashCode(), "EnqueueInternal: Item of type '{0}' enqueued into the normal-priority queue.", callback.GetType().Name);
				}
				if (!this.m_fInProcessing)
				{
					this.m_fInProcessing = true;
					this.QueueProcessingThread();
				}
				else
				{
					ExTraceGlobals.ReplayManagerTracer.TraceDebug((long)this.GetHashCode(), "EnqueueInternal: Processing thread is currently busy. Skipping QueueProcessingThread()...");
				}
			}
			return result;
		}

		private void QueueProcessingThread()
		{
			ThreadPool.QueueUserWorkItem(new WaitCallback(this.ProcessItems));
		}

		private void ProcessItems(object stateIgnored)
		{
			PrioritizedQueue<TCallback>.QueuedItem<TCallback> queuedItem = null;
			for (;;)
			{
				lock (this.m_locker)
				{
					ExTraceGlobals.ReplayManagerTracer.TraceDebug<int, int>((long)this.GetHashCode(), "ProcessItems: Number of high-priority items: {0}; Number of normal-priority items: {1}", this.m_highPriorityQueue.Count, this.m_queue.Count);
					if (this.m_highPriorityQueue.Count > 0)
					{
						queuedItem = this.Dequeue(this.m_highPriorityQueue);
						this.m_itemInProcessing = queuedItem;
						Trace replayManagerTracer = ExTraceGlobals.ReplayManagerTracer;
						long id = (long)this.GetHashCode();
						string formatString = "ProcessItems: Dequeued a high-priority operation of type '{0}'.";
						TCallback callback = queuedItem.Callback;
						replayManagerTracer.TraceDebug<string>(id, formatString, callback.GetType().Name);
					}
					else
					{
						if (this.m_queue.Count <= 0)
						{
							this.m_itemInProcessing = null;
							this.m_fInProcessing = false;
							break;
						}
						queuedItem = this.Dequeue(this.m_queue);
						this.m_itemInProcessing = queuedItem;
						Trace replayManagerTracer2 = ExTraceGlobals.ReplayManagerTracer;
						long id2 = (long)this.GetHashCode();
						string formatString2 = "ProcessItems: Dequeued a normal-priority operation of type '{0}'.";
						TCallback callback2 = queuedItem.Callback;
						replayManagerTracer2.TraceDebug<string>(id2, formatString2, callback2.GetType().Name);
					}
				}
				if (this.IsEnabled)
				{
					this.RunOperation(queuedItem);
				}
				else
				{
					Trace replayManagerTracer3 = ExTraceGlobals.ReplayManagerTracer;
					long id3 = (long)this.GetHashCode();
					string formatString3 = "ProcessItems: Cancelling operation of type '{0}' because the queue is no longer enabled.";
					TCallback callback3 = queuedItem.Callback;
					replayManagerTracer3.TraceDebug<string>(id3, formatString3, callback3.GetType().Name);
					TCallback callback4 = queuedItem.Callback;
					callback4.Cancel();
				}
			}
		}

		private PrioritizedQueue<TCallback>.QueuedItem<TCallback> Dequeue(List<PrioritizedQueue<TCallback>.QueuedItem<TCallback>> queue)
		{
			PrioritizedQueue<TCallback>.QueuedItem<TCallback> result = null;
			if (queue.Count > 0)
			{
				result = queue[0];
				queue.RemoveAt(0);
			}
			return result;
		}

		private void RunOperation(PrioritizedQueue<TCallback>.QueuedItem<TCallback> item)
		{
			bool flag = true;
			try
			{
				TCallback callback = item.Callback;
				callback.ReportStatus(QueuedItemStatus.Started);
				TCallback callback2 = item.Callback;
				callback2.StartTimeUtc = DateTime.UtcNow;
				TCallback callback3 = item.Callback;
				callback3.Execute();
				TCallback callback4 = item.Callback;
				if (callback4.LastException != null)
				{
					Trace replayManagerTracer = ExTraceGlobals.ReplayManagerTracer;
					long id = (long)this.GetHashCode();
					string formatString = "PrioritizedQueue.RunOperation: Operation of type '{0}' returned exception: {1}";
					TCallback callback5 = item.Callback;
					string name = callback5.GetType().Name;
					TCallback callback6 = item.Callback;
					replayManagerTracer.TraceDebug<string, Exception>(id, formatString, name, callback6.LastException);
				}
				else
				{
					Trace replayManagerTracer2 = ExTraceGlobals.ReplayManagerTracer;
					long id2 = (long)this.GetHashCode();
					string formatString2 = "PrioritizedQueue.RunOperation: Operation of type '{0}' completed successfully without any errors.";
					TCallback callback7 = item.Callback;
					replayManagerTracer2.TraceDebug<string>(id2, formatString2, callback7.GetType().Name);
				}
				flag = false;
				TCallback callback8 = item.Callback;
				callback8.ReportStatus(QueuedItemStatus.Completed);
			}
			catch (OperationAbortedException)
			{
				flag = false;
				TCallback callback9 = item.Callback;
				callback9.ReportStatus(QueuedItemStatus.Cancelled);
			}
			finally
			{
				TCallback callback10 = item.Callback;
				callback10.EndTimeUtc = DateTime.UtcNow;
				if (flag)
				{
					TCallback callback11 = item.Callback;
					callback11.ReportStatus(QueuedItemStatus.Failed);
				}
				if (item.CompletedEvent != null)
				{
					item.CompletedEvent.Set();
				}
			}
		}

		private List<PrioritizedQueue<TCallback>.QueuedItem<TCallback>> m_queue;

		private List<PrioritizedQueue<TCallback>.QueuedItem<TCallback>> m_highPriorityQueue;

		private PrioritizedQueue<TCallback>.QueuedItem<TCallback> m_itemInProcessing;

		private bool m_fInProcessing;

		private bool m_fPrepareToStopCalled;

		protected object m_locker;

		[ClassAccessLevel(AccessLevel.Implementation)]
		private class QueuedItem<T> where T : IQueuedCallback
		{
			internal T Callback { get; private set; }

			internal EventWaitHandle CompletedEvent { get; private set; }

			public QueuedItem(T callback, EventWaitHandle waitHandle)
			{
				this.Callback = callback;
				this.CompletedEvent = waitHandle;
			}
		}
	}
}
