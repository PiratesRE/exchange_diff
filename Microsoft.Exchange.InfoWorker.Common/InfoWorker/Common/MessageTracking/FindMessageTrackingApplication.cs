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
	internal sealed class FindMessageTrackingApplication : MessageTrackingApplication
	{
		public FindMessageTrackingApplication(FindMessageTrackingReportRequestTypeWrapper request, ExchangeVersion ewsRequestedVersion) : base(false, false, ewsRequestedVersion)
		{
			this.traceId = this.GetHashCode();
			this.request = request;
		}

		public override IAsyncResult BeginProxyWebRequest(IService service, MailboxData[] mailboxArray, AsyncCallback callback, object asyncState)
		{
			TraceWrapper.SearchLibraryTracer.TraceDebug(this.traceId, "Entering FindMessageTrackingApplication.BeginProxyWebRequest", new object[0]);
			if (Testability.WebServiceCredentials != null)
			{
				service.Credentials = Testability.WebServiceCredentials;
				ServicePointManager.ServerCertificateValidationCallback = ((object obj, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors) => true);
			}
			service.RequestServerVersionValue.Version = VersionConverter.GetRdExchangeVersionType(service.ServiceVersion);
			return service.BeginFindMessageTrackingReport(this.request.PrepareRDRequest(service.ServiceVersion), callback, asyncState);
		}

		public override void EndProxyWebRequest(ProxyWebRequest proxyWebRequest, QueryList queryList, IService service, IAsyncResult asyncResult)
		{
			TraceWrapper.SearchLibraryTracer.TraceDebug(this.traceId, "Entering FindMessageTrackingApplication.EndProxyWebRequest", new object[0]);
			FindMessageTrackingReportResponseMessageType findMessageTrackingReportResponseMessageType = service.EndFindMessageTrackingReport(asyncResult);
			if (findMessageTrackingReportResponseMessageType == null)
			{
				base.HandleNullResponse(proxyWebRequest);
				return;
			}
			if (findMessageTrackingReportResponseMessageType.ResponseClass != ResponseClassType.Success)
			{
				TraceWrapper.SearchLibraryTracer.TraceError<object, string, string>(this.traceId, "{0}: FindMTR proxy web request returned {1} and response code {2}", TraceContext.Get(), Names<ResponseClassType>.Map[(int)findMessageTrackingReportResponseMessageType.ResponseClass], findMessageTrackingReportResponseMessageType.ResponseCode);
			}
			this.ProcessResponseMessages(this.traceId, queryList, findMessageTrackingReportResponseMessageType);
		}

		public override string GetParameterDataString()
		{
			return this.traceId.ToString() + " " + this.request.WrappedRequest.MessageId;
		}

		public override BaseQueryResult CreateQueryResult(LocalizedException exception)
		{
			return new FindMessageTrackingBaseQueryResult(exception);
		}

		public override BaseQuery CreateFromUnknown(RecipientData recipientData, LocalizedException exception)
		{
			return FindMessageTrackingBaseQuery.CreateFromUnknown(recipientData, exception);
		}

		public override BaseQuery CreateFromIndividual(RecipientData recipientData)
		{
			return FindMessageTrackingBaseQuery.CreateFromIndividual(recipientData);
		}

		public override BaseQuery CreateFromIndividual(RecipientData recipientData, LocalizedException exception)
		{
			return FindMessageTrackingBaseQuery.CreateFromIndividual(recipientData, exception);
		}

		public override ThreadCounter Worker
		{
			get
			{
				return FindMessageTrackingApplication.MessageTrackingWorker;
			}
		}

		public override ThreadCounter IOCompletion
		{
			get
			{
				return FindMessageTrackingApplication.MessageTrackingIOCompletion;
			}
		}

		public override LocalizedString Name
		{
			get
			{
				return Strings.MessageTrackingApplicationName;
			}
		}

		private void ProcessResponseMessages(int traceId, QueryList queryList, FindMessageTrackingReportResponseMessageType response)
		{
			if (response == null)
			{
				Application.ProxyWebRequestTracer.TraceError((long)traceId, "{0}: Proxy web request returned NULL FindMessageTrackingReportResponseMessageType", new object[]
				{
					TraceContext.Get()
				});
				return;
			}
			foreach (BaseQuery baseQuery in ((IEnumerable<BaseQuery>)queryList))
			{
				FindMessageTrackingBaseQuery findMessageTrackingBaseQuery = (FindMessageTrackingBaseQuery)baseQuery;
				findMessageTrackingBaseQuery.SetResultOnFirstCall(new FindMessageTrackingBaseQueryResult
				{
					Response = response
				});
			}
		}

		private int traceId;

		private FindMessageTrackingReportRequestTypeWrapper request;

		public static readonly ThreadCounter MessageTrackingWorker = new ThreadCounter();

		public static readonly ThreadCounter MessageTrackingIOCompletion = new ThreadCounter();
	}
}
