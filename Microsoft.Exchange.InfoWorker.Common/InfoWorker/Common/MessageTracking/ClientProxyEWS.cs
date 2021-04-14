using System;
using Microsoft.Exchange.SoapWebClient.EWS;

namespace Microsoft.Exchange.InfoWorker.Common.MessageTracking
{
	internal class ClientProxyEWS : IClientProxy
	{
		public string TargetInfoForLogging { get; private set; }

		public string TargetInfoForDisplay
		{
			get
			{
				return this.TargetInfoForLogging;
			}
		}

		public ClientProxyEWS(ExchangeServiceBinding ewsBinding, Uri uri, int serverVersion)
		{
			if (uri == null)
			{
				throw new ArgumentNullException("uri");
			}
			this.ewsBinding = ewsBinding;
			this.TargetInfoForLogging = uri.ToString();
			this.serverVersion = serverVersion;
		}

		public FindMessageTrackingReportResponseMessageType FindMessageTrackingReport(FindMessageTrackingReportRequestTypeWrapper request, TimeSpan timeout)
		{
			this.ewsBinding.Timeout = (int)Math.Min(timeout.TotalMilliseconds, 2147483647.0);
			return this.ewsBinding.FindMessageTrackingReport(request.PrepareEWSRequest(this.serverVersion));
		}

		public InternalGetMessageTrackingReportResponse GetMessageTrackingReport(GetMessageTrackingReportRequestTypeWrapper request, TimeSpan timeout)
		{
			this.ewsBinding.Timeout = (int)Math.Min(timeout.TotalMilliseconds, 2147483647.0);
			GetMessageTrackingReportResponseMessageType messageTrackingReport = this.ewsBinding.GetMessageTrackingReport(request.PrepareEWSRequest(this.serverVersion));
			MessageTrackingReportId messageTrackingReportId;
			if (!MessageTrackingReportId.TryParse(request.WrappedRequest.MessageTrackingReportId, out messageTrackingReportId))
			{
				throw new ArgumentException("Invalid MessageTrackingReportId, caller should have validated");
			}
			return InternalGetMessageTrackingReportResponse.Create(messageTrackingReportId.Domain, messageTrackingReport);
		}

		private ExchangeServiceBinding ewsBinding;

		private int serverVersion;
	}
}
