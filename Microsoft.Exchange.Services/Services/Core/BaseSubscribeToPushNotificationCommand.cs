using System;
using Microsoft.Exchange.Data.PushNotifications;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.Extensions;
using Microsoft.Exchange.PushNotifications.CrimsonEvents;
using Microsoft.Exchange.PushNotifications.Extensions;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal abstract class BaseSubscribeToPushNotificationCommand<RequestType> : SingleStepServiceCommand<RequestType, ServiceResultNone> where RequestType : BaseSubscribeToPushNotificationRequest
	{
		protected BaseSubscribeToPushNotificationCommand(CallContext callContext, RequestType request) : base(callContext, request)
		{
			this.PushNotificationSubscription = ((request != null) ? request.SubscriptionRequest : null);
		}

		internal override ServiceResult<ServiceResultNone> Execute()
		{
			string subscriptionId = this.GetSubscriptionId();
			MailboxSession session = null;
			ServiceResult<ServiceResultNone> result;
			try
			{
				session = base.MailboxIdentityMailboxSession;
				BaseSubscribeToPushNotificationCommand<SubscribeToPushNotificationRequest>.ValidateSubscription(this.ClassName, subscriptionId, this.PushNotificationSubscription, session, this.ArgumentNullEvent, this.InvalidNotificationTypeEvent);
				BaseSubscribeToPushNotificationCommand<SubscribeToPushNotificationRequest>.LogRequest(this.ClassName, this.RequestedEvent, this.PushNotificationSubscription, subscriptionId, session);
				this.InternalExecute(subscriptionId);
				result = new ServiceResult<ServiceResultNone>(new ServiceResultNone());
			}
			catch (Exception ex)
			{
				if (ex is SaveConflictException)
				{
					BaseSubscribeToPushNotificationCommand<SubscribeToPushNotificationRequest>.LogException(this.ClassName, PushNotificationsCrimsonEvents.SubscriptionKnownError, this.PushNotificationSubscription, subscriptionId, session, ex);
				}
				else if (!(ex is InvalidRequestException))
				{
					BaseSubscribeToPushNotificationCommand<SubscribeToPushNotificationRequest>.LogException(this.ClassName, this.UnexpectedErrorEvent, this.PushNotificationSubscription, subscriptionId, session, ex);
				}
				throw;
			}
			return result;
		}

		protected abstract void InternalExecute(string subscriptionId);

		private protected PushNotificationSubscription PushNotificationSubscription { protected get; private set; }

		protected abstract string ClassName { get; }

		protected abstract PushNotificationsCrimsonEvent ArgumentNullEvent { get; }

		protected abstract PushNotificationsCrimsonEvent InvalidNotificationTypeEvent { get; }

		protected abstract PushNotificationsCrimsonEvent UnexpectedErrorEvent { get; }

		protected abstract PushNotificationsCrimsonEvent RequestedEvent { get; }

		private static void ValidateSubscription(string className, string subscriptionId, PushNotificationSubscription subscription, MailboxSession session, PushNotificationsCrimsonEvent argumentNullEvent, PushNotificationsCrimsonEvent invalidNotificationTypeEvent)
		{
			if (subscription == null || string.IsNullOrEmpty(subscription.AppId) || string.IsNullOrEmpty(subscription.DeviceNotificationId) || string.IsNullOrEmpty(subscription.DeviceNotificationType))
			{
				BaseSubscribeToPushNotificationCommand<RequestType>.LogException(className, argumentNullEvent, subscription, subscriptionId, session, null);
				throw new InvalidRequestException();
			}
			if (!subscription.Platform.SupportsSubscriptions())
			{
				BaseSubscribeToPushNotificationCommand<RequestType>.LogException(className, invalidNotificationTypeEvent, subscription, subscriptionId, session, null);
				throw new InvalidRequestException();
			}
		}

		private static void LogException(string className, PushNotificationsCrimsonEvent crimsonEvent, PushNotificationSubscription subscription, string subscriptionId, MailboxSession session, Exception ex)
		{
			string text = (subscription == null) ? null : subscription.AppId;
			string text2 = (subscription == null) ? null : subscription.DeviceNotificationId;
			string text3 = (subscription == null) ? null : subscription.DeviceNotificationType;
			string text4 = (session == null) ? string.Empty : session.MailboxGuid.ToString();
			string text5 = (ex != null) ? ex.ToTraceString() : string.Empty;
			crimsonEvent.LogPeriodicGeneric(subscriptionId, CrimsonConstants.DefaultLogPeriodicSuppressionInMinutes, new object[]
			{
				text,
				text2,
				text3,
				subscriptionId,
				text4,
				text5
			});
			ExTraceGlobals.NotificationsCallTracer.TraceError(0L, "{0}.Execute: Error: SubscriptionId={1}, AppId = {2}, DeviceNotificationId = {3}, DeviceNotificationType = {4}, MailboxGuid = {5}, Exception = {6}.", new object[]
			{
				className,
				subscriptionId,
				text,
				text2,
				text3,
				text4,
				text5
			});
		}

		private static void LogRequest(string className, PushNotificationsCrimsonEvent crimsonEvent, PushNotificationSubscription subscription, string subscriptionId, MailboxSession session)
		{
			crimsonEvent.LogGeneric(new object[]
			{
				subscription.AppId,
				subscription.DeviceNotificationId,
				subscription.DeviceNotificationType,
				subscriptionId,
				subscription.SubscriptionOption,
				session.MailboxGuid
			});
			ExTraceGlobals.NotificationsCallTracer.TraceDebug(0L, "{0}.Execute: Error: SubscriptionId={1} request processed for AppId = {2}, NotificationId = {3}, NotificationType = {4}, SubscriptionOption = {5}.", new object[]
			{
				className,
				subscriptionId,
				subscription.AppId,
				subscription.DeviceNotificationId,
				subscription.DeviceNotificationType,
				subscription.SubscriptionOption
			});
		}

		private string GetSubscriptionId()
		{
			return PushNotificationSubscriptionItem.GenerateSubscriptionId(base.CallContext.OwaProtocol, base.CallContext.OwaDeviceId, base.CallContext.OwaDeviceType);
		}
	}
}
