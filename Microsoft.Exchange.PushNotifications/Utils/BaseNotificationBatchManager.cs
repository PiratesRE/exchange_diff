using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Threading;

namespace Microsoft.Exchange.PushNotifications.Utils
{
	internal abstract class BaseNotificationBatchManager<TKey, TNotification> : PushNotificationDisposable
	{
		internal BaseNotificationBatchManager(uint batchTimerInSeconds, uint batchSize)
		{
			this.batchSize = batchSize;
			this.activeSet = new ConcurrentDictionary<TKey, TNotification>();
			this.timer = new System.Timers.Timer(batchTimerInSeconds * 1000U);
			this.timer.Elapsed += this.OnTimedEvent;
			this.timer.Start();
		}

		internal virtual bool TryGetPushNotification(TKey key, out TNotification notification)
		{
			ConcurrentDictionary<TKey, TNotification> concurrentDictionary = this.activeSet;
			if (concurrentDictionary != null && concurrentDictionary.ContainsKey(key))
			{
				notification = concurrentDictionary[key];
				return true;
			}
			notification = default(TNotification);
			return false;
		}

		internal virtual void Add(TKey key, TNotification notification)
		{
			using (ReaderLockSlimWrapper readerLockSlimWrapper = new ReaderLockSlimWrapper(this.objLock))
			{
				readerLockSlimWrapper.AcquireLock();
				this.activeSet.AddOrUpdate(key, notification, delegate(TKey originalKey, TNotification originalValue)
				{
					Interlocked.Increment(ref this.discardedNotificationCounter);
					this.Merge(notification, originalValue);
					return notification;
				});
			}
			this.CheckConditionAndDrainNotificationBatch(this.batchSize);
		}

		protected abstract void Merge(TNotification notificationDst, TNotification notificationSrc);

		protected abstract void DrainNotifications(ConcurrentDictionary<TKey, TNotification> notifications);

		protected virtual void ReportDiscardedNotifications(int discarded)
		{
		}

		protected virtual void ReportDrainNotificationsException(AggregateException error)
		{
		}

		protected virtual void ReportBatchCancelled()
		{
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<BaseNotificationBatchManager<TKey, TNotification>>(this);
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				this.running = false;
				if (this.timer != null)
				{
					this.timer.Stop();
					this.timer.Dispose();
					this.timer = null;
				}
				if (this.objLock != null)
				{
					this.objLock.Dispose();
					this.objLock = null;
				}
			}
		}

		protected bool CheckCancellation()
		{
			if (!this.running)
			{
				this.ReportBatchCancelled();
				return true;
			}
			return false;
		}

		private void OnTimedEvent(object source, ElapsedEventArgs e)
		{
			if (!this.running)
			{
				return;
			}
			this.CheckConditionAndDrainNotificationBatch(1U);
		}

		private void CheckConditionAndDrainNotificationBatch(uint checkValue)
		{
			ConcurrentDictionary<TKey, TNotification> concurrentDictionary = null;
			using (UpgradeableLockSlimWrapper upgradeableLockSlimWrapper = new UpgradeableLockSlimWrapper(this.objLock))
			{
				upgradeableLockSlimWrapper.AcquireLock();
				if ((long)this.activeSet.Count >= (long)((ulong)checkValue))
				{
					using (WriterLockSlimWrapper writerLockSlimWrapper = new WriterLockSlimWrapper(this.objLock))
					{
						writerLockSlimWrapper.AcquireLock();
						if ((long)this.activeSet.Count >= (long)((ulong)checkValue))
						{
							concurrentDictionary = this.activeSet;
							this.activeSet = new ConcurrentDictionary<TKey, TNotification>();
						}
					}
				}
			}
			if (concurrentDictionary != null)
			{
				int num = Interlocked.Exchange(ref this.discardedNotificationCounter, 0);
				if (num > 0)
				{
					this.ReportDiscardedNotifications(num);
				}
				this.StartDrainingNotifications(concurrentDictionary);
			}
		}

		private void StartDrainingNotifications(ConcurrentDictionary<TKey, TNotification> notifications)
		{
			if (notifications != null)
			{
				if (this.batchSize == 0U)
				{
					this.DrainNotifications(notifications);
					return;
				}
				Task task = new Task(delegate()
				{
					this.DrainNotifications(notifications);
				});
				task.ContinueWith(delegate(Task t)
				{
					if (t.Exception != null)
					{
						this.ReportDrainNotificationsException(t.Exception);
					}
				}, TaskContinuationOptions.NotOnRanToCompletion | TaskContinuationOptions.NotOnCanceled | TaskContinuationOptions.ExecuteSynchronously);
				task.Start();
			}
		}

		private readonly uint batchSize;

		private int discardedNotificationCounter;

		private ReaderWriterLockSlim objLock = new ReaderWriterLockSlim();

		private volatile bool running = true;

		private System.Timers.Timer timer;

		private ConcurrentDictionary<TKey, TNotification> activeSet;
	}
}
