using System;
using Microsoft.Exchange.InfoWorker.Common.Availability.Proxy;
using Microsoft.Exchange.SoapWebClient.EWS;

namespace Microsoft.Exchange.InfoWorker.Common.MessageTracking
{
	internal class GetMessageTrackingReportRequestTypeWrapper
	{
		internal Microsoft.Exchange.SoapWebClient.EWS.GetMessageTrackingReportRequestType WrappedRequest
		{
			get
			{
				return this.request;
			}
		}

		internal GetMessageTrackingReportRequestTypeWrapper(Microsoft.Exchange.SoapWebClient.EWS.GetMessageTrackingReportRequestType request)
		{
			this.request = request;
		}

		internal Microsoft.Exchange.SoapWebClient.EWS.GetMessageTrackingReportRequestType PrepareEWSRequest(int version)
		{
			VersionConverter.Convert(this.request, version);
			return this.request;
		}

		internal Microsoft.Exchange.InfoWorker.Common.Availability.Proxy.GetMessageTrackingReportRequestType PrepareRDRequest(int version)
		{
			VersionConverter.Convert(this.request, version);
			return MessageConverter.CopyEWSTypeToDispatcherType(this.request);
		}

		private Microsoft.Exchange.SoapWebClient.EWS.GetMessageTrackingReportRequestType request;
	}
}
