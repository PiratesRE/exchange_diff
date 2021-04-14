using System;
using Microsoft.Exchange.Clients.Owa2.Server.Diagnostics;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class SubscribeToNotification : SubscribeToNotificationBase
	{
		public SubscribeToNotification(NotificationSubscribeJsonRequest request, CallContext callContext, SubscriptionData[] subscriptionData) : base(request, callContext, subscriptionData)
		{
		}

		protected override void InternalCreateASubscription(IMailboxContext userContext, SubscriptionData subscription, bool remoteSubscription)
		{
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			UserContext userContext2 = userContext as UserContext;
			switch (subscription.Parameters.NotificationType)
			{
			case NotificationType.HierarchyNotification:
				if (userContext.NotificationManager == null)
				{
					throw new OwaInvalidOperationException("User context does not have a valid NotificationManager. Can't access HierarchyNotificationManager.");
				}
				this.metricType = SubscribeToNotificationMetadata.HierarchyNotificationLatency;
				userContext.NotificationManager.SubscribeToHierarchyNotification(subscription.SubscriptionId);
				return;
			case NotificationType.ReminderNotification:
				if (userContext.NotificationManager == null)
				{
					throw new OwaInvalidOperationException("User context does not have a valid NotificationManager. Can't access ReminderNotificationManager.");
				}
				this.metricType = SubscribeToNotificationMetadata.ReminderNotificationLatency;
				userContext.NotificationManager.SubscribeToReminderNotification(subscription.SubscriptionId);
				return;
			case NotificationType.PlayOnPhoneNotification:
				this.metricType = SubscribeToNotificationMetadata.PlayOnPhoneNotificationLatency;
				if (userContext2 == null)
				{
					throw new OwaInvalidOperationException("UserContext isn't a full user context. Can't access PlayOnPhoneNotificationManager.");
				}
				userContext2.PlayOnPhoneNotificationManager.SubscribeToPlayOnPhoneNotification(subscription.SubscriptionId, subscription.Parameters);
				return;
			case NotificationType.InstantMessageNotification:
				this.metricType = SubscribeToNotificationMetadata.InstantMessageNotificationLatency;
				if (userContext2 == null)
				{
					throw new OwaInvalidOperationException("UserContext isn't a full user context. Can't access InstantMessageManager.");
				}
				if (userContext2.InstantMessageManager != null)
				{
					userContext2.InstantMessageManager.Subscribe(subscription.SubscriptionId);
					return;
				}
				return;
			case NotificationType.NewMailNotification:
				if (userContext.NotificationManager == null)
				{
					throw new OwaInvalidOperationException("User context does not have a valid NotificationManager. Can't access NewMailNotificationManager.");
				}
				this.metricType = SubscribeToNotificationMetadata.NewMailNotificationLatency;
				userContext.NotificationManager.SubscribeToNewMailNotification(subscription.SubscriptionId, subscription.Parameters);
				return;
			case NotificationType.UnseenItemNotification:
				if (userContext.NotificationManager == null)
				{
					throw new OwaInvalidOperationException("User context does not have a valid NotificationManager.");
				}
				this.metricType = SubscribeToNotificationMetadata.UnseenItemNotificationLatency;
				userContext.NotificationManager.SubscribeToUnseenCountNotification(subscription.SubscriptionId, subscription.Parameters, base.CallContext.ADRecipientSessionContext.GetADRecipientSession());
				return;
			case NotificationType.GroupAssociationNotification:
			{
				if (userContext.NotificationManager == null)
				{
					throw new OwaInvalidOperationException("User context does not have a valid NotificationManager. Can't access GroupAssociationNotificationManager.");
				}
				IRecipientSession adrecipientSession = base.CallContext.ADRecipientSessionContext.GetADRecipientSession();
				userContext.NotificationManager.SubscribeToGroupAssociationNotification(subscription.SubscriptionId, adrecipientSession);
				return;
			}
			case NotificationType.PeopleIKnowNotification:
				if (userContext.NotificationManager == null)
				{
					throw new OwaInvalidOperationException("User context does not have a valid NotificationManager. Can't access PeopleIKnowNotificationManager.");
				}
				this.metricType = SubscribeToNotificationMetadata.PeopleIKnowNotificationLatency;
				userContext.NotificationManager.SubscribeToRowNotification(subscription.SubscriptionId, subscription.Parameters, this.request.Header.TimeZoneContext.TimeZoneDefinition.ExTimeZone, base.CallContext, false);
				return;
			}
			base.InternalCreateASubscription(userContext, subscription, false);
		}
	}
}
