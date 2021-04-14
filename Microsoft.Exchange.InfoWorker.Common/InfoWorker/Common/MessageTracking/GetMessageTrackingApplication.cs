using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.InfoWorker.Common.Availability;
using Microsoft.Exchange.InfoWorker.Common.Availability.Proxy;
using Microsoft.Exchange.SoapWebClient.AutoDiscover;
using Microsoft.Exchange.Transport.Logging.Search;

namespace Microsoft.Exchange.InfoWorker.Common.MessageTracking
{
	internal sealed class GetMessageTrackingApplication : MessageTrackingApplication
	{
		public GetMessageTrackingApplication(GetMessageTrackingReportRequestTypeWrapper request, ExchangeVersion ewsRequestedVersion) : base(false, false, ewsRequestedVersion)
		{
			this.traceId = this.GetHashCode();
			this.request = request;
		}

		public override IAsyncResult BeginProxyWebRequest(IService service, MailboxData[] mailboxArray, AsyncCallback callback, object asyncState)
		{
			TraceWrapper.SearchLibraryTracer.TraceDebug(this.traceId, "Entering GetMessageTrackingApplication.BeginProxyWebRequest", new object[0]);
			if (Testability.WebServiceCredentials != null)
			{
				service.Credentials = Testability.WebServiceCredentials;
				ServicePointManager.ServerCertificateValidationCallback = ((object obj, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors) => true);
			}
			service.RequestServerVersionValue.Version = VersionConverter.GetRdExchangeVersionType(service.ServiceVersion);
			return service.BeginGetMessageTrackingReport(this.request.PrepareRDRequest(service.ServiceVersion), callback, asyncState);
		}

		public override void EndProxyWebRequest(ProxyWebRequest proxyWebRequest, QueryList queryList, IService service, IAsyncResult asyncResult)
		{
			TraceWrapper.SearchLibraryTracer.TraceDebug(this.traceId, "Entering GetMessageTrackingApplication.EndProxyWebRequest", new object[0]);
			GetMessageTrackingReportResponseMessageType getMessageTrackingReportResponseMessageType = service.EndGetMessageTrackingReport(asyncResult);
			if (getMessageTrackingReportResponseMessageType == null)
			{
				base.HandleNullResponse(proxyWebRequest);
				return;
			}
			int hashCode = proxyWebRequest.GetHashCode();
			if (getMessageTrackingReportResponseMessageType.ResponseClass != ResponseClassType.Success)
			{
				TraceWrapper.SearchLibraryTracer.TraceError<object, string, string>(this.traceId, "{0}: GetMTR proxy web request returned {1} and response code {2}", TraceContext.Get(), Names<ResponseClassType>.Map[(int)getMessageTrackingReportResponseMessageType.ResponseClass], getMessageTrackingReportResponseMessageType.ResponseCode);
			}
			this.ProcessResponseMessages(hashCode, queryList, getMessageTrackingReportResponseMessageType);
		}

		public override string GetParameterDataString()
		{
			return this.traceId.ToString() + " " + this.request.WrappedRequest.MessageTrackingReportId;
		}

		public override BaseQueryResult CreateQueryResult(LocalizedException exception)
		{
			return new GetMessageTrackingBaseQueryResult(exception);
		}

		public override BaseQuery CreateFromUnknown(RecipientData recipientData, LocalizedException exception)
		{
			return GetMessageTrackingBaseQuery.CreateFromUnknown(recipientData, exception);
		}

		public override BaseQuery CreateFromIndividual(RecipientData recipientData)
		{
			return GetMessageTrackingBaseQuery.CreateFromIndividual(recipientData);
		}

		public override BaseQuery CreateFromIndividual(RecipientData recipientData, LocalizedException exception)
		{
			return GetMessageTrackingBaseQuery.CreateFromIndividual(recipientData, exception);
		}

		public override ThreadCounter Worker
		{
			get
			{
				return GetMessageTrackingApplication.MessageTrackingWorker;
			}
		}

		public override ThreadCounter IOCompletion
		{
			get
			{
				return GetMessageTrackingApplication.MessageTrackingIOCompletion;
			}
		}

		public override LocalizedString Name
		{
			get
			{
				return Strings.MessageTrackingApplicationName;
			}
		}

		private void ProcessResponseMessages(int traceId, QueryList queryList, GetMessageTrackingReportResponseMessageType response)
		{
			if (response == null)
			{
				Application.ProxyWebRequestTracer.TraceError((long)traceId, "{0}: Proxy web request returned NULL GetMessageTrackingReportResponseMessageType", new object[]
				{
					TraceContext.Get()
				});
				return;
			}
			foreach (BaseQuery baseQuery in ((IEnumerable<BaseQuery>)queryList))
			{
				GetMessageTrackingBaseQuery getMessageTrackingBaseQuery = (GetMessageTrackingBaseQuery)baseQuery;
				getMessageTrackingBaseQuery.SetResultOnFirstCall(new GetMessageTrackingBaseQueryResult
				{
					Response = response
				});
			}
		}

		private int traceId;

		private GetMessageTrackingReportRequestTypeWrapper request;

		public static readonly ThreadCounter MessageTrackingWorker = new ThreadCounter();

		public static readonly ThreadCounter MessageTrackingIOCompletion = new ThreadCounter();
	}
}
