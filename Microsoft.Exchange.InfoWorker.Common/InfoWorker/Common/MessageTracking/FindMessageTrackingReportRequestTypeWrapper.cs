using System;
using Microsoft.Exchange.InfoWorker.Common.Availability.Proxy;
using Microsoft.Exchange.SoapWebClient.EWS;

namespace Microsoft.Exchange.InfoWorker.Common.MessageTracking
{
	internal class FindMessageTrackingReportRequestTypeWrapper
	{
		internal Microsoft.Exchange.SoapWebClient.EWS.FindMessageTrackingReportRequestType WrappedRequest
		{
			get
			{
				return this.request;
			}
		}

		internal FindMessageTrackingReportRequestTypeWrapper(Microsoft.Exchange.SoapWebClient.EWS.FindMessageTrackingReportRequestType request)
		{
			this.request = request;
		}

		internal Microsoft.Exchange.SoapWebClient.EWS.FindMessageTrackingReportRequestType PrepareEWSRequest(int version)
		{
			VersionConverter.Convert(this.request, version);
			return this.request;
		}

		internal Microsoft.Exchange.InfoWorker.Common.Availability.Proxy.FindMessageTrackingReportRequestType PrepareRDRequest(int version)
		{
			VersionConverter.Convert(this.request, version);
			return MessageConverter.CopyEWSTypeToDispatcherType(this.request);
		}

		private Microsoft.Exchange.SoapWebClient.EWS.FindMessageTrackingReportRequestType request;
	}
}
