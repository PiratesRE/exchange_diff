using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.InfoWorker.Common.Availability.Proxy;
using Microsoft.Exchange.SoapWebClient.AutoDiscover;
using Microsoft.Exchange.SoapWebClient.EWS;

namespace Microsoft.Exchange.InfoWorker.Common.MessageTracking
{
	internal class ClientProxyRD : IClientProxy
	{
		public string TargetInfoForLogging { get; private set; }

		public string TargetInfoForDisplay { get; private set; }

		public ClientProxyRD(DirectoryContext directoryContext, SmtpAddress proxyRecipient, string domain, ExchangeVersion ewsVersionRequested)
		{
			if (SmtpAddress.Empty.Equals(proxyRecipient) && string.IsNullOrEmpty(domain))
			{
				throw new ArgumentException("Either proxyRecipient or domain must be supplied");
			}
			this.directoryContext = directoryContext;
			this.proxyRecipient = proxyRecipient;
			this.domain = domain;
			this.ewsVersionRequested = ewsVersionRequested;
			string arg = proxyRecipient.ToString();
			string text = (domain == null) ? string.Empty : domain;
			this.TargetInfoForLogging = string.Format("({0}+{1})", arg, text);
			this.TargetInfoForDisplay = text;
		}

		Microsoft.Exchange.SoapWebClient.EWS.FindMessageTrackingReportResponseMessageType IClientProxy.FindMessageTrackingReport(FindMessageTrackingReportRequestTypeWrapper request, TimeSpan timeout)
		{
			FindMessageTrackingQuery findMessageTrackingQuery = new FindMessageTrackingQuery(this.proxyRecipient, this.domain, this.directoryContext, request, this.ewsVersionRequested, timeout);
			FindMessageTrackingQueryResult findMessageTrackingQueryResult = findMessageTrackingQuery.Execute();
			if (findMessageTrackingQueryResult == null)
			{
				TraceWrapper.SearchLibraryTracer.TraceError(this.GetHashCode(), "Empty result in Request Dispatcher FindMessageTrackingQuery.Execute", new object[0]);
				return null;
			}
			Microsoft.Exchange.InfoWorker.Common.Availability.Proxy.FindMessageTrackingReportResponseMessageType response = findMessageTrackingQueryResult.Response;
			return MessageConverter.CopyDispatcherTypeToEWSType(findMessageTrackingQueryResult.Response);
		}

		InternalGetMessageTrackingReportResponse IClientProxy.GetMessageTrackingReport(GetMessageTrackingReportRequestTypeWrapper request, TimeSpan timeout)
		{
			GetMessageTrackingQuery getMessageTrackingQuery = new GetMessageTrackingQuery(this.proxyRecipient, this.directoryContext, request, this.ewsVersionRequested, timeout);
			GetMessageTrackingQueryResult getMessageTrackingQueryResult = getMessageTrackingQuery.Execute();
			if (getMessageTrackingQueryResult == null)
			{
				TraceWrapper.SearchLibraryTracer.TraceError(this.GetHashCode(), "Empty result in Request Dispatcher FindMessageTrackingQuery.Execute", new object[0]);
				return null;
			}
			Microsoft.Exchange.InfoWorker.Common.Availability.Proxy.GetMessageTrackingReportResponseMessageType response = getMessageTrackingQueryResult.Response;
			MessageTrackingReportId messageTrackingReportId;
			if (!MessageTrackingReportId.TryParse(request.WrappedRequest.MessageTrackingReportId, out messageTrackingReportId))
			{
				throw new ArgumentException("Invalid MessageTrackingReportId, caller should have validated");
			}
			return InternalGetMessageTrackingReportResponse.Create(messageTrackingReportId.Domain, response);
		}

		private DirectoryContext directoryContext;

		private SmtpAddress proxyRecipient;

		private string domain;

		private ExchangeVersion ewsVersionRequested;
	}
}
