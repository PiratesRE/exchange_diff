using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.Exchange.PushNotifications;
using Microsoft.Exchange.PushNotifications.Client;
using Microsoft.Exchange.PushNotifications.Extensions;
using Microsoft.Exchange.PushNotifications.Utils;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.ExchangeService;

namespace Microsoft.Exchange.Services.Core
{
	internal class OutlookPushNotificationBatchManager : BaseNotificationBatchManager<string, PendingOutlookPushNotification>
	{
		private OutlookPushNotificationBatchManager(ExchangeServiceTraceDelegate traceDelegate, IOutlookPushNotificationSubscriptionCache subscriptionCache, IOutlookPushNotificationSerializer notificationSerializer) : this(traceDelegate, subscriptionCache, notificationSerializer, new OutlookPublisherServiceProxy(null), 10U, 1000U)
		{
		}

		internal OutlookPushNotificationBatchManager(ExchangeServiceTraceDelegate traceDelegate, IOutlookPushNotificationSubscriptionCache subscriptionCache, IOutlookPushNotificationSerializer notificationSerializer, IOutlookPublisherServiceContract publisherService, uint batchTimerInSeconds, uint batchSize) : base(batchTimerInSeconds, batchSize)
		{
			this.traceDelegate = traceDelegate;
			this.subscriptionCache = subscriptionCache;
			this.publisherService = publisherService;
			this.notificationSerializer = notificationSerializer;
		}

		internal static OutlookPushNotificationBatchManager GetInstance(ExchangeServiceTraceDelegate traceDelegate, IOutlookPushNotificationSubscriptionCache subscriptionCache, IOutlookPushNotificationSerializer notificationSerializer)
		{
			if (OutlookPushNotificationBatchManager.instance == null)
			{
				lock (OutlookPushNotificationBatchManager.instanceLock)
				{
					if (OutlookPushNotificationBatchManager.instance == null)
					{
						OutlookPushNotificationBatchManager.instance = new OutlookPushNotificationBatchManager(traceDelegate, subscriptionCache, notificationSerializer);
					}
				}
			}
			if (OutlookPushNotificationBatchManager.instance.subscriptionCache != subscriptionCache)
			{
				throw new ArgumentException("Changing subscriptionCache is not supported");
			}
			return OutlookPushNotificationBatchManager.instance;
		}

		protected override void Merge(PendingOutlookPushNotification notificationDst, PendingOutlookPushNotification notificationSrc)
		{
			notificationDst.Merge(notificationSrc);
		}

		protected override void ReportDrainNotificationsException(AggregateException error)
		{
			this.traceDelegate("OutlookPushNotificationBatchManager.ReportDrainNotificationsException", "Error: Caught exception " + error.ToString());
		}

		protected override void DrainNotifications(ConcurrentDictionary<string, PendingOutlookPushNotification> pendingNotifications)
		{
			this.traceDelegate("OutlookPushNotificationBatchManager.DrainNotifications", "Processing " + pendingNotifications.Count + " notifications");
			foreach (string text in pendingNotifications.Keys)
			{
				OutlookNotificationBatch batch = new OutlookNotificationBatch();
				if (base.CheckCancellation())
				{
					break;
				}
				this.AddPendingNotificationToBatch(batch, text, pendingNotifications[text]);
				if (!batch.IsEmpty)
				{
					if (base.CheckCancellation())
					{
						break;
					}
					this.publisherService.BeginPublishOutlookNotifications(batch, delegate(IAsyncResult asyncResult)
					{
						try
						{
							this.publisherService.EndPublishOutlookNotifications(asyncResult);
							this.traceDelegate("OutlookPushNotificationBatchManager.DrainNotifications", "EndPublishOutlookNotifications succeeded with " + batch.Count + " notifications");
						}
						catch (Exception ex)
						{
							this.traceDelegate("OutlookPushNotificationBatchManager.DrainNotifications", "Error: IOutlookPublisherServiceContract.BeginPublishOutlookNotifications encountered exception " + ex.ToString());
						}
					}, null);
				}
			}
		}

		private void AddPendingNotificationToBatch(OutlookNotificationBatch batch, string notificationContext, PendingOutlookPushNotification pendingNotification)
		{
			string tenantId;
			List<OutlookServiceNotificationSubscription> activeSubscriptions;
			if (!this.subscriptionCache.QueryMailboxSubscriptions(notificationContext, out tenantId, out activeSubscriptions))
			{
				this.traceDelegate("OutlookPushNotificationBatchManager.AddPendingNotificationToBatch", "Warning: Ignored attempt to publish notifications for an uncached mailbox " + notificationContext.ToNullableString());
				return;
			}
			this.AddPayloadToBatchForAppId(batch, this.notificationSerializer.ConvertToPayloadForHxCalendar(pendingNotification), tenantId, activeSubscriptions, OutlookServiceNotificationSubscription.AppId_HxCalendar);
			this.AddPayloadToBatchForAppId(batch, this.notificationSerializer.ConvertToPayloadForHxMail(pendingNotification), tenantId, activeSubscriptions, OutlookServiceNotificationSubscription.AppId_HxMail);
		}

		private void AddPayloadToBatchForAppId(OutlookNotificationBatch batch, byte[] payload, string tenantId, List<OutlookServiceNotificationSubscription> activeSubscriptions, string appId)
		{
			if (payload != null)
			{
				List<OutlookNotificationRecipient> list = new List<OutlookNotificationRecipient>();
				foreach (OutlookServiceNotificationSubscription outlookServiceNotificationSubscription in activeSubscriptions)
				{
					if (outlookServiceNotificationSubscription.AppId == appId)
					{
						list.Add(new OutlookNotificationRecipient(outlookServiceNotificationSubscription.PackageId, (outlookServiceNotificationSubscription.PackageId == outlookServiceNotificationSubscription.AppId) ? outlookServiceNotificationSubscription.DeviceNotificationId : outlookServiceNotificationSubscription.SubscriptionId));
					}
				}
				if (list.Count > 0)
				{
					OutlookNotification outlookNotification = new OutlookNotification(new OutlookNotificationPayload(tenantId, payload), list);
					if (outlookNotification.IsValid)
					{
						this.traceDelegate("OutlookPushNotificationBatchManager.AddPayloadToBatchForAppId", "Adding notification: " + outlookNotification.ToFullString());
						batch.Add(outlookNotification);
						return;
					}
					this.traceDelegate("OutlookPushNotificationBatchManager.AddPayloadToBatchForAppId", "Warning: Ignored invalid notification" + outlookNotification.ToNullableString(null));
				}
			}
		}

		private static OutlookPushNotificationBatchManager instance = null;

		private static readonly object instanceLock = new object();

		private readonly ExchangeServiceTraceDelegate traceDelegate;

		private readonly IOutlookPushNotificationSubscriptionCache subscriptionCache;

		private readonly IOutlookPushNotificationSerializer notificationSerializer;

		private readonly IOutlookPublisherServiceContract publisherService;
	}
}
