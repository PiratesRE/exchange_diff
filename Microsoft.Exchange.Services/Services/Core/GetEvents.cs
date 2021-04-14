using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class GetEvents : NotificationCommandBase<GetEventsRequest, EwsNotificationType>
	{
		public GetEvents(CallContext callContext, GetEventsRequest request) : base(callContext, request)
		{
			this.subscriptionId = base.Request.SubscriptionId;
			this.lastWatermarkSent = base.Request.Watermark;
			ServiceCommandBase.ThrowIfNullOrEmpty(this.subscriptionId, "SubscriptionId", "GetEvents:PreExecuteCommand");
			ServiceCommandBase.ThrowIfNullOrEmpty(this.lastWatermarkSent, "lastWatermarkSent", "GetEvents:PreExecuteCommand");
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			BaseInfoResponse baseInfoResponse = new GetEventsResponse();
			baseInfoResponse.ProcessServiceResult<EwsNotificationType>(base.Result);
			return baseInfoResponse;
		}

		internal override ServiceResult<EwsNotificationType> Execute()
		{
			ServiceResult<EwsNotificationType> result;
			try
			{
				SubscriptionBase subscriptionBase = Subscriptions.Singleton.Get(this.subscriptionId);
				base.ValidateSubscriptionUpdate(subscriptionBase, new Type[]
				{
					typeof(PullSubscription)
				});
				EwsNotificationType events = subscriptionBase.GetEvents(this.lastWatermarkSent);
				result = new ServiceResult<EwsNotificationType>(events);
			}
			catch (EventNotFoundException)
			{
				Subscriptions.Singleton.Delete(this.subscriptionId);
				throw;
			}
			catch (FinalEventException ex)
			{
				ExTraceGlobals.GetEventsCallTracer.TraceDebug<FinalEventException, Event>((long)this.GetHashCode(), "finalEventException: {0) FinalEvent: {1}", ex, ex.FinalEvent);
				Subscriptions.Singleton.Delete(this.subscriptionId);
				throw;
			}
			catch (ReadEventsFailedException ex2)
			{
				if (ex2.InnerException != null)
				{
					ExTraceGlobals.GetEventsCallTracer.TraceDebug<Exception>((long)this.GetHashCode(), "readEventsFailedException.InnerException: {0}", ex2.InnerException);
				}
				Subscriptions.Singleton.Delete(this.subscriptionId);
				throw;
			}
			catch (ReadEventsFailedTransientException ex3)
			{
				if (ex3.InnerException != null)
				{
					ExTraceGlobals.GetEventsCallTracer.TraceDebug<Exception>((long)this.GetHashCode(), "readEventsFailedTransientException.InnerException: {0}", ex3.InnerException);
				}
				Subscriptions.Singleton.Delete(this.subscriptionId);
				throw;
			}
			return result;
		}

		private string subscriptionId;

		private string lastWatermarkSent;
	}
}
