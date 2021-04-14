using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.Extensions;
using Microsoft.Exchange.PushNotifications;
using Microsoft.Exchange.PushNotifications.Client;
using Microsoft.Exchange.PushNotifications.CrimsonEvents;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal class SubscribeToPushNotification : BaseSubscribeToPushNotificationCommand<SubscribeToPushNotificationRequest>
	{
		public SubscribeToPushNotification(CallContext callContext, SubscribeToPushNotificationRequest request) : base(callContext, request)
		{
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			return new SubscribeToPushNotificationResponse(base.Result.Code, base.Result.Error);
		}

		protected override void InternalExecute(string subscriptionId)
		{
			string hubName = null;
			string owaDeviceId = base.CallContext.OwaDeviceId;
			using (IPushNotificationStorage pushNotificationStorage = PushNotificationStorage.Create(base.MailboxIdentityMailboxSession))
			{
				using (pushNotificationStorage.CreateOrUpdateSubscriptionItem(base.MailboxIdentityMailboxSession, subscriptionId, new PushNotificationServerSubscription(base.PushNotificationSubscription, DateTime.UtcNow, owaDeviceId)))
				{
				}
				PushNotificationsCrimsonEvents.SubscriptionPosted.LogPeriodic<string, string, Guid, string>(string.Format("{0}-{1}", base.PushNotificationSubscription.AppId, base.PushNotificationSubscription.DeviceNotificationId), TimeSpan.FromHours(12.0), base.PushNotificationSubscription.DeviceNotificationId, base.PushNotificationSubscription.AppId, base.MailboxIdentityMailboxSession.MailboxGuid, pushNotificationStorage.TenantId);
				hubName = pushNotificationStorage.TenantId;
			}
			int num = (base.Request != null) ? base.Request.LastUnseenEmailCount : 0;
			string text = base.PushNotificationSubscription.AppId ?? string.Empty;
			if (num > 0)
			{
				string text2 = base.MailboxIdentityMailboxSession.MailboxGuid.ToString();
				PushNotificationsCrimsonEvents.LastUnseenEmailCount.Log<string, int, string>(text, num, text2);
				ExTraceGlobals.NotificationsCallTracer.TraceDebug<string, int, string>(0L, "SubscribeToPushNotification.InternalExecute: App with AppId '{0}' reports {1} unseen Emails in the most recent received notification for mailbox '{2}'.", text, num, text2);
			}
			try
			{
				using (AzureDeviceRegistrationServiceProxy azureDeviceRegistrationServiceProxy = new AzureDeviceRegistrationServiceProxy(null))
				{
					azureDeviceRegistrationServiceProxy.EndDeviceRegistration(azureDeviceRegistrationServiceProxy.BeginDeviceRegistration(new AzureDeviceRegistrationInfo(base.PushNotificationSubscription.DeviceNotificationId, owaDeviceId, text, base.PushNotificationSubscription.RegistrationChallenge, hubName), null, null));
				}
			}
			catch (Exception exception)
			{
				PushNotificationsCrimsonEvents.AzureDeviceRegistrationRequestFailed.LogPeriodic<string, string, string, string, string, string>(base.PushNotificationSubscription.DeviceNotificationId, CrimsonConstants.DefaultLogPeriodicSuppressionInMinutes, text, base.PushNotificationSubscription.DeviceNotificationId, base.PushNotificationSubscription.DeviceNotificationType, subscriptionId, base.MailboxIdentityMailboxSession.MailboxGuid.ToString(), exception.ToTraceString());
			}
		}

		protected override string ClassName
		{
			get
			{
				return "SubscribeToPushNotification";
			}
		}

		protected override PushNotificationsCrimsonEvent ArgumentNullEvent
		{
			get
			{
				return PushNotificationsCrimsonEvents.SubscriptionArgumentNull;
			}
		}

		protected override PushNotificationsCrimsonEvent InvalidNotificationTypeEvent
		{
			get
			{
				return PushNotificationsCrimsonEvents.SubscriptionInvalidNotificationType;
			}
		}

		protected override PushNotificationsCrimsonEvent UnexpectedErrorEvent
		{
			get
			{
				return PushNotificationsCrimsonEvents.SubscriptionUnexpectedError;
			}
		}

		protected override PushNotificationsCrimsonEvent RequestedEvent
		{
			get
			{
				return PushNotificationsCrimsonEvents.SubscriptionRequested;
			}
		}
	}
}
