using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Extensions;
using Microsoft.Exchange.PushNotifications.CrimsonEvents;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal abstract class PushNotificationPublisher<TNotif, TChannel> : PushNotificationPublisherBase where TNotif : PushNotification where TChannel : PushNotificationChannel<TNotif>
	{
		protected PushNotificationPublisher(PushNotificationPublisherSettings settings, ITracer tracer, IThrottlingManager throttlingManager = null, List<IPushNotificationMapping<TNotif>> mappings = null, PushNotificationQueue<TNotif> notificationQueue = null, IPushNotificationOptics optics = null) : base(settings, tracer)
		{
			this.Counters = PublisherCounters.GetInstance(base.AppId);
			this.QueueSizeCounter = new ItemCounter(this.Counters.QueueSize, this.Counters.QueueSizePeak, this.Counters.QueueSizeTotal);
			this.throttlingManager = throttlingManager;
			this.mappings = new Dictionary<Type, IPushNotificationMapping<TNotif>>();
			if (mappings != null)
			{
				foreach (IPushNotificationMapping<TNotif> pushNotificationMapping in mappings)
				{
					this.mappings.Add(pushNotificationMapping.InputType, pushNotificationMapping);
				}
			}
			this.notificationQueue = (notificationQueue ?? new PushNotificationQueue<TNotif>(base.Settings.QueueSize));
			this.optics = (optics ?? PushNotificationOptics.Default);
			this.cancellationTokenSource = new CancellationTokenSource();
			this.channelThreads = new Thread[base.Settings.NumberOfChannels];
			for (int i = 0; i < base.Settings.NumberOfChannels; i++)
			{
				this.channelThreads[i] = new Thread(new ThreadStart(this.ConsumeNotifications));
				this.channelThreads[i].Start();
			}
		}

		private PublisherCountersInstance Counters { get; set; }

		private ItemCounter QueueSizeCounter { get; set; }

		private CancellationToken CancelToken
		{
			get
			{
				return this.cancellationTokenSource.Token;
			}
		}

		public override void Publish(Notification notification, PushNotificationPublishingContext context)
		{
			ArgumentValidator.ThrowIfNull("notification", notification);
			ArgumentValidator.ThrowIfNull("context", context);
			base.CheckDisposed();
			if (!notification.IsValid)
			{
				this.optics.ReportDiscardedByValidation(notification);
				return;
			}
			IPushNotificationMapping<TNotif> pushNotificationMapping;
			if (!this.mappings.TryGetValue(notification.GetType(), out pushNotificationMapping))
			{
				this.optics.ReportDiscardedByUnknownMapping(notification);
				return;
			}
			TNotif tnotif;
			if (pushNotificationMapping.TryMap(notification, context, out tnotif))
			{
				this.optics.ReportProcessed(notification, tnotif, context);
				this.Publish(tnotif);
				return;
			}
			this.optics.ReportDiscardedByFailedMapping(notification);
		}

		public override void Publish(PushNotification notification)
		{
			TNotif tnotif = notification as TNotif;
			if (tnotif == null)
			{
				this.optics.ReportDiscardedByMissmatchingType(notification);
				return;
			}
			this.Publish(notification as TNotif);
		}

		public void Publish(TNotif notification)
		{
			ArgumentValidator.ThrowIfNull("notification", notification);
			base.CheckDisposed();
			if (PushNotificationsCrimsonEvents.NotificationToPublish.IsEnabled(PushNotificationsCrimsonEvent.Provider))
			{
				PushNotificationsCrimsonEvents.NotificationToPublish.Log<string, string>(notification.AppId, notification.ToFullString());
			}
			PushNotificationQueueItem<TNotif> pushNotificationQueueItem = this.PreprocessNotification(notification);
			if (pushNotificationQueueItem != null)
			{
				this.AddToNotificationQueue(pushNotificationQueueItem);
			}
		}

		protected abstract TChannel CreateNotificationChannel();

		protected virtual bool TryPreprocess(TNotif notification)
		{
			if (notification.IsValid)
			{
				OverBudgetException obe;
				if (this.throttlingManager == null || this.throttlingManager.TryApproveNotification(notification, out obe))
				{
					return true;
				}
				this.optics.ReportDiscardedByThrottling(notification, obe);
			}
			else
			{
				this.OnInvalidNotificationFound(notification, new InvalidPushNotificationException(notification.ValidationErrors[0]));
			}
			return false;
		}

		protected void OnInvalidNotificationFound(TNotif notification, Exception ex)
		{
			this.optics.ReportDiscardedByValidation(notification, ex);
			if (this.throttlingManager != null)
			{
				this.throttlingManager.ReportInvalidNotifications(notification);
			}
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				this.notificationQueue.CompleteAdding();
				this.cancellationTokenSource.Cancel();
				for (int i = 0; i < this.channelThreads.Length; i++)
				{
					try
					{
						this.channelThreads[i].Join();
						this.channelThreads[i] = null;
					}
					catch (ThreadStateException exception)
					{
						base.Tracer.TraceError<int, string>((long)this.GetHashCode(), "[InternalDispose] ThreadStateException for channel {0}: '{1}'", i, exception.ToTraceString());
					}
					catch (ThreadInterruptedException exception2)
					{
						base.Tracer.TraceError<int, string>((long)this.GetHashCode(), "[InternalDispose] ThreadStateException for channel {0}: '{1}'", i, exception2.ToTraceString());
					}
				}
				this.notificationQueue.Dispose();
				this.cancellationTokenSource.Dispose();
				this.notificationQueue = null;
				this.cancellationTokenSource = null;
				this.channelThreads = null;
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<PushNotificationPublisher<TNotif, TChannel>>(this);
		}

		private PushNotificationQueueItem<TNotif> PreprocessNotification(TNotif notification)
		{
			AverageTimeCounterBase averageTimeCounterBase = new AverageTimeCounterBase(this.Counters.AveragePreprocessTime, this.Counters.AveragePreprocessTimeBase, true);
			if (this.TryPreprocess(notification))
			{
				averageTimeCounterBase.Stop();
				base.Tracer.TraceDebug<TNotif>((long)this.GetHashCode(), "[PushNotificationPublisher] Preprocess notification succeeded: '{0}'.", notification);
				return new PushNotificationQueueItem<TNotif>
				{
					Notification = notification,
					QueueTimeCounter = new AverageTimeCounterBase(this.Counters.AverageQueueItemTime, this.Counters.AverageQueueItemTimeBase)
				};
			}
			this.Counters.PreprocessError.Increment();
			this.Counters.TotalNotificationsDiscarded.Increment();
			return null;
		}

		private void AddToNotificationQueue(PushNotificationQueueItem<TNotif> prepNotification)
		{
			bool flag = false;
			try
			{
				prepNotification.QueueTimeCounter.Start();
				flag = this.notificationQueue.TryAdd(prepNotification, base.Settings.AddTimeout, this.CancelToken);
				if (flag)
				{
					this.QueueSizeCounter.Increment();
					base.Tracer.TraceDebug<TNotif>((long)this.GetHashCode(), "[PushNotificationPublisher] Notification added to the queue. '{0}'", prepNotification.Notification);
				}
				else
				{
					base.Tracer.TraceWarning<TNotif>((long)this.GetHashCode(), "[PushNotificationPublisher] Unable to add notification to the queue.'{0}'", prepNotification.Notification);
				}
			}
			catch (OperationCanceledException ex)
			{
				throw new ObjectDisposedException(ex.Message, ex);
			}
			catch (InvalidOperationException ex2)
			{
				throw new ObjectDisposedException(ex2.Message, ex2);
			}
			finally
			{
				if (!flag)
				{
					this.Counters.TotalNotificationsDiscarded.Increment();
					this.Counters.EnqueueError.Increment();
				}
			}
		}

		private void ConsumeNotifications()
		{
			bool flag = false;
			while (!flag)
			{
				flag = true;
				try
				{
					PushNotificationQueueItem<TNotif> pushNotificationQueueItem;
					if (this.notificationQueue.TryTake(out pushNotificationQueueItem, -1, this.CancelToken))
					{
						pushNotificationQueueItem.QueueTimeCounter.Stop();
						this.QueueSizeCounter.Decrement();
						using (TChannel tchannel = this.CreateNotificationChannel())
						{
							tchannel.InvalidNotificationDetected += this.ChannelFoundInvalidNotification;
							this.SendNotification(tchannel, pushNotificationQueueItem.Notification);
							foreach (PushNotificationQueueItem<TNotif> pushNotificationQueueItem2 in this.notificationQueue.GetConsumingEnumerable(this.CancelToken))
							{
								pushNotificationQueueItem2.QueueTimeCounter.Stop();
								this.QueueSizeCounter.Decrement();
								this.SendNotification(tchannel, pushNotificationQueueItem2.Notification);
							}
							continue;
						}
					}
					base.Tracer.TraceError((long)this.GetHashCode(), "[PushNotificationPublisher] TryTake returned false");
					flag = false;
				}
				catch (OperationCanceledException)
				{
					base.Tracer.TraceDebug((long)this.GetHashCode(), "[PushNotificationPublisher] Done by OperationCanceledException");
				}
				catch (ObjectDisposedException)
				{
					base.Tracer.TraceDebug((long)this.GetHashCode(), "[PushNotificationPublisher] Done by ObjectDisposedException");
				}
				catch (Exception exception)
				{
					base.Tracer.TraceError<string>((long)this.GetHashCode(), "[PushNotificationPublisher] Unexpected exception '{0}'", exception.ToTraceString());
					PushNotificationsCrimsonEvents.PushNotificationPublisherConsumeError.Log<string, string, string>(base.AppId, string.Empty, exception.ToTraceString());
					flag = base.IsDisposed;
				}
			}
		}

		private void SendNotification(TChannel channel, TNotif notification)
		{
			base.Tracer.TraceDebug<TNotif>((long)this.GetHashCode(), "[PushNotificationPublisher] Sending notification. '{0}'", notification);
			using (new AverageTimeCounter(this.Counters.AveragePublisherSendTime, this.Counters.AveragePublisherSendTimeBase))
			{
				channel.Send(notification, this.CancelToken);
			}
			this.Counters.TotalNotificationsSent.Increment();
			base.Tracer.TraceDebug<TNotif>((long)this.GetHashCode(), "[PushNotificationPublisher] Notification sent. '{0}'", notification);
		}

		private void ChannelFoundInvalidNotification(object sender, InvalidNotificationEventArgs e)
		{
			this.OnInvalidNotificationFound((TNotif)((object)e.Notification), e.Exception);
		}

		private PushNotificationQueue<TNotif> notificationQueue;

		private Dictionary<Type, IPushNotificationMapping<TNotif>> mappings;

		private IThrottlingManager throttlingManager;

		private IPushNotificationOptics optics;

		private CancellationTokenSource cancellationTokenSource;

		private Thread[] channelThreads;
	}
}
