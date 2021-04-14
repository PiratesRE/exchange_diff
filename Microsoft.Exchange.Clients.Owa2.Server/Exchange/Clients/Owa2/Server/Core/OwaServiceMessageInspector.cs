using System;
using System.Web;
using Microsoft.Exchange.Clients.Common;
using Microsoft.Exchange.Clients.Owa2.Server.Diagnostics;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.Services;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf;
using Microsoft.Exchange.WorkloadManagement;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	public class OwaServiceMessageInspector : IOwaServiceMessageInspector
	{
		public void AfterReceiveRequest(HttpRequest httpRequest, string methodName, object request)
		{
			using (CpuTracker.StartCpuTracking("PRE"))
			{
				this.InternalAfterReceiveRequest(httpRequest, methodName, request);
			}
		}

		public void BeforeSendReply(HttpResponse httpResponse, string methodName, object response)
		{
			using (CpuTracker.StartCpuTracking("POST"))
			{
				this.InternalBeforeSendReply();
			}
		}

		private void InternalAfterReceiveRequest(HttpRequest httpRequest, string methodName, object request)
		{
			try
			{
				ExTraceGlobals.CommonAlgorithmTracer.TraceDebug<string>((long)this.GetHashCode(), "[OwaServiceMessageInspector::InternalAfterReceiveRequest] called for method name: {0}", methodName);
				CallContext.ClearCallContextForCurrentThread();
				Globals.UpdateErrorTracingConfiguration();
				OwaApplication.GetRequestDetailsLogger.ActivityScope.SetProperty(ExtensibleLoggerMetadata.EventId, methodName);
				OwaServiceMessage message = new OwaServiceMessage(httpRequest, request);
				message.Headers.Action = methodName;
				message.Properties["HttpOperationName"] = methodName;
				OwaMessageHeaderProcessor messageHeaderProcessor = new OwaMessageHeaderProcessor();
				bool flag = OWAMessageInspector.RequestNeedsHeaderProcessing(methodName);
				bool flag2 = OWAMessageInspector.RequestNeedsQueryStringProcessing(methodName);
				bool flag3 = OWAMessageInspector.RequestNeedsHttpHeaderProcessing(methodName);
				ExTraceGlobals.CommonAlgorithmTracer.TraceDebug((long)this.GetHashCode(), "[OwaServiceMessageInspector::InternalAfterReceiveRequest] processing message headers");
				if (flag || flag2 || flag3)
				{
					if (flag)
					{
						messageHeaderProcessor.ProcessMessageHeaders(message);
						messageHeaderProcessor.ProcessEwsVersionFromHttpHeaders(message);
					}
					else if (flag2)
					{
						messageHeaderProcessor.ProcessMessageHeadersFromQueryString(message);
					}
					else if (flag3)
					{
						messageHeaderProcessor.ProcessHttpHeaders(message, ExchangeVersion.Exchange2013);
					}
				}
				message.Properties["MessageHeaderProcessor"] = messageHeaderProcessor;
				message.Properties["ConnectionCostType"] = 0;
				WebMethodEntry jsonWebMethodEntry;
				if (!OWAMessageInspector.MethodNameToWebMethodEntryMap.Member.TryGetValue(methodName, out jsonWebMethodEntry))
				{
					jsonWebMethodEntry = WebMethodEntry.JsonWebMethodEntry;
				}
				message.Properties["WebMethodEntry"] = jsonWebMethodEntry;
				ExTraceGlobals.CommonAlgorithmTracer.TraceDebug((long)this.GetHashCode(), "[OwaServiceMessageInspector::InternalAfterReceiveRequest] creating CallContext");
				CallContext callContext = OwaApplication.GetRequestDetailsLogger.TrackLatency<CallContext>(ServiceLatencyMetadata.CallContextInitLatency, () => CallContextUtilities.CreateCallContext(message, messageHeaderProcessor, true, ""));
				callContext.IsOwa = true;
				ExTraceGlobals.CommonAlgorithmTracer.TraceDebug((long)this.GetHashCode(), "[OwaServiceMessageInspector::InternalAfterReceiveRequest] CallContext created");
				callContext.ProtocolLog.Set(OwaServerLogger.LoggerData.IsMowaClient, OfflineClientRequestUtilities.IsRequestFromMOWAClient(callContext.HttpContext.Request, callContext.HttpContext.Request.UserAgent) ? 1 : 0);
				bool? flag4 = new bool?(OfflineClientRequestUtilities.IsRequestFromOfflineClient(callContext.HttpContext.Request));
				if (flag4 != null)
				{
					callContext.ProtocolLog.Set(OwaServerLogger.LoggerData.IsOfflineEnabled, flag4.Value ? 1 : 0);
				}
				callContext.OwaExplicitLogonUser = UserContextUtilities.GetExplicitLogonUser(HttpContext.Current);
				if (string.IsNullOrEmpty(callContext.OwaExplicitLogonUser))
				{
					OWAMessageInspector.CheckThatUserProvisionedDevice(methodName, callContext);
					OWAMessageInspector.CheckMowaRemoteWipe(methodName, callContext);
					OWAMessageInspector.CheckClientVersion(callContext);
					OWAMessageInspector.CheckMowaDisabled(callContext);
					OWAMessageInspector.CheckMobileDevicePolicyIsCorrect(methodName, callContext);
				}
				OWAMessageInspector.MarkResponseNonCacheable(methodName);
				callContext.WorkloadType = WorkloadType.Owa;
				callContext.UsingWcfDispatcher = false;
				callContext.ProtocolLog.Set(OwaServerLogger.LoggerData.UsingWcfHttpHandler, 0);
				if (ExchangeVersion.Current == ExchangeVersion.Exchange2007)
				{
					ExchangeVersion.Current = ExchangeVersion.Exchange2013;
				}
				if (OWAMessageInspector.ShouldCreateUserContext(callContext))
				{
					UserContext userContext = UserContextManager.GetMailboxContext(callContext.HttpContext, callContext.EffectiveCaller, true) as UserContext;
					if (userContext != null)
					{
						callContext.OwaCulture = userContext.UserCulture;
						if (userContext.FeaturesManager != null)
						{
							callContext.FeaturesManager = userContext.FeaturesManager;
							if (userContext.FeaturesManager.ServerSettings.OwaMailboxSessionCloning.Enabled)
							{
								callContext.OwaUserContextKey = userContext.Key.ToString();
							}
						}
					}
				}
			}
			catch (LocalizedException ex)
			{
				ExTraceGlobals.CommonAlgorithmTracer.TraceError<string, LocalizedException>((long)this.GetHashCode(), "[OwaServiceMessageInspector::InternalAfterReceiveRequest] Caught localized exception trying to process message. Type: {0} Exception: {1}", ex.GetType().Name, ex);
				OwaServerTraceLogger.AppendToLog(new TraceLogEvent("OWAMessageInspector", null, "InternalAfterReceiveRequest", string.Format("OwaServiceFaultException_InnerException - {0}", ex)));
				RequestDetailsLoggerBase<RequestDetailsLogger>.SafeLogRequestException(OwaApplication.GetRequestDetailsLogger, ex, "OwaServiceFaultException_InnerException");
				throw OwaFaultExceptionUtilities.CreateFault(ex);
			}
			catch (Exception ex2)
			{
				ExTraceGlobals.CommonAlgorithmTracer.TraceError<string, Exception>((long)this.GetHashCode(), "[OwaServiceMessageInspector::InternalAfterReceiveRequest] Caught exception trying to process message. Type: {0} Exception: {1}", ex2.GetType().Name, ex2);
				RequestDetailsLoggerBase<RequestDetailsLogger>.SafeLogRequestException(OwaApplication.GetRequestDetailsLogger, ex2, "OwaServiceFaultException_InnerException");
				throw;
			}
			ExTraceGlobals.CommonAlgorithmTracer.TraceDebug((long)this.GetHashCode(), "[OwaServiceMessageInspector::InternalAfterReceiveRequest] completed");
		}

		private void InternalBeforeSendReply()
		{
			OwaServerTraceLogger.SaveTraces();
			CallContext.SetServiceUnavailableForTransientErrorResponse(CallContext.Current);
		}
	}
}
