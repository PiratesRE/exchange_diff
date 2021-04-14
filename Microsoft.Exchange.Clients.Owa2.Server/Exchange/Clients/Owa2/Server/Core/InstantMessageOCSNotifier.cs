using System;
using System.Collections.Generic;
using Microsoft.Exchange.Clients.Owa.Server.LyncIMLogging;
using Microsoft.Exchange.InstantMessaging;
using Microsoft.Exchange.Services;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal sealed class InstantMessageOCSNotifier : InstantMessageNotifier
	{
		internal InstantMessageOCSNotifier(UserContext userContext) : base(userContext)
		{
			this.deliverySuccessNotifications = new List<DeliverySuccessNotification>();
		}

		public override IList<NotificationPayloadBase> ReadDataAndResetState()
		{
			IList<NotificationPayloadBase> result = base.ReadDataAndResetState();
			lock (this.deliverySuccessNotifications)
			{
				foreach (DeliverySuccessNotification deliverySuccessNotification in this.deliverySuccessNotifications)
				{
					ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSNotifier.ReadDataAndResetState. BeginNotifyDeliverySuccess Message Id: {0}", new object[]
					{
						deliverySuccessNotification.MessageId
					});
					deliverySuccessNotification.Provider.NotifyDeliverySuccess(deliverySuccessNotification);
				}
				this.deliverySuccessNotifications.Clear();
			}
			return result;
		}

		internal void RegisterDeliverySuccessNotification(InstantMessageOCSProvider provider, IIMModality context, int messageId, RequestDetailsLogger logger)
		{
			lock (this.deliverySuccessNotifications)
			{
				this.deliverySuccessNotifications.Add(new DeliverySuccessNotification(provider, context, messageId, logger));
			}
		}

		protected override void Cancel()
		{
			base.Cancel();
			lock (this.deliverySuccessNotifications)
			{
				this.deliverySuccessNotifications.Clear();
			}
		}

		private List<DeliverySuccessNotification> deliverySuccessNotifications;
	}
}
