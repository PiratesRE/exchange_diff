using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.PushNotifications;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.PushNotifications.CrimsonEvents;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class ApnsPublisher : PushNotificationPublisher<ApnsNotification, ApnsChannel>
	{
		public ApnsPublisher(ApnsPublisherSettings publisherSettings, IApnsFeedbackProvider feedbackProvider, IThrottlingManager throttlingManager, List<IPushNotificationMapping<ApnsNotification>> mappings = null) : this(publisherSettings, feedbackProvider, throttlingManager, ExTraceGlobals.ApnsPublisherTracer, mappings)
		{
		}

		protected ApnsPublisher(ApnsPublisherSettings publisherSettings, IApnsFeedbackProvider feedbackProvider, IThrottlingManager throttlingManager, ITracer tracer, List<IPushNotificationMapping<ApnsNotification>> mappings = null) : base(publisherSettings, tracer, throttlingManager, mappings, null, null)
		{
			ArgumentValidator.ThrowIfNull("feedbackProvider", feedbackProvider);
			this.channelSettings = publisherSettings.ChannelSettings;
			this.feedbackProvider = feedbackProvider;
		}

		protected override ApnsChannel CreateNotificationChannel()
		{
			return new ApnsChannel(this.channelSettings, base.Tracer);
		}

		protected override bool TryPreprocess(ApnsNotification notification)
		{
			if (!base.TryPreprocess(notification))
			{
				return false;
			}
			ApnsFeedbackValidationResult apnsFeedbackValidationResult = this.feedbackProvider.ValidateNotification(notification);
			base.Tracer.TraceDebug<ApnsNotification, string>((long)this.GetHashCode(), "[TryPreprocess] FeedbackValidationResult for '{0}': '{1}'.", notification, apnsFeedbackValidationResult.ToString());
			if (apnsFeedbackValidationResult == ApnsFeedbackValidationResult.Uncertain && (ExDateTime)notification.LastSubscriptionUpdate < ExDateTime.UtcNow.Subtract(TimeSpan.FromDays(3.0)))
			{
				base.Tracer.TraceDebug<ApnsNotification, DateTime>((long)this.GetHashCode(), "[TryPreprocess] Expiring '{0}' because the last subscription update was too long ago: '{1}'.", notification, notification.LastSubscriptionUpdate);
				apnsFeedbackValidationResult = ApnsFeedbackValidationResult.Expired;
			}
			if (apnsFeedbackValidationResult == ApnsFeedbackValidationResult.Expired)
			{
				ExTraceGlobals.PublisherManagerTracer.TraceError<string>((long)this.GetHashCode(), "[TryPreprocess] APNs notification dropped by feedback: '{0}'.", notification.ToFullString());
				PushNotificationTracker.ReportDropped(notification, "ApnsTokenExpired");
				if (PushNotificationsCrimsonEvents.ApnsPublisherFeedbackBlock.IsEnabled(PushNotificationsCrimsonEvent.Provider))
				{
					PushNotificationsCrimsonEvents.ApnsPublisherFeedbackBlock.Log<string, string>(notification.AppId, notification.ToFullString());
				}
				return false;
			}
			return true;
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<ApnsPublisher>(this);
		}

		private ApnsChannelSettings channelSettings;

		private IApnsFeedbackProvider feedbackProvider;
	}
}
