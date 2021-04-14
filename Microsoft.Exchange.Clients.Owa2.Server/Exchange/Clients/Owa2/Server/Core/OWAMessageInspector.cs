using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Web;
using Microsoft.Exchange.Clients.Common;
using Microsoft.Exchange.Clients.Owa2.Server.Diagnostics;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage.Configuration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Services;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf;
using Microsoft.Exchange.WorkloadManagement;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class OWAMessageInspector : IDispatchMessageInspector
	{
		public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
		{
			HttpContext httpContext = HttpContext.Current;
			RequestDetailsLogger current = RequestDetailsLoggerBase<RequestDetailsLogger>.GetCurrent(httpContext);
			RequestDetailsLogger.LogEvent(current, OwaServerLogger.LoggerData.OwaMessageInspectorReceiveRequestBegin);
			object result;
			using (CpuTracker.StartCpuTracking("PRE"))
			{
				result = this.InternalAfterReceiveRequest(ref request, current);
			}
			RequestDetailsLogger.LogEvent(current, OwaServerLogger.LoggerData.OwaMessageInspectorReceiveRequestEnd);
			return result;
		}

		public void BeforeSendReply(ref Message reply, object correlationState)
		{
			HttpContext httpContext = HttpContext.Current;
			RequestDetailsLogger current = RequestDetailsLoggerBase<RequestDetailsLogger>.GetCurrent(httpContext);
			RequestDetailsLogger.LogEvent(current, OwaServerLogger.LoggerData.OwaMessageInspectorEndRequestBegin);
			using (CpuTracker.StartCpuTracking("POST"))
			{
				this.InternalBeforeSendReply();
			}
			RequestDetailsLogger.LogEvent(current, OwaServerLogger.LoggerData.OwaMessageInspectorEndRequestEnd);
		}

		private static string GetMethodName(Message request)
		{
			string result = null;
			object obj;
			if (request.Properties.TryGetValue("HttpOperationName", out obj) && obj is string)
			{
				result = (string)obj;
			}
			else
			{
				HttpRequestMessageProperty httpRequestMessageProperty = request.Properties[HttpRequestMessageProperty.Name] as HttpRequestMessageProperty;
				if (httpRequestMessageProperty != null)
				{
					result = httpRequestMessageProperty.Headers[OWADispatchOperationSelector.Action];
				}
			}
			return result;
		}

		public static bool RequestNeedsHeaderProcessing(string methodName)
		{
			return !string.IsNullOrEmpty(methodName) && !OWAMessageInspector.noHeaderProcessingMethodMap.Value.Contains(methodName.ToLowerInvariant());
		}

		public static bool RequestNeedsQueryStringProcessing(string methodName)
		{
			return !string.IsNullOrEmpty(methodName) && !OWAMessageInspector.noHeaderQueryProcessingMethodMap.Value.Contains(methodName.ToLowerInvariant());
		}

		public static bool RequestNeedsHttpHeaderProcessing(string methodName)
		{
			return !string.IsNullOrEmpty(methodName) && !OWAMessageInspector.noHeaderHttpHeaderProcessingMethodMap.Value.Contains(methodName.ToLowerInvariant());
		}

		public static void CheckMowaRemoteWipe(string methodName, CallContext callContext)
		{
			if (methodName.Equals("NotifyAppWipe", StringComparison.OrdinalIgnoreCase))
			{
				Microsoft.Exchange.Diagnostics.Components.Clients.ExTraceGlobals.MobileDevicePolicyTracer.TraceDebug(0L, "[OWAMessageInspector::CheckMowaRemoteWipe] Executing NotifyAppWipe method. Skipping RemoteWipe check.");
				return;
			}
			if (methodName.Equals("PingOwa", StringComparison.OrdinalIgnoreCase))
			{
				Microsoft.Exchange.Diagnostics.Components.Clients.ExTraceGlobals.MobileDevicePolicyTracer.TraceDebug(0L, "[OWAMessageInspector::CheckMowaRemoteWipe] Executing PingOwa method. Skipping RemoteWipe check.");
				return;
			}
			if (callContext.IsRemoteWipeRequested)
			{
				Microsoft.Exchange.Diagnostics.Components.Clients.ExTraceGlobals.AppWipeTracer.TraceDebug(0L, "[OWAMessageInspector::CheckMowaRemoteWipe] Remote wipe has been requested. Sending remote wipe error.");
				callContext.MarkRemoteWipeAsSent();
				throw new OwaRemoteWipeException("A remote wipe has been requested for this device.", callContext.GetEffectiveAccessingSmtpAddress());
			}
		}

		public static void CheckMowaDisabled(CallContext callContext)
		{
			if (callContext.IsMowa && callContext.AccessingPrincipal != null && !callContext.AccessingPrincipal.MailboxInfo.Configuration.IsMowaEnabled)
			{
				throw new OwaMowaDisabledException("Mowa is disabled for this mailbox", callContext.GetEffectiveAccessingSmtpAddress());
			}
		}

		public static void CheckMobileDevicePolicyIsCorrect(string methodName, CallContext callContext)
		{
			if (!callContext.IsMowa)
			{
				Microsoft.Exchange.Diagnostics.Components.Clients.ExTraceGlobals.MobileDevicePolicyTracer.TraceDebug(0L, "[OWAMessageInspector::CheckMobileDevicePolicyIsCorrect] Request is not coming from a MOWA session. Skipping policy check.");
				return;
			}
			if (string.IsNullOrEmpty(callContext.MobileDevicePolicyId))
			{
				Microsoft.Exchange.Diagnostics.Components.Clients.ExTraceGlobals.MobileDevicePolicyTracer.TraceDebug(0L, "[OWAMessageInspector::CheckMobileDevicePolicyIsCorrect] Client isn't passing policy information (old client). Skipping policy check.");
				return;
			}
			if (methodName.Equals("GetOwaUserConfiguration", StringComparison.OrdinalIgnoreCase))
			{
				Microsoft.Exchange.Diagnostics.Components.Clients.ExTraceGlobals.MobileDevicePolicyTracer.TraceDebug(0L, "[OWAMessageInspector::CheckMobileDevicePolicyIsCorrect] Executing GetOwaUserConfiguration method. Skipping policy check.");
				callContext.UpdateLastPolicyTime();
				return;
			}
			if (methodName.Equals("PingOwa", StringComparison.OrdinalIgnoreCase))
			{
				Microsoft.Exchange.Diagnostics.Components.Clients.ExTraceGlobals.MobileDevicePolicyTracer.TraceDebug(0L, "[OWAMessageInspector::CheckMobileDevicePolicyIsCorrect] Executing PingOwa method. Skipping policy check.");
				return;
			}
			ADObjectId policy = null;
			MobileDevicePolicyData policyData = MobileDevicePolicyDataFactory.GetPolicyData(callContext.AccessingPrincipal, out policy);
			callContext.UpdatePolicyApplied(policy);
			string mobileDevicePolicyId = callContext.MobileDevicePolicyId;
			if (policyData != null && !string.Equals(policyData.PolicyIdentifier, mobileDevicePolicyId, StringComparison.Ordinal))
			{
				callContext.MarkDeviceAsBlockedByPolicy();
				string effectiveAccessingSmtpAddress = callContext.GetEffectiveAccessingSmtpAddress();
				Microsoft.Exchange.Diagnostics.Components.Clients.ExTraceGlobals.MobileDevicePolicyTracer.TraceWarning(0L, "[OWAMessageInspector::CheckMobileDevicePolicyIsCorrect] Policy Identifier does not match expected value. Expected: '{0}'. Actual: '{1}'. Method: '{2}'. User: '{3}'.", new object[]
				{
					policyData.PolicyIdentifier,
					mobileDevicePolicyId,
					methodName,
					effectiveAccessingSmtpAddress
				});
				throw new OwaInvalidMobileDevicePolicyException(string.Format("The presented mobile device policy id '{0}' is not valid. Method '{1}' is being rejected.", mobileDevicePolicyId, methodName), effectiveAccessingSmtpAddress, policyData.PolicyIdentifier);
			}
			callContext.MarkDeviceAsAllowed();
		}

		public static void CheckThatUserProvisionedDevice(string method, CallContext callContext)
		{
			if (method.Equals("Provision", StringComparison.OrdinalIgnoreCase) || method.Equals("Deprovision", StringComparison.OrdinalIgnoreCase) || method.Equals("GetTimeZoneOffsets", StringComparison.OrdinalIgnoreCase) || method.Equals("GetOwaUserConfiguration", StringComparison.OrdinalIgnoreCase) || method.Equals("PingOwa", StringComparison.OrdinalIgnoreCase))
			{
				return;
			}
			if (!callContext.IsDeviceIdProvisioned())
			{
				throw new OwaDeviceNotProvisionedException(string.Format("The presented device-id {0} has not been provisioned. Method {1} is being rejected.", callContext.OwaDeviceId, method), callContext.GetEffectiveAccessingSmtpAddress());
			}
		}

		public static void MarkResponseNonCacheable(string method)
		{
			if (OWAMessageInspector.dontMarkAsNoCacheNoStoreMethodMap.Value.Contains(method.ToLowerInvariant()))
			{
				return;
			}
			HttpUtilities.MakePageNoCacheNoStore(HttpContext.Current.Response);
		}

		internal static void InternalCheckClientVersion(CallContext callContext, Func<string, string> headerGetter)
		{
			if (EsoRequest.IsEsoRequest(callContext.HttpContext.Request))
			{
				return;
			}
			string text = headerGetter("X-OWA-VersionId");
			string clientOwsVersionString = headerGetter("X-OWA-ClientOWSVersion");
			UserContext userContext = UserContextManager.GetMailboxContext(HttpContext.Current, null, true) as UserContext;
			bool flag = userContext != null && userContext.FeaturesManager.ServerSettings.OwaVNext.Enabled;
			bool flag2;
			if (userContext != null && userContext.FeaturesManager.ClientServerSettings.OwaVersioning.Enabled)
			{
				flag2 = OwaVersionId.Supports(clientOwsVersionString, flag);
			}
			else
			{
				flag2 = OwaVersionId.Matches(text, flag);
			}
			if (flag2)
			{
				return;
			}
			string arg = flag ? OwaVersionId.VNext : OwaVersionId.Current;
			string text2 = string.Format("Server={0}, Client={1}", arg, text);
			string actionId = headerGetter("X-OWA-ActionId");
			if (CallContext.IsQueuedActionId(actionId))
			{
				Microsoft.Exchange.Diagnostics.Components.Services.ExTraceGlobals.CommonAlgorithmTracer.TraceDebug<string>(0L, "[OWAMessageInspector::CheckClientVersion] Ignoring version mismatch because this is a queued action {0}", text2);
				return;
			}
			Microsoft.Exchange.Diagnostics.Components.Services.ExTraceGlobals.CommonAlgorithmTracer.TraceWarning<string>(0L, "[OWAMessageInspector::CheckClientVersion] Client/Server version mismatch: {0}", text2);
			throw new OwaVersionException(text2, callContext.GetEffectiveAccessingSmtpAddress());
		}

		public static void CheckClientVersion(CallContext callContext)
		{
			object obj;
			if (OperationContext.Current != null && OperationContext.Current.IncomingMessageProperties.TryGetValue(HttpRequestMessageProperty.Name, out obj))
			{
				HttpRequestMessageProperty reqProp = (HttpRequestMessageProperty)obj;
				OWAMessageInspector.InternalCheckClientVersion(callContext, (string name) => reqProp.Headers[name]);
				return;
			}
			if (callContext.HttpContext != null)
			{
				HttpRequest request = callContext.HttpContext.Request;
				OWAMessageInspector.InternalCheckClientVersion(callContext, (string name) => request.Headers[name]);
			}
		}

		public static bool ShouldCreateUserContext(CallContext callContext)
		{
			if (!EsoRequest.IsEsoRequest(callContext.HttpContext.Request))
			{
				return true;
			}
			bool flag = callContext.EffectiveCallerSid.Equals(OWAMessageInspector.LocalSystemSid);
			bool isLocal = callContext.HttpContext.Request.IsLocal;
			if (flag && isLocal)
			{
				return false;
			}
			throw new OwaInvalidRequestException(string.Format("EsoRequest Invalid Request from '{0}' originated in '{1}'.", callContext.EffectiveCaller.PrimarySmtpAddress ?? "<unknown SMTP Address>", callContext.HttpContext.Request.UserHostAddress ?? "<unknown host>"));
		}

		private object InternalAfterReceiveRequest(ref Message request, RequestDetailsLogger logger)
		{
			try
			{
				Globals.UpdateErrorTracingConfiguration();
				IActivityScope activityScope = OwaApplication.GetRequestDetailsLogger.ActivityScope;
				HttpRequestMessageProperty httpRequestMessageProperty = (HttpRequestMessageProperty)request.Properties[HttpRequestMessageProperty.Name];
				string value = httpRequestMessageProperty.Headers[OWADispatchOperationSelector.Action];
				activityScope.SetProperty(ExtensibleLoggerMetadata.EventId, value);
				string value2 = httpRequestMessageProperty.Headers["X-OWA-ClientBuildVersion"];
				if (!string.IsNullOrEmpty(value2))
				{
					activityScope.SetProperty(OwaServerLogger.LoggerData.ClientBuildVersion, value2);
				}
				else
				{
					activityScope.SetProperty(OwaServerLogger.LoggerData.ClientBuildVersion, "NA");
				}
				string value3 = httpRequestMessageProperty.Headers["X-EWS-TargetVersion"];
				if (!string.IsNullOrEmpty(value3))
				{
					activityScope.SetProperty(OwaServerLogger.LoggerData.RequestVersion, value3);
				}
				else
				{
					activityScope.SetProperty(OwaServerLogger.LoggerData.RequestVersion, "NA");
				}
				JsonMessageHeaderProcessor jsonMessageHeaderProcessor = new JsonMessageHeaderProcessor();
				string methodName = OWAMessageInspector.GetMethodName(request);
				bool flag = OWAMessageInspector.RequestNeedsHeaderProcessing(methodName);
				bool flag2 = OWAMessageInspector.RequestNeedsQueryStringProcessing(methodName);
				bool flag3 = OWAMessageInspector.RequestNeedsHttpHeaderProcessing(methodName);
				if (flag || flag2 || flag3)
				{
					using (MessageBuffer messageBuffer = request.CreateBufferedCopy(int.MaxValue))
					{
						Message request2 = messageBuffer.CreateMessage();
						if (flag)
						{
							jsonMessageHeaderProcessor.ProcessMessageHeaders(request2);
							jsonMessageHeaderProcessor.ProcessEwsVersionFromHttpHeaders(request);
						}
						else if (flag2)
						{
							jsonMessageHeaderProcessor.ProcessMessageHeadersFromQueryString(request2);
						}
						else if (flag3)
						{
							jsonMessageHeaderProcessor.ProcessHttpHeaders(request, ExchangeVersion.Exchange2013);
						}
						request = messageBuffer.CreateMessage();
					}
				}
				request.Properties["MessageHeaderProcessor"] = jsonMessageHeaderProcessor;
				request.Properties["ConnectionCostType"] = 0;
				WebMethodEntry jsonWebMethodEntry;
				if (!OWAMessageInspector.MethodNameToWebMethodEntryMap.Member.TryGetValue(methodName, out jsonWebMethodEntry))
				{
					jsonWebMethodEntry = WebMethodEntry.JsonWebMethodEntry;
				}
				request.Properties["WebMethodEntry"] = jsonWebMethodEntry;
				MessageHeaderProcessor messageHeaderProcessor = (MessageHeaderProcessor)request.Properties["MessageHeaderProcessor"];
				messageHeaderProcessor.MarkMessageHeaderAsUnderstoodIfExists(request, "RequestServerVersion", "http://schemas.microsoft.com/exchange/services/2006/types");
				RequestDetailsLogger.LogEvent(logger, OwaServerLogger.LoggerData.CallContextInitBegin);
				Message requestRef = request;
				CallContext callContext = OwaApplication.GetRequestDetailsLogger.TrackLatency<CallContext>(ServiceLatencyMetadata.CallContextInitLatency, () => CallContextUtilities.CreateCallContext(requestRef, messageHeaderProcessor, true, ""));
				RequestDetailsLogger.LogEvent(logger, OwaServerLogger.LoggerData.CallContextInitEnd);
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
				callContext.UsingWcfDispatcher = true;
				callContext.ProtocolLog.Set(OwaServerLogger.LoggerData.UsingWcfHttpHandler, 1);
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
				Microsoft.Exchange.Diagnostics.Components.Services.ExTraceGlobals.CommonAlgorithmTracer.TraceError<string, string>((long)this.GetHashCode(), "[OWAMessageInspector::AfterReceiveRequest] Caught localized exception trying to create callcontext.  Class: {0}, Message: {1}", ex.GetType().FullName, ex.Message);
				OwaServerTraceLogger.AppendToLog(new TraceLogEvent("OWAMessageInspector", null, "InternalAfterReceiveRequest", string.Format("OwaServiceFaultException_InnerException - {0}", ex)));
				RequestDetailsLoggerBase<RequestDetailsLogger>.SafeLogRequestException(OwaApplication.GetRequestDetailsLogger, ex, "OwaServiceFaultException_InnerException");
				throw OwaFaultExceptionUtilities.CreateFault(ex);
			}
			return null;
		}

		private void InternalBeforeSendReply()
		{
			OwaServerTraceLogger.SaveTraces();
			CallContext.SetServiceUnavailableForTransientErrorResponse(CallContext.Current);
		}

		private const int MustUseInt32MaxValueOrWcfWillEventuallyBlowUp = 2147483647;

		private static List<string> rbacMethods = new List<string>
		{
			"GetBposNavBarData",
			"GetItem",
			"GetFileAttachment",
			"GetOwaUserConfiguration",
			"LogDatapoint",
			"GetPersonaPhoto",
			"GetPersona",
			"GetOrganizationHierarchyForPersona",
			"GetNotesForPersona",
			"ProcessSuiteStorage"
		};

		public static LazyMember<Dictionary<string, WebMethodEntry>> MethodNameToWebMethodEntryMap = new LazyMember<Dictionary<string, WebMethodEntry>>(delegate()
		{
			Dictionary<string, WebMethodEntry> result = new Dictionary<string, WebMethodEntry>(OWAMessageInspector.rbacMethods.Count);
			OWAMessageInspector.rbacMethods.ForEach(delegate(string method)
			{
				result.Add(method, WebMethodMetadata.Entries[method]);
			});
			return result;
		});

		private static readonly SecurityIdentifier LocalSystemSid = new SecurityIdentifier(WellKnownSidType.LocalSystemSid, null);

		private static readonly Lazy<HashSet<string>> noHeaderProcessingMethodMap = new Lazy<HashSet<string>>(() => JsonMessageHeaderProcessor.BuildNoHeaderProcessingMap(typeof(OWAService)));

		private static readonly Lazy<HashSet<string>> noHeaderQueryProcessingMethodMap = new Lazy<HashSet<string>>(() => JsonMessageHeaderProcessor.BuildNoHeaderQueryProcessingMap(typeof(OWAService)));

		private static readonly Lazy<HashSet<string>> noHeaderHttpHeaderProcessingMethodMap = new Lazy<HashSet<string>>(() => JsonMessageHeaderProcessor.BuildNoHeaderHttpHeaderProcessingMap(typeof(OWAService)));

		private static readonly Lazy<HashSet<string>> dontMarkAsNoCacheNoStoreMethodMap = new Lazy<HashSet<string>>(() => JsonMessageHeaderProcessor.BuildServiceMethodMap<JsonResponseOptionsAttribute>(typeof(OWAService), (JsonResponseOptionsAttribute attr) => attr.IsCacheable));
	}
}
