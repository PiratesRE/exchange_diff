using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading;
using System.Web;
using System.Xml;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Net.Protocols;
using Microsoft.Exchange.Security;
using Microsoft.Exchange.Security.Authentication;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.Security.OAuth;
using Microsoft.Exchange.Security.PartnerToken;
using Microsoft.Exchange.Services.Wcf;
using Microsoft.Exchange.SoapWebClient;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Exchange.WorkloadManagement;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal class ProxyServiceTask<T> : BaseServiceTask<T>, IDisposeTrackable, IDisposable
	{
		static ProxyServiceTask()
		{
			CertificateValidationManager.RegisterCallback("EWS", Global.RemoteCertificateValidationCallback);
		}

		public ProxyServiceTask(BaseRequest request, CallContext callContext, ServiceAsyncResult<T> serviceAsyncResult, WebServicesInfo[] services) : this(request, callContext, serviceAsyncResult, services, ProxyServiceTask<T>.ConstructServersAttemptedString(services))
		{
		}

		private ProxyServiceTask(BaseRequest request, CallContext callContext, ServiceAsyncResult<T> serviceAsyncResult, WebServicesInfo[] services, string serversAttemptedForLog) : base(request, callContext, serviceAsyncResult)
		{
			this.services = services;
			this.disposeTracker = this.GetDisposeTracker();
			this.budget = callContext.Budget;
			this.serversAttemptedForLog = serversAttemptedForLog;
			if (services != null)
			{
				this.ProxyToCafe = services.Any((WebServicesInfo wsInfo) => wsInfo.IsCafeUrl);
			}
		}

		private static string ConstructServersAttemptedString(WebServicesInfo[] services)
		{
			if (services == null || services.Length == 0)
			{
				return string.Empty;
			}
			string[] array = new string[services.Length];
			for (int i = 0; i < services.Length; i++)
			{
				array[i] = services[i].ServerFullyQualifiedDomainName;
			}
			return string.Join("|", array);
		}

		public bool ProxyToCafe { get; private set; }

		public virtual DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<ProxyServiceTask<T>>(this);
		}

		public void SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
			}
		}

		public void Dispose()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Dispose();
			}
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool fromDispose)
		{
			if (!this.disposed)
			{
				if (fromDispose)
				{
					lock (this.instanceLock)
					{
						this.InternalDispose();
						return;
					}
				}
				this.InternalDispose();
			}
		}

		protected virtual void InternalDispose()
		{
			if (!this.disposed)
			{
				lock (this.eventLock)
				{
					if (this.proxyDoneEvent != null)
					{
						if (this.registeredWaitHandle != null)
						{
							this.registeredWaitHandle.Unregister(this.proxyDoneEvent);
							this.registeredWaitHandle = null;
						}
						this.proxyDoneEvent.Close();
						this.proxyDoneEvent = null;
						if (this.authenticationContext != null)
						{
							this.authenticationContext.Dispose();
						}
						this.authenticationContext = null;
					}
				}
			}
			this.disposed = true;
		}

		public override IBudget Budget
		{
			get
			{
				return this.budget;
			}
		}

		protected internal override TaskExecuteResult InternalExecute(TimeSpan queueAndDelay, TimeSpan totalTime)
		{
			base.SendWatsonReportOnGrayException(delegate()
			{
				try
				{
					if (!this.ProxyRequest())
					{
						ExTraceGlobals.ProxyEvaluatorTracer.TraceError((long)this.GetHashCode(), "[ProxyServiceTask.InternalExecute] Failed to proxy task.");
						this.executionException = FaultExceptionUtilities.CreateFault(new NoRespondingCASInDestinationSiteException(), FaultParty.Receiver);
					}
				}
				catch (FaultException ex)
				{
					ExTraceGlobals.ProxyEvaluatorTracer.TraceError<FaultException>((long)this.GetHashCode(), "[ProxyServiceTask.InternalExecute] Caught FaultException when proxying task.  Exception: {0}", ex);
					this.executionException = ex;
				}
			});
			return TaskExecuteResult.ProcessingComplete;
		}

		protected internal override TaskExecuteResult InternalCancelStep(LocalizedException exception)
		{
			if (exception != null)
			{
				this.executionException = exception;
				RequestDetailsLoggerBase<RequestDetailsLogger>.SafeLogRequestException(base.CallContext.ProtocolLog, exception, "ProxyServiceTask_Cancel");
			}
			else
			{
				this.executionException = new TransientException(CoreResources.GetLocalizedString((CoreResources.IDs)3995283118U), new Exception("[ProxyServiceTask.InternalCancelStep()] called without an exception."));
			}
			return TaskExecuteResult.ProcessingComplete;
		}

		protected internal override void InternalComplete(TimeSpan queueAndDelayTime, TimeSpan totalTime)
		{
			if (this.executionException == null)
			{
				ExTraceGlobals.ProxyEvaluatorTracer.TraceDebug<int>((long)this.GetHashCode(), "[{0}] [ProxyServiceTask.InternalComplete] Was called no exception. No-op.", this.GetHashCode());
				this.queueAndDelayTime = queueAndDelayTime;
				return;
			}
			string arg = (this.proxiedToService == null) ? string.Empty : this.proxiedToService.Url.ToString();
			ExTraceGlobals.ProxyEvaluatorTracer.TraceError<int, string>((long)this.GetHashCode(), "[{0}] [ProxyServiceTask.InternalComplete] Encountered error during execution. Exception: {1}", this.GetHashCode(), this.executionException.Message);
			this.FinishRequest(string.Format("[CWE,P({0})]", arg), queueAndDelayTime, TimeSpan.Zero, this.executionException);
		}

		protected internal override void InternalTimeout(TimeSpan queueAndDelayTime, TimeSpan totalTime)
		{
			string arg = (this.proxiedToService == null) ? "<NULL>" : this.proxiedToService.Url.ToString();
			ExTraceGlobals.ProxyEvaluatorTracer.TraceDebug<string, TimeSpan, TimeSpan>((long)this.GetHashCode(), "[ProxyServiceTask.InternalTimeout] Encountered timeout for proxy task sending to CAS: '{0}'.  QueueAndDelay: {1}, TotalTime: {2}", arg, queueAndDelayTime, totalTime);
			this.FinishRequest(string.Format("[T,P({0})]", arg), queueAndDelayTime, TimeSpan.Zero, null);
		}

		protected override void FinishRequest(string logType, TimeSpan queueAndDelayTime, TimeSpan totalTime, Exception exception)
		{
			ExTraceGlobals.ProxyEvaluatorTracer.TraceDebug<int, string, string>((long)this.GetHashCode(), "[{0}] [ProxyServiceTask.FinishRequest] Called. Exception={1}, logType={2}", this.GetHashCode(), (exception != null) ? exception.ToString() : "<no exception>", (!string.IsNullOrEmpty(logType)) ? logType : "<null/empty>");
			bool flag = false;
			if (!this.completed)
			{
				lock (this.instanceLock)
				{
					if (!this.completed)
					{
						ExTraceGlobals.ProxyEvaluatorTracer.TraceDebug((long)this.GetHashCode(), "[ProxyServiceTask.FinishRequest] Completing request");
						base.FinishRequest(logType, queueAndDelayTime, totalTime, exception);
						flag = true;
						this.completed = true;
					}
				}
			}
			if (!flag)
			{
				ExTraceGlobals.ProxyEvaluatorTracer.TraceDebug((long)this.GetHashCode(), "[ProxyServiceTask.FinishRequest] WCF request was already finished, so there is nothing for us to complete.");
			}
			this.Dispose();
		}

		protected internal override ResourceKey[] InternalGetResources()
		{
			return null;
		}

		private void TimeoutCallback(object state, bool timedOut)
		{
			if (!this.completed)
			{
				lock (this.instanceLock)
				{
					if (!this.completed && timedOut)
					{
						ExTraceGlobals.ProxyEvaluatorTracer.TraceDebug<string, Uri>((long)this.GetHashCode(), "[ProxyServiceTask.TimeoutCallback] Request timed out during proxy for task {0}, Url: {1}", base.Description, this.webRequest.Address);
						this.webRequest.Abort();
					}
				}
			}
		}

		private bool ProxyRequest()
		{
			IIdentity identity = base.CallContext.HttpContext.User.Identity;
			WindowsIdentity windowsIdentity = identity as WindowsIdentity;
			OAuthIdentity oauthIdentity = identity as OAuthIdentity;
			ClientSecurityContextIdentity clientSecurityContextIdentity = identity as ClientSecurityContextIdentity;
			ExternalIdentity externalIdentity = identity as ExternalIdentity;
			PartnerIdentity partnerIdentity = identity as PartnerIdentity;
			WebServicesInfo[] array = this.services;
			int i = 0;
			while (i < array.Length)
			{
				WebServicesInfo webServicesInfo = array[i];
				bool result;
				if (!base.CallContext.HttpContext.Response.IsClientConnected)
				{
					this.HandleClientDisconnect();
					result = false;
				}
				else
				{
					bool flag = false;
					if (oauthIdentity != null)
					{
						flag = this.ProxyRequestToInternalCAS(webServicesInfo, oauthIdentity);
					}
					else if (externalIdentity != null)
					{
						flag = this.ProxyRequestToExternalCAS(webServicesInfo, externalIdentity);
					}
					else if (partnerIdentity != null)
					{
						flag = this.ProxyRequestToInternalCAS(webServicesInfo, partnerIdentity);
					}
					else if (clientSecurityContextIdentity != null)
					{
						flag = this.ProxyRequestToInternalCAS(webServicesInfo, clientSecurityContextIdentity);
					}
					else if (windowsIdentity != null)
					{
						flag = this.ProxyRequestToInternalCAS(webServicesInfo, windowsIdentity);
					}
					if (!flag)
					{
						ExTraceGlobals.ProxyEvaluatorTracer.TraceDebug<string>((long)this.GetHashCode(), "[ProxyServiceTask.ProxyRequest] An attempt to proxy has failed for task {0}", base.Description);
						PerformanceMonitor.UpdateTotalProxyFailoversCount();
						i++;
						continue;
					}
					result = true;
				}
				return result;
			}
			ExTraceGlobals.ProxyEvaluatorTracer.TraceDebug<string>((long)this.GetHashCode(), "[ProxyServiceTask.ProxyRequest] Could not proxy request for task {0} after trying all applicable CAS servers.", base.Description);
			RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericInfo(base.CallContext.ProtocolLog, "ProxyServersAttempted", this.serversAttemptedForLog);
			ProxyEventLogHelper.LogNoRespondingDestinationCAS(this.services[0].SiteDistinguishedName);
			return false;
		}

		private void HandleClientDisconnect()
		{
			base.CallContext.HttpContext.Response.Clear();
			base.CallContext.HttpContext.Response.StatusCode = 8;
			this.LogProxyException("N/A", "ClientDisconnectedFromRequest");
			ExTraceGlobals.ProxyEvaluatorTracer.TraceDebug<string>(0L, "[ProxyServiceTask.HandleClientDisconnect] Client disconnected for task {0}", base.Description);
		}

		private HttpWebRequest CreateWebRequest(string targetUrl)
		{
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(targetUrl);
			httpWebRequest.Method = "POST";
			HttpRequest request = base.CallContext.HttpContext.Request;
			this.CopyHttpHeaders(request, httpWebRequest);
			if (VariantConfiguration.InvariantNoFlightingSnapshot.Global.GlobalCriminalCompliance.Enabled && EWSSettings.AreGccStoredSecretKeysValid)
			{
				this.CopyOrCreateNewXGccProxyInfoHeader(httpWebRequest);
			}
			httpWebRequest.Credentials = CredentialCache.DefaultCredentials;
			httpWebRequest.ServicePoint.Expect100Continue = false;
			httpWebRequest.KeepAlive = false;
			httpWebRequest.Timeout = (int)this.MaxExecutionTime.TotalMilliseconds + 60000;
			httpWebRequest.Proxy = new WebProxy();
			httpWebRequest.ServerCertificateValidationCallback = Global.RemoteCertificateValidationCallback;
			CertificateValidationManager.SetComponentId(httpWebRequest, "EWS");
			return httpWebRequest;
		}

		private void CopyOrCreateNewXGccProxyInfoHeader(HttpWebRequest toRequest)
		{
			string value;
			if ((GccUtils.TryGetGccProxyInfo(base.CallContext.HttpContext, out value) || GccUtils.TryCreateGccProxyInfo(base.CallContext.HttpContext, out value)) && !string.IsNullOrEmpty(value))
			{
				toRequest.Headers.Add("X-GCC-PROXYINFO", value);
			}
		}

		private void CopyHttpHeaders(HttpRequest originalRequest, HttpWebRequest toRequest)
		{
			toRequest.ContentType = originalRequest.ContentType;
			toRequest.TransferEncoding = null;
			toRequest.SendChunked = false;
			toRequest.UserAgent = ProxyServiceTask<T>.userAgentPrefix + originalRequest.UserAgent;
			string[] allKeys = originalRequest.Headers.AllKeys;
			foreach (string text in allKeys)
			{
				if (!ProxyServiceTask<T>.specialHeaders.Member.Contains(text.ToUpperInvariant()))
				{
					string value = originalRequest.Headers[text];
					toRequest.Headers.Add(text, value);
				}
			}
			if (originalRequest.RequestContext.HttpContext.Items.Contains("AnchorMailboxHintKey"))
			{
				string text2 = originalRequest.RequestContext.HttpContext.Items["AnchorMailboxHintKey"] as string;
				if (SmtpAddress.IsValidSmtpAddress(text2))
				{
					toRequest.Headers[WellKnownHeader.AnchorMailbox] = text2;
				}
				toRequest.Headers[WellKnownHeader.Authorization] = originalRequest.Headers[WellKnownHeader.Authorization];
				toRequest.Headers[WellKnownHeader.XIsFromBackend] = Global.BooleanTrue;
			}
		}

		private bool ProxyRequestToInternalCAS(WebServicesInfo service, WindowsIdentity windowsIdentity)
		{
			ExTraceGlobals.ProxyEvaluatorTracer.TraceDebug<string, string>(0L, "[ProxyServiceTask.ProxyRequestToInternalCAS] Attempting to proxy request internally for task {0} to CAS {1} (called with a WindowsIdentity)", base.Description, service.ServerFullyQualifiedDomainName);
			ProxyHeaderValue proxyHeaderValue = null;
			this.suggesterSid = this.GetProxyHeaderValue(windowsIdentity, service, ref proxyHeaderValue);
			this.proxyHeaderValue = proxyHeaderValue;
			Message messageToProxy = this.GetMessageToProxy();
			ProxyHeaderXmlWriter.AddProxyHeader(messageToProxy, this.proxyHeaderValue);
			return this.ProxyRequestToSingleCAS(service, messageToProxy, service.Url.ToString(), true, null);
		}

		private bool ProxyRequestToInternalCAS(WebServicesInfo service, ClientSecurityContextIdentity cscIdentity)
		{
			ExTraceGlobals.ProxyEvaluatorTracer.TraceDebug<string, string>(0L, "[ProxyServiceTask.ProxyRequestToInternalCAS] Attempting to proxy request internally for task {0} to CAS {1} (called with a ClientSecurityContextIdentity)", base.Description, service.ServerFullyQualifiedDomainName);
			ProxyHeaderValue proxyHeaderValue = null;
			this.suggesterSid = this.GetProxyHeaderValue(cscIdentity, service, ref proxyHeaderValue);
			this.proxyHeaderValue = proxyHeaderValue;
			Message messageToProxy = this.GetMessageToProxy();
			ProxyHeaderXmlWriter.AddProxyHeader(messageToProxy, this.proxyHeaderValue);
			return this.ProxyRequestToSingleCAS(service, messageToProxy, service.Url.ToString(), true, null);
		}

		private bool ProxyRequestToInternalCAS(WebServicesInfo service, OAuthIdentity oauthIdentity)
		{
			ExTraceGlobals.ProxyEvaluatorTracer.TraceDebug<string, string>(0L, "[ProxyServiceTask.ProxyRequestToInternalCAS] Attempting to proxy request internally for task {0} to CAS {1} (called with OAuthIdentity)", base.Description, service.ServerFullyQualifiedDomainName);
			if (service.ServerVersionNumber < Server.E15MinVersion)
			{
				ExTraceGlobals.ProxyEvaluatorTracer.TraceWarning<int>(0L, "[ProxyServiceTask.ProxyRequestToInternalCAS] returning false since the backend version is {0}, not E15+", service.ServerVersionNumber);
				this.LogProxyException(this.proxiedToService.ServerFullyQualifiedDomainName, "NotE15BE_" + service.ServerVersionNumber);
				return false;
			}
			Message messageToProxy = this.GetMessageToProxy();
			ProxyHeaderXmlWriter.RemoveProxyHeaders(messageToProxy);
			return this.ProxyRequestToSingleCAS(service, messageToProxy, service.Url.ToString(), true, oauthIdentity.ToCommonAccessToken(service.ServerVersionNumber));
		}

		private bool ProxyRequestToInternalCAS(WebServicesInfo service, PartnerIdentity partnerIdentity)
		{
			ExTraceGlobals.ProxyEvaluatorTracer.TraceDebug<string, string, DelegatedPrincipal>(0L, "[ProxyServiceTask.ProxyRequestToInternalCAS] Attempting to proxy request internally for task {0} to CAS {1}, the caller is {2}", base.Description, service.ServerFullyQualifiedDomainName, partnerIdentity.DelegatedPrincipal);
			PartnerAccessToken partnerAccessToken = new PartnerAccessToken(partnerIdentity);
			this.proxyHeaderValue = new ProxyHeaderValue(ProxyHeaderType.PartnerToken, partnerAccessToken.GetBytes());
			Message messageToProxy = this.GetMessageToProxy();
			ProxyHeaderXmlWriter.AddProxyHeader(messageToProxy, this.proxyHeaderValue);
			return this.ProxyRequestToSingleCAS(service, messageToProxy, service.Url.ToString(), true, null);
		}

		private bool ProxyRequestToExternalCAS(WebServicesInfo casInstance, ExternalIdentity externalIdentity)
		{
			ExTraceGlobals.ProxyEvaluatorTracer.TraceDebug<string, string>(0L, "[ProxyServiceTask.ProxyRequestToExternalCAS] Attempting to proxy request externally for task {0} to CAS {1}", base.Description, casInstance.ServerFullyQualifiedDomainName);
			Message messageToProxy = this.GetMessageToProxy();
			ProxyHeaderXmlWriter.RemoveProxyHeaders(messageToProxy);
			string text = casInstance.Url.ToString();
			if (externalIdentity.WSSecurityHeader != null)
			{
				text = EwsWsSecurityUrl.Fix(text);
				ExTraceGlobals.ProxyEvaluatorTracer.TraceDebug<string>((long)this.GetHashCode(), "[ProxyServiceTask.ProxyRequestToExternalCAS] Correcting EWS target due to WSSecurity Header.  Corrected target: '{0}'", text);
			}
			return this.ProxyRequestToSingleCAS(casInstance, messageToProxy, text, false, null);
		}

		private Message GetMessageToProxy()
		{
			Message message = EWSSettings.MessageCopyForProxyOnly;
			Message result;
			using (MessageBuffer messageBuffer = message.CreateBufferedCopy(int.MaxValue))
			{
				message = messageBuffer.CreateMessage();
				EWSSettings.MessageCopyForProxyOnly = messageBuffer.CreateMessage();
				result = message;
			}
			return result;
		}

		private bool ProxyRequestToSingleCAS(WebServicesInfo casInstance, Message messageToProxy, string targetUrl, bool needsImpersonation, CommonAccessToken commonAccessToken = null)
		{
			byte[] array;
			int num;
			this.GetMessageBuffer(messageToProxy, out array, out num);
			this.requestStart = ExDateTime.UtcNow;
			this.webRequest = this.CreateWebRequest(targetUrl);
			if (commonAccessToken != null)
			{
				this.webRequest.Headers["X-CommonAccessToken"] = commonAccessToken.Serialize();
			}
			this.webRequest.ContentLength = (long)num;
			PerformanceMonitor.UpdateTotalProxyRequestBytesCount((long)num);
			this.requestBuffer = array;
			this.proxiedToService = casInstance;
			try
			{
				if (!Global.ProxyToSelf && needsImpersonation && this.proxiedToService != null && !this.proxiedToService.IsCafeUrl)
				{
					ExTraceGlobals.ProxyEvaluatorTracer.TraceDebug((long)this.GetHashCode(), "[ProxyServiceTask.ProxyRequestToSingleCAS] Impersonating NetworkService for HTTP proxy call.");
					this.webRequest.Credentials = CredentialCache.DefaultNetworkCredentials;
					if (Global.SendXBEServerExceptionHeaderToCafe)
					{
						Stopwatch stopwatch = new Stopwatch();
						try
						{
							stopwatch.Start();
							string value = this.GenerateKerberosAuthHeader(this.webRequest.Address.Host);
							if (!string.IsNullOrEmpty(value))
							{
								this.webRequest.Headers[WellKnownHeader.Authorization] = value;
							}
						}
						finally
						{
							stopwatch.Stop();
							long elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
							RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericInfo(base.CallContext.ProtocolLog, "KerbAuthLatency", elapsedMilliseconds);
						}
					}
				}
				if (needsImpersonation)
				{
					IActivityScope currentActivityScope = ActivityContext.GetCurrentActivityScope();
					if (currentActivityScope != null)
					{
						currentActivityScope.SerializeTo(this.webRequest);
					}
				}
				this.webRequest.BeginGetRequestStream(new AsyncCallback(this.RequestCallback), null);
			}
			catch (WebException ex)
			{
				ExTraceGlobals.ProxyEvaluatorTracer.TraceError<string, WebException>((long)this.GetHashCode(), "[ProxyServiceTask.ProxyRequestToSingleCAS] Encountered WebException sending request for task {0}. Exception: {1}", base.Description, ex);
				this.LogProxyException(this.webRequest.Host, string.Format("BeginGetRequestStream_{0}_{1}", ex.Status, ex.Message));
				return false;
			}
			lock (this.eventLock)
			{
				if (this.proxyDoneEvent != null)
				{
					this.registeredWaitHandle = ThreadPool.RegisterWaitForSingleObject(this.proxyDoneEvent, new WaitOrTimerCallback(this.TimeoutCallback), null, Global.ProxyTimeout, true);
				}
			}
			return true;
		}

		private string GenerateKerberosAuthHeader(string host)
		{
			byte[] bytes = null;
			if (this.authenticationContext != null)
			{
				this.authenticationContext.Dispose();
				this.authenticationContext = null;
			}
			this.authenticationContext = new AuthenticationContext();
			string result;
			try
			{
				string text = "HTTP/" + host;
				ExTraceGlobals.ProxyEvaluatorTracer.TraceDebug<string>((long)this.GetHashCode(), "[ProxyServiceTask::GenerateKerberosAuthHeader]: SPN {0}", text);
				this.authenticationContext.InitializeForOutboundNegotiate(AuthenticationMechanism.Kerberos, text, null, null);
				SecurityStatus securityStatus = this.authenticationContext.NegotiateSecurityContext(null, out bytes);
				if (securityStatus != SecurityStatus.OK && securityStatus != SecurityStatus.ContinueNeeded)
				{
					ExTraceGlobals.ProxyEvaluatorTracer.TraceDebug<SecurityStatus>((long)this.GetHashCode(), "[ProxyServiceTask::GenerateKerberosAuthHeader]: NegotiateSecurityContext failed with {0}", securityStatus);
					throw new HttpException(500, string.Format("NegotiateSecurityContext failed with for host '{0}' with status '{1}'", host, securityStatus));
				}
				string @string = Encoding.ASCII.GetString(bytes);
				result = "Negotiate " + @string;
			}
			catch (HttpException ex)
			{
				RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericInfo(base.CallContext.ProtocolLog, "GenerateKerberosAuthHeaderFailure", ex.Message);
				result = string.Empty;
			}
			finally
			{
				if (this.authenticationContext != null)
				{
					this.authenticationContext.Dispose();
				}
			}
			return result;
		}

		private void GetMessageBuffer(Message message, out byte[] messageBuffer, out int messageBufferLength)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (XmlWriter xmlWriter = XmlWriter.Create(memoryStream))
				{
					message.WriteMessage(xmlWriter);
					xmlWriter.Flush();
					memoryStream.Flush();
					messageBufferLength = (int)memoryStream.Position;
					messageBuffer = memoryStream.GetBuffer();
				}
			}
		}

		private void RequestCallback(IAsyncResult asyncResult)
		{
			this.ExecuteWithinCallContext("RequestCallback", delegate(ref bool success)
			{
				Stream stream = null;
				try
				{
					stream = this.webRequest.EndGetRequestStream(asyncResult);
					stream.BeginWrite(this.requestBuffer, 0, (int)this.webRequest.ContentLength, new AsyncCallback(this.BeginWriteCallback), stream);
					ExTraceGlobals.ProxyEvaluatorTracer.TraceDebug<long>((long)this.GetHashCode(), "[ProxyServiceTask.RequestCallback] Request callback was called and a write was started of length {0} bytes", this.webRequest.ContentLength);
					success = true;
				}
				catch (WebException ex)
				{
					if (stream != null)
					{
						stream.Close();
					}
					ExTraceGlobals.ProxyEvaluatorTracer.TraceError<string, WebException>((long)this.GetHashCode(), "[ProxyServiceTask.RequestCallback] Encountered WebException for task {0}, Exception: {1}", this.Description, ex);
					this.ProcessAsyncWebException(ex, "RequestCallback");
				}
			});
		}

		private void ExecuteWithinCallContext(string info, ProxyServiceTask<T>.ExecuteWithinCallContextDelegate action)
		{
			ExTraceGlobals.ProxyEvaluatorTracer.TraceDebug<string, string>((long)this.GetHashCode(), "Starting {0}: {1}", info, base.Description);
			bool success = false;
			if (!this.completed)
			{
				lock (this.instanceLock)
				{
					if (!this.completed)
					{
						CallContext current = CallContext.Current;
						try
						{
							CallContext.SetCurrent(base.CallContext);
							bool flag2 = false;
							if (base.CallContext.AccessingPrincipal != null && ExUserTracingAdaptor.Instance.IsTracingEnabledUser(base.CallContext.AccessingPrincipal.LegacyDn))
							{
								flag2 = true;
								BaseTrace.CurrentThreadSettings.EnableTracing();
							}
							try
							{
								base.SendWatsonReportOnGrayException(delegate()
								{
									action(ref success);
								}, delegate()
								{
									lock (this.eventLock)
									{
										if (this.proxyDoneEvent != null)
										{
											this.proxyDoneEvent.Set();
										}
									}
									base.Complete(TimeSpan.Zero, TimeSpan.Zero);
								});
							}
							finally
							{
								if (flag2)
								{
									BaseTrace.CurrentThreadSettings.DisableTracing();
								}
							}
						}
						finally
						{
							CallContext.SetCurrent(current);
							ExTraceGlobals.ProxyEvaluatorTracer.TraceDebug<string, string>((long)this.GetHashCode(), "Finished {0}: {1}", info, base.Description);
							if (!success)
							{
								lock (this.eventLock)
								{
									if (this.proxyDoneEvent != null)
									{
										this.proxyDoneEvent.Set();
									}
								}
								this.Dispose();
							}
						}
					}
				}
			}
		}

		private void BeginWriteCallback(IAsyncResult result)
		{
			this.ExecuteWithinCallContext("BeginWriteCallback", delegate(ref bool success)
			{
				Stream stream = result.AsyncState as Stream;
				try
				{
					try
					{
						stream.EndWrite(result);
					}
					catch (IOException arg)
					{
						ExTraceGlobals.ProxyEvaluatorTracer.TraceError<IOException>((long)this.GetHashCode(), "[ProxyServiceTask::BeginWriteCallback] Encountered IOException: {0}", arg);
						this.ResubmitTask(false);
						return;
					}
					finally
					{
						stream.Close();
					}
					this.webRequest.BeginGetResponse(new AsyncCallback(this.ResponseCallback), null);
					ExTraceGlobals.ProxyEvaluatorTracer.TraceDebug((long)this.GetHashCode(), "[ProxyServiceTask.BeginWriteCallback] Write callback was called, starting to get response.");
					success = true;
				}
				catch (WebException ex)
				{
					ExTraceGlobals.ProxyEvaluatorTracer.TraceError<WebException>((long)this.GetHashCode(), "[ProxyServiceTask::BeginWriteCallback] Encountered WebException: {0}", ex);
					this.ProcessAsyncWebException(ex, "BeginWriteCallback");
				}
			});
		}

		private bool ProcessAsyncWebException(WebException webException, string caller)
		{
			ExTraceGlobals.ProxyEvaluatorTracer.TraceDebug(0L, "[ProxyServiceTask::ProcessAsyncWebException] Web exception encountered.  URL: '{0}', Server: '{1}', Code: '{2}', Message: '{3}'", new object[]
			{
				this.proxiedToService.Url,
				this.proxiedToService.ServerFullyQualifiedDomainName,
				webException.Status,
				webException.Message
			});
			string text = null;
			HttpWebResponse httpWebResponse = webException.Response as HttpWebResponse;
			if (httpWebResponse != null)
			{
				ExTraceGlobals.ProxyEvaluatorTracer.TraceDebug<string, int, string>(0L, "[ProxyServiceTask::ProcessAsyncWebException] HTTP Response code: {0}, Value: {1}, Description: {2}", httpWebResponse.StatusCode.ToString(), (int)httpWebResponse.StatusCode, httpWebResponse.StatusDescription);
				if (httpWebResponse.StatusCode == HttpStatusCode.Unauthorized)
				{
					ExTraceGlobals.ProxyEvaluatorTracer.TraceError(0L, "[ProxyServiceTask::ProcessAsyncWebException] Proxy request failed with Unauthorized.");
					ProxyEventLogHelper.LogKerberosConfigurationProblem(this.proxiedToService);
					text = string.Format("{0}_{1}", httpWebResponse.StatusCode.ToString(), httpWebResponse.StatusDescription);
				}
			}
			else
			{
				if (webException.Status == WebExceptionStatus.RequestCanceled || webException.Status == WebExceptionStatus.Timeout)
				{
					ExTraceGlobals.ProxyEvaluatorTracer.TraceDebug(0L, "[ProxyServiceTask::ProcessAsyncWebException] Web request timed out.");
				}
				else if (webException.Status == WebExceptionStatus.ConnectFailure)
				{
					ExTraceGlobals.ProxyEvaluatorTracer.TraceError<Uri>(0L, "[ProxyServiceTask::ProcessAsyncWebException] Failed to connect to destination CAS '{0}'", this.proxiedToService.Url);
					BadCASCache.Singleton.Add(this.proxiedToService.ServerFullyQualifiedDomainName, string.Empty);
				}
				else if (webException.Status == WebExceptionStatus.TrustFailure)
				{
					ExTraceGlobals.ProxyEvaluatorTracer.TraceError<string, bool>(0L, "[ProxyServiceTask::ProcessAsyncWebException] Could not establish trust relationship with destination CAS '{0}'.  AllowInternalUntrustedCerts is set to {1}.", this.proxiedToService.ServerFullyQualifiedDomainName, EWSSettings.AllowInternalUntrustedCerts);
					ProxyEventLogHelper.LogNoTrustedCertificateOnDestinationCAS(this.proxiedToService.ServerFullyQualifiedDomainName);
					BadCASCache.Singleton.Add(this.proxiedToService.ServerFullyQualifiedDomainName, string.Empty);
				}
				else
				{
					ExTraceGlobals.ProxyEvaluatorTracer.TraceDebug(0L, "[ProxyServiceTask::ProcessAsyncWebException] webException.Response was either null or not an HttpWebResponse.");
				}
				text = string.Format("{0}_{1}", webException.Status, webException.Message);
			}
			base.Request.HandleStaleCache();
			if (!string.IsNullOrEmpty(text))
			{
				this.LogProxyException(this.proxiedToService.ServerFullyQualifiedDomainName, caller + "_" + text);
				this.ResubmitTask(true);
				return true;
			}
			return false;
		}

		private void LogProxyException(string fqdn, string exceptionMessage)
		{
			RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericError(base.CallContext.ProtocolLog, "ProxyException['" + fqdn + "']", exceptionMessage);
		}

		public void ResponseCallback(IAsyncResult result)
		{
			this.ExecuteWithinCallContext("ResponseCallback", delegate(ref bool success)
			{
				TimeSpan timeSpan = ExDateTime.UtcNow - this.requestStart;
				try
				{
					HttpWebResponse response = this.webRequest.EndGetResponse(result) as HttpWebResponse;
					PerformanceMonitor.UpdateProxyResponseTimePerformanceCounter((long)timeSpan.TotalMilliseconds);
					ExTraceGlobals.ProxyEvaluatorTracer.TraceDebug<TimeSpan>((long)this.GetHashCode(), "[ProxyServiceTask.ResponseCallback] Response callback was called. Total proxy time was {0}", timeSpan);
					if (this.suggesterSid != null)
					{
						ProxySuggesterSidCache.Singleton.Add(this.suggesterSid, string.Empty);
					}
					this.ProcessResponseMessageAndCompleteIfNecessary(timeSpan, response, ProxyResult.Success);
					success = true;
				}
				catch (WebException ex)
				{
					if (!this.ProcessAsyncWebException(ex, "ResponseCallback"))
					{
						HttpWebResponse response2 = (HttpWebResponse)ex.Response;
						string empty = string.Empty;
						switch (this.GetSoapFaultType(ex, response2, out empty))
						{
						case ProxyServiceTask<T>.SoapFaultType.NotSoapFault:
							ExTraceGlobals.ProxyEvaluatorTracer.TraceDebug<string>(0L, "[ProxyServiceTask::ProxyRequestToSingleCAS] Web exception was not soap fault.  Failing this proxy '{0}' and adding this server to the bad CAS cache.", this.proxiedToService.ServerFullyQualifiedDomainName);
							BadCASCache.Singleton.Add(this.proxiedToService.ServerFullyQualifiedDomainName, string.Empty);
							this.LogProxyException(this.proxiedToService.ServerFullyQualifiedDomainName, "ResponseCallback_" + empty);
							this.ResubmitTask(true);
							break;
						case ProxyServiceTask<T>.SoapFaultType.Proxier:
							ExTraceGlobals.ProxyEvaluatorTracer.TraceDebug(0L, "[ProxyServiceTask::ProxyRequestToSingleCAS] Proxy token has expired.  Must resend.");
							ProxySuggesterSidCache.Singleton.Remove(this.suggesterSid);
							this.LogProxyException(this.proxiedToService.ServerFullyQualifiedDomainName, "ResponseCallback_" + empty);
							this.ResubmitTask(false);
							break;
						case ProxyServiceTask<T>.SoapFaultType.Client:
							this.ProcessResponseMessageAndCompleteIfNecessary(timeSpan, response2, ProxyResult.SoapFault);
							break;
						}
					}
				}
			});
		}

		private void ResubmitTask(bool useNextCAS)
		{
			try
			{
				lock (this.instanceLock)
				{
					this.completed = true;
				}
				WebServicesInfo[] array = null;
				if (useNextCAS)
				{
					ExTraceGlobals.ProxyEvaluatorTracer.TraceDebug<int>((long)this.GetHashCode(), "[ProxyServiceTask.ResubmitTask] Moving to next CAS for proxy attempt.  CAS Array length: {0}", this.services.Length - 1);
					if (this.services.Length <= 1)
					{
						ExTraceGlobals.ProxyEvaluatorTracer.TraceDebug((long)this.GetHashCode(), "[ProxyServiceTask.ResubmitTask] Needed to move to next CAS for proxying, but there are no more in the list.");
						RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericInfo(base.CallContext.ProtocolLog, "ProxyServersAttempted", this.serversAttemptedForLog);
						base.CompleteWCFRequest(new NoRespondingCASInDestinationSiteException());
						return;
					}
					array = new WebServicesInfo[this.services.Length - 1];
					Array.Copy(this.services, 1, array, 0, array.Length);
				}
				else
				{
					ExTraceGlobals.ProxyEvaluatorTracer.TraceDebug((long)this.GetHashCode(), "[ProxyServiceTask.ResubmitTask] Resubmitting request to same CAS.");
					array = this.services;
				}
				lock (this.eventLock)
				{
					if (this.proxyDoneEvent != null)
					{
						this.proxyDoneEvent.Set();
					}
				}
				ProxyServiceTask<T> task = new ProxyServiceTask<T>(base.Request, base.CallContext, base.ServiceAsyncResult, array, this.serversAttemptedForLog);
				if (base.CallContext.HttpContext != null)
				{
					base.CallContext.HttpContext.Items["WlmQueueReSubmitTime"] = Stopwatch.GetTimestamp();
				}
				if (!base.CallContext.WorkloadManager.TrySubmitNewTask(task))
				{
					ExTraceGlobals.ProxyEvaluatorTracer.TraceError((long)this.GetHashCode(), "[ProxyServiceTask.ResubmitTask] Failed to resubmit task to the workload manager.");
					RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericError(base.CallContext.ProtocolLog, "ProxyServiceTask_ResubmitTask", "ErrorServerBusy: Failed to resubmit task to the workload manager.");
					base.CompleteWCFRequest(new ServerBusyException());
				}
			}
			finally
			{
				this.Dispose();
			}
		}

		protected virtual void ProcessResponseMessageAndCompleteIfNecessary(TimeSpan elapsed, HttpWebResponse response, ProxyResult result)
		{
			lock (this.eventLock)
			{
				if (this.proxyDoneEvent != null)
				{
					this.proxyDoneEvent.Set();
				}
			}
			EWSSettings.ProxyResponse = response;
			this.SetProxyHopHeaders(this.proxiedToService);
			ExTraceGlobals.ThrottlingTracer.TraceDebug<ProxyResult, string>((long)this.GetHashCode(), "[ProxyServiceTask.SetResponseMessageAndComplete] Proxied response returned with result {0} for task '{1}'", result, base.Description);
			this.FinishRequest(string.Format("[C,P({0})]", this.proxiedToService.Url.ToString()), this.queueAndDelayTime, elapsed, null);
		}

		private ProxyServiceTask<T>.SoapFaultType GetSoapFaultType(WebException webException, HttpWebResponse response, out string exceptionMessage)
		{
			exceptionMessage = string.Format("{0}_{1}", response.StatusCode.ToString(), response.StatusDescription);
			if (response.StatusCode != HttpStatusCode.InternalServerError)
			{
				return ProxyServiceTask<T>.SoapFaultType.NotSoapFault;
			}
			Stream responseStream = response.GetResponseStream();
			if (!responseStream.CanSeek)
			{
				ExTraceGlobals.ProxyEvaluatorTracer.TraceError<string, string>((long)this.GetHashCode(), "[ProxyServiceTask::GetSoapFaultType] Web Response stream is non-seekable.  Returning SoapFaultType.Client instead.  ExceptionType: {0}, ExceptionMessage: {1}", webException.GetType().FullName, webException.Message);
				return ProxyServiceTask<T>.SoapFaultType.Client;
			}
			long position = responseStream.Position;
			ProxyServiceTask<T>.SoapFaultType result;
			try
			{
				StreamReader streamReader = new StreamReader(responseStream);
				char[] array = new char["<?xml version=\"1.0\"".Length];
				int num = streamReader.Read(array, 0, "<?xml version=\"1.0\"".Length);
				if (num < "<?xml version=\"1.0\"".Length)
				{
					result = ProxyServiceTask<T>.SoapFaultType.NotSoapFault;
				}
				else
				{
					string strA = new string(array, 0, array.Length);
					if (string.Compare(strA, "<?xml version=\"1.0\"", StringComparison.CurrentCultureIgnoreCase) != 0)
					{
						result = ProxyServiceTask<T>.SoapFaultType.NotSoapFault;
					}
					else
					{
						responseStream.Position = position;
						using (XmlReader xmlReader = SafeXmlFactory.CreateSafeXmlReader(responseStream))
						{
							while (xmlReader.Read())
							{
								if (xmlReader.LocalName == "Subcode" && (xmlReader.NamespaceURI == "http://schemas.xmlsoap.org/soap/envelope/" || xmlReader.NamespaceURI == "http://www.w3.org/2003/05/soap-envelope"))
								{
									xmlReader.Read();
									string text = xmlReader.ReadElementContentAsString();
									string[] array2 = text.Split(new char[]
									{
										':'
									});
									text = array2[array2.Length - 1];
									exceptionMessage = text;
									return this.DetermineFaultResponseCodeType(text);
								}
								if (xmlReader.LocalName == "ResponseCode" && xmlReader.NamespaceURI == "http://schemas.microsoft.com/exchange/services/2006/errors")
								{
									string text2 = xmlReader.ReadElementContentAsString();
									exceptionMessage = text2;
									return this.DetermineFaultResponseCodeType(text2);
								}
							}
						}
						result = ProxyServiceTask<T>.SoapFaultType.NotSoapFault;
					}
				}
			}
			finally
			{
				responseStream.Position = position;
			}
			return result;
		}

		private ProxyServiceTask<T>.SoapFaultType DetermineFaultResponseCodeType(string responseCodeString)
		{
			ExTraceGlobals.ProxyEvaluatorTracer.TraceDebug<string>((long)this.GetHashCode(), "[ProxyServiceTask.DetermineFaultResponseCodeType] Encountered response code {0} from destination CAS.", responseCodeString);
			if (responseCodeString == ResponseCodeType.ErrorProxyTokenExpired.ToString())
			{
				return ProxyServiceTask<T>.SoapFaultType.Proxier;
			}
			return ProxyServiceTask<T>.SoapFaultType.Client;
		}

		protected void SetProxyHopHeaders(WebServicesInfo casInstance)
		{
			if (Global.WriteProxyHopHeaders)
			{
				Dictionary<string, string> dictionary = new Dictionary<string, string>(4);
				dictionary.Add("InitialProxyCAS", this.TruncateHeader(LocalServer.GetServer().Id.DistinguishedName));
				dictionary.Add("InitialProxySite", this.TruncateHeader(LocalServer.GetServer().ServerSite.DistinguishedName));
				if (!string.IsNullOrEmpty(casInstance.ServerDistinguishedName))
				{
					dictionary.Add("FinalProxyCAS", this.TruncateHeader(casInstance.ServerDistinguishedName));
				}
				if (!string.IsNullOrEmpty(casInstance.SiteDistinguishedName))
				{
					dictionary.Add("FinalProxySite", this.TruncateHeader(casInstance.SiteDistinguishedName));
				}
				EWSSettings.ProxyHopHeaders = dictionary;
			}
		}

		private string TruncateHeader(string inHeader)
		{
			if (inHeader.Length > ProxyServiceTask<T>.MaxHeaderLength)
			{
				return inHeader.Substring(0, ProxyServiceTask<T>.MaxHeaderLength) + "...";
			}
			return inHeader;
		}

		private SuggesterSidCompositeKey GetProxyHeaderValue(WindowsIdentity windowsIdentity, WebServicesInfo service, ref ProxyHeaderValue proxyHeaderValue)
		{
			SuggesterSidCompositeKey suggesterSidCompositeKey = new SuggesterSidCompositeKey(windowsIdentity.User, service.ServerFullyQualifiedDomainName);
			try
			{
				if (ProxySuggesterSidCache.Singleton.Contains(suggesterSidCompositeKey) || proxyHeaderValue == null || proxyHeaderValue.ProxyHeaderType == ProxyHeaderType.SuggesterSid)
				{
					using (ClientSecurityContext clientSecurityContext = new ClientSecurityContext(windowsIdentity))
					{
						proxyHeaderValue = ProxySecurityContextEncoder.GetHeaderValueForCAS(clientSecurityContext, service, base.CallContext.GetEffectiveAccessingSmtpAddress());
					}
				}
			}
			catch (InvalidProxySecurityContextException exception)
			{
				throw FaultExceptionUtilities.CreateFault(exception, FaultParty.Sender);
			}
			return suggesterSidCompositeKey;
		}

		private SuggesterSidCompositeKey GetProxyHeaderValue(ClientSecurityContextIdentity cscIdentity, WebServicesInfo service, ref ProxyHeaderValue proxyHeaderValue)
		{
			SuggesterSidCompositeKey suggesterSidCompositeKey = new SuggesterSidCompositeKey(cscIdentity.Sid, service.ServerFullyQualifiedDomainName);
			try
			{
				if (ProxySuggesterSidCache.Singleton.Contains(suggesterSidCompositeKey) || proxyHeaderValue == null || proxyHeaderValue.ProxyHeaderType == ProxyHeaderType.SuggesterSid)
				{
					using (ClientSecurityContext clientSecurityContext = cscIdentity.CreateClientSecurityContext())
					{
						proxyHeaderValue = ProxySecurityContextEncoder.GetHeaderValueForCAS(clientSecurityContext, service, base.CallContext.GetEffectiveAccessingSmtpAddress());
					}
				}
			}
			catch (AuthzException innerException)
			{
				throw FaultExceptionUtilities.CreateFault(new InternalServerErrorException(innerException), FaultParty.Receiver);
			}
			catch (InvalidProxySecurityContextException exception)
			{
				throw FaultExceptionUtilities.CreateFault(exception, FaultParty.Sender);
			}
			return suggesterSidCompositeKey;
		}

		private const string XmlDeclaration = "<?xml version=\"1.0\"";

		private const string HTTPHeaderAccept = "ACCEPT";

		private const string HTTPHeaderAcceptEncoding = "ACCEPT-ENCODING";

		private const string HTTPHeaderConnection = "CONNECTION";

		private const string HTTPHeaderTransferEncoding = "TRANSFER-ENCODING";

		private const string HTTPHeaderXGccProxyInfo = "X-GCC-PROXYINFO";

		private const string WlmQueueReSubmitKey = "WlmQueueReSubmitTime";

		private const string SpnPrefixForHttp = "HTTP/";

		private const string PrefixForKerbAuthBlob = "Negotiate ";

		private readonly DisposeTracker disposeTracker;

		private static readonly int MaxHeaderLength = 256;

		private readonly string serversAttemptedForLog;

		private AuthenticationContext authenticationContext;

		private static readonly FileVersionInfo VersionInfo = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);

		private static readonly string userAgentPrefix = "ExchangeWebServicesProxy/CrossSite/EXCH/" + ProxyServiceTask<T>.VersionInfo.FileVersion + "/";

		private static LazyMember<List<string>> specialHeaders = new LazyMember<List<string>>(() => new List<string>
		{
			"AUTHORIZATION",
			"ACCEPT",
			"ACCEPT-ENCODING",
			"CONNECTION",
			"CONTENT-LENGTH",
			"CONTENT-TYPE",
			"EXPECT",
			"DATE",
			"HOST",
			"IF-MODIFIED-SINCE",
			"MSEXCHPROXYURI",
			"PROXY-CONNECTION",
			"RANGE",
			"REFERER",
			"TRANSFER-ENCODING",
			"USER-AGENT",
			"X-COMMONACCESSTOKEN",
			"X-GCC-PROXYINFO",
			"X-ISFROMCAFE"
		});

		private WebServicesInfo[] services;

		private ExDateTime requestStart;

		private SuggesterSidCompositeKey suggesterSid;

		private ProxyHeaderValue proxyHeaderValue;

		private byte[] requestBuffer;

		private bool disposed;

		private RegisteredWaitHandle registeredWaitHandle;

		private object instanceLock = new object();

		private bool completed;

		private HttpWebRequest webRequest;

		protected ManualResetEvent proxyDoneEvent = new ManualResetEvent(false);

		protected object eventLock = new object();

		protected WebServicesInfo proxiedToService;

		protected TimeSpan queueAndDelayTime;

		protected IBudget budget;

		private delegate void ExecuteWithinCallContextDelegate(ref bool success);

		internal enum SoapFaultType
		{
			NotSoapFault,
			Proxier,
			Client
		}
	}
}
