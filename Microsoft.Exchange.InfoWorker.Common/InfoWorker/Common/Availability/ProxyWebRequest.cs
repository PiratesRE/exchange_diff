using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Text;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.InfoWorker.RequestDispatch;
using Microsoft.Exchange.Diagnostics.FaultInjection;
using Microsoft.Exchange.InfoWorker.Common.Availability.Proxy;
using Microsoft.Exchange.InfoWorker.EventLog;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.SoapWebClient;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal sealed class ProxyWebRequest : AsyncWebRequest, IDisposable
	{
		internal ProxyWebRequest(Application application, ClientContext clientContext, RequestType requestType, RequestLogger requestLogger, QueryList queryList, TargetServerVersion targetVersion, ProxyAuthenticator proxyAuthenticator, WebServiceUri webServiceUri, UriSource source) : base(application, clientContext, requestLogger, "ProxyWebRequest")
		{
			this.proxyAuthenticator = proxyAuthenticator;
			this.url = webServiceUri.Uri.OriginalString;
			this.protocol = webServiceUri.Protocol;
			this.source = source;
			this.queryList = queryList;
			this.requestType = requestType;
			this.service = application.CreateService(webServiceUri, targetVersion, requestType);
			if (proxyAuthenticator.AuthenticatorType == AuthenticatorType.WSSecurity || proxyAuthenticator.AuthenticatorType == AuthenticatorType.OAuth)
			{
				if (webServiceUri.ServerVersion >= Globals.E15Version && clientContext.RequestSchemaVersion >= ExchangeVersionType.Exchange2012)
				{
					this.service.RequestServerVersionValue = new RequestServerVersion
					{
						Version = ExchangeVersionType.Exchange2012
					};
				}
				else
				{
					this.service.RequestServerVersionValue = new RequestServerVersion
					{
						Version = ExchangeVersionType.Exchange2009
					};
				}
			}
			else if (targetVersion >= TargetServerVersion.E15 && clientContext.RequestSchemaVersion == ExchangeVersionType.Exchange2012)
			{
				this.service.RequestServerVersionValue = new RequestServerVersion
				{
					Version = ExchangeVersionType.Exchange2012
				};
			}
			this.service.CookieContainer = new CookieContainer();
			this.service.requestTypeValue = new RequestTypeHeader();
			if (requestType == RequestType.CrossSite || requestType == RequestType.IntraSite)
			{
				if (Configuration.BypassProxyForCrossSiteRequests)
				{
					this.service.Proxy = new WebProxy();
				}
				this.service.requestTypeValue.RequestType = ProxyRequestType.CrossSite;
				if (requestType == RequestType.CrossSite)
				{
					this.failedCounter = PerformanceCounters.CrossSiteCalendarFailuresPerSecond;
					this.averageProcessingTimeCounter = PerformanceCounters.AverageCrossSiteFreeBusyRequestProcessingTime;
					this.averageProcessingTimeCounterBase = PerformanceCounters.AverageCrossSiteFreeBusyRequestProcessingTimeBase;
					this.requestStatisticsType = RequestStatisticsType.CrossSiteProxy;
				}
				else
				{
					this.failedCounter = PerformanceCounters.IntraSiteProxyFreeBusyFailuresPerSecond;
					this.averageProcessingTimeCounter = PerformanceCounters.AverageIntraSiteProxyFreeBusyRequestProcessingTime;
					this.averageProcessingTimeCounterBase = PerformanceCounters.AverageIntraSiteProxyFreeBusyRequestProcessingTimeBase;
					this.requestStatisticsType = RequestStatisticsType.IntraSiteProxy;
				}
			}
			else
			{
				bool flag = false;
				if (Configuration.BypassProxyForCrossForestRequests)
				{
					this.service.Proxy = new WebProxy();
					flag = true;
				}
				this.service.requestTypeValue.RequestType = ProxyRequestType.CrossForest;
				if (requestType == RequestType.FederatedCrossForest)
				{
					if (proxyAuthenticator.AuthenticatorType == AuthenticatorType.OAuth)
					{
						this.failedCounter = PerformanceCounters.FederatedByOAuthFreeBusyFailuresPerSecond;
						this.averageProcessingTimeCounter = PerformanceCounters.AverageFederatedByOAuthFreeBusyRequestProcessingTime;
						this.averageProcessingTimeCounterBase = PerformanceCounters.AverageFederatedByOAuthFreeBusyRequestProcessingTimeBase;
						this.requestStatisticsType = RequestStatisticsType.OAuthProxy;
						this.service.requestTypeValue = null;
					}
					else
					{
						this.failedCounter = PerformanceCounters.FederatedFreeBusyFailuresPerSecond;
						this.averageProcessingTimeCounter = PerformanceCounters.AverageFederatedFreeBusyRequestProcessingTime;
						this.averageProcessingTimeCounterBase = PerformanceCounters.AverageFederatedFreeBusyRequestProcessingTimeBase;
						this.requestStatisticsType = RequestStatisticsType.FederatedProxy;
					}
					if (!flag)
					{
						Server localServer = LocalServerCache.LocalServer;
						if (localServer != null && localServer.InternetWebProxy != null)
						{
							ProxyWebRequest.ProxyWebRequestTracer.TraceDebug<object, Uri>((long)this.GetHashCode(), "{0}: Using custom InternetWebProxy {1}", TraceContext.Get(), localServer.InternetWebProxy);
							this.service.Proxy = new WebProxy(localServer.InternetWebProxy);
						}
					}
				}
				else
				{
					this.failedCounter = PerformanceCounters.CrossForestCalendarFailuresPerSecond;
					this.averageProcessingTimeCounter = PerformanceCounters.AverageCrossForestFreeBusyRequestProcessingTime;
					this.averageProcessingTimeCounterBase = PerformanceCounters.AverageCrossForestFreeBusyRequestProcessingTimeBase;
					this.requestStatisticsType = RequestStatisticsType.CrossForestProxy;
				}
			}
			if (!Configuration.DisableGzipForProxyRequests)
			{
				this.service.EnableDecompression = true;
			}
			string address = this.queryList[0].Email.Address;
			ProxyWebRequest.ProxyWebRequestTracer.TraceDebug<object, string, WebServiceUri>((long)this.GetHashCode(), "{0}: Adding Anchor Mailbox Header {1} to the request to {2}.", TraceContext.Get(), address, webServiceUri);
			this.service.HttpHeaders.Add("X-AnchorMailbox", address);
			if (!string.IsNullOrEmpty(base.ClientContext.RequestId))
			{
				this.service.HttpHeaders.Add("client-request-id", base.ClientContext.RequestId);
			}
		}

		internal string Url
		{
			get
			{
				return this.url;
			}
		}

		protected override IAsyncResult BeginInvoke()
		{
			this.service.UserAgent = this.GetUserAgent();
			MailboxData[] array = new MailboxData[this.queryList.Count];
			for (int i = 0; i < this.queryList.Count; i++)
			{
				this.queryList[i].Target = this.Url;
				array[i] = new MailboxData(this.queryList[i].Email);
			}
			ProxyWebRequest.ProxyWebRequestTracer.TraceDebug<object, RequestType, string>((long)this.GetHashCode(), "{0}: Sending request {1} to {2}", TraceContext.Get(), this.requestType, this.url);
			this.serverRequestId = Microsoft.Exchange.Diagnostics.Trace.TraceCasStart(CasTraceEventType.Availability);
			if (this.service.SupportsProxyAuthentication)
			{
				this.proxyAuthenticator.Authenticate((CustomSoapHttpClientProtocol)this.service);
			}
			this.webServiceCallTimer = Stopwatch.StartNew();
			Stopwatch stopwatch = Stopwatch.StartNew();
			IAsyncResult result = base.Application.BeginProxyWebRequest(this.service, array, new AsyncCallback(base.Complete), null);
			stopwatch.Stop();
			this.queryList.LogLatency("PWRBI", stopwatch.ElapsedMilliseconds);
			return result;
		}

		protected override bool ShouldCallBeginInvokeByNewThread
		{
			get
			{
				return this.proxyAuthenticator.AuthenticatorType == AuthenticatorType.OAuth;
			}
		}

		public override void Abort()
		{
			base.Abort();
			if (this.service != null)
			{
				this.service.Abort();
			}
		}

		protected override bool IsImpersonating
		{
			get
			{
				return this.proxyAuthenticator.AuthenticatorType == AuthenticatorType.NetworkCredentials;
			}
		}

		protected override void EndInvoke(IAsyncResult asyncResult)
		{
			try
			{
				ProxyWebRequest.FaultInjectionTracer.TraceTest(2607164733U);
				base.Application.EndProxyWebRequest(this, this.queryList, this.service, asyncResult);
				if (this.requestType == RequestType.CrossForest || this.requestType == RequestType.FederatedCrossForest)
				{
					RemoteServiceUriCache.Validate(this.url);
				}
				this.IncrementPerfCounter();
			}
			finally
			{
				if (!base.Aborted)
				{
					this.queryList.LogLatency("PWRC", this.webServiceCallTimer.ElapsedMilliseconds);
				}
				this.TraceCompleteRequest();
			}
		}

		private void TraceCompleteRequest()
		{
			if (ETWTrace.ShouldTraceCasStop(this.serverRequestId))
			{
				string serverAddress;
				string clientOperation;
				this.GetUrlData(out serverAddress, out clientOperation);
				string serviceProviderOperationData = this.GetServiceProviderOperationData();
				Microsoft.Exchange.Diagnostics.Trace.TraceCasStop(CasTraceEventType.Availability, this.serverRequestId, 0, 0, serverAddress, TraceContext.Get(), "ProxyWebRequest::CompleteRequest", serviceProviderOperationData, clientOperation);
			}
			this.serverRequestId = Guid.Empty;
		}

		protected override void HandleException(Exception exception)
		{
			if (ProxyWebRequest.ProxyWebRequestTracer.IsTraceEnabled(TraceType.ErrorTrace))
			{
				ProxyWebRequest.ProxyWebRequestTracer.TraceError<object, Exception, ProxyAuthenticator>((long)this.GetHashCode(), "{0}: Exception occurred while completing proxy web request. Exception info is {1}. Caller SIDs: {2}", TraceContext.Get(), exception, this.proxyAuthenticator);
			}
			if (this.requestType == RequestType.CrossForest || this.requestType == RequestType.FederatedCrossForest)
			{
				Exception ex = exception;
				if (exception is GrayException)
				{
					ex = exception.InnerException;
				}
				if (ex is UriFormatException)
				{
					RemoteServiceUriCache.Invalidate(this.url);
				}
				if (HttpWebRequestExceptionHandler.IsConnectionException(ex, ProxyWebRequest.ProxyWebRequestTracer))
				{
					RemoteServiceUriCache.Invalidate(this.url);
				}
			}
			ProxyWebRequestProcessingException ex2 = this.GenerateException(exception);
			this.SetExceptionInResultList(ex2);
			Globals.AvailabilityLogger.LogEvent(InfoWorkerEventLogConstants.Tuple_ProxyWebRequestFailed, this.url, new object[]
			{
				Globals.ProcessId,
				this,
				this.proxyAuthenticator,
				ex2
			});
			this.failedCounter.Increment();
		}

		private ProxyWebRequestProcessingException GenerateException(Exception exception)
		{
			return new ProxyWebRequestProcessingException(Strings.descProxyRequestFailed, exception);
		}

		private void SetExceptionInResultList(LocalizedException exception)
		{
			ProxyWebRequest.ProxyWebRequestTracer.TraceDebug<object, LocalizedException>((long)this.GetHashCode(), "{0}: Setting exception to all queries: {1}", TraceContext.Get(), exception);
			this.queryList.SetResultInAllQueries(base.Application.CreateQueryResult(exception));
		}

		private string GetServiceProviderOperationData()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("Mailbox list = ");
			foreach (BaseQuery baseQuery in ((IEnumerable<BaseQuery>)this.queryList))
			{
				stringBuilder.Append((baseQuery.Email != null) ? baseQuery.Email.ToString() : "<null>");
				stringBuilder.Append(", ");
			}
			stringBuilder.AppendFormat(base.Application.GetParameterDataString(), new object[0]);
			return stringBuilder.ToString();
		}

		private void GetUrlData(out string serverName, out string clientOperation)
		{
			serverName = string.Empty;
			clientOperation = string.Empty;
			if (!string.IsNullOrEmpty(this.url))
			{
				try
				{
					Uri uri = new Uri(this.url);
					serverName = uri.Host;
					clientOperation = uri.PathAndQuery;
				}
				catch (UriFormatException ex)
				{
					serverName = this.url;
					clientOperation = this.url;
					ProxyWebRequest.ProxyWebRequestTracer.TraceDebug(0L, ex.ToString());
				}
			}
		}

		public void Dispose()
		{
			if (this.service != null)
			{
				this.service.Dispose();
			}
		}

		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"ProxyWebRequest ",
				this.requestType,
				" from ",
				TraceContext.Get(),
				" to ",
				this.url
			});
		}

		private string GetUserAgent()
		{
			string type = string.Empty;
			switch (this.requestType)
			{
			case RequestType.IntraSite:
			case RequestType.CrossSite:
				type = "CrossSite";
				break;
			case RequestType.CrossForest:
			case RequestType.FederatedCrossForest:
				type = "CrossForest";
				break;
			}
			UserAgent userAgent = new UserAgent("ASProxy", type, (this.source == UriSource.Directory) ? "Directory" : "EmailDomain", this.protocol);
			return userAgent.ToString();
		}

		private void IncrementPerfCounter()
		{
			this.webServiceCallTimer.Stop();
			this.averageProcessingTimeCounter.IncrementBy(this.webServiceCallTimer.ElapsedTicks);
			this.averageProcessingTimeCounterBase.Increment();
			base.RequestLogger.Add(RequestStatistics.Create(this.requestStatisticsType, this.webServiceCallTimer.ElapsedMilliseconds, this.queryList.Count, this.url));
		}

		private const CasTraceEventType AvailabilityTraceEventType = CasTraceEventType.Availability;

		public const string ProxyWebRequestBeginInvokeMarker = "PWRBI";

		public const string ProxyWebRequestCompleteMarker = "PWRC";

		internal const string AnchorMailboxHeader = "X-AnchorMailbox";

		private static readonly Microsoft.Exchange.Diagnostics.Trace ProxyWebRequestTracer = ExTraceGlobals.ProxyWebRequestTracer;

		private static readonly FaultInjectionTrace FaultInjectionTracer = ExTraceGlobals.FaultInjectionTracer;

		private Guid serverRequestId = Guid.Empty;

		private UriSource source = UriSource.Unknown;

		private string protocol = string.Empty;

		private QueryList queryList;

		private RequestType requestType;

		private ProxyAuthenticator proxyAuthenticator;

		private string url;

		private IService service;

		private Stopwatch webServiceCallTimer;

		private RequestStatisticsType requestStatisticsType;

		private ExPerformanceCounter failedCounter;

		private ExPerformanceCounter averageProcessingTimeCounter;

		private ExPerformanceCounter averageProcessingTimeCounterBase;
	}
}
