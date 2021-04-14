using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Services.Wcf;
using Microsoft.Exchange.WorkloadManagement;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("GetStreamingEventsType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class GetStreamingEventsRequest : BaseRequest
	{
		[XmlArrayItem(ElementName = "SubscriptionId", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", Type = typeof(string))]
		[XmlArray(ElementName = "SubscriptionIds", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public string[] SubscriptionIds { get; set; }

		[XmlElement("ConnectionTimeout", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public int ConnectionTimeout { get; set; }

		internal override ServiceCommandBase GetServiceCommand(CallContext callContext)
		{
			return new GetStreamingEvents(callContext, this);
		}

		internal override BaseServerIdInfo GetProxyInfo(CallContext callContext)
		{
			return null;
		}

		internal override ITask CreateServiceTask<T>(ServiceAsyncResult<T> serviceAsyncResult)
		{
			return new AsyncServiceTask<T>(this, CallContext.Current, serviceAsyncResult);
		}

		internal override ProxyServiceTask<T> CreateProxyServiceTask<T>(ServiceAsyncResult<T> serviceAsyncResult, CallContext callContext, WebServicesInfo[] services)
		{
			return new HangingProxyServiceTask<T>(this, callContext, serviceAsyncResult, services, StreamingConnection.PeriodicConnectionCheckInterval * 2, new Func<BaseSoapResponse>(this.GetCloseClientConnectionResponse));
		}

		internal override ResourceKey[] GetResources(CallContext callContext, int taskStep)
		{
			return null;
		}

		internal override WebServicesInfo[] PerformServiceDiscovery(CallContext callContext)
		{
			if (this.SubscriptionIds.Length != 1)
			{
				return null;
			}
			return BasePullRequest.PerformServiceDiscoveryForSubscriptionId(this.SubscriptionIds[0], callContext, this);
		}

		private BaseSoapResponse GetCloseClientConnectionResponse()
		{
			GetStreamingEventsSoapResponse getStreamingEventsSoapResponse = new GetStreamingEventsSoapResponse();
			GetStreamingEventsResponse getStreamingEventsResponse = new GetStreamingEventsResponse();
			GetStreamingEventsResponseMessage getStreamingEventsResponseMessage = new GetStreamingEventsResponseMessage(ServiceResultCode.Success, null);
			getStreamingEventsResponseMessage.SetConnectionStatus(ConnectionStatus.Closed);
			getStreamingEventsResponse.AddResponse(getStreamingEventsResponseMessage);
			getStreamingEventsSoapResponse.Body = getStreamingEventsResponse;
			return getStreamingEventsSoapResponse;
		}
	}
}
