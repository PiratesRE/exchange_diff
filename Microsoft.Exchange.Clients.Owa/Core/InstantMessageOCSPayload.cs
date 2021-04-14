using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.InstantMessaging;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal sealed class InstantMessageOCSPayload : InstantMessagePayload
	{
		internal InstantMessageOCSPayload(UserContext userContext) : base(userContext)
		{
			this.deliverySuccessNotifications = new List<InstantMessageOCSPayload.DeliverySuccessNotification>();
		}

		internal void RegisterDeliverySuccessNotification(IIMModality context, int messageId)
		{
			lock (this.deliverySuccessNotifications)
			{
				this.deliverySuccessNotifications.Add(new InstantMessageOCSPayload.DeliverySuccessNotification(context, messageId));
			}
		}

		public override string ReadDataAndResetState()
		{
			string result = base.ReadDataAndResetState();
			lock (this.deliverySuccessNotifications)
			{
				foreach (InstantMessageOCSPayload.DeliverySuccessNotification deliverySuccessNotification in this.deliverySuccessNotifications)
				{
					ExTraceGlobals.InstantMessagingTracer.TraceDebug<int>((long)this.GetHashCode(), "InstantMessageOCSPayload.ReadDataAndResetState. BeginNotifyDeliverySuccess Message Id: {0}", deliverySuccessNotification.MessageId);
					deliverySuccessNotification.Context.BeginNotifyDeliverySuccess(deliverySuccessNotification.MessageId, new AsyncCallback(this.NotifyDeliverySuccessCallback), deliverySuccessNotification.Context);
				}
				this.deliverySuccessNotifications.Clear();
			}
			return result;
		}

		protected override void Cancel()
		{
			base.Cancel();
			lock (this.deliverySuccessNotifications)
			{
				this.deliverySuccessNotifications.Clear();
			}
		}

		private void NotifyDeliverySuccessCallback(IAsyncResult result)
		{
			IIMModality iimmodality = null;
			try
			{
				ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSPayload.NotifyDeliverySuccessCallback.");
				iimmodality = (result.AsyncState as IIMModality);
				if (iimmodality == null)
				{
					ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSPayload.NotifyDeliverySuccessCallback. instantMessaging is null.");
				}
				else
				{
					iimmodality.EndNotifyDeliverySuccess(result);
				}
			}
			catch (InstantMessagingException ex)
			{
				if (!iimmodality.IsConnected)
				{
					ExTraceGlobals.InstantMessagingTracer.TraceDebug<InstantMessagingException>((long)this.GetHashCode(), "InstantMessageOCSPayload.NotifyDeliverySuccessCallback. Ignoring exception because IM conversation is not connected : {0}.", ex);
				}
				else
				{
					InstantMessagingError code = ex.Code;
					switch (code)
					{
					case 18102:
						if (ex.SubCode == 9)
						{
							ExTraceGlobals.InstantMessagingTracer.TraceError<InstantMessagingException>((long)this.GetHashCode(), "InstantMessageOCSPayload.NotifyDeliverySuccessCallback. OcsFailureResponse. {0}", ex);
							goto IL_113;
						}
						InstantMessageUtilities.SendWatsonReport("InstantMessageOCSPayload.NotifyDeliverySuccessCallback", this.userContext, ex);
						goto IL_113;
					case 18103:
						break;
					case 18104:
						ExTraceGlobals.InstantMessagingTracer.TraceError<InstantMessagingException>((long)this.GetHashCode(), "InstantMessageOCSPayload.NotifyDeliverySuccessCallback. Failed to send delivery success notification. OcsFailureResponse. {0}", ex);
						goto IL_113;
					default:
						if (code == 18201)
						{
							ExTraceGlobals.InstantMessagingTracer.TraceError<InstantMessagingException>((long)this.GetHashCode(), "InstantMessageOCSPayload.NotifyDeliverySuccessCallback. OcsFailureResponse. {0}", ex);
							goto IL_113;
						}
						break;
					}
					InstantMessageUtilities.SendWatsonReport("InstantMessageOCSPayload.NotifyDeliverySuccessCallback", this.userContext, ex);
				}
				IL_113:;
			}
			catch (Exception exception)
			{
				InstantMessageUtilities.SendWatsonReport("InstantMessageOCSPayload.NotifyDeliverySuccessCallback", this.userContext, exception);
			}
		}

		private List<InstantMessageOCSPayload.DeliverySuccessNotification> deliverySuccessNotifications;

		private struct DeliverySuccessNotification
		{
			internal DeliverySuccessNotification(IIMModality context, int messageId)
			{
				this.Context = context;
				this.MessageId = messageId;
			}

			internal IIMModality Context;

			internal int MessageId;
		}
	}
}
