using System;
using System.Web;
using System.Xml;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class GetStreamingEvents : NotificationCommandBase<GetStreamingEventsRequest, XmlNode>, IAsyncServiceCommand
	{
		public GetStreamingEvents(CallContext callContext, GetStreamingEventsRequest request) : base(callContext, request)
		{
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			if (this.CompleteRequestAsyncCallback == null)
			{
				using (EwsResponseWireWriter ewsResponseWireWriter = EwsResponseWireWriter.Create(CallContext.Current))
				{
					try
					{
						ExTraceGlobals.SubscriptionsTracer.TraceDebug((long)this.GetHashCode(), "[GetStreamingEvents::GetResponse] Writing the response.");
						GetStreamingEventsSoapResponse getStreamingEventsSoapResponse = new GetStreamingEventsSoapResponse();
						getStreamingEventsSoapResponse.Body = StreamingConnection.CreateErrorResponse(base.Result.Error, base.Request.SubscriptionIds);
						ewsResponseWireWriter.WriteResponseToWire(getStreamingEventsSoapResponse, false);
						getStreamingEventsSoapResponse.Body = StreamingConnection.CreateConnectionResponse(ConnectionStatus.Closed);
						ewsResponseWireWriter.WriteResponseToWire(getStreamingEventsSoapResponse, true);
						ewsResponseWireWriter.FinishWritesAndCompleteResponse(null);
					}
					catch (HttpException arg)
					{
						ExTraceGlobals.SubscriptionsTracer.TraceDebug<HttpException>((long)this.GetHashCode(), "[GetStreamingEvents::GetResponse] Exception occurred while writing the response: {0}", arg);
					}
				}
			}
			return null;
		}

		internal override ServiceResult<XmlNode> Execute()
		{
			ExTraceGlobals.GetEventsCallTracer.TraceDebug((long)this.GetHashCode(), "GetStreamingEvents.Execute called");
			ServiceCommandBase.ThrowIfNullOrEmpty<string>(base.Request.SubscriptionIds, "SubscriptionIds", "GetStreamingEvents:Execute");
			TimeSpan connectionLifetime = TimeSpan.FromMinutes((double)base.Request.ConnectionTimeout);
			StreamingConnection.CreateConnection(CallContext.Current, base.Request.SubscriptionIds, connectionLifetime, this.CompleteRequestAsyncCallback);
			return new ServiceResult<XmlNode>(null);
		}

		public CompleteRequestAsyncCallback CompleteRequestAsyncCallback { get; set; }
	}
}
