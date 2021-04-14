using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.PushNotifications.CrimsonEvents;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal class UnsubscribeToPushNotification : BaseSubscribeToPushNotificationCommand<UnsubscribeToPushNotificationRequest>
	{
		public UnsubscribeToPushNotification(CallContext callContext, UnsubscribeToPushNotificationRequest request) : base(callContext, request)
		{
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			return new UnsubscribeToPushNotificationResponse(base.Result.Code, base.Result.Error);
		}

		protected override void InternalExecute(string subscriptionId)
		{
			using (IPushNotificationStorage pushNotificationStorage = PushNotificationStorage.Find(base.MailboxIdentityMailboxSession))
			{
				if (pushNotificationStorage != null)
				{
					pushNotificationStorage.DeleteSubscription(subscriptionId);
				}
			}
		}

		protected override string ClassName
		{
			get
			{
				return "UnsubscribeToPushNotification";
			}
		}

		protected override PushNotificationsCrimsonEvent ArgumentNullEvent
		{
			get
			{
				return PushNotificationsCrimsonEvents.UnsubscriptionArgumentNull;
			}
		}

		protected override PushNotificationsCrimsonEvent InvalidNotificationTypeEvent
		{
			get
			{
				return PushNotificationsCrimsonEvents.UnsubscriptionInvalidNotificationType;
			}
		}

		protected override PushNotificationsCrimsonEvent UnexpectedErrorEvent
		{
			get
			{
				return PushNotificationsCrimsonEvents.UnsubscriptionUnexpectedError;
			}
		}

		protected override PushNotificationsCrimsonEvent RequestedEvent
		{
			get
			{
				return PushNotificationsCrimsonEvents.UnsubscriptionRequested;
			}
		}
	}
}
