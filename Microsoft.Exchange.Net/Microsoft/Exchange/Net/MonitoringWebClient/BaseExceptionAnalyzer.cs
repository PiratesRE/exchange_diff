using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Net.MonitoringWebClient
{
	internal abstract class BaseExceptionAnalyzer : IExceptionAnalyzer
	{
		protected virtual bool AffinityCheck
		{
			get
			{
				return true;
			}
		}

		public BaseExceptionAnalyzer(Dictionary<string, RequestTarget> hostNameSourceMapping)
		{
			if (hostNameSourceMapping != null)
			{
				this.hostNameSourceMapping = new ConcurrentDictionary<string, RequestTarget>();
				foreach (string key in hostNameSourceMapping.Keys)
				{
					this.AddHostNameMapping(key, hostNameSourceMapping[key]);
				}
			}
		}

		protected abstract Dictionary<RequestTarget, Dictionary<FailureReason, FailingComponent>> ComponentMatrix { get; }

		protected abstract FailingComponent DefaultComponent { get; }

		private static HashSet<WebExceptionStatus> GetNetworkRelatedErrors()
		{
			return new HashSet<WebExceptionStatus>
			{
				WebExceptionStatus.ConnectFailure,
				WebExceptionStatus.ConnectionClosed,
				WebExceptionStatus.KeepAliveFailure,
				WebExceptionStatus.NameResolutionFailure,
				WebExceptionStatus.PipelineFailure,
				WebExceptionStatus.ReceiveFailure,
				WebExceptionStatus.RequestProhibitedByProxy,
				WebExceptionStatus.SecureChannelFailure,
				WebExceptionStatus.SendFailure,
				WebExceptionStatus.TrustFailure,
				WebExceptionStatus.Timeout
			};
		}

		public static bool IsNetworkRelatedError(WebExceptionStatus status)
		{
			return BaseExceptionAnalyzer.NetworkRelatedErrors.Contains(status);
		}

		public static Dictionary<WebExceptionStatus, int> GetRetriableErrors()
		{
			return new Dictionary<WebExceptionStatus, int>
			{
				{
					WebExceptionStatus.Timeout,
					1
				}
			};
		}

		public static Dictionary<HttpStatusCode, int> GetStatusCodes()
		{
			return new Dictionary<HttpStatusCode, int>
			{
				{
					(HttpStatusCode)449,
					3
				}
			};
		}

		public void Analyze(TestId currentTestStep, HttpWebRequestWrapper request, HttpWebResponseWrapper response, Exception exception, IResponseTracker responseTracker, Action<ScenarioException> trackingDelegate)
		{
			if (exception == null)
			{
				throw new ArgumentNullException("exception");
			}
			if (exception is ScenarioException)
			{
				throw exception;
			}
			exception = this.PreProcessException(exception, request, response, responseTracker);
			RequestTarget requestTarget = this.GetRequestTarget(request);
			FailureReason failureReason = this.GetFailureReason(exception, request, response);
			FailingComponent failingComponent = this.GetFailingComponent(requestTarget, failureReason);
			string text = null;
			HttpWebResponseWrapperException ex = exception as HttpWebResponseWrapperException;
			if (ex != null)
			{
				text = ex.ExceptionHint;
				if (requestTarget == RequestTarget.Ecp && failureReason == FailureReason.RequestTimeout)
				{
					ExDateTime now = ExDateTime.Now;
					text = "Execution time (ms): " + (now - request.SentTime).TotalMilliseconds.ToString() + Environment.NewLine + text;
				}
			}
			ScenarioException ex2 = new ScenarioException(string.Empty, exception, requestTarget, failureReason, failingComponent, text);
			trackingDelegate(ex2);
			throw ex2;
		}

		private Exception PreProcessException(Exception exception, HttpWebRequestWrapper request, HttpWebResponseWrapper response, IResponseTracker responseTracker)
		{
			HttpWebResponseWrapperException ex = exception as HttpWebResponseWrapperException;
			if (ex != null && ex.Status != null && ex.Status.Value == WebExceptionStatus.TrustFailure)
			{
				SslError sslError = SslValidationHelper.ReadSslError(request.ConnectionGroupName);
				if (sslError != null)
				{
					return new SslNegotiationException(sslError.Description, request, sslError, exception);
				}
			}
			if (ex is MissingKeywordException && this.AffinityCheck)
			{
				Exception ex2 = this.CheckAffinityBreak(responseTracker, ex);
				if (ex2 != null)
				{
					return ex2;
				}
			}
			return exception;
		}

		private Exception CheckAffinityBreak(IResponseTracker responseTracker, HttpWebResponseWrapperException httpWrapperException)
		{
			if (responseTracker == null)
			{
				return null;
			}
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			foreach (ResponseTrackerItem responseTrackerItem in responseTracker.Items)
			{
				if (responseTrackerItem.IsE14CasServer.Equals(true))
				{
					if (dictionary.ContainsKey(responseTrackerItem.TargetHost))
					{
						string text = dictionary[responseTrackerItem.TargetHost];
						if (!text.Equals(responseTrackerItem.RespondingServer, StringComparison.OrdinalIgnoreCase))
						{
							return new BrokenAffinityException(string.Format("Affinity has been broken for hostname {0}. Both servers {1} and {2} responded through this VIP ({3}). Check with networking team for broken affinity rules in the LB.", new object[]
							{
								responseTrackerItem.TargetHost,
								text,
								responseTrackerItem.RespondingServer,
								responseTrackerItem.TargetIpAddress
							}), httpWrapperException.Request, httpWrapperException.Response, responseTrackerItem.TargetHost, text, responseTrackerItem.RespondingServer, httpWrapperException);
						}
					}
					else
					{
						dictionary.Add(responseTrackerItem.TargetHost, responseTrackerItem.RespondingServer);
					}
				}
			}
			return null;
		}

		public ScenarioException GetExceptionForScenarioTimeout(TimeSpan maxAllowedTime, TimeSpan totalTime, ResponseTrackerItem item)
		{
			RequestTarget failureSource = (RequestTarget)Enum.Parse(typeof(RequestTarget), item.TargetType);
			FailureReason failureReason = FailureReason.ScenarioTimeout;
			FailingComponent failingComponentForScenarioTimeout = this.GetFailingComponentForScenarioTimeout(failureSource, failureReason, item);
			ScenarioTimeoutException ex = new ScenarioTimeoutException(MonitoringWebClientStrings.ScenarioTimedOut(maxAllowedTime.TotalSeconds, totalTime.TotalSeconds), (item.Response != null) ? item.Response.Request : null, item.Response);
			return new ScenarioException(ex.Message, ex, failureSource, failureReason, failingComponentForScenarioTimeout, ex.ExceptionHint);
		}

		public virtual RequestTarget GetRequestTarget(HttpWebRequestWrapper request)
		{
			if (request == null)
			{
				return RequestTarget.Monitor;
			}
			if (this.hostNameSourceMapping == null)
			{
				return RequestTarget.Unknown;
			}
			if (request.StepId == TestId.MeasureClientLatency)
			{
				return RequestTarget.LocalClient;
			}
			string text = this.hostNameSourceMapping.Keys.FirstOrDefault((string hostKey) => request.RequestUri.Host.EndsWith(hostKey, StringComparison.OrdinalIgnoreCase));
			if (!string.IsNullOrEmpty(text))
			{
				return this.hostNameSourceMapping[text];
			}
			if (AdfsLogonPage.IsAdfsRequest(request))
			{
				this.AddHostNameMapping(request.RequestUri.Host, RequestTarget.Adfs);
				return RequestTarget.Adfs;
			}
			return RequestTarget.Unknown;
		}

		public virtual List<string> GetHostNames(RequestTarget requestTarget)
		{
			List<string> list = new List<string>();
			if (this.hostNameSourceMapping != null)
			{
				foreach (string text in this.hostNameSourceMapping.Keys)
				{
					if (this.hostNameSourceMapping[text] == requestTarget)
					{
						list.Add(text);
					}
				}
			}
			return list;
		}

		protected virtual FailingComponent GetFailingComponentForScenarioTimeout(RequestTarget failureSource, FailureReason failureReason, ResponseTrackerItem item)
		{
			return this.GetFailingComponent(failureSource, failureReason);
		}

		protected FailingComponent GetFailingComponent(RequestTarget failureSource, FailureReason failureReason)
		{
			if (this.ComponentMatrix.ContainsKey(failureSource) && this.ComponentMatrix[failureSource].ContainsKey(failureReason))
			{
				return this.ComponentMatrix[failureSource][failureReason];
			}
			return this.DefaultComponent;
		}

		public virtual HttpWebResponseWrapperException VerifyResponse(HttpWebRequestWrapper request, HttpWebResponseWrapper response, CafeErrorPageValidationRules cafeErrorPageValidationRules)
		{
			CafeErrorPage cafeErrorPage;
			if (CafeErrorPage.TryParse(response, cafeErrorPageValidationRules, out cafeErrorPage))
			{
				return new CafeErrorPageException(MonitoringWebClientStrings.CafeErrorPage, request, response, cafeErrorPage);
			}
			return null;
		}

		protected virtual FailureReason GetFailureReason(Exception exception, HttpWebRequestWrapper request, HttpWebResponseWrapper response)
		{
			if (exception is PassiveDatabaseException)
			{
				return FailureReason.PassiveDatabase;
			}
			if (exception is SslNegotiationException)
			{
				return FailureReason.SslNegotiation;
			}
			if (exception is BrokenAffinityException)
			{
				return FailureReason.BrokenAffinity;
			}
			if (exception is LogonException)
			{
				LogonException ex = exception as LogonException;
				switch (ex.LogonErrorType)
				{
				case LogonErrorType.BadUserNameOrPassword:
					return FailureReason.BadUserNameOrPassword;
				case LogonErrorType.AccountLocked:
					return FailureReason.AccountLocked;
				default:
					return FailureReason.LogonError;
				}
			}
			else
			{
				if (exception is TaskCanceledException)
				{
					return FailureReason.CancelationRequested;
				}
				if (exception is CafeErrorPageException)
				{
					CafeErrorPageException ex2 = exception as CafeErrorPageException;
					return ex2.CafeErrorPage.FailureReason;
				}
				if (exception is HttpWebResponseWrapperException)
				{
					HttpWebResponseWrapperException ex3 = exception as HttpWebResponseWrapperException;
					if (ex3 != null)
					{
						WebExceptionStatus valueOrDefault = ex3.Status.GetValueOrDefault();
						WebExceptionStatus? webExceptionStatus;
						if (webExceptionStatus != null)
						{
							switch (valueOrDefault)
							{
							case WebExceptionStatus.NameResolutionFailure:
							case WebExceptionStatus.ProxyNameResolutionFailure:
								return FailureReason.NameResolution;
							case WebExceptionStatus.ConnectFailure:
							case WebExceptionStatus.ReceiveFailure:
							case WebExceptionStatus.SendFailure:
							case WebExceptionStatus.PipelineFailure:
							case WebExceptionStatus.ProtocolError:
							case WebExceptionStatus.ConnectionClosed:
							case WebExceptionStatus.TrustFailure:
							case WebExceptionStatus.SecureChannelFailure:
							case WebExceptionStatus.ServerProtocolViolation:
							case WebExceptionStatus.KeepAliveFailure:
							case WebExceptionStatus.RequestProhibitedByProxy:
								return FailureReason.NetworkConnection;
							case WebExceptionStatus.RequestCanceled:
								return FailureReason.RequestTimeout;
							case WebExceptionStatus.Timeout:
								return FailureReason.ConnectionTimeout;
							}
						}
					}
					if (exception is UnexpectedStatusCodeException)
					{
						return FailureReason.UnexpectedHttpResponseCode;
					}
					if (exception is MissingKeywordException)
					{
						return FailureReason.MissingKeyword;
					}
				}
				if (exception is NameResolutionException)
				{
					return FailureReason.NameResolution;
				}
				if (exception is SocketException)
				{
					return FailureReason.NetworkConnection;
				}
				if (exception is IOException)
				{
					return FailureReason.NetworkConnection;
				}
				return FailureReason.Unknown;
			}
		}

		private void AddHostNameMapping(string key, RequestTarget value)
		{
			this.hostNameSourceMapping.AddOrUpdate(key, value, (string addKey, RequestTarget originalValue) => originalValue);
		}

		private ConcurrentDictionary<string, RequestTarget> hostNameSourceMapping;

		private static readonly HashSet<WebExceptionStatus> NetworkRelatedErrors = BaseExceptionAnalyzer.GetNetworkRelatedErrors();
	}
}
