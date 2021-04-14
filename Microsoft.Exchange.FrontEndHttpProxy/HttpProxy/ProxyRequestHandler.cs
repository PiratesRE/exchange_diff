using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.WebSockets;
using Microsoft.Exchange.Clients.Security;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic.Cafe;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.ServerLocator;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.HttpProxy;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.HttpProxy.Common;
using Microsoft.Exchange.HttpProxy.EventLogs;
using Microsoft.Exchange.HttpProxy.Routing;
using Microsoft.Exchange.HttpProxy.Routing.RoutingDestinations;
using Microsoft.Exchange.HttpProxy.Routing.Serialization;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Net.MonitoringWebClient;
using Microsoft.Exchange.Net.Protocols;
using Microsoft.Exchange.Security.Authentication;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.Security.OAuth;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.HttpProxy
{
	internal abstract class ProxyRequestHandler : IHttpAsyncHandler, IHttpHandler, IAsyncResult, IDisposeTrackable, IDisposable, IRequestContext
	{
		internal ProxyRequestHandler()
		{
			this.disposeTracker = this.GetDisposeTracker();
			this.TraceContext = 0;
			this.State = ProxyRequestHandler.ProxyState.None;
			this.ServerAsyncState = new AsyncStateHolder(this);
			this.UseRoutingHintForAnchorMailbox = true;
			this.HasPreemptivelyCheckedForRoutingHint = false;
			OutstandingRequests.AddRequest(this);
		}

		public bool IsReusable
		{
			get
			{
				return false;
			}
		}

		public object AsyncState
		{
			get
			{
				object result;
				lock (this.LockObject)
				{
					result = this.asyncState;
				}
				return result;
			}
		}

		public bool IsCompleted
		{
			get
			{
				bool result;
				lock (this.LockObject)
				{
					result = (this.State == ProxyRequestHandler.ProxyState.Completed || this.State == ProxyRequestHandler.ProxyState.CleanedUp);
				}
				return result;
			}
		}

		public bool CompletedSynchronously
		{
			get
			{
				return false;
			}
		}

		public WaitHandle AsyncWaitHandle
		{
			get
			{
				WaitHandle result;
				lock (this.LockObject)
				{
					ExTraceGlobals.VerboseTracer.TraceDebug<int>((long)this.GetHashCode(), "[ProxyRequestHandler::AsyncWaitHandle::get]: Context {0}", this.TraceContext);
					if (this.completedWaitHandle == null)
					{
						this.completedWaitHandle = new ManualResetEvent(false);
						if (this.IsCompleted && !this.completedWaitHandle.Set())
						{
							ExTraceGlobals.VerboseTracer.TraceError<int>((long)this.GetHashCode(), "[ProxyRequestHandler::AsyncWaitHandle::get]: Failed to set the WaitHandle. This condition can lead to possible deadlock. Context {0}", this.TraceContext);
							throw new InvalidOperationException("Unable to set wait handle.");
						}
					}
					result = this.completedWaitHandle;
				}
				return result;
			}
		}

		public IAuthBehavior AuthBehavior { get; private set; }

		public HttpContext HttpContext { get; private set; }

		public LatencyTracker LatencyTracker { get; private set; }

		public Guid ActivityId
		{
			get
			{
				if (this.Logger != null)
				{
					return this.Logger.ActivityId;
				}
				return Guid.Empty;
			}
		}

		public int TraceContext { get; private set; }

		public RequestDetailsLogger Logger { get; private set; }

		public bool IsDisposed
		{
			get
			{
				return this.disposed;
			}
		}

		internal HttpApplication HttpApplication { get; private set; }

		internal HttpRequest ClientRequest { get; private set; }

		internal Stream ClientRequestStream { get; set; }

		internal HttpResponse ClientResponse { get; private set; }

		internal HttpWebRequest ServerRequest { get; private set; }

		internal Stream ServerRequestStream { get; private set; }

		internal HttpWebResponse ServerResponse { get; private set; }

		internal Stream ServerResponseStream { get; private set; }

		internal AsyncStateHolder ServerAsyncState { get; private set; }

		internal object LockObject
		{
			get
			{
				return this.lockObject;
			}
		}

		internal bool ResponseHeadersSent { get; set; }

		protected virtual bool ImplementsOutOfBandProxyLogon
		{
			get
			{
				return false;
			}
		}

		protected virtual HttpStatusCode StatusCodeSignifyingOutOfBandProxyLogonNeeded
		{
			get
			{
				throw new NotImplementedException("Should not be called - out-of-band proxy logon unsupported.");
			}
		}

		protected virtual bool WillContentBeChangedDuringStreaming
		{
			get
			{
				return false;
			}
		}

		protected virtual bool WillAddProtocolSpecificCookiesToServerRequest
		{
			get
			{
				return false;
			}
		}

		protected virtual bool WillAddProtocolSpecificCookiesToClientResponse
		{
			get
			{
				return false;
			}
		}

		protected virtual bool ShouldForceUnbufferedClientResponseOutput
		{
			get
			{
				return false;
			}
		}

		protected virtual bool ShouldSendFullActivityScope
		{
			get
			{
				return true;
			}
		}

		protected ProxyRequestHandler.ProxyState State { get; set; }

		protected PfdTracer PfdTracer { get; set; }

		private protected bool UseRoutingHintForAnchorMailbox { protected get; private set; }

		protected bool HasPreemptivelyCheckedForRoutingHint { get; set; }

		protected bool IsAnchorMailboxFromRoutingHint { get; set; }

		protected bool IsRetryOnErrorEnabled
		{
			get
			{
				return HttpProxySettings.MaxRetryOnError.Value > 0;
			}
		}

		protected bool IsRetryOnConnectivityErrorEnabled
		{
			get
			{
				return this.IsRetryOnErrorEnabled && HttpProxySettings.RetryOnConnectivityErrorEnabled.Value;
			}
		}

		protected bool IsRetryingOnError
		{
			get
			{
				return this.retryOnErrorCounter > 0;
			}
		}

		protected bool ShouldRetryOnError
		{
			get
			{
				return this.retryOnErrorCounter < HttpProxySettings.MaxRetryOnError.Value;
			}
		}

		private bool IsInRoutingState
		{
			get
			{
				bool result;
				lock (this.LockObject)
				{
					result = (this.State == ProxyRequestHandler.ProxyState.None || this.State == ProxyRequestHandler.ProxyState.Initializing || this.State == ProxyRequestHandler.ProxyState.CalculateBackEnd || this.State == ProxyRequestHandler.ProxyState.CalculateBackEndSecondRound || this.State == ProxyRequestHandler.ProxyState.PrepareServerRequest || this.State == ProxyRequestHandler.ProxyState.ProxyRequestData || this.State == ProxyRequestHandler.ProxyState.WaitForServerResponse || this.State == ProxyRequestHandler.ProxyState.WaitForProxyLogonRequestStream || this.State == ProxyRequestHandler.ProxyState.WaitForProxyLogonResponse);
				}
				return result;
			}
		}

		private bool IsInRetryableState
		{
			get
			{
				bool result;
				lock (this.LockObject)
				{
					result = (this.State == ProxyRequestHandler.ProxyState.None || this.State == ProxyRequestHandler.ProxyState.Initializing || this.State == ProxyRequestHandler.ProxyState.CalculateBackEnd || this.State == ProxyRequestHandler.ProxyState.CalculateBackEndSecondRound || this.State == ProxyRequestHandler.ProxyState.PrepareServerRequest || this.State == ProxyRequestHandler.ProxyState.ProxyRequestData || this.State == ProxyRequestHandler.ProxyState.WaitForServerResponse);
				}
				return result;
			}
		}

		private bool IsInPostRoutingState
		{
			get
			{
				bool result;
				lock (this.LockObject)
				{
					result = (this.State == ProxyRequestHandler.ProxyState.ProxyResponseData || this.State == ProxyRequestHandler.ProxyState.Completed || this.State == ProxyRequestHandler.ProxyState.CleanedUp);
				}
				return result;
			}
		}

		public IAsyncResult BeginProcessRequest(HttpContext context, AsyncCallback cb, object extraData)
		{
			RequestDetailsLoggerBase<RequestDetailsLogger>.SafeSetLogger(this.Logger, HttpProxyMetadata.ModuleToHandlerSwitchingLatency, this.LatencyTracker.GetCurrentLatency(LatencyTrackerKey.ModuleToHandlerSwitchingLatency));
			this.LogElapsedTime("E_BPR");
			lock (this.LockObject)
			{
				this.LatencyTracker.StartTracking(LatencyTrackerKey.RequestHandlerLatency, false);
				AspNetHelper.AddTimestampHeaderIfNecessary(context.Request.Headers, "X-FrontEnd-Handler-Begin");
				this.asyncCallback = cb;
				this.asyncState = extraData;
				this.PfdTracer = new PfdTracer(this.TraceContext, this.GetHashCode());
				this.PfdTracer.TraceRequest("ClientRequest", this.ClientRequest);
				if (ExTraceGlobals.VerboseTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					ExTraceGlobals.VerboseTracer.TraceDebug<int, string, Uri>((long)this.GetHashCode(), "[ProxyRequestHandler::BeginProcessRequest]: Called for Context {0}; method {1}; url {2}", this.TraceContext, this.ClientRequest.HttpMethod, this.ClientRequest.Url);
				}
				this.DoProtocolSpecificBeginRequestLogging();
				ThreadPool.QueueUserWorkItem(new WaitCallback(this.BeginCalculateTargetBackEnd));
				this.State = ProxyRequestHandler.ProxyState.CalculateBackEnd;
				this.LogElapsedTime("L_BPR");
			}
			return this;
		}

		public void EndProcessRequest(IAsyncResult result)
		{
			try
			{
				this.LogElapsedTime("E_EPR");
				lock (this.LockObject)
				{
					Exception ex = this.asyncException;
					if (ExTraceGlobals.VerboseTracer.IsTraceEnabled(TraceType.DebugTrace))
					{
						ExTraceGlobals.VerboseTracer.TraceDebug<int, string>((long)this.GetHashCode(), "[ProxyRequestHandler::EndProcessRequest]: Called for Context {0}; status code {1}", this.TraceContext, (ex != null) ? ex.ToString() : this.ClientResponse.StatusCode.ToString(CultureInfo.InvariantCulture));
					}
					bool isClientConnected = this.ClientResponse.IsClientConnected;
					this.Dispose();
					if (ex != null)
					{
						PerfCounters.UpdateMovingPercentagePerformanceCounter(PerfCounters.HttpProxyCountersInstance.MovingPercentageMailboxServerFailure);
						if (isClientConnected)
						{
							Diagnostics.ReportException(ex, FrontEndHttpProxyEventLogConstants.Tuple_InternalServerError, null, "Exception from EndProcessRequest event: {0}");
							throw new AggregateException(new Exception[]
							{
								ex
							});
						}
						if (!AspNetHelper.IsExceptionExpectedWhenDisconnected(ex))
						{
							this.InspectDisconnectException(ex);
						}
					}
				}
			}
			finally
			{
				long currentLatency = this.LatencyTracker.GetCurrentLatency(LatencyTrackerKey.HandlerCompletionLatency);
				if (currentLatency >= 0L)
				{
					RequestDetailsLoggerBase<RequestDetailsLogger>.SafeSetLogger(this.Logger, HttpProxyMetadata.HandlerCompletionLatency, currentLatency);
				}
				long currentLatency2 = this.LatencyTracker.GetCurrentLatency(LatencyTrackerKey.RequestHandlerLatency);
				if (currentLatency2 >= 0L)
				{
					RequestDetailsLoggerBase<RequestDetailsLogger>.SafeSetLogger(this.Logger, HttpProxyMetadata.RequestHandlerLatency, currentLatency2);
				}
				this.LogElapsedTime("L_EPR");
				this.LatencyTracker.StartTracking(LatencyTrackerKey.HandlerToModuleSwitchingLatency, false);
			}
		}

		public void ProcessRequest(HttpContext context)
		{
			ExTraceGlobals.VerboseTracer.TraceError((long)this.GetHashCode(), "[ProxyRequestHandler::ProcessRequest]: ProcessRequest() should never be called!");
			throw new NotSupportedException();
		}

		public DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<ProxyRequestHandler>(this);
		}

		public void SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
				this.disposeTracker = null;
			}
		}

		public void Dispose()
		{
			Diagnostics.SendWatsonReportOnUnhandledException(delegate()
			{
				lock (this.LockObject)
				{
					if (!this.disposed)
					{
						long num = 0L;
						LatencyTracker.GetLatency(delegate()
						{
							this.Cleanup();
						}, out num);
						if (num > 50L)
						{
							RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericInfo(this.Logger, "HandlerCleanupLatency", num);
						}
						this.disposed = true;
						GC.SuppressFinalize(this);
					}
				}
			}, new Diagnostics.LastChanceExceptionHandler(RequestDetailsLogger.LastChanceExceptionHandler));
		}

		internal void Run(HttpContext context)
		{
			Diagnostics.SendWatsonReportOnUnhandledException(delegate()
			{
				try
				{
					this.LatencyTracker = LatencyTracker.FromHttpContext(context);
					this.Logger = RequestDetailsLoggerBase<RequestDetailsLogger>.GetCurrent(context);
					this.LogElapsedTime("E_Run");
					RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericInfo(this.Logger, "ProxyState-Run", this.State);
					this.HttpApplication = context.ApplicationInstance;
					this.HttpContext = context;
					this.ClientRequest = context.Request;
					this.ClientResponse = context.Response;
					this.TraceContext = (int)context.Items[Constants.TraceContextKey];
					this.ResponseHeadersSent = false;
					this.AuthBehavior = DefaultAuthBehavior.CreateAuthBehavior(this.HttpContext);
					if (ExTraceGlobals.VerboseTracer.IsTraceEnabled(TraceType.DebugTrace))
					{
						ExTraceGlobals.VerboseTracer.TraceDebug<int, string, Uri>((long)this.GetHashCode(), "[ProxyRequestHandler::Run]: Called for Context {0}; method {1}; url {2}", this.TraceContext, this.ClientRequest.HttpMethod, this.ClientRequest.Url);
					}
					this.State = ProxyRequestHandler.ProxyState.Initializing;
					try
					{
						if (ExTraceGlobals.VerboseTracer.IsTraceEnabled(TraceType.DebugTrace))
						{
							ExTraceGlobals.VerboseTracer.TraceDebug<int, string, Uri>((long)this.GetHashCode(), "[ProxyRequestHandler::Run]: Calling OnInitializingHandler for Context {0}; method {1}; url {2}", this.TraceContext, this.ClientRequest.HttpMethod, this.ClientRequest.Url);
						}
						this.OnInitializingHandler();
					}
					catch (HttpException ex)
					{
						ExTraceGlobals.VerboseTracer.TraceDebug<HttpException>(0L, "[ProxyRequestHandler::Run] HttpException thrown during handler initialization: {0}", ex);
						string text = ex.ToString();
						if (this.Logger != null)
						{
							this.Logger.AppendGenericError("ProxyRequestHandler_Run_Exception", text);
						}
						this.SetResponseStatusIfHeadersUnsent(this.ClientResponse, ex.GetHttpCode());
						this.ClientResponse.SuppressContent = true;
						this.HttpContext.ApplicationInstance.CompleteRequest();
						this.SetRequestFailureContext(RequestFailureContext.RequestFailurePoint.FrontEnd, ex.GetHttpCode(), ex.Message, text, null, null);
						this.Dispose();
						return;
					}
					PerfCounters.HttpProxyCountersInstance.TotalProxyRequests.Increment();
					if (ExTraceGlobals.VerboseTracer.IsTraceEnabled(TraceType.DebugTrace))
					{
						ExTraceGlobals.VerboseTracer.TraceDebug<int, string, Uri>((long)this.GetHashCode(), "[ProxyRequestHandler::Run]: Remapping handler for Context {0}; method {1}; url {2}", this.TraceContext, this.ClientRequest.HttpMethod, this.ClientRequest.Url);
					}
					context.RemapHandler(this);
				}
				finally
				{
					this.LogElapsedTime("L_Run");
				}
			}, new Diagnostics.LastChanceExceptionHandler(RequestDetailsLogger.LastChanceExceptionHandler));
		}

		internal void CallThreadEntranceMethod(ExWatson.MethodDelegate method)
		{
			try
			{
				this.HttpContext.SetActivityScopeOnCurrentThread(this.Logger);
				Diagnostics.SendWatsonReportOnUnhandledException(method);
			}
			catch (Exception ex)
			{
				lock (this.lockObject)
				{
					if (this.State != ProxyRequestHandler.ProxyState.CleanedUp && this.State != ProxyRequestHandler.ProxyState.Completed)
					{
						this.CompleteWithError(ex, "CallThreadEntranceMethod");
					}
				}
			}
		}

		protected void CompleteForRedirect(string redirectUrl)
		{
			this.LogElapsedTime("E_CompRedir");
			ExTraceGlobals.VerboseTracer.TraceError<int, string>((long)this.GetHashCode(), "[ProxyRequestHandler::CompleteForRedirect]: Context {0}; redirectUrl {1}", this.TraceContext, redirectUrl);
			this.Logger.AppendGenericError("Redirected", redirectUrl);
			this.Complete();
			this.LogElapsedTime("L_CompRedir");
		}

		protected virtual void Cleanup()
		{
			this.LogElapsedTime("E_Cleanup");
			if (this.State != ProxyRequestHandler.ProxyState.CleanedUp)
			{
				ExTraceGlobals.VerboseTracer.TraceDebug<int>((long)this.GetHashCode(), "[ProxyRequestHandler::Cleanup]: Context {0}", this.TraceContext);
				if (this.ServerRequest != null)
				{
					try
					{
						this.ServerRequest.Abort();
						this.ServerRequest = null;
					}
					catch (Exception)
					{
					}
				}
				ProxyRequestHandler.DisposeIfNotNullAndCatchExceptions(this.ServerAsyncState);
				this.ServerAsyncState = null;
				ProxyRequestHandler.DisposeIfNotNullAndCatchExceptions(this.completedWaitHandle);
				this.completedWaitHandle = null;
				ProxyRequestHandler.DisposeIfNotNullAndCatchExceptions(this.bufferedRegionStream);
				this.bufferedRegionStream = null;
				ProxyRequestHandler.DisposeIfNotNullAndCatchExceptions(this.ClientRequestStream);
				this.ClientRequestStream = null;
				ProxyRequestHandler.DisposeIfNotNullAndCatchExceptions(this.ServerRequestStream);
				this.ServerRequestStream = null;
				ProxyRequestHandler.DisposeIfNotNullAndCatchExceptions(this.ServerResponseStream);
				this.ServerResponseStream = null;
				ProxyRequestHandler.DisposeIfNotNullAndCatchExceptions(this.ServerResponse);
				this.ServerResponse = null;
				this.CleanUpRequestStreamsAndBuffer();
				this.CleanUpResponseStreamsAndBuffer();
				ProxyRequestHandler.DisposeIfNotNullAndCatchExceptions(this.mailboxServerLocator);
				this.mailboxServerLocator = null;
				ProxyRequestHandler.DisposeIfNotNullAndCatchExceptions(this.authenticationContext);
				this.authenticationContext = null;
				OutstandingRequests.RemoveRequest(this);
				ProxyRequestHandler.DisposeIfNotNullAndCatchExceptions(this.disposeTracker);
				this.disposeTracker = null;
				this.State = ProxyRequestHandler.ProxyState.CleanedUp;
			}
			this.LogElapsedTime("L_Cleanup");
		}

		protected virtual void OnInitializingHandler()
		{
			if (!LiveIdBasicAuthModule.SyncADBackendOnly && this.AuthBehavior.AuthState == AuthState.FrontEndFullAuth)
			{
				DatacenterRedirectStrategy.CheckLiveIdBasicPartialAuthResult(this.HttpContext);
			}
		}

		protected virtual void DoProtocolSpecificBeginRequestLogging()
		{
			if ((this.ClientRequest.Url.LocalPath.IndexOf("owa/service.svc") > 0 || this.ClientRequest.Url.LocalPath.IndexOf("owa/integrated/service.svc") > 0) && this.ClientRequest.QueryString["Action"] != null)
			{
				RequestDetailsLoggerBase<RequestDetailsLogger>.SafeSetLogger(this.Logger, HttpProxyMetadata.ProtocolAction, this.ClientRequest.QueryString["Action"]);
			}
		}

		protected virtual void StartOutOfBandProxyLogon(object extraData)
		{
			throw new NotImplementedException("Should never start an out-of-band proxy logon - not supported!");
		}

		protected virtual bool ShouldCopyCookieToClientResponse(Cookie cookie)
		{
			return true;
		}

		protected virtual void CopySupplementalCookiesToClientResponse()
		{
		}

		protected void CopyServerCookieToClientResponse(Cookie serverCookie)
		{
			HttpCookie httpCookie = new HttpCookie(serverCookie.Name, serverCookie.Value);
			httpCookie.Path = serverCookie.Path;
			httpCookie.Expires = serverCookie.Expires;
			httpCookie.HttpOnly = serverCookie.HttpOnly;
			httpCookie.Secure = serverCookie.Secure;
			this.ClientResponse.Cookies.Add(httpCookie);
		}

		protected virtual void SetProtocolSpecificServerRequestParameters(HttpWebRequest serverRequest)
		{
		}

		protected virtual void AddProtocolSpecificHeadersToServerRequest(WebHeaderCollection headers)
		{
			this.LogElapsedTime("E_AddHeaders");
			string fullRawUrl = this.ClientRequest.GetFullRawUrl();
			try
			{
				headers[Constants.MsExchProxyUri] = fullRawUrl;
			}
			catch (ArgumentException)
			{
				headers[Constants.MsExchProxyUri] = Uri.EscapeUriString(fullRawUrl);
			}
			if ((HttpProxySettings.SerializeClientAccessContext.Value & 1) == 1 && !string.IsNullOrWhiteSpace(this.ClientRequest.Headers[Constants.XIsFromCafe]))
			{
				throw new HttpException(400, "Invalid headers(1)");
			}
			headers[Constants.XIsFromCafe] = Constants.IsFromCafeHeaderValue;
			headers[Constants.XSourceCafeServer] = HttpProxyGlobals.LocalMachineFqdn.Member;
			if ((HttpProxySettings.SerializeClientAccessContext.Value & 2) == 2 && !string.IsNullOrWhiteSpace(this.ClientRequest.Headers["X-CommonAccessToken"]))
			{
				throw new HttpException(400, "Invalid headers(2)");
			}
			if (this.AuthBehavior.AuthState != AuthState.BackEndFullAuth)
			{
				if (this.ClientRequest.IsAuthenticated)
				{
					string text = this.ClientRequest.Headers["X-CommonAccessToken"];
					if (!string.IsNullOrWhiteSpace(text))
					{
						CommonAccessToken token = CommonAccessToken.Deserialize(text);
						if (token.IsSystemOrMachineAccount())
						{
							throw new HttpException(400, "Bad context");
						}
						WindowsIdentity windowsIdentity = null;
						if (this.HttpContext != null && this.HttpContext.User != null)
						{
							windowsIdentity = (this.HttpContext.User.Identity as WindowsIdentity);
						}
						if (windowsIdentity == null || !windowsIdentity.IsSystemOrTrustedMachineAccount())
						{
							throw new HttpException(400, "Unauthorized to send context");
						}
						RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericInfo(this.Logger, "CT", "TMA");
						headers["X-CommonAccessToken"] = text;
					}
					else
					{
						CommonAccessToken commonAccessToken = AspNetHelper.FixupCommonAccessToken(this.HttpContext, this.AnchoredRoutingTarget.BackEndServer.Version);
						if (commonAccessToken == null)
						{
							commonAccessToken = (this.HttpContext.Items["Item-CommonAccessToken"] as CommonAccessToken);
						}
						if (commonAccessToken == null)
						{
							throw new HttpException(400, "No context to send");
						}
						if (commonAccessToken.IsSystemOrMachineAccount())
						{
							throw new HttpException(400, "Cannot serialize context");
						}
						headers["X-CommonAccessToken"] = commonAccessToken.Serialize();
					}
				}
				else if (this.ShouldBackendRequestBeAnonymous())
				{
					RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericInfo(this.Logger, "CT", "BEAn");
					headers["X-CommonAccessToken"] = new CommonAccessToken(AccessTokenType.Anonymous).Serialize();
				}
				else
				{
					RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericInfo(this.Logger, "CT", "An");
					headers["X-CommonAccessToken"] = new CommonAccessToken(AccessTokenType.Anonymous).Serialize();
				}
			}
			string value = this.HttpContext.Items[Constants.WLIDMemberName] as string;
			if (!string.IsNullOrEmpty(value))
			{
				headers[Constants.WLIDMemberNameHeaderName] = value;
				headers[Constants.LiveIdMemberName] = value;
			}
			string value2 = this.HttpContext.Items[Constants.MissingDirectoryUserObjectKey] as string;
			if (!string.IsNullOrEmpty(value2))
			{
				headers[Constants.MissingDirectoryUserObjectHeader] = value2;
			}
			string value3 = this.HttpContext.Items[Constants.OrganizationContextKey] as string;
			if (!string.IsNullOrEmpty(value3))
			{
				headers[Constants.OrganizationContextHeader] = value3;
			}
			if (!Utilities.IsPartnerHostedOnly && !VariantConfiguration.InvariantNoFlightingSnapshot.Cafe.NoVDirLocationHint.Enabled && HttpProxyGlobals.VdirObject.Member != null)
			{
				string value4 = HttpProxyGlobals.VdirObject.Member.Id.ObjectGuid.ToString();
				headers[Constants.VDirObjectID] = value4;
			}
			if (!this.IsBackendServerCacheValidationEnabled && !this.ShouldRetryOnError)
			{
				this.RemoveNotNeededHttpContextContent();
			}
			this.LogElapsedTime("L_AddHeaders");
		}

		protected virtual void DoProtocolSpecificBeginProcess()
		{
		}

		protected virtual bool ShouldBackendRequestBeAnonymous()
		{
			return false;
		}

		protected virtual bool ShouldBlockCurrentOAuthRequest()
		{
			return this.ProxyToDownLevel;
		}

		protected void PrepareServerRequest(HttpWebRequest serverRequest)
		{
			this.LogElapsedTime("E_PrepSvrReq");
			if (this.ClientRequest.IsAuthenticated)
			{
				OAuthIdentity oauthIdentity = this.HttpContext.User.Identity as OAuthIdentity;
				if (oauthIdentity != null)
				{
					if (this.ShouldBlockCurrentOAuthRequest())
					{
						throw new HttpException(403, "Cannot proxy OAuth request to down level server.", InvalidOAuthTokenException.OAuthRequestProxyToDownLevelException.Value);
					}
					if (!oauthIdentity.IsAppOnly)
					{
						RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericInfo(this.Logger, "ActAsUserVerified", oauthIdentity.ActAsUser.IsUserVerified);
					}
				}
			}
			serverRequest.ServicePoint.Expect100Continue = false;
			serverRequest.ServicePoint.BindIPEndPointDelegate = new BindIPEndPoint(this.BindIPEndPointCallback);
			if (this.AuthBehavior.AuthState == AuthState.BackEndFullAuth || this.ShouldBackendRequestBeAnonymous() || !this.ClientRequest.IsAuthenticated || (HttpProxySettings.TestBackEndSupportEnabled.Value && !string.IsNullOrEmpty(this.ClientRequest.Headers[Constants.TestBackEndUrlRequestHeaderKey])))
			{
				serverRequest.ConnectionGroupName = "Unauthenticated";
				RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericInfo(this.Logger, "Krb", "UA");
			}
			else
			{
				serverRequest.ConnectionGroupName = Constants.KerberosPackageValue;
				long num = 0L;
				LatencyTracker.GetLatency(delegate()
				{
					serverRequest.Headers[Constants.AuthorizationHeader] = KerberosUtilities.GenerateKerberosAuthHeader(serverRequest.Address.Host, this.TraceContext, ref this.authenticationContext, ref this.kerberosChallenge);
				}, out num);
				RequestDetailsLoggerBase<RequestDetailsLogger>.SafeSetLogger(this.Logger, HttpProxyMetadata.KerberosAuthHeaderLatency, num);
			}
			serverRequest.AutomaticDecompression = DecompressionMethods.None;
			string text = this.ClientRequest.Headers[Constants.AcceptEncodingHeaderName];
			if (!string.IsNullOrEmpty(text))
			{
				if (-1 != text.IndexOf(Constants.GzipHeaderValue, StringComparison.OrdinalIgnoreCase))
				{
					serverRequest.Headers[Constants.AcceptEncodingHeaderName] = Constants.GzipHeaderValue;
				}
				else if (-1 != text.IndexOf(Constants.DeflateHeaderValue, StringComparison.OrdinalIgnoreCase))
				{
					serverRequest.Headers[Constants.AcceptEncodingHeaderName] = Constants.DeflateHeaderValue;
				}
			}
			serverRequest.AllowAutoRedirect = false;
			serverRequest.SendChunked = false;
			serverRequest.ServerCertificateValidationCallback = ProxyApplication.RemoteCertificateValidationCallback;
			CertificateValidationManager.SetComponentId(serverRequest, Constants.CertificateValidationComponentId);
			if (HttpProxyRegistry.AreGccStoredSecretKeysValid.Member)
			{
				this.CopyOrCreateNewXGccProxyInfoHeader(serverRequest);
			}
			if (HttpProxySettings.CafeV1RUMEnabled.Value)
			{
				this.AddRoutingEntryHeaderToRequest(serverRequest);
			}
			this.CopyHeadersToServerRequest(serverRequest);
			this.CopyCookiesToServerRequest(serverRequest);
			this.SetProtocolSpecificServerRequestParameters(serverRequest);
			this.AddProtocolSpecificHeadersToServerRequest(serverRequest.Headers);
			TimeSpan timeout;
			if (this.ShouldSetRequestTimeout(out timeout))
			{
				this.SetupRequestTimeout(serverRequest, timeout);
			}
			this.LogElapsedTime("L_PrepSvrReq");
		}

		protected virtual bool TryHandleProtocolSpecificResponseErrors(WebException e)
		{
			return false;
		}

		protected virtual bool TryHandleProtocolSpecificRequestErrors(Exception e)
		{
			return false;
		}

		protected void BeginProxyRequest(object extraData)
		{
			this.CallThreadEntranceMethod(delegate
			{
				this.LogElapsedTime("E_BegProxyReq");
				try
				{
					lock (this.LockObject)
					{
						PerfCounters.IncrementMovingPercentagePerformanceCounterBase(PerfCounters.HttpProxyCountersInstance.MovingPercentageMailboxServerFailure);
						try
						{
							Uri uri = this.GetTargetBackEndServerUrl();
							if (!string.Equals(uri.Host, this.AnchoredRoutingTarget.BackEndServer.Fqdn, StringComparison.OrdinalIgnoreCase))
							{
								throw new HttpException(503, "Service Unavailable");
							}
							bool flag2 = false;
							ExTraceGlobals.FaultInjectionTracer.TraceTest<bool>(44828U, ref flag2);
							if (flag2)
							{
								throw new HttpException(500, "RequestFailureContextTests");
							}
							bool flag3 = false;
							ExTraceGlobals.FaultInjectionTracer.TraceTest<bool>(3448122685U, ref flag3);
							if (flag3)
							{
								throw new WebException("RequestFailureContextTests", WebExceptionStatus.Success);
							}
							if (HttpProxySettings.TestBackEndSupportEnabled.Value)
							{
								string testBackEndUrl = this.ClientRequest.GetTestBackEndUrl();
								if (!string.IsNullOrEmpty(testBackEndUrl))
								{
									uri = new Uri(testBackEndUrl);
								}
							}
							if (this.AnchoredRoutingTarget != null)
							{
								RequestDetailsLoggerBase<RequestDetailsLogger>.SafeSetLogger(this.Logger, HttpProxyMetadata.TargetServerVersion, Utilities.FormatServerVersion(this.AnchoredRoutingTarget.BackEndServer.Version));
								this.PfdTracer.TraceProxyTarget(this.AnchoredRoutingTarget);
							}
							RequestDetailsLoggerBase<RequestDetailsLogger>.SafeSetLogger(this.Logger, HttpProxyMetadata.ProxyAction, "Proxy");
							RequestDetailsLoggerBase<RequestDetailsLogger>.SafeSetLogger(this.Logger, HttpProxyMetadata.TargetServer, uri.Host);
							this.Logger.SetRoutingType(uri);
							string text = this.ClientRequest.Url.AbsoluteUri.ToLower(CultureInfo.InvariantCulture);
							string strB = uri.AbsoluteUri.ToLower(CultureInfo.InvariantCulture);
							if (string.Compare(text, strB) == 0 && !text.Contains("mrsproxy") && !text.Contains("mailboxreplicationservice"))
							{
								throw new HttpException(403, "Redirect loop detected");
							}
							this.ClientResponse.Headers[WellKnownHeader.XCalculatedBETarget] = uri.Host;
							if (this.ClientRequest.GetHttpRequestBase().IsProxyTestProbeRequest())
							{
								this.CompleteForLocalProbe();
							}
							else
							{
								this.ServerRequest = this.CreateServerRequest(uri);
								PerfCounters.IncrementMovingPercentagePerformanceCounterBase(PerfCounters.HttpProxyCountersInstance.MovingPercentageNewProxyConnectionCreation);
								if (this.HttpContext.IsWebSocketRequest)
								{
									if (!this.ProtocolSupportsWebSocket())
									{
										this.SetResponseStatusIfHeadersUnsent(this.ClientResponse, 501, Constants.NotImplementedStatusDescription);
										throw new HttpException(501, Constants.NotImplementedStatusDescription);
									}
									this.Logger.LogCurrentTime("ProxyWebSocketData");
									this.State = ProxyRequestHandler.ProxyState.ProxyWebSocketData;
									this.ProcessWebSocketRequest(this.HttpContext);
								}
								else if (this.ClientRequest.HasBody())
								{
									this.LatencyTracker.StartTracking(LatencyTrackerKey.BackendRequestInitLatency, this.IsRetryingOnError);
									this.ServerRequest.BeginGetRequestStream(new AsyncCallback(ProxyRequestHandler.RequestStreamReadyCallback), this.ServerAsyncState);
									this.Logger.LogCurrentTime("BeginGetRequestStream");
									this.State = ProxyRequestHandler.ProxyState.ProxyRequestData;
								}
								else
								{
									if (this.ClientRequest.ContentLength > 0)
									{
										this.SetResponseStatusIfHeadersUnsent(this.ClientResponse, 501, Constants.NotImplementedStatusDescription);
										throw new HttpException(501, Constants.NotImplementedStatusDescription);
									}
									this.BeginGetServerResponse();
								}
							}
						}
						catch (InvalidOAuthTokenException ex)
						{
							HttpException ex2 = new HttpException((ex.ErrorCategory == OAuthErrorCategory.InternalError) ? 500 : 401, string.Empty, ex);
							this.CompleteWithError(ex2, "BeginProxyRequest");
						}
						catch (WebException ex3)
						{
							this.CompleteWithError(ex3, "BeginProxyRequest");
						}
						catch (HttpProxyException ex4)
						{
							this.CompleteWithError(ex4, "BeginProxyRequest");
						}
						catch (HttpException ex5)
						{
							this.CompleteWithError(ex5, "BeginProxyRequest");
						}
						catch (IOException ex6)
						{
							this.CompleteWithError(ex6, "BeginProxyRequest");
						}
						catch (SocketException ex7)
						{
							this.CompleteWithError(ex7, "BeginProxyRequest");
						}
					}
				}
				finally
				{
					this.LogElapsedTime("L_BegProxyReq");
				}
			});
		}

		protected HttpWebRequest CreateServerRequest(Uri targetUrl)
		{
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(targetUrl);
			if (!HttpProxySettings.UseDefaultWebProxy.Value)
			{
				httpWebRequest.Proxy = NullWebProxy.Instance;
			}
			httpWebRequest.ServicePoint.ConnectionLimit = HttpProxySettings.ServicePointConnectionLimit.Value;
			httpWebRequest.Method = this.ClientRequest.HttpMethod;
			httpWebRequest.Headers[Constants.OriginatingClientIpHeader] = AspNetHelper.GetClientIpAsProxyHeader(this.ClientRequest);
			httpWebRequest.Headers[Constants.OriginatingClientPortHeader] = AspNetHelper.GetClientPortAsProxyHeader(this.HttpContext);
			this.PrepareServerRequest(httpWebRequest);
			if (string.IsNullOrWhiteSpace(httpWebRequest.Headers["X-CommonAccessToken"]))
			{
				throw new HttpException(400, "Missing context");
			}
			this.PfdTracer.TraceRequest("ProxyRequest", httpWebRequest);
			this.PfdTracer.TraceHeaders("ProxyRequest", this.ClientRequest.Headers, httpWebRequest.Headers);
			this.PfdTracer.TraceCookies("ProxyRequest", this.ClientRequest.Cookies, httpWebRequest.CookieContainer);
			ExTraceGlobals.VerboseTracer.TraceDebug<int, Uri>((long)this.GetHashCode(), "[ProxyRequestHandler::CreateServerRequest]: Context {0}; Target address {1}", this.TraceContext, httpWebRequest.Address);
			return httpWebRequest;
		}

		protected virtual bool ShouldCopyCookieToServerRequest(HttpCookie cookie)
		{
			return true;
		}

		protected virtual bool ShouldCopyHeaderToServerRequest(string headerName)
		{
			return !string.Equals(headerName, "X-CommonAccessToken", StringComparison.OrdinalIgnoreCase) && !string.Equals(headerName, Constants.XIsFromCafe, StringComparison.OrdinalIgnoreCase) && !string.Equals(headerName, Constants.XSourceCafeServer, StringComparison.OrdinalIgnoreCase) && !string.Equals(headerName, Constants.MsExchProxyUri, StringComparison.OrdinalIgnoreCase) && !string.Equals(headerName, "X-MSExchangeActivityCtx", StringComparison.OrdinalIgnoreCase) && !string.Equals(headerName, "client-request-id", StringComparison.OrdinalIgnoreCase) && !string.Equals(headerName, "return-client-request-id", StringComparison.OrdinalIgnoreCase) && !string.Equals(headerName, Constants.OriginatingClientIpHeader, StringComparison.OrdinalIgnoreCase) && (!headerName.StartsWith(Constants.XBackendHeaderPrefix, StringComparison.OrdinalIgnoreCase) || this.ClientRequest.GetHttpRequestBase().IsProbeRequest());
		}

		protected virtual void AddProtocolSpecificCookiesToServerRequest(CookieContainer cookieContainer)
		{
			throw new InvalidOperationException();
		}

		protected virtual void HandleLogoffRequest()
		{
		}

		protected virtual void ResetForRetryOnError()
		{
			this.haveStartedOutOfBandProxyLogon = false;
			this.haveReceivedAuthChallenge = false;
			this.UseRoutingHintForAnchorMailbox = true;
			this.IsAnchorMailboxFromRoutingHint = false;
			this.HasPreemptivelyCheckedForRoutingHint = false;
			this.AuthBehavior.ResetState();
		}

		protected TResult ParseClientRequest<TResult>(Func<Stream, TResult> parseMethod, int bufferSize)
		{
			Func<int, byte[]> func = null;
			Action<byte[]> action = null;
			this.LogElapsedTime("E_ParseReq");
			TResult result;
			lock (this.LockObject)
			{
				if (this.bufferedRegionStream == null || !this.IsRetryOnErrorEnabled)
				{
					BufferPool bufferPool = null;
					this.bufferedRegionStream = new BufferedRegionStream(this.ClientRequest.GetBufferlessInputStream());
					if (bufferSize < 512)
					{
						BufferedRegionStream bufferedRegionStream = this.bufferedRegionStream;
						int bufferSize2 = bufferSize;
						if (func == null)
						{
							func = ((int size) => new byte[size]);
						}
						Func<int, byte[]> acquireFunc = func;
						if (action == null)
						{
							action = delegate(byte[] buffer)
							{
							};
						}
						bufferedRegionStream.SetBufferedRegion(bufferSize2, acquireFunc, action);
					}
					else
					{
						this.bufferedRegionStream.SetBufferedRegion(bufferSize, delegate(int size)
						{
							bufferPool = this.GetBufferPool(bufferSize);
							return bufferPool.Acquire();
						}, delegate(byte[] memory)
						{
							bufferPool.Release(memory);
						});
					}
				}
				else
				{
					try
					{
						this.bufferedRegionStream.Position = 0L;
					}
					catch (InvalidOperationException ex)
					{
						this.Logger.AppendGenericError("ParseClientRequest", ex.ToString());
						throw new HttpProxyException(HttpStatusCode.InternalServerError, HttpProxySubErrorCode.CannotReplayRequest, "Cannot replay request as bufferedRegionStream cannot be reset.");
					}
				}
				this.ClientRequestStream = this.bufferedRegionStream;
				TResult tresult = parseMethod(this.ClientRequestStream);
				this.ClientRequestStream.Position = 0L;
				this.LogElapsedTime("L_ParseReq");
				result = tresult;
			}
			return result;
		}

		protected void Complete()
		{
			this.LogElapsedTime("E_Complete");
			RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericInfo(this.Logger, "ProxyState-Complete", this.State);
			this.LatencyTracker.StartTracking(LatencyTrackerKey.HandlerCompletionLatency, false);
			if (this.ClientResponse != null)
			{
				this.PfdTracer.TraceResponse("ClientResponse", this.ClientResponse);
			}
			try
			{
				this.HttpApplication.CompleteRequest();
			}
			finally
			{
				this.FinalizeRequestHandlerLatencies();
			}
			this.State = ProxyRequestHandler.ProxyState.Completed;
			this.HttpContext.Items[Constants.RequestCompletedHttpContextKeyName] = true;
			ThreadPool.QueueUserWorkItem(new WaitCallback(this.MakeCallback));
			this.LogElapsedTime("L_Complete");
		}

		protected bool IsAuthenticationChallengeFromBackend(WebException exception)
		{
			return exception.Status == WebExceptionStatus.ProtocolError && exception.Response != null && ((HttpWebResponse)exception.Response).StatusCode == HttpStatusCode.Unauthorized && !this.ShouldBackendRequestBeAnonymous() && !this.haveReceivedAuthChallenge;
		}

		protected bool TryFindKerberosChallenge(string authenticationHeader, out bool foundNegotiatePackageName)
		{
			if (KerberosUtilities.TryFindKerberosChallenge(authenticationHeader, this.TraceContext, out this.kerberosChallenge, out foundNegotiatePackageName))
			{
				this.haveReceivedAuthChallenge = true;
				return true;
			}
			return false;
		}

		protected void LogElapsedTime(string latencyName)
		{
			if (HttpProxySettings.DetailedLatencyTracingEnabled.Value && this.LatencyTracker != null)
			{
				this.LatencyTracker.LogElapsedTime(this.Logger, latencyName);
			}
		}

		protected AnchorMailbox CreateAnchorMailboxFromRoutingHint()
		{
			if (!this.UseRoutingHintForAnchorMailbox)
			{
				return null;
			}
			AnchorMailbox anchorMailbox = AnchorMailboxFactory.TryCreateFromRoutingHint(this, !this.IsRetryingOnError);
			if (anchorMailbox != null)
			{
				this.IsAnchorMailboxFromRoutingHint = true;
			}
			return anchorMailbox;
		}

		protected void ThrowWebExceptionForRetryOnErrorTest(WebResponse webResponse, params int[] checkShouldInvalidateVal)
		{
			if (this.ShouldRetryOnError)
			{
				int shouldInvalidateVal = -1;
				ExTraceGlobals.FaultInjectionTracer.TraceTest<int>(2256940349U, ref shouldInvalidateVal);
				if (shouldInvalidateVal != -1)
				{
					if (checkShouldInvalidateVal != null && checkShouldInvalidateVal.Length != 0)
					{
						if (!checkShouldInvalidateVal.Any((int x) => x == shouldInvalidateVal))
						{
							return;
						}
					}
					webResponse.Headers[Constants.BEServerExceptionHeaderName] = Constants.IllegalCrossServerConnectionExceptionType;
					if (this.retryOnErrorCounter == 1)
					{
						webResponse.Headers[WellKnownHeader.XDBMountedOnServer] = string.Format("{0}~{1}~{2}", default(Guid), ComputerInformation.DnsFullyQualifiedDomainName, Server.E15MinVersion);
					}
					string empty = string.Empty;
					ExTraceGlobals.FaultInjectionTracer.TraceTest<string>(3330682173U, ref empty);
					if (!string.IsNullOrEmpty(empty) && !empty.StartsWith("BEAuth"))
					{
						bool flag = false;
						if (string.Equals(empty, "Unauthorized", StringComparison.OrdinalIgnoreCase))
						{
							flag = true;
							HttpWebResponse obj = (HttpWebResponse)webResponse;
							typeof(HttpWebResponse).GetField("m_StatusCode", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(obj, HttpStatusCode.Unauthorized);
							typeof(DefaultAuthBehavior).GetProperty("AuthState", BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty).SetValue(this.AuthBehavior, AuthState.BackEndFullAuth);
						}
						WebExceptionStatus status;
						if (string.Equals(empty, "Retryable", StringComparison.OrdinalIgnoreCase))
						{
							status = WebExceptionStatus.KeepAliveFailure;
						}
						else if (string.Equals(empty, "NonRetryable", StringComparison.OrdinalIgnoreCase))
						{
							status = WebExceptionStatus.Timeout;
						}
						else if (flag)
						{
							status = WebExceptionStatus.ProtocolError;
						}
						else
						{
							status = WebExceptionStatus.UnknownError;
						}
						throw new WebException(string.Format("Fault injection at ThrowWebExceptionForRetryOnErrorTest. retryOnErrorCounter:{0}, shouldInvalidateVal:{1}, throwWebException:{2}", this.retryOnErrorCounter, shouldInvalidateVal, empty), null, status, webResponse);
					}
				}
			}
		}

		private static void ResponseReadyCallback(IAsyncResult result)
		{
			ProxyRequestHandler proxyRequestHandler = AsyncStateHolder.Unwrap<ProxyRequestHandler>(result);
			if (result.CompletedSynchronously)
			{
				ThreadPool.QueueUserWorkItem(new WaitCallback(proxyRequestHandler.OnResponseReady), result);
				return;
			}
			proxyRequestHandler.OnResponseReady(result);
		}

		private static void RequestStreamReadyCallback(IAsyncResult result)
		{
			ProxyRequestHandler proxyRequestHandler = AsyncStateHolder.Unwrap<ProxyRequestHandler>(result);
			ExTraceGlobals.VerboseTracer.TraceDebug<int>((long)proxyRequestHandler.GetHashCode(), "[ProxyRequestHandler::RequestStreamReadyCallback]: Context {0}", proxyRequestHandler.TraceContext);
			if (result.CompletedSynchronously)
			{
				ThreadPool.QueueUserWorkItem(new WaitCallback(proxyRequestHandler.OnRequestStreamReady), result);
				return;
			}
			proxyRequestHandler.OnRequestStreamReady(result);
		}

		private static void DisposeIfNotNullAndCatchExceptions(IDisposable objectToDispose)
		{
			if (objectToDispose == null)
			{
				return;
			}
			try
			{
				objectToDispose.Dispose();
			}
			catch (Exception)
			{
			}
		}

		private IPEndPoint BindIPEndPointCallback(ServicePoint servicePoint, IPEndPoint remoteEndPoint, int retryCount)
		{
			this.Logger.AppendGenericInfo("NewConnection", remoteEndPoint.Address + '&' + retryCount);
			PerfCounters.UpdateMovingPercentagePerformanceCounter(PerfCounters.HttpProxyCountersInstance.MovingPercentageNewProxyConnectionCreation);
			return null;
		}

		private BufferPool GetBufferPool(int bufferSize)
		{
			BufferPoolCollection.BufferSize bufferSize2;
			if (!BufferPoolCollection.AutoCleanupCollection.TryMatchBufferSize(bufferSize, out bufferSize2))
			{
				throw new InvalidOperationException("Could not get buffer size for BufferedRegionStream buffer.");
			}
			return BufferPoolCollection.AutoCleanupCollection.Acquire(bufferSize2);
		}

		private void FinalizeRequestHandlerLatencies()
		{
			this.UpdateRoutingFailurePerfCounter(false);
			long currentLatency = this.LatencyTracker.GetCurrentLatency(LatencyTrackerKey.BackendResponseInitLatency);
			if (currentLatency >= 0L)
			{
				RequestDetailsLoggerBase<RequestDetailsLogger>.SafeSetLogger(this.Logger, HttpProxyMetadata.TotalProxyingLatency, currentLatency);
			}
			RequestDetailsLoggerBase<RequestDetailsLogger>.SafeSetLogger(this.Logger, HttpProxyMetadata.GlsLatencyBreakup, this.LatencyTracker.GlsLatencyBreakup);
			RequestDetailsLoggerBase<RequestDetailsLogger>.SafeSetLogger(this.Logger, HttpProxyMetadata.TotalGlsLatency, this.LatencyTracker.TotalGlsLatency);
			RequestDetailsLoggerBase<RequestDetailsLogger>.SafeSetLogger(this.Logger, HttpProxyMetadata.AccountForestLatencyBreakup, this.LatencyTracker.AccountForestLatencyBreakup);
			RequestDetailsLoggerBase<RequestDetailsLogger>.SafeSetLogger(this.Logger, HttpProxyMetadata.TotalAccountForestLatency, this.LatencyTracker.TotalAccountForestDirectoryLatency);
			RequestDetailsLoggerBase<RequestDetailsLogger>.SafeSetLogger(this.Logger, HttpProxyMetadata.ResourceForestLatencyBreakup, this.LatencyTracker.ResourceForestLatencyBreakup);
			RequestDetailsLoggerBase<RequestDetailsLogger>.SafeSetLogger(this.Logger, HttpProxyMetadata.TotalResourceForestLatency, this.LatencyTracker.TotalResourceForestDirectoryLatency);
			RequestDetailsLoggerBase<RequestDetailsLogger>.SafeSetLogger(this.Logger, ServiceLatencyMetadata.CallerADLatency, this.LatencyTracker.AdLatency);
			RequestDetailsLoggerBase<RequestDetailsLogger>.SafeSetLogger(this.Logger, HttpProxyMetadata.SharedCacheLatencyBreakup, this.LatencyTracker.SharedCacheLatencyBreakup);
			RequestDetailsLoggerBase<RequestDetailsLogger>.SafeSetLogger(this.Logger, HttpProxyMetadata.TotalSharedCacheLatency, this.LatencyTracker.TotalSharedCacheLatency);
			if (ExTraceGlobals.VerboseTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.VerboseTracer.TraceDebug<int, long, string>((long)this.GetHashCode(), "[ProxyRequestHandler::Complete]: Context {0}; ElapsedTime {1}; StatusCode {2}", this.TraceContext, this.LatencyTracker.GetCurrentLatency(LatencyTrackerKey.RequestHandlerLatency), (this.asyncException != null) ? this.asyncException.ToString() : this.ClientResponse.StatusCode.ToString(CultureInfo.InvariantCulture));
			}
		}

		private bool ShouldSetRequestTimeout(out TimeSpan timeout)
		{
			timeout = TimeSpan.Zero;
			string text = this.ClientRequest.Headers[Constants.FrontEndToBackEndTimeout];
			if (string.IsNullOrEmpty(text))
			{
				ExTraceGlobals.VerboseTracer.TraceDebug<int>((long)this.GetHashCode(), "[ProxyRequestHandler::ShouldSetRequestTimeout]: Timeout header not passed in the client request.", this.TraceContext);
				return false;
			}
			if (!this.ClientRequest.GetHttpRequestBase().IsProbeRequest())
			{
				if (ExTraceGlobals.VerboseTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					ExTraceGlobals.VerboseTracer.TraceDebug<string, int>((long)this.GetHashCode(), "[ProxyRequestHandler::ShouldSetRequestTimeout]: Not a monitoring request. Timeout won't be set. UserAgent: {0}", this.ClientRequest.UserAgent, this.TraceContext);
				}
				return false;
			}
			int num = -1;
			if (!int.TryParse(text, out num) || num < 0 || num > 300)
			{
				ExTraceGlobals.VerboseTracer.TraceDebug<string, string, int>((long)this.GetHashCode(), "[ProxyRequestHandler::ShouldSetRequestTimeout]: Invalid value used on the {0} header. Value: {1}", Constants.FrontEndToBackEndTimeout, text, this.TraceContext);
				return false;
			}
			timeout = TimeSpan.FromSeconds((double)num);
			return true;
		}

		private void SetupRequestTimeout(HttpWebRequest serverRequest, TimeSpan timeout)
		{
			this.requestState = new RequestState(new TimerCallback(this.RequestTimeoutCallback), serverRequest, (int)timeout.TotalMilliseconds);
		}

		private void RequestTimeoutCallback(object asyncObj)
		{
			HttpWebRequest httpWebRequest = asyncObj as HttpWebRequest;
			ExTraceGlobals.VerboseTracer.TraceDebug<int, int>((long)this.GetHashCode(), "[ProxyRequestHandler::RequestTimeoutCallback]: Request timed out. Request state: {0}", this.requestState.State, this.TraceContext);
			if (this.requestState.TryTransitionFromExecutingToTimedOut())
			{
				ExTraceGlobals.VerboseTracer.TraceDebug<int>((long)this.GetHashCode(), "[ProxyRequestHandler::RequestTimeoutCallback]: Calling request.Abort()", this.TraceContext);
				httpWebRequest.Abort();
			}
		}

		private bool ShouldCopyProxyResponseHeader(string headerName)
		{
			return !WebHeaderCollection.IsRestricted(headerName) && ProxyRequestHandler.RestrictedHeaders.All((string h) => StringComparer.OrdinalIgnoreCase.Compare(headerName, h) != 0);
		}

		private void CopyOrCreateNewXGccProxyInfoHeader(HttpWebRequest toRequest)
		{
			this.LogElapsedTime("E_XGcc");
			try
			{
				string value;
				if ((GccUtils.TryGetGccProxyInfo(this.HttpContext, out value) || GccUtils.TryCreateGccProxyInfo(this.HttpContext, out value)) && !string.IsNullOrEmpty(value))
				{
					toRequest.Headers.Add("X-GCC-PROXYINFO", value);
				}
			}
			finally
			{
				this.LogElapsedTime("L_XGcc");
			}
		}

		private void SetException(Exception e)
		{
			if (ExTraceGlobals.VerboseTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.VerboseTracer.TraceDebug<int, Exception>((long)this.GetHashCode(), "[ProxyRequestHandler::SetException]: Context {0}; Exception {1}", this.TraceContext, e);
			}
			if (this.asyncException == null)
			{
				this.asyncException = e;
			}
		}

		private void MakeCallback(object extraData)
		{
			this.asyncCallback(this);
		}

		private void OnResponseReady(object extraData)
		{
			this.CallThreadEntranceMethod(delegate
			{
				this.LogElapsedTime("E_OnRespReady");
				try
				{
					this.LatencyTracker.LogElapsedTimeAsLatency(this.Logger, LatencyTrackerKey.BackendProcessingLatency, HttpProxyMetadata.BackendProcessingLatency);
					IAsyncResult asyncResult = extraData as IAsyncResult;
					lock (this.LockObject)
					{
						ExTraceGlobals.VerboseTracer.TraceDebug<int>((long)this.GetHashCode(), "[ProxyRequestHandler::OnResponseReady]: Context {0}", this.TraceContext);
						if (this.requestState != null)
						{
							this.requestState.TryTransitionFromExecutingToFinished();
						}
						this.ServerRequest.Headers.Clear();
						try
						{
							this.Logger.LogCurrentTime("OnResponseReady");
							ConcurrencyGuardHelper.DecrementTargetBackendDagAndForest(this);
							WebResponse webResponse = this.ServerRequest.EndGetResponse(asyncResult);
							this.ThrowWebExceptionForRetryOnErrorTest(webResponse, new int[0]);
							this.Logger.LogCurrentTime("EndGetResponse");
							if (this.IsRetryOnErrorEnabled)
							{
								ProxyRequestHandler.DisposeIfNotNullAndCatchExceptions(this.ServerResponse);
								this.ServerResponse = null;
							}
							this.ServerResponse = (HttpWebResponse)webResponse;
							RequestDetailsLoggerBase<RequestDetailsLogger>.SafeSetLogger(this.Logger, HttpProxyMetadata.BackEndStatus, (int)this.ServerResponse.StatusCode);
							RequestDetailsLogger.PublishBackendDataToLog(this.Logger, this.ServerResponse);
						}
						catch (WebException ex)
						{
							this.Logger.LogCurrentTime("EndGetResponse");
							HttpStatusCode httpStatusCode = HttpStatusCode.OK;
							if (ex.Response != null)
							{
								httpStatusCode = ((HttpWebResponse)ex.Response).StatusCode;
								RequestDetailsLoggerBase<RequestDetailsLogger>.SafeSetLogger(this.Logger, HttpProxyMetadata.BackEndStatus, (int)httpStatusCode);
							}
							else
							{
								RequestDetailsLoggerBase<RequestDetailsLogger>.SafeSetLogger(this.Logger, HttpProxyMetadata.BackEndStatus, ex.Status);
							}
							if (this.IsAuthenticationChallengeFromBackend(ex))
							{
								bool flag2 = false;
								string text = ex.Response.Headers[Constants.AuthenticationHeader];
								if (this.TryFindKerberosChallenge(text, out flag2))
								{
									if (this.AuthBehavior.AuthState != AuthState.BackEndFullAuth)
									{
										ThreadPool.QueueUserWorkItem(new WaitCallback(this.BeginProxyRequest));
										this.State = ProxyRequestHandler.ProxyState.PrepareServerRequest;
										return;
									}
								}
								else if ((HttpProxyGlobals.ProtocolType == ProtocolType.Owa || HttpProxyGlobals.ProtocolType == ProtocolType.OwaCalendar || HttpProxyGlobals.ProtocolType == ProtocolType.Ecp) && this.ProxyToDownLevel && !flag2)
								{
									AspNetHelper.TransferToErrorPage(this.HttpContext, ErrorFE.FEErrorCodes.CAS14WithNoWIA);
									return;
								}
								if (!string.IsNullOrEmpty(text) && text.Contains(Constants.BearerAuthenticationType))
								{
									string text2 = ex.Response.Headers[MSDiagnosticsHeader.HeaderNameFromBackend];
									if (!string.IsNullOrEmpty(text2))
									{
										this.ClientResponse.AppendHeader(Constants.AuthenticationHeader, ConfigProvider.Instance.Configuration.ChallengeResponseString);
										this.HandleMSDiagnosticsHeader(text2);
									}
								}
							}
							if (this.ImplementsOutOfBandProxyLogon && !this.haveStartedOutOfBandProxyLogon && ex.Status == WebExceptionStatus.ProtocolError && ex.Response != null && httpStatusCode == this.StatusCodeSignifyingOutOfBandProxyLogonNeeded)
							{
								this.haveStartedOutOfBandProxyLogon = true;
								this.StartOutOfBandProxyLogon(null);
								return;
							}
							if (this.TryHandleProtocolSpecificResponseErrors(ex))
							{
								return;
							}
							this.CompleteWithError(ex, "HandleResponseError");
							return;
						}
						this.ProcessResponse(null);
					}
				}
				finally
				{
					this.LogElapsedTime("L_OnRespReady");
				}
			});
		}

		private void OnRequestStreamReady(object extraData)
		{
			this.CallThreadEntranceMethod(delegate
			{
				this.LogElapsedTime("E_ReqStreamReady");
				try
				{
					IAsyncResult asyncResult = extraData as IAsyncResult;
					lock (this.LockObject)
					{
						try
						{
							this.Logger.LogCurrentTime("OnRequestStreamReady");
							if (this.IsRetryOnErrorEnabled)
							{
								ProxyRequestHandler.DisposeIfNotNullAndCatchExceptions(this.ServerRequestStream);
								this.ServerRequestStream = null;
							}
							this.ServerRequestStream = this.ServerRequest.EndGetRequestStream(asyncResult);
							this.LatencyTracker.LogElapsedTimeAsLatency(this.Logger, LatencyTrackerKey.BackendRequestInitLatency, HttpProxyMetadata.BackendRequestInitLatency);
							if (this.ClientRequestStream == null)
							{
								this.ClientRequestStream = this.ClientRequest.GetBufferlessInputStream();
							}
							this.BeginRequestStreaming();
						}
						catch (WebException ex)
						{
							if (ex.Response != null)
							{
								RequestDetailsLoggerBase<RequestDetailsLogger>.SafeSetLogger(this.Logger, HttpProxyMetadata.BackEndStatus, (int)((HttpWebResponse)ex.Response).StatusCode);
							}
							else
							{
								RequestDetailsLoggerBase<RequestDetailsLogger>.SafeSetLogger(this.Logger, HttpProxyMetadata.BackEndStatus, ex.Status);
							}
							this.CompleteWithError(ex, "OnRequestStreamReady");
						}
						catch (HttpException ex2)
						{
							this.CompleteWithError(ex2, "OnRequestStreamReady");
						}
						catch (HttpProxyException ex3)
						{
							this.CompleteWithError(ex3, "OnRequestStreamReady");
						}
						catch (IOException ex4)
						{
							this.CompleteWithError(ex4, "OnRequestStreamReady");
						}
					}
				}
				finally
				{
					this.LogElapsedTime("L_ReqStreamReady");
				}
			});
		}

		private void BeginGetServerResponse()
		{
			this.LogElapsedTime("E_BegGetSvrResp");
			try
			{
				ConcurrencyGuardHelper.IncrementTargetBackendDagAndForest(this);
				this.LatencyTracker.StartTracking(LatencyTrackerKey.BackendResponseInitLatency, this.IsRetryingOnError);
				this.ServerRequest.BeginGetResponse(new AsyncCallback(ProxyRequestHandler.ResponseReadyCallback), this.ServerAsyncState);
				this.Logger.LogCurrentTime("BeginGetResponse");
				this.State = ProxyRequestHandler.ProxyState.WaitForServerResponse;
			}
			catch
			{
				ConcurrencyGuardHelper.DecrementTargetBackendDagAndForest(this);
				throw;
			}
			finally
			{
				this.LogElapsedTime("L_BegGetSvrResp");
				this.LatencyTracker.LogElapsedTimeAsLatency(this.Logger, LatencyTrackerKey.BackendResponseInitLatency, HttpProxyMetadata.BackendResponseInitLatency);
				this.LatencyTracker.StartTracking(LatencyTrackerKey.BackendProcessingLatency, this.IsRetryingOnError);
			}
		}

		private void CopyHeadersToServerRequest(HttpWebRequest destination)
		{
			this.LogElapsedTime("E_CopyHeadersSvrReq");
			NameValueCollection headers = this.ClientRequest.Headers;
			foreach (object obj in headers)
			{
				string text = (string)obj;
				try
				{
					string key;
					switch (key = text.ToUpperInvariant())
					{
					case "ACCEPT":
						destination.Accept = headers[text];
						continue;
					case "CONNECTION":
						HttpWebHelper.SetConnectionHeader(destination, headers[text]);
						continue;
					case "CONTENT-TYPE":
						destination.ContentType = headers[text];
						continue;
					case "CONTENT-LENGTH":
						if (!this.WillContentBeChangedDuringStreaming)
						{
							destination.ContentLength = long.Parse(headers[text], CultureInfo.InvariantCulture);
							continue;
						}
						continue;
					case "IF-MODIFIED-SINCE":
						HttpWebHelper.SetIfModifiedSince(destination, headers[text]);
						continue;
					case "PROXY-CONNECTION":
					case "COOKIE":
					case "ACCEPT-ENCODING":
					case "PROXY-AUTHORIZATION":
					case "HOST":
					case "EXPECT":
					case "X-EXCOMPID":
						continue;
					case "RANGE":
						HttpWebHelper.SetRange(destination, headers[text]);
						continue;
					case "REFERER":
						destination.Referer = headers[text];
						continue;
					case "TRANSFER-ENCODING":
					{
						string text2 = headers[text];
						if (text2 != null && this.ClientRequest.CanHaveBody() && text2.IndexOf("chunked", StringComparison.OrdinalIgnoreCase) >= 0)
						{
							destination.SendChunked = true;
							continue;
						}
						continue;
					}
					case "USER-AGENT":
						destination.UserAgent = headers[text];
						continue;
					case "AUTHORIZATION":
						if (this.AuthBehavior.AuthState == AuthState.BackEndFullAuth)
						{
							destination.Headers.Add(text, headers[text]);
							continue;
						}
						continue;
					}
					if (!WebHeaderCollection.IsRestricted(text) && this.ShouldCopyHeaderToServerRequest(text))
					{
						destination.Headers.Add(text, headers[text]);
					}
				}
				catch (ArgumentException innerException)
				{
					throw new HttpException(400, "Invalid HTTP header: " + text, innerException);
				}
			}
			if (this.ShouldSendFullActivityScope)
			{
				this.Logger.ActivityScope.SerializeTo(destination);
			}
			else
			{
				this.Logger.ActivityScope.SerializeMinimalTo(destination);
			}
			this.LogElapsedTime("L_CopyHeadersSvrReq");
		}

		private void CopyCookiesToClientResponse()
		{
			this.LogElapsedTime("E_CopyCookiesClientResp");
			foreach (object obj in this.ServerResponse.Cookies)
			{
				Cookie cookie = (Cookie)obj;
				if (cookie.Name.Equals("CopyLiveIdAuthCookieFromBE", StringComparison.OrdinalIgnoreCase))
				{
					LiveIdAuthenticationModule.ProcessFrontEndLiveIdAuthCookie(this.HttpContext, cookie.Value);
				}
				else if (this.ShouldCopyCookieToClientResponse(cookie))
				{
					this.CopyServerCookieToClientResponse(cookie);
				}
			}
			if (this.WillAddProtocolSpecificCookiesToClientResponse)
			{
				this.CopySupplementalCookiesToClientResponse();
			}
			this.LogElapsedTime("L_CopyCookiesClientResp");
		}

		private void SetResponseStatusIfHeadersUnsent(HttpResponse response, int status)
		{
			if (!this.ResponseHeadersSent)
			{
				response.StatusCode = status;
			}
		}

		private void SetResponseStatusIfHeadersUnsent(HttpResponse response, int status, string description)
		{
			if (!this.ResponseHeadersSent)
			{
				response.StatusCode = status;
				response.StatusDescription = description;
			}
		}

		private void SetResponseHeaderIfHeadersUnsent(HttpResponse response, string name, string value)
		{
			if (!this.ResponseHeadersSent)
			{
				response.Headers[name] = value;
			}
		}

		private void CopyHeadersToClientResponse()
		{
			this.LogElapsedTime("E_CopyHeadersClientResp");
			foreach (object obj in this.ServerResponse.Headers)
			{
				string text = (string)obj;
				string key;
				switch (key = text.ToUpperInvariant())
				{
				case "CONTENT-LENGTH":
				{
					HttpMethod httpMethod = this.ClientRequest.GetHttpMethod();
					if (httpMethod == HttpMethod.Head)
					{
						this.ClientResponse.Headers[text] = this.ServerResponse.Headers[text];
						continue;
					}
					continue;
				}
				case "CONTENT-TYPE":
					this.ClientResponse.ContentType = this.ServerResponse.ContentType;
					continue;
				case "CACHE-CONTROL":
					AspNetHelper.SetCacheability(this.ClientResponse, this.ServerResponse.Headers[text]);
					continue;
				case "X-FROMBACKEND-CLIENTCONNECTION":
					this.ClientResponse.Headers.Add(HttpRequestHeader.Connection.ToString(), this.ServerResponse.Headers[text]);
					continue;
				case "X-MS-DIAGNOSTICS-FROM-BACKEND":
				{
					string diagnostics = this.ServerResponse.Headers[MSDiagnosticsHeader.HeaderNameFromBackend];
					this.HandleMSDiagnosticsHeader(diagnostics);
					continue;
				}
				case "WWW-AUTHENTICATE":
					if (this.AuthBehavior.ShouldCopyAuthenticationHeaderToClientResponse)
					{
						this.ClientResponse.Headers.Add(text, this.ServerResponse.Headers[text]);
						continue;
					}
					continue;
				}
				if (this.ShouldCopyProxyResponseHeader(text))
				{
					this.ClientResponse.Headers.Add(text, this.ServerResponse.Headers[text]);
				}
			}
			this.LogElapsedTime("L_CopyHeadersClientResp");
		}

		private void CopyCookiesToServerRequest(HttpWebRequest serverRequest)
		{
			this.LogElapsedTime("E_CopyCookiesSvrReq");
			if (serverRequest.CookieContainer == null)
			{
				serverRequest.CookieContainer = new CookieContainer();
			}
			serverRequest.CookieContainer.PerDomainCapacity = int.MaxValue;
			for (int i = 0; i < this.ClientRequest.Cookies.Count; i++)
			{
				HttpCookie httpCookie = this.ClientRequest.Cookies[i];
				if (this.ShouldCopyCookieToServerRequest(httpCookie))
				{
					try
					{
						Cookie cookie = new Cookie();
						cookie.Name = httpCookie.Name;
						cookie.Value = httpCookie.Value;
						cookie.Domain = serverRequest.Address.Host;
						cookie.Path = httpCookie.Path;
						cookie.Expires = httpCookie.Expires;
						cookie.HttpOnly = httpCookie.HttpOnly;
						cookie.Secure = httpCookie.Secure;
						serverRequest.CookieContainer.Add(cookie);
					}
					catch (CookieException)
					{
					}
				}
			}
			if (this.WillAddProtocolSpecificCookiesToServerRequest)
			{
				this.AddProtocolSpecificCookiesToServerRequest(serverRequest.CookieContainer);
			}
			this.LogElapsedTime("L_CopyCookiesSvrReq");
		}

		private void ProcessResponse(WebException exception)
		{
			this.LogElapsedTime("E_ProcResp");
			ExTraceGlobals.VerboseTracer.TraceDebug<int, ProxyRequestHandler.ProxyState>((long)this.GetHashCode(), "[ProxyRequestHandler::ProcessResponse]: Context {0}; State {1}", this.TraceContext, this.State);
			if (this.ServerResponse == null)
			{
				if (this.ShouldRetryOnError)
				{
					this.RemoveNotNeededHttpContextContent();
					ProxyRequestHandler.DisposeIfNotNullAndCatchExceptions(this.ClientRequestStream);
					this.ClientRequestStream = null;
				}
				this.SetResponseStatusIfHeadersUnsent(this.ClientResponse, 503, "Unable to reach destination");
				this.Complete();
				ExTraceGlobals.VerboseTracer.TraceDebug<int>((long)this.GetHashCode(), "[ProxyRequestHandler::ProcessResponse]: Context {0}; NULL ServerResponse. Returning 503", this.TraceContext);
				return;
			}
			this.PfdTracer.TraceResponse("ProxyResponse", this.ServerResponse);
			if (ExTraceGlobals.VerboseTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.VerboseTracer.TraceDebug<int, HttpStatusCode>((long)this.GetHashCode(), "[ProxyRequestHandler::ProcessResponse]: Context {0}; ServerResponse.StatusCode {1}", this.TraceContext, this.ServerResponse.StatusCode);
			}
			if (this.HandleRoutingError(this.ServerResponse))
			{
				if (this.RecalculateTargetBackend())
				{
					return;
				}
			}
			else if (this.ShouldRetryOnError)
			{
				this.RemoveNotNeededHttpContextContent();
				ProxyRequestHandler.DisposeIfNotNullAndCatchExceptions(this.ClientRequestStream);
				this.ClientRequestStream = null;
			}
			int statusCode = (int)this.ServerResponse.StatusCode;
			this.SetResponseStatusIfHeadersUnsent(this.ClientResponse, statusCode, Utilities.GetTruncatedString(this.ServerResponse.StatusDescription, 512));
			if (exception != null)
			{
				this.SetRequestFailureContext(RequestFailureContext.RequestFailurePoint.BackEnd, statusCode, statusCode.ToString(), exception.ToString(), null, new WebExceptionStatus?(exception.Status));
			}
			if (exception == null && !this.IsRoutingError(this.ServerResponse) && this.newBackendForRetry != null)
			{
				Guid key = this.newBackendForRetry.Value.Key;
				BackEndServer value = this.newBackendForRetry.Value.Value;
				string resourceForestFqdn;
				if (Utilities.TryExtractForestFqdnFromServerFqdn(value.Fqdn, out resourceForestFqdn))
				{
					MailboxServerCache.Instance.Add(key, value, resourceForestFqdn, false, this);
					ExTraceGlobals.VerboseTracer.TraceDebug<int>((long)this.GetHashCode(), "[ProxyRequestHandler::ProcessResponse]: Context {0}; Added the X-DBMountedOn header value to the cache after successful retry.", this.TraceContext);
				}
			}
			this.CopyHeadersToClientResponse();
			this.CopyCookiesToClientResponse();
			this.HandleLogoffRequest();
			this.ClientResponse.ContentType = this.ServerResponse.ContentType;
			this.PfdTracer.TraceHeaders("ProxyResponse", this.ServerResponse.Headers, this.ClientResponse.Headers);
			this.PfdTracer.TraceCookies("ProxyResponse", this.ServerResponse.Cookies, this.ClientResponse.Cookies);
			this.CleanUpRequestStreamsAndBuffer();
			ProxyRequestHandler.DisposeIfNotNullAndCatchExceptions(this.authenticationContext);
			this.authenticationContext = null;
			this.kerberosChallenge = null;
			this.BeginResponseStreaming();
			this.LogElapsedTime("L_ProcResp");
		}

		private void LogForRetry(string key, bool logLatency, params HttpProxyMetadata[] preservedMetadataEntries)
		{
			StringBuilder stringBuilder = new StringBuilder();
			string format = "{0}:{1} ";
			if (logLatency)
			{
				stringBuilder.AppendFormat(format, "TotalRequestTime", this.LatencyTracker.GetCurrentLatency(LatencyTrackerKey.ProxyModuleLatency));
				stringBuilder.AppendFormat(format, "Delay", this.delayOnRetryOnError);
				stringBuilder.AppendFormat(format, "State", this.State);
			}
			foreach (HttpProxyMetadata httpProxyMetadata in preservedMetadataEntries)
			{
				stringBuilder.AppendFormat(format, httpProxyMetadata, this.Logger.Get(httpProxyMetadata) ?? string.Empty);
				RequestDetailsLoggerBase<RequestDetailsLogger>.SafeSetLogger(this.Logger, httpProxyMetadata, string.Empty);
			}
			this.Logger.AppendGenericError(key, stringBuilder.ToString());
		}

		private void RemoveNotNeededHttpContextContent()
		{
			if (this.ClientRequest.IsAuthenticated)
			{
				if (!this.ProxyToDownLevel)
				{
					IIdentity identity = this.HttpContext.User.Identity;
					WindowsIdentity windowsIdentity = identity as WindowsIdentity;
					if (windowsIdentity != null)
					{
						this.HttpContext.User = new GenericPrincipal(new GenericIdentity(identity.GetSafeName(true), identity.AuthenticationType), null);
						windowsIdentity.Dispose();
					}
				}
				this.HttpContext.Items["Item-CommonAccessToken"] = null;
			}
		}

		private void HandleMSDiagnosticsHeader(string diagnostics)
		{
			if (string.IsNullOrEmpty(diagnostics))
			{
				return;
			}
			if (this.HttpContext == null || this.HttpContext.User == null)
			{
				return;
			}
			OAuthIdentity oauthIdentity = this.HttpContext.User.Identity as OAuthIdentity;
			if (oauthIdentity != null)
			{
				OAuthErrors oauthErrors;
				string text;
				if (MSDiagnosticsHeader.TryParseHeaderFromBackend(diagnostics, out oauthErrors, out text))
				{
					OAuthErrorCategory errorCategory = OAuthErrorsUtil.GetErrorCategory(oauthErrors);
					this.HttpContext.Items["OAuthError"] = text;
					this.HttpContext.Items["OAuthErrorCategory"] = errorCategory + "-BE";
					string str = this.HttpContext.Items["OAuthExtraInfo"] as string;
					this.HttpContext.Items["OAuthExtraInfo"] = str + string.Format("ErrorCode:{0}", oauthErrors);
					MSDiagnosticsHeader.AppendToResponse(errorCategory, text, this.ClientResponse);
					return;
				}
				this.Logger.AppendAuthError("OAuth", diagnostics);
			}
		}

		private void AlterAuthBehaviorStateForBEAuthTest()
		{
			string empty = string.Empty;
			ExTraceGlobals.FaultInjectionTracer.TraceTest<string>(3330682173U, ref empty);
			if (!string.IsNullOrEmpty(empty) && empty.StartsWith("BEAuth"))
			{
				PropertyInfo property = typeof(DefaultAuthBehavior).GetProperty("AuthState", BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty);
				if (empty.Contains("FrontEndContinueAuth-FrontEndContinueAuth"))
				{
					property.SetValue(this.AuthBehavior, AuthState.FrontEndContinueAuth);
					return;
				}
				if (empty.Contains("BackEndFullAuth-BackEndFullAuth"))
				{
					property.SetValue(this.AuthBehavior, AuthState.BackEndFullAuth);
					return;
				}
				if (empty.Contains("FrontEndContinueAuth-BackEndFullAuth"))
				{
					if (!this.IsRetryingOnError)
					{
						property.SetValue(this.AuthBehavior, AuthState.FrontEndContinueAuth);
						return;
					}
					property.SetValue(this.AuthBehavior, AuthState.BackEndFullAuth);
					return;
				}
				else if (empty.Contains("BackEndFullAuth-FrontEndContinueAuth"))
				{
					if (!this.IsRetryingOnError)
					{
						property.SetValue(this.AuthBehavior, AuthState.BackEndFullAuth);
						return;
					}
					property.SetValue(this.AuthBehavior, AuthState.FrontEndContinueAuth);
				}
			}
		}

		private void AddRoutingEntryHeaderToRequest(HttpWebRequest serverRequest)
		{
			this.routingEntry = this.AnchoredRoutingTarget.AnchorMailbox.GetRoutingEntry();
			if (this.routingEntry != null)
			{
				string value = RoutingEntryHeaderSerializer.Serialize(this.routingEntry);
				serverRequest.Headers["X-LegacyRoutingEntry"] = value;
				RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericInfo(this.Logger, "RoutingEntry", value);
			}
		}

		protected virtual void ExposeExceptionToClientResponse(Exception ex)
		{
		}

		protected virtual bool ShouldLogClientDisconnectError(Exception ex)
		{
			return true;
		}

		protected void CompleteWithError(Exception ex, string label)
		{
			ExTraceGlobals.VerboseTracer.TraceError<int, string, string>((long)this.GetHashCode(), "[ProxyRequestHandler::CompleteWithError]: Context {0}; Label {1}; Exception: {2}", this.TraceContext, label, ex.ToString());
			this.ExposeExceptionToClientResponse(ex);
			if (ex is MaxConcurrencyReachedException)
			{
				MaxConcurrencyReachedException ex2 = ex as MaxConcurrencyReachedException;
				HttpProxySubErrorCode errorCode = HttpProxySubErrorCode.TooManyOutstandingProxyRequests;
				if (object.ReferenceEquals(ex2.Guard, ConcurrencyGuards.TargetDag))
				{
					errorCode = HttpProxySubErrorCode.TooManyOutstandingProxyRequestsToDag;
				}
				else if (object.ReferenceEquals(ex2.Guard, ConcurrencyGuards.TargetForest))
				{
					errorCode = HttpProxySubErrorCode.ServerKerberosAuthenticationFailure;
				}
				ex = new HttpProxyException(HttpStatusCode.ServiceUnavailable, errorCode, ((MaxConcurrencyReachedException)ex).Message, ex);
			}
			if (ex is WebException)
			{
				if (!this.HandleWebException(ex as WebException))
				{
					return;
				}
			}
			else if (ex is HttpProxyException)
			{
				if (!this.HandleHttpProxyException(ex as HttpProxyException))
				{
					return;
				}
			}
			else if (ex is HttpException)
			{
				if (!this.HandleHttpException(ex as HttpException))
				{
					return;
				}
			}
			else if (this.IsPossibleException(ex))
			{
				this.Logger.AppendGenericError("PossibleException", ex.GetType().Name);
				this.HttpContext.Server.ClearError();
				this.SetResponseStatusIfHeadersUnsent(this.ClientResponse, 500);
				this.LogErrorCode(Constants.InternalServerErrorStatusCode);
			}
			else
			{
				this.SetException(ex);
				this.UpdateRoutingFailurePerfCounter(true);
				this.Logger.AppendGenericError("UnexpectedException", ex.ToString());
			}
			this.Complete();
		}

		protected virtual void LogWebException(WebException exception)
		{
			this.Logger.AppendGenericError("WebExceptionStatus", exception.Status.ToString());
			HttpWebResponse httpWebResponse = (HttpWebResponse)exception.Response;
			int num = 0;
			if (httpWebResponse != null)
			{
				num = (int)httpWebResponse.StatusCode;
				this.Logger.AppendGenericError("ResponseStatusCode", num.ToString());
			}
			if (num != 500)
			{
				this.Logger.AppendGenericError("WebException", exception.ToString());
			}
		}

		protected bool HandleWebExceptionConnectivityError(WebException exception)
		{
			HttpWebHelper.ConnectivityError connectivityError = HttpWebHelper.CheckConnectivityError(exception);
			HttpWebResponse response = (HttpWebResponse)exception.Response;
			if (this.IsRetryOnConnectivityErrorEnabled && this.IsInRetryableState && connectivityError == HttpWebHelper.ConnectivityError.Retryable)
			{
				ExTraceGlobals.VerboseTracer.TraceDebug<int>((long)this.GetHashCode(), "[ProxyRequestHandler::HandleWebExceptionConnectivityError]: Context {0}; retryable connectivity exception thrown; will invalidate cache and set delay.", this.TraceContext);
				this.InvalidateBackEndServerCacheSetDelay(response, false);
				if (this.RecalculateTargetBackend())
				{
					return true;
				}
			}
			else if (connectivityError != HttpWebHelper.ConnectivityError.None)
			{
				ExTraceGlobals.VerboseTracer.TraceDebug<int>((long)this.GetHashCode(), "[ProxyRequestHandler::HandleWebExceptionConnectivityError]: Context {0}; nonretryable connectivity exception thrown; will invalidate cache.", this.TraceContext);
				this.InvalidateBackEndServerCache(response);
			}
			return false;
		}

		private void LogErrorCode(string errorCode)
		{
			RequestDetailsLoggerBase<RequestDetailsLogger>.SafeSetLogger(this.Logger, ServiceCommonMetadata.ErrorCode, errorCode);
		}

		private bool HandleWebException(WebException exception)
		{
			this.LogWebException(exception);
			if (this.HandleWebExceptionConnectivityError(exception))
			{
				return false;
			}
			string errorCode = exception.Status.ToString();
			if (exception.Response != null)
			{
				HttpWebResponse httpWebResponse = (HttpWebResponse)exception.Response;
				int statusCode = (int)httpWebResponse.StatusCode;
				if (statusCode != 401 || this.AuthBehavior.ShouldCopyAuthenticationHeaderToClientResponse)
				{
					this.HttpContext.Server.ClearError();
					ProxyRequestHandler.DisposeIfNotNullAndCatchExceptions(this.ServerResponse);
					this.ServerResponse = null;
					this.ServerResponse = (HttpWebResponse)exception.Response;
					this.ProcessResponse(exception);
					return false;
				}
				this.haveReceivedAuthChallenge = false;
				bool flag = false;
				if (this.IsAuthenticationChallengeFromBackend(exception) && this.TryFindKerberosChallenge(exception.Response.Headers[Constants.AuthenticationHeader], out flag))
				{
					this.HttpContext.Server.ClearError();
					this.SetResponseStatusIfHeadersUnsent(this.ClientResponse, 503, Utilities.GetTruncatedString("Failed authentication on backend server: " + httpWebResponse.StatusDescription, 512));
					this.SetRequestFailureContext(RequestFailureContext.RequestFailurePoint.BackEnd, statusCode, statusCode.ToString(), exception.ToString(), null, null);
					errorCode = Constants.ServerKerberosAuthenticationFailureErrorCode;
				}
				else
				{
					this.SetResponseStatusIfHeadersUnsent(this.ClientResponse, statusCode, Utilities.GetTruncatedString(httpWebResponse.StatusDescription, 512));
					this.SetRequestFailureContext(RequestFailureContext.RequestFailurePoint.BackEnd, statusCode, statusCode.ToString(), exception.ToString(), null, null);
				}
			}
			else if (exception.Status == WebExceptionStatus.Timeout || exception.Status == WebExceptionStatus.RequestCanceled)
			{
				this.HttpContext.Server.ClearError();
				this.SetResponseStatusIfHeadersUnsent(this.ClientResponse, 504);
				this.SetRequestFailureContext(RequestFailureContext.RequestFailurePoint.BackEnd, -1, exception.Status.ToString(), exception.Message, new HttpProxySubErrorCode?(HttpProxySubErrorCode.BackEndRequestTimedOut), new WebExceptionStatus?(exception.Status));
			}
			else
			{
				this.HttpContext.Server.ClearError();
				this.SetResponseStatusIfHeadersUnsent(this.ClientResponse, 503);
				this.SetRequestFailureContext(RequestFailureContext.RequestFailurePoint.BackEnd, -1, exception.Status.ToString(), exception.Message, null, new WebExceptionStatus?(exception.Status));
			}
			this.LogErrorCode(errorCode);
			return true;
		}

		private bool HandleHttpProxyException(HttpProxyException exception)
		{
			this.HttpContext.Server.ClearError();
			string text = exception.ErrorCode.ToString();
			if (exception.StatusCode != HttpStatusCode.Unauthorized)
			{
				this.Logger.AppendGenericError("HttpProxyException", exception.ToString());
			}
			if (exception.StatusCode == HttpStatusCode.InternalServerError && !(exception.InnerException is NonUniqueRecipientException))
			{
				this.UpdateRoutingFailurePerfCounter(true);
			}
			this.SetResponseHeaderIfHeadersUnsent(this.ClientResponse, Constants.CafeErrorCodeHeaderName, text);
			this.SetResponseStatusIfHeadersUnsent(this.ClientResponse, (int)exception.StatusCode);
			this.SetRequestFailureContext(RequestFailureContext.RequestFailurePoint.FrontEnd, (int)exception.StatusCode, exception.ErrorCode.ToString(), exception.Message, new HttpProxySubErrorCode?(exception.ErrorCode), null);
			this.LogErrorCode(text);
			return true;
		}

		private bool HandleHttpException(HttpException exception)
		{
			string errorCode = string.Empty;
			InvalidOAuthTokenException ex = exception.InnerException as InvalidOAuthTokenException;
			if (ex != null)
			{
				errorCode = Constants.InvalidOAuthTokenErrorCode;
				this.HandleOAuthException(ex);
			}
			ServerSideTransferException ex2 = exception as ServerSideTransferException;
			if (ex2 != null)
			{
				AspNetHelper.TransferToRedirectPage(this.HttpContext, ex2.RedirectUrl, ex2.RedirectType);
				this.CompleteForRedirect(ex2.RedirectUrl);
				return false;
			}
			if (exception.GetHttpCode() == 302)
			{
				this.ClientResponse.Redirect(exception.Message, false);
				this.CompleteForRedirect(exception.Message);
				return false;
			}
			if (this.ClientResponse != null && !this.ClientResponse.IsClientConnected && AspNetHelper.IsExceptionExpectedWhenDisconnected(exception))
			{
				if (this.ShouldLogClientDisconnectError(exception))
				{
					this.Logger.AppendGenericError("HttpException", "ClientDisconnect");
					errorCode = Constants.ClientDisconnectErrorCode;
				}
			}
			else
			{
				this.HttpContext.Server.ClearError();
				int httpCode = exception.GetHttpCode();
				string value = exception.Message;
				if (httpCode == 500)
				{
					value = exception.ToString();
				}
				this.Logger.AppendGenericError("HttpException", value);
				errorCode = ((HttpStatusCode)httpCode).ToString();
				this.SetResponseStatusIfHeadersUnsent(this.ClientResponse, httpCode);
				try
				{
					this.ClientResponse.Write(exception.Message);
					this.SetRequestFailureContext(RequestFailureContext.RequestFailurePoint.FrontEnd, exception.GetHttpCode(), exception.Message, exception.ToString(), null, null);
				}
				catch
				{
				}
			}
			this.LogErrorCode(errorCode);
			return true;
		}

		private void HandleOAuthException(InvalidOAuthTokenException exception)
		{
			MSDiagnosticsHeader.AppendChallengeAndDiagnosticsHeadersToResponse(this.ClientResponse, exception.ErrorCategory, exception.Message);
			this.HttpContext.Items["OAuthError"] = exception.ToString();
			this.HttpContext.Items["OAuthErrorCategory"] = exception.ErrorCategory.ToString();
			string str = this.HttpContext.Items["OAuthExtraInfo"] as string;
			this.HttpContext.Items["OAuthExtraInfo"] = str + string.Format("ErrorCode:{0}", exception.ErrorCode);
		}

		private bool IsPossibleException(Exception exception)
		{
			return exception is SocketException || exception is IOException || exception is ProtocolViolationException;
		}

		private void UpdateRoutingFailurePerfCounter(bool wasFailure)
		{
			if (!PerfCounters.RoutingErrorsEnabled)
			{
				return;
			}
			string siteName;
			if (this.AnchoredRoutingTarget == null)
			{
				siteName = string.Empty;
			}
			else
			{
				siteName = this.AnchoredRoutingTarget.GetSiteName();
			}
			if (!wasFailure)
			{
				PerfCounters.IncrementMovingPercentagePerformanceCounterBase(PerfCounters.GetHttpProxyPerSiteCountersInstance(siteName).MovingPercentageRoutingFailure);
				PerfCounters.IncrementMovingPercentagePerformanceCounterBase(PerfCounters.GetHttpProxyPerSiteCountersInstance(string.Empty).MovingPercentageRoutingFailure);
				return;
			}
			if (this.IsInRoutingState)
			{
				PerfCounters.UpdateMovingPercentagePerformanceCounter(PerfCounters.GetHttpProxyPerSiteCountersInstance(siteName).MovingPercentageRoutingFailure);
				PerfCounters.GetHttpProxyPerSiteCountersInstance(siteName).TotalFailedRequests.Increment();
				return;
			}
			if (this.IsInPostRoutingState)
			{
				return;
			}
			throw new NotImplementedException("No implementation for ProxyState");
		}

		private void SetRequestFailureContext(RequestFailureContext.RequestFailurePoint failurePoint, int statusCode, string error, string details, HttpProxySubErrorCode? httpProxySubErrorCode = null, WebExceptionStatus? webExceptionStatus = null)
		{
			RequestFailureContext value = new RequestFailureContext(failurePoint, statusCode, error, details, httpProxySubErrorCode, webExceptionStatus, null);
			this.HttpContext.Items[RequestFailureContext.HttpContextKeyName] = value;
		}

		private void InspectDisconnectException(Exception exception)
		{
			HttpException ex = exception as HttpException;
			if (ex != null)
			{
				int errorCode = ex.ErrorCode;
				int num = 0;
				if (ex.InnerException != null && ex.InnerException is COMException)
				{
					num = ((COMException)ex.InnerException).ErrorCode;
				}
				string message = string.Format("Unexpected HttpException with error code {0} - {1}, details {2}", errorCode, num, ex.ToString());
				throw new HttpException(message, exception);
			}
			throw exception;
		}

		protected virtual bool UseBackEndCacheForDownLevelServer
		{
			get
			{
				return true;
			}
		}

		protected DatacenterRedirectStrategy DatacenterRedirectStrategy
		{
			get
			{
				if (this.datacenterRedirectStrategy == null)
				{
					this.datacenterRedirectStrategy = this.CreateDatacenterRedirectStrategy();
				}
				return this.datacenterRedirectStrategy;
			}
		}

		protected virtual bool IsBackendServerCacheValidationEnabled
		{
			get
			{
				return false;
			}
		}

		protected AnchoredRoutingTarget AnchoredRoutingTarget { get; set; }

		protected bool ProxyToDownLevel { get; set; }

		protected virtual DatacenterRedirectStrategy CreateDatacenterRedirectStrategy()
		{
			return new DefaultRedirectStrategy(this);
		}

		protected virtual AnchorMailbox ResolveAnchorMailbox()
		{
			this.LogElapsedTime("E_BaseResAnchMbx");
			AnchorMailbox anchorMailbox = null;
			if (!this.HasPreemptivelyCheckedForRoutingHint)
			{
				anchorMailbox = this.CreateAnchorMailboxFromRoutingHint();
			}
			if (anchorMailbox == null)
			{
				anchorMailbox = AnchorMailboxFactory.CreateFromCaller(this);
			}
			this.LogElapsedTime("L_BaseResAnchMbx");
			return anchorMailbox;
		}

		protected virtual bool ShouldRecalculateProxyTarget()
		{
			return false;
		}

		protected virtual AnchoredRoutingTarget TryFastTargetCalculationByAnchorMailbox(AnchorMailbox anchorMailbox)
		{
			this.LogElapsedTime("E_TryFastTargetCalc");
			BackEndServer backEndServer = anchorMailbox.TryDirectBackEndCalculation();
			if (backEndServer != null)
			{
				ExTraceGlobals.VerboseTracer.TraceDebug<AnchoredRoutingTarget>((long)this.GetHashCode(), "[ProxyRequestHandler::TryFastTargetCalculationByAnchorMailbox]: Resolved target {0} directly from anchor mailbox.", this.AnchoredRoutingTarget);
				return new AnchoredRoutingTarget(anchorMailbox, backEndServer);
			}
			this.LogElapsedTime("L_TryFastTargetCalc");
			return null;
		}

		protected virtual BackEndServer GetDownLevelClientAccessServer(AnchorMailbox anchorMailbox, BackEndServer mailboxServer)
		{
			this.LogElapsedTime("E_GetDLCAS");
			Uri uri = null;
			BackEndServer downLevelClientAccessServer = DownLevelServerManager.Instance.GetDownLevelClientAccessServer<WebServicesService>(anchorMailbox, mailboxServer, ClientAccessType.InternalNLBBypass, this.Logger, false, out uri);
			this.LogElapsedTime("L_GetDLCAS");
			return downLevelClientAccessServer;
		}

		protected virtual AnchoredRoutingTarget TryDirectTargetCalculation()
		{
			return null;
		}

		protected virtual bool HandleBackEndCalculationException(Exception exception, AnchorMailbox anchorMailbox, string label)
		{
			this.LogElapsedTime("E_HandleBECalcEx");
			bool result;
			try
			{
				ExTraceGlobals.VerboseTracer.TraceError<int, AnchorMailbox, Exception>((long)this.GetHashCode(), "[ProxyRequestHandler::HandleBackEndCalculationException]: Context {0}. Handling backend calculation exception for anchor mailbox {1}. Exception: {2}", this.TraceContext, anchorMailbox, exception);
				if (exception is HttpException || exception is HttpProxyException)
				{
					this.CompleteWithError(exception, label);
					result = true;
				}
				else if (exception is ADTransientException || exception is DataValidationException || exception is DataSourceOperationException || exception is NonUniqueRecipientException)
				{
					Exception ex = new HttpProxyException(HttpStatusCode.InternalServerError, HttpProxySubErrorCode.DirectoryOperationError, exception.Message, exception);
					this.CompleteWithError(ex, label);
					result = true;
				}
				else
				{
					if (exception is RemoteForestDownLevelServerException)
					{
						RemoteForestDownLevelServerException ex2 = (RemoteForestDownLevelServerException)exception;
						try
						{
							this.Logger.AppendGenericError("RemoteForestDownLevelServerException", string.Format("{0}@{1}", ex2.DatabaseId, ex2.ResourceForest));
							this.DatacenterRedirectStrategy.RedirectMailbox(anchorMailbox);
						}
						catch (HttpException ex3)
						{
							this.CompleteWithError(ex3, label);
							return true;
						}
						catch (HttpProxyException ex4)
						{
							this.CompleteWithError(ex4, label);
							return true;
						}
					}
					if (exception is ServerLocatorClientException || exception is ServerLocatorClientTransientException || exception is MailboxServerLocatorException || exception is AmServerTransientException || exception is AmServerException)
					{
						PerfCounters.HttpProxyCountersInstance.MailboxServerLocatorFailedCalls.Increment();
						PerfCounters.UpdateMovingPercentagePerformanceCounter(PerfCounters.HttpProxyCountersInstance.MovingPercentageMailboxServerLocatorFailedCalls);
						Exception ex = new HttpProxyException(HttpStatusCode.InternalServerError, HttpProxySubErrorCode.ServerLocatorError, exception.Message, exception);
						this.CompleteWithError(ex, label);
						result = true;
					}
					else if (exception is DatabaseNotFoundException)
					{
						this.OnDatabaseNotFound(anchorMailbox);
						Exception ex;
						if (anchorMailbox is DatabaseNameAnchorMailbox)
						{
							ex = new HttpProxyException(HttpStatusCode.NotFound, HttpProxySubErrorCode.DatabaseNameNotFound, exception.Message, exception);
						}
						else if (anchorMailbox is DatabaseGuidAnchorMailbox)
						{
							ex = new HttpProxyException(HttpStatusCode.NotFound, HttpProxySubErrorCode.DatabaseGuidNotFound, exception.Message, exception);
						}
						else
						{
							ex = new HttpProxyException(HttpStatusCode.InternalServerError, HttpProxySubErrorCode.DatabaseGuidNotFound, exception.Message, exception);
						}
						this.CompleteWithError(ex, label);
						result = true;
					}
					else if (exception is ServerNotFoundException)
					{
						Exception ex;
						if (anchorMailbox is ServerInfoAnchorMailbox)
						{
							ex = new HttpProxyException(HttpStatusCode.NotFound, HttpProxySubErrorCode.ServerNotFound, exception.Message, exception);
						}
						else if (anchorMailbox != null && anchorMailbox.AnchorSource == AnchorSource.ServerVersion)
						{
							ex = new HttpProxyException(HttpStatusCode.NotFound, HttpProxySubErrorCode.ServerVersionNotFound, exception.Message, exception);
						}
						else
						{
							ex = new HttpProxyException(HttpStatusCode.InternalServerError, HttpProxySubErrorCode.ServerNotFound, exception.Message, exception);
						}
						this.CompleteWithError(ex, label);
						result = true;
					}
					else if (exception is ObjectNotFoundException)
					{
						Exception ex = new HttpProxyException(HttpStatusCode.InternalServerError, HttpProxySubErrorCode.ServerNotFound, exception.Message, exception);
						this.CompleteWithError(ex, label);
						result = true;
					}
					else if (exception is ServiceDiscoveryPermanentException || exception is ServiceDiscoveryTransientException)
					{
						Exception ex = new HttpProxyException(HttpStatusCode.InternalServerError, HttpProxySubErrorCode.ServerDiscoveryError, exception.Message, exception);
						this.CompleteWithError(ex, label);
						result = true;
					}
					else
					{
						ExTraceGlobals.VerboseTracer.TraceError<int, AnchorMailbox, Exception>((long)this.GetHashCode(), "[ProxyRequestHandler::HandleBackEndCalculationException]: Context {0}. BackEnd calculation exception unhandled for anchor mailbox {1}. Exception: {2}", this.TraceContext, anchorMailbox, exception);
						result = false;
					}
				}
			}
			finally
			{
				this.LogElapsedTime("L_HandleBECalcEx");
			}
			return result;
		}

		protected virtual void RedirectIfNeeded(BackEndServer mailbox)
		{
		}

		protected virtual void OnDatabaseNotFound(AnchorMailbox anchorMailbox)
		{
			ExTraceGlobals.VerboseTracer.TraceDebug<int>((long)this.GetHashCode(), "[ProxyRequestHandler::OnDatabaseNotFound]: Context {0}; invalidating cache.", this.TraceContext);
			if (anchorMailbox == null)
			{
				return;
			}
			ADObjectId adobjectId = null;
			DatabaseBasedAnchorMailbox databaseBasedAnchorMailbox = anchorMailbox as DatabaseBasedAnchorMailbox;
			if (databaseBasedAnchorMailbox != null)
			{
				try
				{
					adobjectId = databaseBasedAnchorMailbox.GetDatabase();
				}
				catch (DatabaseNotFoundException)
				{
				}
			}
			if (adobjectId != null)
			{
				RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericInfo(this.Logger, "OnDatabaseNotFound", adobjectId.ObjectGuid);
				MailboxServerCache.Instance.Remove(adobjectId.ObjectGuid, this);
			}
			anchorMailbox.InvalidateCache();
		}

		protected virtual void UpdateOrInvalidateAnchorMailboxCache(Guid mdbGuid, string resourceForest)
		{
			this.AnchoredRoutingTarget.AnchorMailbox.InvalidateCache();
		}

		protected virtual bool IsRoutingError(HttpWebResponse response)
		{
			string text;
			return this.TryGetSpecificHeaderFromResponse(response, "ProxyRequestHandler::IsRoutingError", Constants.BEServerExceptionHeaderName, Constants.IllegalCrossServerConnectionExceptionType, out text) || this.TryGetSpecificHeaderFromResponse(response, "ProxyRequestHandler::IsRoutingError", Constants.BEServerRoutingErrorHeaderName, null, out text);
		}

		protected virtual AnchoredRoutingTarget DoProtocolSpecificRoutingTargetOverride(AnchoredRoutingTarget routingTarget)
		{
			if (routingTarget == null)
			{
				throw new ArgumentNullException("routingTarget");
			}
			this.RedirectIfNeeded(routingTarget.BackEndServer);
			return null;
		}

		protected virtual bool ShouldContinueProxy()
		{
			return false;
		}

		protected virtual Uri GetTargetBackEndServerUrl()
		{
			this.LogElapsedTime("E_TargetBEUrl");
			Uri result;
			try
			{
				UrlAnchorMailbox urlAnchorMailbox = this.AnchoredRoutingTarget.AnchorMailbox as UrlAnchorMailbox;
				if (urlAnchorMailbox != null)
				{
					result = urlAnchorMailbox.Url;
				}
				else
				{
					UriBuilder clientUrlForProxy = this.GetClientUrlForProxy();
					clientUrlForProxy.Scheme = Uri.UriSchemeHttps;
					clientUrlForProxy.Host = this.AnchoredRoutingTarget.BackEndServer.Fqdn;
					clientUrlForProxy.Port = 444;
					if (this.AnchoredRoutingTarget.BackEndServer.Version < Server.E15MinVersion)
					{
						this.ProxyToDownLevel = true;
						RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericInfo(this.Logger, "ProxyToDownLevel", true);
						clientUrlForProxy.Port = 443;
					}
					result = clientUrlForProxy.Uri;
				}
			}
			finally
			{
				this.LogElapsedTime("L_TargetBEUrl");
			}
			return result;
		}

		protected virtual UriBuilder GetClientUrlForProxy()
		{
			return new UriBuilder(this.ClientRequest.Url);
		}

		protected virtual MailboxServerLocator CreateMailboxServerLocator(Guid databaseGuid, string domainName, string resourceForest)
		{
			return MailboxServerLocator.Create(databaseGuid, domainName, resourceForest, !this.IsRetryingOnError);
		}

		protected bool TryGetSpecificHeaderFromResponse(HttpWebResponse response, string functionName, string headerName, string expectedHeaderValue, out string headerValue)
		{
			headerValue = null;
			if (response != null)
			{
				headerValue = response.Headers[headerName];
				if (!string.IsNullOrEmpty(headerValue) && (expectedHeaderValue == null || headerValue.Equals(expectedHeaderValue, StringComparison.OrdinalIgnoreCase)))
				{
					ExTraceGlobals.VerboseTracer.TraceDebug((long)this.GetHashCode(), "[{3}]: Context {0}; found {1} in {2} header.", new object[]
					{
						this.TraceContext,
						headerValue,
						headerName,
						functionName
					});
					RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericInfo(this.Logger, "RoutingError", headerName + "(" + headerValue + ")");
					return true;
				}
			}
			return false;
		}

		protected virtual void BeginValidateBackendServerCache()
		{
			throw new NotImplementedException("Backend server cache validation not implemented for this handler");
		}

		protected void BeginProxyRequestOrRecalculate()
		{
			lock (this.LockObject)
			{
				if (this.State != ProxyRequestHandler.ProxyState.CalculateBackEndSecondRound && this.ShouldRecalculateProxyTarget())
				{
					ExTraceGlobals.VerboseTracer.TraceDebug<int>((long)this.GetHashCode(), "[ProxyRequestHandler::BeginProxyRequestOrRecalculate]: context {0}, protocal require 2nd round calculation. Start 2nd round BeginCalculateTargetBackEnd again.", this.TraceContext);
					ThreadPool.QueueUserWorkItem(new WaitCallback(this.BeginCalculateTargetBackEnd));
					this.State = ProxyRequestHandler.ProxyState.CalculateBackEndSecondRound;
				}
				else if (this.ShouldContinueProxy())
				{
					ExTraceGlobals.VerboseTracer.TraceDebug<int>((long)this.GetHashCode(), "[ProxyRequestHandler::BeginProxyRequestOrRecalculate]: ShouldProxy == false.  No need to process futher.  Context {0}.", this.TraceContext);
					this.Complete();
				}
				else
				{
					ExTraceGlobals.VerboseTracer.TraceDebug<int, AnchoredRoutingTarget>((long)this.GetHashCode(), "[ProxyRequestHandler::BeginProxyRequestOrRecalculate]: Enqueue BeginProxyRequest. Context {0}, final AnchoredRoutingTarget {1}.", this.TraceContext, this.AnchoredRoutingTarget);
					ThreadPool.QueueUserWorkItem(new WaitCallback(this.BeginProxyRequest));
					this.State = ProxyRequestHandler.ProxyState.PrepareServerRequest;
				}
			}
		}

		protected bool HandleRoutingError(HttpWebResponse response)
		{
			bool returnValue = false;
			if (this.IsRoutingError(response))
			{
				this.InvalidateBackEndServerCacheSetDelay(response, false);
				returnValue = true;
			}
			try
			{
				Diagnostics.SendWatsonReportOnUnhandledException(delegate()
				{
					this.HandleRoutingUpdateModuleResponse(response, !returnValue);
				}, new Diagnostics.LastChanceExceptionHandler(RequestDetailsLogger.LastChanceExceptionHandler));
			}
			catch (ArgumentException ex)
			{
				RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericError(this.Logger, "HandleRUMResponseError", ex.Message);
			}
			return returnValue;
		}

		protected bool RecalculateTargetBackend()
		{
			if (this.ShouldRetryOnError)
			{
				this.retryOnErrorCounter++;
				PerfCounters.HttpProxyCountersInstance.TotalRetryOnError.Increment();
				this.LogForRetry(string.Format("WillRetryOnError({0}/{1})-LastTryData", this.retryOnErrorCounter, HttpProxySettings.MaxRetryOnError.Value), true, RequestDetailsLogger.PreservedHttpProxyMetadata);
				this.ResetForRetryOnError();
				lock (this.LockObject)
				{
					if (this.delayOnRetryOnError > 0)
					{
						Task.Delay(this.delayOnRetryOnError).ContinueWith(new Action<Task>(this.BeginCalculateTargetBackEnd));
					}
					else
					{
						ThreadPool.QueueUserWorkItem(new WaitCallback(this.BeginCalculateTargetBackEnd));
					}
					this.State = ProxyRequestHandler.ProxyState.CalculateBackEnd;
				}
				return true;
			}
			return false;
		}

		protected void BeginContinueOnAuthenticate(object extraData)
		{
			this.CallThreadEntranceMethod(delegate
			{
				this.LogElapsedTime("E_BeginContinueOnAuthenticate");
				try
				{
					this.AuthBehavior.ContinueOnAuthenticate(this.HttpApplication, new AsyncCallback(this.ContinueOnAuthenticateCallBack));
				}
				catch (Exception arg)
				{
					ExTraceGlobals.VerboseTracer.TraceError<Exception, int, ProxyRequestHandler.ProxyState>((long)this.GetHashCode(), "[ProxyRequestHandler::BeginContinueOnAuthenticate]: An error occurred while trying to authenticate: {0}; Context {1}; State {2}", arg, this.TraceContext, this.State);
					throw;
				}
				finally
				{
					this.LogElapsedTime("L_BeginContinueOnAuthenticate");
				}
			});
		}

		private static void MailboxServerLocatorCompletedCallback(IAsyncResult result)
		{
			MailboxServerLocatorAsyncState mailboxServerLocatorAsyncState = (MailboxServerLocatorAsyncState)result.AsyncState;
			ExTraceGlobals.VerboseTracer.TraceDebug<int>((long)mailboxServerLocatorAsyncState.ProxyRequestHandler.GetHashCode(), "[ProxyRequestHandler::MailboxServerLocatorCompletedCallback]: Context {0}", mailboxServerLocatorAsyncState.ProxyRequestHandler.TraceContext);
			if (result.CompletedSynchronously)
			{
				ThreadPool.QueueUserWorkItem(new WaitCallback(mailboxServerLocatorAsyncState.ProxyRequestHandler.OnCalculateTargetBackEndCompleted), new TargetCalculationCallbackBeacon(mailboxServerLocatorAsyncState, result));
				return;
			}
			mailboxServerLocatorAsyncState.ProxyRequestHandler.OnCalculateTargetBackEndCompleted(new TargetCalculationCallbackBeacon(mailboxServerLocatorAsyncState, result));
		}

		private void HandleRoutingUpdateModuleResponse(HttpWebResponse response, bool invalidateAnchorMailboxCache)
		{
			DatabaseGuidRoutingDestination databaseGuidRoutingDestination = null;
			string value;
			bool flag = this.ShouldUpdateAnchorMailboxCache(response, out value, out databaseGuidRoutingDestination);
			if (flag)
			{
				if (invalidateAnchorMailboxCache)
				{
					this.UpdateOrInvalidateAnchorMailboxCache(databaseGuidRoutingDestination.DatabaseGuid, databaseGuidRoutingDestination.ResourceForest);
				}
				this.AnchoredRoutingTarget.AnchorMailbox.UpdateCache(new AnchorMailboxCacheEntry
				{
					Database = new ADObjectId(databaseGuidRoutingDestination.DatabaseGuid, databaseGuidRoutingDestination.ResourceForest),
					DomainName = databaseGuidRoutingDestination.DomainName
				});
				PerfCounters.HttpProxyCacheCountersInstance.RouteRefresherSuccessfulAnchorMailboxCacheUpdates.Increment();
				RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericInfo(this.Logger, "AnchorMailboxRoutingEntryUpdate", value);
			}
		}

		private bool ShouldUpdateAnchorMailboxCache(HttpWebResponse response, out string routingUpdateHeaderValue, out DatabaseGuidRoutingDestination updatedRoutingDestination)
		{
			updatedRoutingDestination = null;
			if (this.TryGetSpecificHeaderFromResponse(response, "[ProxyRequestHandler::ShouldUpdateAnchorMailboxCache]", "X-RoutingEntryUpdate", null, out routingUpdateHeaderValue) && !string.IsNullOrEmpty(routingUpdateHeaderValue))
			{
				updatedRoutingDestination = (RoutingEntryHeaderSerializer.Deserialize(routingUpdateHeaderValue).Destination as DatabaseGuidRoutingDestination);
				if (updatedRoutingDestination != null)
				{
					return true;
				}
			}
			return false;
		}

		private bool InvalidateBackEndServerCache(HttpWebResponse response)
		{
			ExTraceGlobals.VerboseTracer.TraceDebug<int>((long)this.GetHashCode(), "[ProxyRequestHandler::InvalidatingBackEndServerCache]: Context {0}; invalidating cache.", this.TraceContext);
			this.newBackendForRetry = null;
			ADObjectId adobjectId = null;
			DatabaseBasedAnchorMailbox databaseBasedAnchorMailbox = this.AnchoredRoutingTarget.AnchorMailbox as DatabaseBasedAnchorMailbox;
			if (databaseBasedAnchorMailbox != null)
			{
				adobjectId = databaseBasedAnchorMailbox.GetDatabase();
			}
			Guid guid = (adobjectId != null) ? adobjectId.ObjectGuid : Guid.Empty;
			BackEndServer backEndServer = null;
			string text;
			if (this.IsRetryOnErrorEnabled && this.TryGetSpecificHeaderFromResponse(response, "InvalidatingBackEndServerCache", WellKnownHeader.XDBMountedOnServer, null, out text))
			{
				Fqdn fqdn;
				int version;
				if (Utilities.TryParseDBMountedOnServerHeader(text, out guid, out fqdn, out version))
				{
					bool flag = adobjectId != null && !adobjectId.ObjectGuid.Equals(guid);
					if (string.Equals(this.AnchoredRoutingTarget.BackEndServer.Fqdn, fqdn.ToString(), StringComparison.InvariantCultureIgnoreCase))
					{
						if (flag)
						{
							RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericInfo(this.Logger, "MdbGuidMismatch-Error", string.Format("{0}~{1}", adobjectId.ObjectGuid, guid));
						}
						return false;
					}
					if (flag)
					{
						RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericInfo(this.Logger, "MdbGuidMismatch", string.Format("{0}~{1}", adobjectId.ObjectGuid, guid));
					}
					backEndServer = new BackEndServer(fqdn, version);
				}
				else
				{
					this.Logger.AppendGenericError("InvalidDBMountedServerHeader", text);
				}
			}
			string text2 = null;
			if (backEndServer != null)
			{
				RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericInfo(this.Logger, "UpdateBackEndServerCache", guid);
				if (Utilities.TryExtractForestFqdnFromServerFqdn(backEndServer.Fqdn, out text2))
				{
					ExTraceGlobals.VerboseTracer.TraceDebug<Guid, BackEndServer, string>((long)this.GetHashCode(), "[ProxyRequestHandler::InvalidatingBackEndServerCache]: Adding entry to MailboxServerCache for MDB {0}, mapping to BE server {1} in forest {2}.", guid, backEndServer, text2);
					this.newBackendForRetry = new KeyValuePair<Guid, BackEndServer>?(new KeyValuePair<Guid, BackEndServer>(guid, backEndServer));
				}
				else
				{
					this.Logger.AppendGenericError("UnableToDetermineForestFqdnFromBackendServerFqdn", backEndServer.Fqdn);
				}
			}
			if (adobjectId != null && this.newBackendForRetry == null)
			{
				RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericInfo(this.Logger, "InvalidatingBackEndServerCache", adobjectId.ObjectGuid);
				MailboxServerCache.Instance.Remove(adobjectId.ObjectGuid, this);
			}
			this.UpdateOrInvalidateAnchorMailboxCache(guid, text2);
			return true;
		}

		private void InvalidateBackEndServerCacheSetDelay(HttpWebResponse response, bool alwaysDelay)
		{
			if (this.IsRetryingOnError)
			{
				PerfCounters.HttpProxyCountersInstance.FailedRetryOnError.Increment();
			}
			bool flag = this.InvalidateBackEndServerCache(response);
			this.delayOnRetryOnError = 0;
			if (alwaysDelay || !flag || this.IsRetryingOnError)
			{
				this.delayOnRetryOnError = HttpProxySettings.DelayOnRetryOnError.Value;
			}
			if (!this.ShouldRetryOnError)
			{
				RequestDetailsLoggerBase<RequestDetailsLogger>.SafeSetLogger(this.Logger, ServiceCommonMetadata.ErrorCode, "RoutingError");
			}
		}

		private void BeginCalculateTargetBackEnd(object extraData)
		{
			this.CallThreadEntranceMethod(delegate
			{
				this.LogElapsedTime("E_BeginCalcTargetBE");
				try
				{
					lock (this.LockObject)
					{
						this.LatencyTracker.StartTracking((this.State == ProxyRequestHandler.ProxyState.CalculateBackEnd) ? LatencyTrackerKey.CalculateTargetBackEndLatency : LatencyTrackerKey.CalculateTargetBackEndSecondRoundLatency, this.IsRetryingOnError);
						AnchorMailbox anchorMailbox = null;
						try
						{
							this.DoProtocolSpecificBeginProcess();
							this.InternalBeginCalculateTargetBackEnd(out anchorMailbox);
							if (anchorMailbox != null)
							{
								this.Logger.ActivityScope.SetProperty(ActivityStandardMetadata.TenantId, anchorMailbox.GetOrganizationNameForLogging());
							}
						}
						catch (Exception exception)
						{
							if (!this.HandleBackEndCalculationException(exception, anchorMailbox, "BeginCalculateTargetBackEnd"))
							{
								ProxyRequestHandler.DisposeIfNotNullAndCatchExceptions(this.mailboxServerLocator);
								this.mailboxServerLocator = null;
								throw;
							}
						}
					}
				}
				finally
				{
					this.LogElapsedTime("L_BeginCalcTargetBE");
				}
			});
		}

		private void InternalBeginCalculateTargetBackEnd(out AnchorMailbox anchorMailbox)
		{
			this.LogElapsedTime("E_IntBeginCalcTargetBE");
			try
			{
				anchorMailbox = null;
				this.AnchoredRoutingTarget = this.TryDirectTargetCalculation();
				ExTraceGlobals.VerboseTracer.TraceDebug<AnchoredRoutingTarget>((long)this.GetHashCode(), "[ProxyRequestHandler::InternalBeginCalculateTargetBackEnd]: TryDirectTargetCalculation returns {0}", this.AnchoredRoutingTarget);
				if (this.AnchoredRoutingTarget != null)
				{
					ThreadPool.QueueUserWorkItem(new WaitCallback(this.OnCalculateTargetBackEndCompleted), new TargetCalculationCallbackBeacon(this.AnchoredRoutingTarget));
					anchorMailbox = this.AnchoredRoutingTarget.AnchorMailbox;
				}
				else
				{
					anchorMailbox = this.ResolveAnchorMailbox();
					ExTraceGlobals.VerboseTracer.TraceDebug<AnchorMailbox>((long)this.GetHashCode(), "[ProxyRequestHandler::InternalBeginCalculateTargetBackEnd]: ResolveAnchorMailbox returns {0}", anchorMailbox);
					string text = anchorMailbox.ToString();
					if (anchorMailbox.OriginalAnchorMailbox != null)
					{
						text = string.Format("{0}-{1}", anchorMailbox.OriginalAnchorMailbox.ToString(), text);
					}
					RequestDetailsLoggerBase<RequestDetailsLogger>.SafeSetLogger(this.Logger, HttpProxyMetadata.AnchorMailbox, text);
					PerfCounters.HttpProxyCacheCountersInstance.OverallCacheEffectivenessRateBase.Increment();
					NegativeAnchorMailboxCacheEntry negativeCacheEntry = anchorMailbox.GetNegativeCacheEntry();
					if (negativeCacheEntry != null)
					{
						throw new HttpProxyException(negativeCacheEntry.ErrorCode, negativeCacheEntry.SubErrorCode, "NegativeCache:" + negativeCacheEntry.SourceObject);
					}
					this.AnchoredRoutingTarget = this.TryFastTargetCalculationByAnchorMailbox(anchorMailbox);
					ExTraceGlobals.VerboseTracer.TraceDebug<AnchoredRoutingTarget>((long)this.GetHashCode(), "[ProxyRequestHandler::InternalBeginCalculateTargetBackEnd]: TryFastTargetCalculationByAnchorMailbox returns {0}", this.AnchoredRoutingTarget);
					if (this.AnchoredRoutingTarget != null)
					{
						ThreadPool.QueueUserWorkItem(new WaitCallback(this.OnCalculateTargetBackEndCompleted), new TargetCalculationCallbackBeacon(this.AnchoredRoutingTarget));
						if (anchorMailbox.CacheEntryCacheHit)
						{
							PerfCounters.HttpProxyCacheCountersInstance.OverallCacheEffectivenessRate.Increment();
						}
					}
					else
					{
						DatabaseBasedAnchorMailbox databaseBasedAnchorMailbox = (DatabaseBasedAnchorMailbox)anchorMailbox;
						ADObjectId database = databaseBasedAnchorMailbox.GetDatabase();
						if (database == null)
						{
							if (this.UseRoutingHintForAnchorMailbox && this.IsAnchorMailboxFromRoutingHint)
							{
								this.UseRoutingHintForAnchorMailbox = false;
								this.IsAnchorMailboxFromRoutingHint = false;
								ExTraceGlobals.VerboseTracer.TraceDebug<AnchorMailbox>((long)this.GetHashCode(), "[ProxyRequestHandler::InternalBeginCalculateTargetBackEnd]: Current anchor mailbox {0} cannot be associated with a database. Will calculate again without using routing hint.", anchorMailbox);
								this.LogForRetry("RetryInternalBeginCalculateTargetBackEnd", false, new HttpProxyMetadata[]
								{
									HttpProxyMetadata.AnchorMailbox,
									HttpProxyMetadata.RoutingHint
								});
								this.InternalBeginCalculateTargetBackEnd(out anchorMailbox);
							}
							else if (this.AuthBehavior.AuthState != AuthState.FrontEndFullAuth && this.AuthBehavior.ShouldDoFullAuthOnUnresolvedAnchorMailbox && !this.AuthBehavior.IsFullyAuthenticated() && !AnchorMailbox.AllowMissingTenant.Value)
							{
								RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericInfo(this.Logger, "ContinueAuth", "Database-Null");
								ThreadPool.QueueUserWorkItem(new WaitCallback(this.BeginContinueOnAuthenticate));
							}
							else
							{
								BackEndServer randomE15Server = MailboxServerCache.Instance.GetRandomE15Server(this);
								this.AnchoredRoutingTarget = new AnchoredRoutingTarget(anchorMailbox, randomE15Server);
								ExTraceGlobals.VerboseTracer.TraceDebug<AnchorMailbox, AnchoredRoutingTarget>((long)this.GetHashCode(), "[ProxyRequestHandler::InternalBeginCalculateTargetBackEnd]: Current anchor mailbox {0} cannot be associated with a database. Will use random routing target {1}", anchorMailbox, this.AnchoredRoutingTarget);
								ThreadPool.QueueUserWorkItem(new WaitCallback(this.OnCalculateTargetBackEndCompleted), new TargetCalculationCallbackBeacon(this.AnchoredRoutingTarget));
								if (anchorMailbox.CacheEntryCacheHit)
								{
									PerfCounters.HttpProxyCacheCountersInstance.OverallCacheEffectivenessRate.Increment();
								}
							}
						}
						else
						{
							if (this.IsRetryOnErrorEnabled)
							{
								bool flag = false;
								ExTraceGlobals.FaultInjectionTracer.TraceTest<bool>(2379006271U, ref flag);
								if (flag && this.retryOnErrorCounter == 0)
								{
									MailboxServerCache.Instance.Remove(database.ObjectGuid, this);
								}
							}
							BackEndServer backEndServer = null;
							if (this.IsRetryingOnError && this.newBackendForRetry != null)
							{
								backEndServer = this.newBackendForRetry.Value.Value;
								ExTraceGlobals.VerboseTracer.TraceDebug<BackEndServer>((long)this.GetHashCode(), "[ProxyRequestHandler::InternalBeginCalculateTargetBackEnd]: Using backend returned by the previous proxy attempt '{0}'", backEndServer);
							}
							else if (MailboxServerCache.Instance.TryGet(database.ObjectGuid, this, out backEndServer))
							{
								if (anchorMailbox.CacheEntryCacheHit)
								{
									PerfCounters.HttpProxyCacheCountersInstance.OverallCacheEffectivenessRate.Increment();
								}
								ExTraceGlobals.VerboseTracer.TraceDebug<BackEndServer>((long)this.GetHashCode(), "[ProxyRequestHandler::InternalBeginCalculateTargetBackEnd]: Found back end server {0} in cache.", backEndServer);
							}
							if (backEndServer != null)
							{
								ThreadPool.QueueUserWorkItem(new WaitCallback(this.OnCalculateTargetBackEndCompleted), new TargetCalculationCallbackBeacon(anchorMailbox, backEndServer));
							}
							else
							{
								if (this.IsRetryOnErrorEnabled)
								{
									bool flag2 = false;
									ExTraceGlobals.FaultInjectionTracer.TraceTest<bool>(2379006271U, ref flag2);
									if (flag2)
									{
										ThreadPool.QueueUserWorkItem(new WaitCallback(this.OnCalculateTargetBackEndCompleted), new TargetCalculationCallbackBeacon(anchorMailbox, MailboxServerCache.Instance.GetRandomE15Server(this)));
										return;
									}
								}
								string text2 = null;
								if (anchorMailbox is UserBasedAnchorMailbox)
								{
									text2 = ((UserBasedAnchorMailbox)anchorMailbox).GetDomainName();
								}
								PerfCounters.IncrementMovingPercentagePerformanceCounterBase(PerfCounters.HttpProxyCountersInstance.MovingPercentageMailboxServerLocatorFailedCalls);
								if (this.IsRetryOnErrorEnabled)
								{
									ProxyRequestHandler.DisposeIfNotNullAndCatchExceptions(this.mailboxServerLocator);
									this.mailboxServerLocator = null;
								}
								this.mailboxServerLocator = this.CreateMailboxServerLocator(database.ObjectGuid, text2, database.PartitionFQDN);
								if (HttpProxySettings.TestBackEndSupportEnabled.Value)
								{
									string testBackEndUrl = this.ClientRequest.GetTestBackEndUrl();
									if (!string.IsNullOrEmpty(testBackEndUrl))
									{
										this.mailboxServerLocator.SkipServerLocatorQuery = true;
									}
								}
								MailboxServerLocatorAsyncState mailboxServerLocatorAsyncState = new MailboxServerLocatorAsyncState
								{
									AnchorMailbox = anchorMailbox,
									Locator = this.mailboxServerLocator,
									ProxyRequestHandler = this
								};
								RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericInfo(this.Logger, "ServerLocatorCall", string.Concat(new object[]
								{
									MailboxServerLocator.UseResourceForest.Value ? "RF" : "DM",
									":",
									database.ObjectGuid,
									"~",
									text2,
									"~",
									database.PartitionFQDN
								}));
								ExTraceGlobals.VerboseTracer.TraceDebug<ADObjectId>((long)this.GetHashCode(), "[ProxyRequestHandler::InternalBeginCalculateTargetBackEnd]: Begin resolving backend server for database {0}", database);
								PerfCounters.HttpProxyCountersInstance.MailboxServerLocatorCalls.Increment();
								this.mailboxServerLocator.BeginGetServer(new AsyncCallback(ProxyRequestHandler.MailboxServerLocatorCompletedCallback), mailboxServerLocatorAsyncState);
							}
						}
					}
				}
			}
			finally
			{
				this.LogElapsedTime("L_IntBeginCalcTargetBE");
			}
		}

		private void OnCalculateTargetBackEndCompleted(object extraData)
		{
			this.CallThreadEntranceMethod(delegate
			{
				this.LogElapsedTime("E_OnCalcTargetBEComp");
				try
				{
					lock (this.LockObject)
					{
						ProxyRequestHandler.ProxyState state = this.State;
						TargetCalculationCallbackBeacon targetCalculationCallbackBeacon = (TargetCalculationCallbackBeacon)extraData;
						try
						{
							this.InternalOnCalculateTargetBackEndCompleted(targetCalculationCallbackBeacon);
						}
						catch (Exception exception)
						{
							if (!this.HandleBackEndCalculationException(exception, targetCalculationCallbackBeacon.AnchorMailbox, "OnCalculateTargetBackEndCompleted"))
							{
								throw;
							}
						}
						finally
						{
							LatencyTrackerKey trackingKey;
							if (state == ProxyRequestHandler.ProxyState.CalculateBackEnd)
							{
								trackingKey = LatencyTrackerKey.CalculateTargetBackEndLatency;
							}
							else
							{
								trackingKey = LatencyTrackerKey.CalculateTargetBackEndSecondRoundLatency;
							}
							RequestDetailsLoggerBase<RequestDetailsLogger>.SafeSetLogger(this.Logger, HttpProxyMetadata.CalculateTargetBackendLatency, this.LatencyTracker.GetCurrentLatency(trackingKey));
						}
					}
				}
				finally
				{
					this.LogElapsedTime("L_OnCalcTargetBEComp");
				}
			});
		}

		private void InternalOnCalculateTargetBackEndCompleted(TargetCalculationCallbackBeacon beacon)
		{
			this.LogElapsedTime("E_IntOnCalcTargetBEComp");
			if (beacon.State == TargetCalculationCallbackState.TargetResolved)
			{
				ExTraceGlobals.VerboseTracer.TraceDebug<AnchoredRoutingTarget>((long)this.GetHashCode(), "[ProxyRequestHandler::InternalOnCalculateTargetBackEndCompleted]: Routing target already resolved as {0}", beacon.AnchoredRoutingTarget);
			}
			else
			{
				BackEndServer server = null;
				if (beacon.State == TargetCalculationCallbackState.LocatorCallback)
				{
					ExTraceGlobals.VerboseTracer.TraceDebug<AnchorMailbox>((long)this.GetHashCode(), "[ProxyRequestHandler::InternalOnCalculateTargetBackEndCompleted]: Called by MailboxServerLocator async callback with anchor mailbox {0}.", beacon.AnchorMailbox);
					server = this.ProcessMailboxServerLocatorCallBack(beacon.MailboxServerLocatorAsyncResult, beacon.MailboxServerLocatorAsyncState);
				}
				else
				{
					ExTraceGlobals.VerboseTracer.TraceDebug<AnchorMailbox, BackEndServer>((long)this.GetHashCode(), "[ProxyRequestHandler::InternalOnCalculateTargetBackEndCompleted]: Called by self callback with anchor mailbox {0} and mailbox server {1}.", beacon.AnchorMailbox, beacon.MailboxServer);
					server = beacon.MailboxServer;
				}
				if (server.Version < Server.E15MinVersion)
				{
					long num = 0L;
					BackEndServer latency = LatencyTracker.GetLatency<BackEndServer>(() => this.GetDownLevelClientAccessServer(beacon.AnchorMailbox, server), out num);
					RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericInfo(this.Logger, "ClientAccessServer", latency.Fqdn);
					RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericInfo(this.Logger, "ResolveCasLatency", num);
					server = latency;
					ExTraceGlobals.VerboseTracer.TraceDebug<string, int>((long)this.GetHashCode(), "[ProxyRequestHandler::InternalOnCalculateTargetBackEndCompleted]: Resolved down level Client Access server {0} with version {1}", server.Fqdn, server.Version);
				}
				this.AnchoredRoutingTarget = new AnchoredRoutingTarget(beacon.AnchorMailbox, server);
			}
			ExTraceGlobals.VerboseTracer.TraceDebug<AnchoredRoutingTarget>((long)this.GetHashCode(), "[ProxyRequestHandler::InternalOnCalculateTargetBackEndCompleted]: Will use target {0} for proxy.", this.AnchoredRoutingTarget);
			AnchoredRoutingTarget anchoredRoutingTarget = this.DoProtocolSpecificRoutingTargetOverride(this.AnchoredRoutingTarget);
			if (anchoredRoutingTarget != null)
			{
				ExTraceGlobals.VerboseTracer.TraceDebug<AnchoredRoutingTarget, AnchoredRoutingTarget>((long)this.GetHashCode(), "[ProxyRequestHandler::InternalOnCalculateTargetBackEndCompleted]: Routing target overridden from {0} to {1}.", this.AnchoredRoutingTarget, anchoredRoutingTarget);
				this.AnchoredRoutingTarget = anchoredRoutingTarget;
			}
			this.AuthBehavior.SetState(this.AnchoredRoutingTarget.BackEndServer.Version);
			this.AlterAuthBehaviorStateForBEAuthTest();
			string value = string.Format("BEVersion-{0}", this.AnchoredRoutingTarget.BackEndServer.Version);
			if (this.AuthBehavior.AuthState == AuthState.FrontEndContinueAuth && !this.AuthBehavior.IsFullyAuthenticated())
			{
				RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericInfo(this.Logger, "ContinueAuth", value);
				ThreadPool.QueueUserWorkItem(new WaitCallback(this.BeginContinueOnAuthenticate));
			}
			else
			{
				if (this.AuthBehavior.IsFullyAuthenticated())
				{
					RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericInfo(this.Logger, "FEAuth", value);
				}
				else
				{
					RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericInfo(this.Logger, "BEAuth", value);
				}
				this.BeginValidateBackendServerCacheOrProxyOrRecalculate();
			}
			this.LogElapsedTime("L_IntOnCalcTargetBEComp");
		}

		private void BeginValidateBackendServerCacheOrProxyOrRecalculate()
		{
			if (this.IsBackendServerCacheValidationEnabled)
			{
				this.BeginValidateBackendServerCache();
				return;
			}
			this.BeginProxyRequestOrRecalculate();
		}

		private void ContinueOnAuthenticateCallBack(object extraData)
		{
			if (!this.AuthBehavior.IsFullyAuthenticated())
			{
				this.AuthBehavior.SetFailureStatus();
				this.Complete();
				return;
			}
			if (this.AnchoredRoutingTarget == null && this.AuthBehavior.ShouldDoFullAuthOnUnresolvedAnchorMailbox)
			{
				RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericInfo(this.Logger, "AnchoredRoutingTarget-Null", "BeginCalculateTargetBackEnd");
				ThreadPool.QueueUserWorkItem(new WaitCallback(this.BeginCalculateTargetBackEnd));
				return;
			}
			this.BeginValidateBackendServerCacheOrProxyOrRecalculate();
		}

		private BackEndServer ProcessMailboxServerLocatorCallBack(IAsyncResult asyncResult, MailboxServerLocatorAsyncState asyncState)
		{
			this.LogElapsedTime("E_ProcSvrLocCB");
			AnchorMailbox anchorMailbox = asyncState.AnchorMailbox;
			BackEndServer backEndServer = null;
			try
			{
				backEndServer = MailboxServerCache.Instance.ServerLocatorEndGetServer(asyncState.Locator, asyncResult, this);
			}
			finally
			{
				RequestDetailsLoggerBase<RequestDetailsLogger>.SafeSetLogger(this.Logger, HttpProxyMetadata.ServerLocatorHost, string.Join(";", asyncState.Locator.LocatorServiceHosts));
				RequestDetailsLoggerBase<RequestDetailsLogger>.SafeSetLogger(this.Logger, HttpProxyMetadata.ServerLocatorLatency, asyncState.Locator.Latency);
				this.LatencyTracker.HandleGlsLatency(asyncState.Locator.GlsLatencies);
				this.LatencyTracker.HandleResourceLatency(asyncState.Locator.DirectoryLatencies);
			}
			PerfCounters.HttpProxyCountersInstance.MailboxServerLocatorLatency.RawValue = asyncState.Locator.Latency;
			PerfCounters.HttpProxyCountersInstance.MailboxServerLocatorAverageLatency.IncrementBy(asyncState.Locator.Latency);
			PerfCounters.HttpProxyCountersInstance.MailboxServerLocatorAverageLatencyBase.Increment();
			PerfCounters.UpdateMovingAveragePerformanceCounter(PerfCounters.HttpProxyCountersInstance.MovingAverageMailboxServerLocatorLatency, asyncState.Locator.Latency);
			PerfCounters.IncrementMovingPercentagePerformanceCounterBase(PerfCounters.HttpProxyCountersInstance.MovingPercentageMailboxServerLocatorRetriedCalls);
			if (asyncState.Locator.LocatorServiceHosts.Length > 1)
			{
				PerfCounters.HttpProxyCountersInstance.MailboxServerLocatorRetriedCalls.Increment();
				PerfCounters.UpdateMovingPercentagePerformanceCounter(PerfCounters.HttpProxyCountersInstance.MovingPercentageMailboxServerLocatorRetriedCalls);
			}
			ExTraceGlobals.VerboseTracer.TraceDebug<string, int>((long)this.GetHashCode(), "[ProxyRequestHandler::ProcessMailboxServerLocatorCallBack]: MailboxServerLocator returns server {0} with version {1}.", backEndServer.Fqdn, backEndServer.Version);
			this.LogElapsedTime("L_ProcSvrLocCB");
			return backEndServer;
		}

		protected virtual bool UseSmartBufferSizing
		{
			get
			{
				return false;
			}
		}

		private bool IsSmartBufferSizingEnabled
		{
			get
			{
				return this.UseSmartBufferSizing && HttpProxySettings.UseSmartBufferSizing.Value;
			}
		}

		protected virtual StreamProxy BuildRequestStreamProxy(StreamProxy.StreamProxyType streamProxyType, Stream source, Stream target, byte[] buffer)
		{
			return new StreamProxy(streamProxyType, source, target, buffer, this);
		}

		protected virtual StreamProxy BuildRequestStreamProxySmartSizing(StreamProxy.StreamProxyType streamProxyType, Stream source, Stream target)
		{
			return new StreamProxy(streamProxyType, source, target, HttpProxySettings.RequestBufferSize.Value, HttpProxySettings.MinimumRequestBufferSize.Value, this);
		}

		protected virtual StreamProxy BuildResponseStreamProxy(StreamProxy.StreamProxyType streamProxyType, Stream source, Stream target, byte[] buffer)
		{
			return new StreamProxy(streamProxyType, source, target, buffer, this);
		}

		protected virtual StreamProxy BuildResponseStreamProxySmartSizing(StreamProxy.StreamProxyType streamProxyType, Stream source, Stream target)
		{
			return new StreamProxy(streamProxyType, source, target, HttpProxySettings.ResponseBufferSize.Value, HttpProxySettings.MinimumResponseBufferSize.Value, this);
		}

		protected virtual BufferPool GetRequestBufferPool()
		{
			return BufferPoolCollection.AutoCleanupCollection.Acquire(HttpProxySettings.RequestBufferSize.Value);
		}

		protected virtual BufferPool GetResponseBufferPool()
		{
			return BufferPoolCollection.AutoCleanupCollection.Acquire(HttpProxySettings.ResponseBufferSize.Value);
		}

		private void BeginRequestStreaming()
		{
			this.LogElapsedTime("E_BegReqStrm");
			try
			{
				ExTraceGlobals.VerboseTracer.TraceDebug<int, ProxyRequestHandler.ProxyState>((long)this.GetHashCode(), "[ProxyRequestHandler::BeginRequestStreaming]: Context {0}; State {1}", this.TraceContext, this.State);
				if (this.requestStreamProxy == null)
				{
					this.InitializeRequestBufferPool();
					if (this.IsSmartBufferSizingEnabled)
					{
						this.requestStreamProxy = this.BuildRequestStreamProxySmartSizing(StreamProxy.StreamProxyType.Request, this.ClientRequestStream, this.ServerRequestStream);
					}
					else
					{
						this.requestStreamProxy = this.BuildRequestStreamProxy(StreamProxy.StreamProxyType.Request, this.ClientRequestStream, this.ServerRequestStream, this.requestStreamBufferPool.AcquireBuffer());
					}
				}
				else
				{
					if (this.requestStreamProxy.NumberOfReadsCompleted > 1L)
					{
						this.Logger.AppendGenericError("CannotReplay-NumReads/TotalBytes", this.requestStreamProxy.NumberOfReadsCompleted.ToString() + '/' + this.requestStreamProxy.TotalBytesProxied.ToString());
						throw new HttpProxyException(HttpStatusCode.InternalServerError, HttpProxySubErrorCode.CannotReplayRequest, "Cannot replay request where the number of initial reads wasn't 1.");
					}
					this.requestStreamProxy.SetTargetStreamForBufferedSend(this.ServerRequestStream);
				}
				try
				{
					this.requestStreamProxy.BeginProcess(new AsyncCallback(this.RequestStreamProxyCompleted), this);
				}
				catch (StreamProxyException exception)
				{
					this.HandleStreamProxyError(exception, this.requestStreamProxy);
				}
			}
			finally
			{
				this.LogElapsedTime("L_BegReqStrm");
			}
		}

		private void BeginResponseStreaming()
		{
			this.LogElapsedTime("E_BegRespStrm");
			try
			{
				ExTraceGlobals.VerboseTracer.TraceDebug<int, ProxyRequestHandler.ProxyState>((long)this.GetHashCode(), "[ProxyRequestHandler::BeginResponseStreaming]: Context {0}; State {1}", this.TraceContext, this.State);
				this.InitializeResponseBufferPool();
				if (this.IsSmartBufferSizingEnabled)
				{
					this.responseStreamProxy = this.BuildResponseStreamProxySmartSizing(StreamProxy.StreamProxyType.Response, this.ServerResponse.GetResponseStream(), this.ClientResponse.OutputStream);
				}
				else
				{
					this.responseStreamProxy = this.BuildResponseStreamProxy(StreamProxy.StreamProxyType.Response, this.ServerResponse.GetResponseStream(), this.ClientResponse.OutputStream, this.responseStreamBufferPool.AcquireBuffer());
				}
				this.responseStreamProxy.AuxTargetStream = this.captureResponseStream;
				this.State = ProxyRequestHandler.ProxyState.ProxyResponseData;
				try
				{
					this.responseStreamProxy.BeginProcess(new AsyncCallback(this.ResponseStreamProxyCompleted), this);
				}
				catch (StreamProxyException exception)
				{
					this.HandleStreamProxyError(exception, this.responseStreamProxy);
				}
			}
			finally
			{
				this.LogElapsedTime("L_BegRespStrm");
			}
		}

		private void InitializeRequestBufferPool()
		{
			this.LogElapsedTime("E_InitReqBufPool");
			try
			{
				if (!this.requestBufferInitialized)
				{
					if (!this.IsSmartBufferSizingEnabled)
					{
						if (this.ClientRequest.IsRequestChunked() || (long)this.ClientRequest.ContentLength >= HttpProxySettings.RequestBufferBoundary.Member)
						{
							this.requestStreamBufferPool = new ProxyRequestHandler.BufferPoolWithBuffer(this.GetRequestBufferPool());
						}
						else
						{
							BufferPoolCollection.BufferSize bufferSize;
							if (!BufferPoolCollection.AutoCleanupCollection.TryMatchBufferSize(this.ClientRequest.ContentLength, out bufferSize))
							{
								throw new InvalidOperationException("Failed to get buffer size for request stream buffer.");
							}
							this.requestStreamBufferPool = new ProxyRequestHandler.BufferPoolWithBuffer(bufferSize);
						}
					}
					this.requestBufferInitialized = true;
				}
			}
			finally
			{
				this.LogElapsedTime("L_InitReqBufPool");
			}
		}

		private void InitializeResponseBufferPool()
		{
			this.LogElapsedTime("E_InitRespBufPool");
			try
			{
				if (!this.responseBufferInitialized)
				{
					bool flag = false;
					if (this.ShouldForceUnbufferedClientResponseOutput || this.ServerResponse.IsChunkedResponse())
					{
						this.ClientResponse.BufferOutput = false;
						flag = true;
					}
					if (!this.IsSmartBufferSizingEnabled)
					{
						if (flag)
						{
							this.responseStreamBufferPool = new ProxyRequestHandler.BufferPoolWithBuffer(this.GetResponseBufferPool());
						}
						else if (this.ServerResponse.ContentLength >= HttpProxySettings.ResponseBufferBoundary.Member)
						{
							this.responseStreamBufferPool = new ProxyRequestHandler.BufferPoolWithBuffer(HttpProxySettings.ResponseBufferSize.Value);
						}
						else
						{
							BufferPoolCollection.BufferSize bufferSize;
							if (!BufferPoolCollection.AutoCleanupCollection.TryMatchBufferSize((int)this.ServerResponse.ContentLength, out bufferSize))
							{
								throw new InvalidOperationException("Could not get buffer size for response stream buffer.");
							}
							this.responseStreamBufferPool = new ProxyRequestHandler.BufferPoolWithBuffer(bufferSize);
						}
					}
					this.responseBufferInitialized = true;
				}
			}
			finally
			{
				this.LogElapsedTime("L_InitRespBufPool");
			}
		}

		[Conditional("DEBUG")]
		private void InitializeCaptureResponseStream()
		{
			if (!string.IsNullOrEmpty(HttpProxySettings.CaptureResponsesLocation.Value) && string.IsNullOrEmpty(this.ClientRequest.GetTestBackEndUrl()))
			{
				string text = this.ClientRequest.Headers[Constants.CaptureResponseIdHeaderKey];
				if (!string.IsNullOrEmpty(text))
				{
					string path = Path.Combine(HttpProxySettings.CaptureResponsesLocation.Value, text + ".header");
					lock (ProxyRequestHandler.MultiRequestLockObject)
					{
						if (!File.Exists(path))
						{
							using (StreamWriter streamWriter = new StreamWriter(path))
							{
								for (int i = 0; i < this.ServerResponse.Headers.Count; i++)
								{
									streamWriter.Write(this.ServerResponse.Headers.Keys[i]);
									streamWriter.Write(": ");
									streamWriter.Write(this.ServerResponse.Headers[i]);
									streamWriter.Write(Environment.NewLine);
								}
								streamWriter.Flush();
							}
							string path2 = Path.Combine(HttpProxySettings.CaptureResponsesLocation.Value, text + ".txt");
							this.captureResponseStream = File.OpenWrite(path2);
						}
					}
				}
			}
		}

		private void RequestStreamProxyCompleted(IAsyncResult result)
		{
			this.CallThreadEntranceMethod(delegate
			{
				this.LogElapsedTime("E_ReqStrmProxyComp");
				try
				{
					lock (this.lockObject)
					{
						try
						{
							this.requestStreamProxy.EndProcess(result);
						}
						catch (StreamProxyException exception)
						{
							if (!this.HandleStreamProxyError(exception, this.requestStreamProxy))
							{
								return;
							}
						}
						finally
						{
							long totalBytesProxied = this.requestStreamProxy.TotalBytesProxied;
							ExTraceGlobals.VerboseTracer.TraceDebug<int, long>((long)this.GetHashCode(), "[ProxyRequestHandler::RequestStreamProxyCompleted]: Context {0}; Bytes copied {1}", this.TraceContext, totalBytesProxied);
							RequestDetailsLoggerBase<RequestDetailsLogger>.SafeSetLogger(this.Logger, ServiceCommonMetadata.RequestSize, totalBytesProxied);
						}
						try
						{
							Stream serverRequestStream = this.ServerRequestStream;
							this.ServerRequestStream = null;
							serverRequestStream.Flush();
							serverRequestStream.Dispose();
							if (!this.ShouldRetryOnError)
							{
								ProxyRequestHandler.DisposeIfNotNullAndCatchExceptions(this.ClientRequestStream);
								this.ClientRequestStream = null;
							}
							this.BeginGetServerResponse();
						}
						catch (WebException ex)
						{
							this.CompleteWithError(ex, "RequestStreamProxyCompleted");
						}
						catch (HttpException ex2)
						{
							this.CompleteWithError(ex2, "RequestStreamProxyCompleted");
						}
						catch (HttpProxyException ex3)
						{
							this.CompleteWithError(ex3, "RequestStreamProxyCompleted");
						}
						catch (IOException ex4)
						{
							this.CompleteWithError(ex4, "RequestStreamProxyCompleted");
						}
					}
				}
				finally
				{
					this.LogElapsedTime("L_ReqStrmProxyComp");
				}
			});
		}

		private void ResponseStreamProxyCompleted(IAsyncResult result)
		{
			this.CallThreadEntranceMethod(delegate
			{
				this.LogElapsedTime("E_RespStrmProxyComp");
				try
				{
					lock (this.lockObject)
					{
						try
						{
							this.responseStreamProxy.EndProcess(result);
						}
						catch (StreamProxyException exception)
						{
							if (!this.HandleStreamProxyError(exception, this.responseStreamProxy))
							{
								return;
							}
						}
						finally
						{
							long totalBytesProxied = this.responseStreamProxy.TotalBytesProxied;
							ExTraceGlobals.VerboseTracer.TraceDebug<int, long>((long)this.GetHashCode(), "[ProxyRequestHandler::ResponseStreamProxyCompleted]: Context {0}; Bytes copied {1}", this.TraceContext, totalBytesProxied);
							RequestDetailsLoggerBase<RequestDetailsLogger>.SafeSetLogger(this.Logger, ServiceCommonMetadata.ResponseSize, totalBytesProxied);
						}
						this.Complete();
					}
				}
				finally
				{
					this.LogElapsedTime("L_RespStrmProxyComp");
				}
			});
		}

		private bool HandleStreamProxyError(StreamProxyException exception, StreamProxy streamProxy)
		{
			this.LogElapsedTime("E_HandleSPErr");
			bool result;
			try
			{
				Exception innerException = exception.InnerException;
				string text = string.Format("StreamProxy-{0}-{1}", streamProxy.ProxyType, streamProxy.StreamState);
				ExTraceGlobals.VerboseTracer.TraceDebug<int, string, Exception>((long)this.GetHashCode(), "[ProxyRequestHandler::HandleStreamProxyError]: Context {0}; Handling StreamProxy error at label {1}. Exception: {2}", this.TraceContext, text, innerException);
				if (streamProxy.ProxyType == StreamProxy.StreamProxyType.Request && this.TryHandleProtocolSpecificRequestErrors(exception.InnerException))
				{
					result = true;
				}
				else
				{
					this.Logger.AppendGenericError("StreamProxy", text);
					this.CompleteWithError(innerException, text);
					result = false;
				}
			}
			finally
			{
				this.LogElapsedTime("L_HandleSPErr");
			}
			return result;
		}

		private void CleanUpRequestStreamsAndBuffer()
		{
			this.LogElapsedTime("E_CleanUpReqBuf");
			if (this.requestStreamBufferPool != null)
			{
				this.requestStreamBufferPool.Release();
				this.requestStreamBufferPool = null;
			}
			ProxyRequestHandler.DisposeIfNotNullAndCatchExceptions(this.requestStreamProxy);
			this.requestStreamProxy = null;
			ProxyRequestHandler.DisposeIfNotNullAndCatchExceptions(this.bufferedRegionStream);
			this.bufferedRegionStream = null;
			ProxyRequestHandler.DisposeIfNotNullAndCatchExceptions(this.ClientRequestStream);
			this.ClientRequestStream = null;
			this.LogElapsedTime("L_CleanUpReqBuf");
		}

		private void CleanUpResponseStreamsAndBuffer()
		{
			this.LogElapsedTime("E_CleanUpRespBuf");
			if (this.responseStreamBufferPool != null)
			{
				this.responseStreamBufferPool.Release();
				this.responseStreamBufferPool = null;
			}
			ProxyRequestHandler.DisposeIfNotNullAndCatchExceptions(this.responseStreamProxy);
			this.responseStreamProxy = null;
			ProxyRequestHandler.DisposeIfNotNullAndCatchExceptions(this.captureResponseStream);
			this.captureResponseStream = null;
			this.LogElapsedTime("L_CleanUpRespBuf");
		}

		internal void CompleteForLocalProbe()
		{
			int value = HttpProxySettings.DelayProbeResponseSeconds.Value;
			if (value > 0)
			{
				Thread.Sleep(TimeSpan.FromSeconds((double)value));
			}
			this.SetResponseStatusIfHeadersUnsent(this.ClientResponse, 200);
			this.Complete();
		}

		protected virtual bool ProtocolSupportsWebSocket()
		{
			return false;
		}

		private void ProcessWebSocketRequest(HttpContext context)
		{
			this.serverRequestHeaders = this.ServerRequest.Headers;
			context.AcceptWebSocketRequest(new Func<AspNetWebSocketContext, Task>(this.HandleWebSocket));
			this.Complete();
		}

		private async Task ConnectToBackend(Uri uri, CancellationToken cancellationToken)
		{
			ExTraceGlobals.VerboseTracer.TraceDebug<string>((long)this.GetHashCode(), "[ProxyRequestHandler::ConnectToBackend]: Connecting to {0} via WebSocket protocol.", uri.ToString());
			this.backendSocket = new ClientWebSocket();
			this.backendSocket.Options.UseDefaultCredentials = true;
			for (int i = 0; i < this.serverRequestHeaders.Count; i++)
			{
				string key = this.serverRequestHeaders.GetKey(i);
				string[] values = this.serverRequestHeaders.GetValues(i);
				if (values != null)
				{
					foreach (string headerValue in values)
					{
						if (!ProxyRequestHandler.FrontEndHeaders.Contains(key))
						{
							this.backendSocket.Options.SetRequestHeader(key, headerValue);
						}
					}
				}
			}
			try
			{
				await this.backendSocket.ConnectAsync(uri, cancellationToken);
				ExTraceGlobals.VerboseTracer.TraceDebug<string>((long)this.GetHashCode(), "[ProxyRequestHandler::ConnectToBackend]: Successfully connected to {0} via WebSocket protocol.", uri.ToString());
			}
			catch (Exception)
			{
				if (this.timeoutCancellationTokenSource.IsCancellationRequested)
				{
					ExTraceGlobals.VerboseTracer.TraceError<string>((long)this.GetHashCode(), "[ProxyRequestHandler::ConnectToBackend]: Timed out trying to connect to {0} via WebSocket protocol.", uri.ToString());
					this.CleanupWebSocket(true);
				}
				else
				{
					if (!this.disposeCancellationTokenSource.IsCancellationRequested)
					{
						throw;
					}
					ExTraceGlobals.VerboseTracer.TraceError((long)this.GetHashCode(), "[ProxyRequestHandler::ConnectToBackend]: Proxy request handler has been disposed.");
					this.CleanupWebSocket(false);
				}
			}
		}

		private async Task HandleWebSocket(WebSocketContext webSocketContext)
		{
			try
			{
				this.disposeCancellationTokenSource = new CancellationTokenSource();
				this.timeoutCancellationTokenSource = new CancellationTokenSource();
				await this.HandleWebSocketInternal((AspNetWebSocketContext)webSocketContext);
			}
			catch (Exception source)
			{
				ExceptionDispatchInfo exceptionDispatchInfo = ExceptionDispatchInfo.Capture(source);
				Diagnostics.SendWatsonReportOnUnhandledException(delegate()
				{
					exceptionDispatchInfo.Throw();
				});
			}
			finally
			{
				Diagnostics.SendWatsonReportOnUnhandledException(delegate()
				{
					this.CleanupWebSocket(false);
				});
				ProxyRequestHandler.DisposeIfNotNullAndCatchExceptions(this.disposeCancellationTokenSource);
				ProxyRequestHandler.DisposeIfNotNullAndCatchExceptions(this.timeoutCancellationTokenSource);
				ProxyRequestHandler.DisposeIfNotNullAndCatchExceptions(this.cancellationTokenSource);
			}
		}

		private async Task HandleWebSocketInternal(AspNetWebSocketContext webSocketContext)
		{
			this.clientSocket = webSocketContext.WebSocket;
			this.timeoutCancellationTokenSource.CancelAfter(ProxyRequestHandler.Timeout);
			this.cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(this.timeoutCancellationTokenSource.Token, this.disposeCancellationTokenSource.Token);
			Uri uri = this.GetTargetBackEndServerUrl();
			await this.ConnectToBackend(new UriBuilder(uri)
			{
				Scheme = ((StringComparer.OrdinalIgnoreCase.Compare(uri.Scheme, "https") == 0) ? "wss" : "ws")
			}.Uri, this.cancellationTokenSource.Token);
			this.bufferPool = this.GetBufferPool(ProxyRequestHandler.MaxMessageSize.Value);
			this.clientBuffer = this.bufferPool.Acquire();
			this.backendBuffer = this.bufferPool.Acquire();
			await Task.WhenAll(new List<Task>
			{
				this.ProxyWebSocketData(this.clientSocket, this.backendSocket, this.clientBuffer, true, this.cancellationTokenSource.Token).ContinueWith<Task>(async delegate(Task _)
				{
					if (this.backendSocket.State == WebSocketState.Open)
					{
						await this.backendSocket.CloseOutputAsync(WebSocketCloseStatus.EndpointUnavailable, string.Empty, this.cancellationTokenSource.Token);
					}
				}),
				this.ProxyWebSocketData(this.backendSocket, this.clientSocket, this.backendBuffer, false, this.cancellationTokenSource.Token).ContinueWith<Task>(async delegate(Task _)
				{
					if (this.clientSocket.State == WebSocketState.Open)
					{
						await this.clientSocket.CloseOutputAsync(WebSocketCloseStatus.EndpointUnavailable, string.Empty, this.cancellationTokenSource.Token);
					}
				})
			});
		}

		private async Task ProxyWebSocketData(WebSocket receiveSocket, WebSocket sendSocket, byte[] receiveBuffer, bool fromClientToBackend, CancellationToken cancellationToken)
		{
			while (receiveSocket.State == WebSocketState.Open && sendSocket.State == WebSocketState.Open)
			{
				ExTraceGlobals.VerboseTracer.TraceDebug<string>((long)this.GetHashCode(), "[ProxyRequestHandler::ProxyWebSocketData]: Receiving WebSocket data from the {0}.", fromClientToBackend ? "client" : "backend");
				WebSocketReceiveResult result = await this.ReceiveAsyncWithTryCatch(async () => await receiveSocket.ReceiveAsync(new ArraySegment<byte>(receiveBuffer), cancellationToken));
				if (result != null)
				{
					if (result.MessageType == WebSocketMessageType.Close)
					{
						await Task.WhenAll(new Task[]
						{
							receiveSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, cancellationToken),
							sendSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, cancellationToken)
						});
					}
					else
					{
						int count = result.Count;
						while (!result.EndOfMessage)
						{
							if (count >= ProxyRequestHandler.MaxMessageSize.Value)
							{
								string closeMessage = string.Format("Maximum message size: {0} bytes.", ProxyRequestHandler.MaxMessageSize.Value);
								await Task.WhenAll(new Task[]
								{
									receiveSocket.CloseAsync(WebSocketCloseStatus.MessageTooBig, closeMessage, cancellationToken),
									sendSocket.CloseAsync(WebSocketCloseStatus.MessageTooBig, closeMessage, cancellationToken)
								});
								return;
							}
							result = await this.ReceiveAsyncWithTryCatch(async () => await receiveSocket.ReceiveAsync(new ArraySegment<byte>(receiveBuffer, count, ProxyRequestHandler.MaxMessageSize.Value - count), cancellationToken));
							if (result == null)
							{
								return;
							}
							count += result.Count;
						}
						if (result.MessageType == WebSocketMessageType.Text)
						{
							await this.SendWebSocketMessage(receiveBuffer, count, sendSocket, WebSocketMessageType.Text, cancellationToken);
							continue;
						}
						if (result.MessageType == WebSocketMessageType.Binary)
						{
							await this.SendWebSocketMessage(receiveBuffer, count, sendSocket, WebSocketMessageType.Binary, cancellationToken);
							continue;
						}
						throw new NotImplementedException(string.Format("Unexpected WebSocket message type: {0}.", result.MessageType));
					}
				}
				return;
			}
		}

		private async Task SendWebSocketMessage(byte[] receiveBuffer, int bytesCount, WebSocket sendSocket, WebSocketMessageType webSocketMessageType, CancellationToken cancellationToken)
		{
			ArraySegment<byte> outputBuffer = new ArraySegment<byte>(receiveBuffer, 0, bytesCount);
			await this.SendAsyncWithTryCatch(async delegate
			{
				await sendSocket.SendAsync(outputBuffer, webSocketMessageType, true, cancellationToken);
			});
		}

		private async Task<WebSocketReceiveResult> ReceiveAsyncWithTryCatch(Func<Task<WebSocketReceiveResult>> func)
		{
			try
			{
				return await func();
			}
			catch (InvalidOperationException ex)
			{
				ExTraceGlobals.VerboseTracer.TraceError<string>((long)this.GetHashCode(), "[ProxyRequestHandler::ProxyWebSocketData]: ReceiveAsync failed: {0}.", ex.Message);
			}
			catch (WebSocketException ex2)
			{
				ExTraceGlobals.VerboseTracer.TraceError<string>((long)this.GetHashCode(), "[ProxyRequestHandler::ProxyWebSocketData]: ReceiveAsync failed: {0}.", ex2.Message);
			}
			catch (Exception)
			{
				if (this.timeoutCancellationTokenSource.IsCancellationRequested)
				{
					ExTraceGlobals.VerboseTracer.TraceError((long)this.GetHashCode(), "[ProxyRequestHandler::ProxyWebSocketData]: Timed out trying to read data.");
					this.CleanupWebSocket(true);
				}
				else
				{
					if (!this.disposeCancellationTokenSource.IsCancellationRequested)
					{
						throw;
					}
					ExTraceGlobals.VerboseTracer.TraceError((long)this.GetHashCode(), "[ProxyRequestHandler::ProxyWebSocketData]: Proxy request handler has been disposed.");
					this.CleanupWebSocket(false);
				}
			}
			return null;
		}

		private async Task SendAsyncWithTryCatch(Func<Task> func)
		{
			try
			{
				await func();
			}
			catch (WebSocketException ex)
			{
				ExTraceGlobals.VerboseTracer.TraceError<string>((long)this.GetHashCode(), "[ProxyRequestHandler::ProxyWebSocketData]: SendAsync failed: {0}.", ex.Message);
			}
			catch (Exception)
			{
				if (this.timeoutCancellationTokenSource.IsCancellationRequested)
				{
					ExTraceGlobals.VerboseTracer.TraceError((long)this.GetHashCode(), "[ProxyRequestHandler::ProxyWebSocketData]: Timed out trying to send data.");
					this.CleanupWebSocket(true);
				}
				else
				{
					if (!this.disposeCancellationTokenSource.IsCancellationRequested)
					{
						throw;
					}
					ExTraceGlobals.VerboseTracer.TraceError((long)this.GetHashCode(), "[ProxyRequestHandler::ProxyWebSocketData]: Proxy request handler has been disposed.");
					this.CleanupWebSocket(false);
				}
			}
		}

		private void CleanupWebSocket(bool timedOut)
		{
			try
			{
				if (this.backendSocket != null && this.backendSocket.State != WebSocketState.Closed)
				{
					Task task = this.backendSocket.CloseAsync(timedOut ? WebSocketCloseStatus.EndpointUnavailable : WebSocketCloseStatus.NormalClosure, string.Empty, this.timeoutCancellationTokenSource.Token);
					task.Wait((int)ProxyRequestHandler.Timeout.TotalMilliseconds, this.timeoutCancellationTokenSource.Token);
				}
				if (this.clientSocket != null && this.clientSocket.State != WebSocketState.Closed)
				{
					Task task2 = this.clientSocket.CloseAsync(timedOut ? WebSocketCloseStatus.EndpointUnavailable : WebSocketCloseStatus.NormalClosure, string.Empty, this.timeoutCancellationTokenSource.Token);
					task2.Wait((int)ProxyRequestHandler.Timeout.TotalMilliseconds, this.timeoutCancellationTokenSource.Token);
				}
			}
			catch (Exception)
			{
				if (this.timeoutCancellationTokenSource.IsCancellationRequested)
				{
					ExTraceGlobals.VerboseTracer.TraceError((long)this.GetHashCode(), "[ProxyRequestHandler::CleanupWebSocket]: Timed out trying to close WebSocket connections.");
				}
			}
			if (this.disposeCancellationTokenSource != null)
			{
				this.disposeCancellationTokenSource.Cancel();
			}
			if (this.clientBuffer != null)
			{
				this.bufferPool.Release(this.clientBuffer);
				this.clientBuffer = null;
			}
			if (this.backendBuffer != null)
			{
				this.bufferPool.Release(this.backendBuffer);
				this.backendBuffer = null;
			}
			if (this.backendSocket != null)
			{
				this.backendSocket.Dispose();
				this.backendSocket = null;
			}
		}

		private const int MaxStatusDescriptionLength = 512;

		protected const int E12E14TargetPort = 443;

		protected const int TargetPort = 444;

		private const string RoutingErrorLogString = "RoutingError";

		private static readonly object MultiRequestLockObject = new object();

		private static readonly HashSet<string> RestrictedHeaders = new HashSet<string>
		{
			"set-cookie",
			"server",
			"x-powered-by",
			"x-aspnet-version",
			"www-authenticate",
			"persistent-auth",
			Constants.BEServerExceptionHeaderName,
			Constants.BEServerRoutingErrorHeaderName,
			"X-MSExchangeActivityCtx",
			"request-id",
			"client-request-id",
			Constants.BEResourcePath,
			"X-FromBackend-ServerAffinity",
			WellKnownHeader.XDBMountedOnServer,
			"X-RoutingEntryUpdate"
		};

		private readonly object lockObject = new object();

		private AsyncCallback asyncCallback;

		private object asyncState;

		private ManualResetEvent completedWaitHandle;

		private Exception asyncException;

		private DisposeTracker disposeTracker;

		private bool haveStartedOutOfBandProxyLogon;

		private bool haveReceivedAuthChallenge;

		private int retryOnErrorCounter;

		private BufferedRegionStream bufferedRegionStream;

		private AuthenticationContext authenticationContext;

		private string kerberosChallenge;

		private int delayOnRetryOnError;

		private RequestState requestState;

		private IRoutingEntry routingEntry;

		private bool disposed;

		private KeyValuePair<Guid, BackEndServer>? newBackendForRetry;

		private MailboxServerLocator mailboxServerLocator;

		private DatacenterRedirectStrategy datacenterRedirectStrategy;

		private StreamProxy requestStreamProxy;

		private ProxyRequestHandler.BufferPoolWithBuffer requestStreamBufferPool;

		private bool requestBufferInitialized;

		private StreamProxy responseStreamProxy;

		private ProxyRequestHandler.BufferPoolWithBuffer responseStreamBufferPool;

		private bool responseBufferInitialized;

		private FileStream captureResponseStream;

		private static readonly HashSet<string> FrontEndHeaders = new HashSet<string>
		{
			"Connection",
			"Upgrade",
			"Sec-WebSocket-Key",
			"Sec-WebSocket-Version"
		};

		private static readonly TimeSpan Timeout = TimeSpan.FromSeconds((double)new IntAppSettingsEntry("HttpProxy.WebSocketTimeout", 60, ExTraceGlobals.VerboseTracer).Value);

		private static readonly IntAppSettingsEntry MaxMessageSize = new IntAppSettingsEntry("HttpProxy.MaxWebSocketMessageSize", 16384, ExTraceGlobals.VerboseTracer);

		private WebSocket clientSocket;

		private ClientWebSocket backendSocket;

		private BufferPool bufferPool;

		private byte[] clientBuffer;

		private byte[] backendBuffer;

		private CancellationTokenSource timeoutCancellationTokenSource;

		private CancellationTokenSource disposeCancellationTokenSource;

		private CancellationTokenSource cancellationTokenSource;

		private WebHeaderCollection serverRequestHeaders;

		public enum SupportBackEndCookie
		{
			V1 = 1,
			V2,
			All
		}

		protected enum ProxyState
		{
			None,
			Initializing,
			CalculateBackEnd,
			CalculateBackEndSecondRound,
			PrepareServerRequest,
			ProxyRequestData,
			WaitForServerResponse,
			ProxyResponseData,
			Completed,
			CleanedUp,
			WaitForProxyLogonRequestStream,
			WaitForProxyLogonResponse,
			ProxyWebSocketData
		}

		public class BufferPoolWithBuffer
		{
			public BufferPoolWithBuffer(BufferPoolCollection.BufferSize bufferSize)
			{
				this.bufferPool = BufferPoolCollection.AutoCleanupCollection.Acquire(bufferSize);
			}

			public BufferPoolWithBuffer(BufferPool bufferPool)
			{
				this.bufferPool = bufferPool;
			}

			public byte[] AcquireBuffer()
			{
				if (this.buffer == null)
				{
					this.buffer = this.bufferPool.Acquire();
				}
				return this.buffer;
			}

			public void Release()
			{
				if (this.buffer == null)
				{
					return;
				}
				try
				{
					this.bufferPool.Release(this.buffer);
					this.buffer = null;
					this.bufferPool = null;
				}
				catch (Exception)
				{
				}
			}

			private BufferPool bufferPool;

			private byte[] buffer;
		}
	}
}
