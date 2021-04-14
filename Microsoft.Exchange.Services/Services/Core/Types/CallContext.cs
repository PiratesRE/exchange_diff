using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Web;
using System.Text;
using System.Threading;
using System.Web;
using Microsoft.Exchange.AirSync;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ClientAccessRules;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.InfoWorker.Common.Availability;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Net.Protocols;
using Microsoft.Exchange.Security.Authentication;
using Microsoft.Exchange.Services.DispatchPipe.Ews;
using Microsoft.Exchange.Services.Wcf;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Exchange.WorkloadManagement;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal class CallContext : IDisposeTrackable, IDisposable
	{
		internal static void SetCurrent(CallContext callContext)
		{
			ExTraceGlobals.CommonAlgorithmTracer.TraceDebug(0L, (callContext == null) ? "<Null>" : callContext.ToString());
			CallContext callContext2 = CallContext.Current;
			CallContext.current = callContext;
			if (callContext != null && !callContext.isDisposed)
			{
				CallContext.AssertContextVariables(callContext.HttpContext, callContext.ProtocolLog);
				if (callContext.ProtocolLog != null && callContext.ProtocolLog.ActivityScope != null)
				{
					ActivityContext.SetThreadScope(callContext.ProtocolLog.ActivityScope);
				}
				if (callContext.OwaCulture != null)
				{
					Thread.CurrentThread.CurrentCulture = callContext.OwaCulture;
					Thread.CurrentThread.CurrentUICulture = callContext.OwaCulture;
					return;
				}
			}
			else
			{
				ActivityContext.ClearThreadScope();
				if (callContext2 != null)
				{
					if (callContext2.previousThreadCulture != null)
					{
						Thread.CurrentThread.CurrentCulture = callContext2.previousThreadCulture;
						callContext2.previousThreadCulture = null;
					}
					if (callContext2.previousThreadUICulture != null)
					{
						Thread.CurrentThread.CurrentUICulture = callContext2.previousThreadUICulture;
						callContext2.previousThreadUICulture = null;
					}
				}
			}
		}

		internal static void ClearCallContextForCurrentThread()
		{
			if (CallContext.current != null)
			{
				string arg = CallContext.current.MethodName;
				bool arg2 = CallContext.current.isDisposed;
				ExTraceGlobals.CommonAlgorithmTracer.TraceError<string, bool>((long)CallContext.current.GetHashCode(), "[CallContext::ResetCallContextForThread] CallContext.current is non-null. Was created for method name: {0}. IsDisposed {1}. ", arg, arg2);
				CallContext.current = null;
			}
		}

		private static void DisposeObject(IDisposable disposable)
		{
			if (disposable != null)
			{
				disposable.Dispose();
			}
		}

		private static bool CanUserAgentUseBackingCache(string userAgent)
		{
			return string.IsNullOrEmpty(userAgent) || !userAgent.Contains("MSEXCHMON");
		}

		private bool MatchesUserAgent(string pattern)
		{
			return UserAgentPattern.IsMatch(pattern, this.userAgent);
		}

		internal int GetAccessingPrincipalServerVersion()
		{
			if (this.AccessingPrincipal == null)
			{
				return BaseServerIdInfo.InvalidServerVersion;
			}
			if (this.AccessingPrincipal is RemoteUserMailboxPrincipal)
			{
				return Server.E14SP1MinVersion;
			}
			return this.AccessingPrincipal.MailboxInfo.Location.ServerVersion;
		}

		private CallContext(AppWideStoreSessionCache mailboxSessionBackingCache, AcceptedDomainCache acceptedDomainCache, UserWorkloadManager workloadManager, ProxyCASStatus proxyCASStatus, AuthZClientInfo effectiveCallerClientInfo, ExchangePrincipal effectiveCallerExchangePrincipal, ADRecipientSessionContext adRecipientSessionContext, MailboxAccessType mailboxAccessType, ProxyRequestType? availabilityProxyRequestType, CultureInfo serverCulture, CultureInfo clientCulture, string userAgent, bool isWSSecurityUser, CallContext.UserKind userKind, OriginalCallerContext originalCallerContext, RequestedLogonType requestedLogonType, WebMethodEntry webMethodEntry, AuthZBehavior authZBehavior) : this()
		{
			this.workloadManager = workloadManager;
			this.acceptedDomainCache = acceptedDomainCache;
			this.proxyCASStatus = proxyCASStatus;
			this.mailboxAccessType = mailboxAccessType;
			this.availabilityProxyRequestType = availabilityProxyRequestType;
			this.effectiveCallerAuthZClientInfo = effectiveCallerClientInfo;
			this.effectiveCallerExchangePrincipal = effectiveCallerExchangePrincipal;
			this.adRecipientSessionContext = adRecipientSessionContext;
			this.serverCulture = serverCulture;
			this.clientCulture = clientCulture;
			this.userAgent = userAgent;
			this.isWSSecurityUser = isWSSecurityUser;
			this.userKind = userKind;
			this.sessionCache = new SessionCache(mailboxSessionBackingCache, this);
			this.originalCallerContext = originalCallerContext;
			this.requestedLogonType = requestedLogonType;
			this.webMethodEntry = webMethodEntry;
			this.authZBehavior = authZBehavior;
			if (Global.EnableMailboxLogger && this.WorkloadType == WorkloadType.Ews && SyncStateStorage.GetMailboxLoggingEnabled(this.SessionCache.GetMailboxIdentityMailboxSession(), null))
			{
				this.mailboxLogger = new MailboxLoggerHandler(this.SessionCache.GetMailboxIdentityMailboxSession(), "EWS", "All", false);
				this.mailboxLogger.SetData(MailboxLogDataName.RequestHeader, "Test Header");
				this.mailboxLogger.SaveLogToMailbox();
			}
		}

		protected CallContext(HttpContext httpContext, EwsOperationContextBase operationContext, RequestDetailsLogger requestDetailsLogger) : this(httpContext, operationContext, requestDetailsLogger, false)
		{
		}

		protected CallContext(HttpContext httpContext, EwsOperationContextBase operationContext, RequestDetailsLogger requestDetailsLogger, bool isMock)
		{
			this.AssertContexts(httpContext, operationContext, requestDetailsLogger);
			this.httpContext = httpContext;
			this.operationContext = operationContext;
			this.protocolLog = requestDetailsLogger;
			if (!isMock)
			{
				this.isWebSocketRequest = ((httpContext.Items != null && httpContext.Items["IsWebSocketRequest"] != null) || httpContext.IsWebSocketRequest);
			}
			this.InitializeOwaFields();
			this.disposeTracker = this.GetDisposeTracker();
		}

		protected CallContext() : this(HttpContext.Current, EwsOperationContextBase.Current, RequestDetailsLogger.Current)
		{
		}

		protected void CheckDisposed()
		{
			if (this.isDisposed)
			{
				string objectName = string.Format("Instance of {0} for method {1} was already disposed on thread {2}, accessed on thread {3}", new object[]
				{
					base.GetType().Name,
					this.MethodName,
					this.disposerThreadId,
					Thread.CurrentThread.ManagedThreadId
				});
				throw new ObjectDisposedException(objectName);
			}
		}

		public ProxyCASStatus ProxyCASStatus
		{
			get
			{
				return this.proxyCASStatus;
			}
		}

		internal ProxyRequestType? AvailabilityProxyRequestType
		{
			get
			{
				return this.availabilityProxyRequestType;
			}
		}

		public virtual string GetEffectiveAccessingSmtpAddress()
		{
			return this.EffectiveCaller.PrimarySmtpAddress;
		}

		public CultureInfo GetSessionCulture(StoreSession session)
		{
			if (!session.MailboxOwner.MailboxInfo.IsArchive)
			{
				return session.Culture;
			}
			return this.clientCulture;
		}

		public string OwaUserContextKey
		{
			get
			{
				return this.owaUserContextKey;
			}
			set
			{
				this.owaUserContextKey = value;
			}
		}

		public string SoapAction
		{
			get
			{
				return this.soapAction;
			}
		}

		public MailboxAccessType MailboxAccessType
		{
			get
			{
				return this.mailboxAccessType;
			}
		}

		public ADUser AccessingADUser
		{
			get
			{
				if (this.EffectiveCaller.UserIdentity != null)
				{
					return this.EffectiveCaller.UserIdentity.ADUser;
				}
				return null;
			}
		}

		public ExchangePrincipal AccessingPrincipal
		{
			get
			{
				return this.effectiveCallerExchangePrincipal;
			}
		}

		public string OrganizationalUnitName
		{
			get
			{
				if (this.AccessingPrincipal == null)
				{
					return string.Empty;
				}
				if (this.AccessingPrincipal.MailboxInfo != null && this.AccessingPrincipal.MailboxInfo.OrganizationId != null && this.AccessingPrincipal.MailboxInfo.OrganizationId.OrganizationalUnit != null)
				{
					return this.AccessingPrincipal.MailboxInfo.OrganizationId.OrganizationalUnit.Name;
				}
				if (this.AccessingPrincipal.ObjectId != null && this.AccessingPrincipal.ObjectId.DomainId != null)
				{
					return this.AccessingPrincipal.ObjectId.DomainId.ToCanonicalName();
				}
				return string.Empty;
			}
		}

		public ExchangePrincipal MailboxIdentityPrincipal
		{
			get
			{
				if (!string.IsNullOrEmpty(this.OwaExplicitLogonUser))
				{
					return ExchangePrincipalCache.GetFromCache(this.OwaExplicitLogonUser, this.ADRecipientSessionContext);
				}
				return this.AccessingPrincipal;
			}
		}

		public SecurityIdentifier EffectiveCallerSid
		{
			get
			{
				if (this.EffectiveCaller.ClientSecurityContext == null)
				{
					throw new ServiceAccessDeniedException(CoreResources.IDs.MessageOperationRequiresUserContext);
				}
				return this.EffectiveCaller.ClientSecurityContext.UserSid;
			}
		}

		public SessionCache SessionCache
		{
			get
			{
				return this.sessionCache;
			}
		}

		public UserWorkloadManager WorkloadManager
		{
			get
			{
				return this.workloadManager;
			}
		}

		public IEwsBudget Budget
		{
			get
			{
				this.CheckDisposed();
				return this.callerBudget;
			}
		}

		public CultureInfo ClientCulture
		{
			get
			{
				return this.clientCulture;
			}
		}

		public CultureInfo ServerCulture
		{
			get
			{
				return this.serverCulture;
			}
		}

		public CultureInfo OwaCulture
		{
			get
			{
				return this.owaCulture;
			}
			set
			{
				this.owaCulture = value;
				this.clientCulture = value;
				EWSSettings.ClientCulture = value;
				this.previousThreadCulture = Thread.CurrentThread.CurrentCulture;
				this.previousThreadUICulture = Thread.CurrentThread.CurrentUICulture;
				Thread.CurrentThread.CurrentCulture = value;
				Thread.CurrentThread.CurrentUICulture = value;
			}
		}

		public AcceptedDomain DefaultDomain
		{
			get
			{
				return this.acceptedDomainCache.DefaultDomain;
			}
		}

		public string Description
		{
			get
			{
				if (this.description == null)
				{
					this.description = string.Format("RC:{0};Action:{1};Caller:{2}", EWSSettings.RequestCorrelation, this.SoapAction, (this.Budget == null) ? "<NULL>" : this.Budget.Owner.ToString());
				}
				return this.description;
			}
		}

		public WebMethodEntry WebMethodEntry
		{
			get
			{
				return this.webMethodEntry;
			}
		}

		public AuthZBehavior AuthZBehavior
		{
			get
			{
				return this.authZBehavior;
			}
		}

		public OriginalCallerContext OriginalCallerContext
		{
			get
			{
				return this.originalCallerContext;
			}
		}

		public IOwaCallback OwaCallback
		{
			get
			{
				return this.owaCallback;
			}
			set
			{
				this.owaCallback = value;
			}
		}

		public WorkloadType WorkloadType
		{
			get
			{
				return this.workloadType;
			}
			set
			{
				this.workloadType = value;
			}
		}

		public bool BackgroundLoad
		{
			get
			{
				return this.backgroundLoad;
			}
			set
			{
				this.backgroundLoad = value;
			}
		}

		public bool IsRequestTracingEnabled
		{
			get
			{
				return this.isRequestTracingEnabled;
			}
			set
			{
				this.isRequestTracingEnabled = value;
			}
		}

		public bool IsSmimeInstalled { get; private set; }

		public bool IsOwa { get; set; }

		public bool IsTransientErrorResponse { get; set; }

		public bool IsServiceUnavailableOnTransientError { get; set; }

		public IFeaturesManager FeaturesManager { get; set; }

		public string CallerApplicationId { get; set; }

		public bool IsWebSocketRequest
		{
			get
			{
				return this.isWebSocketRequest;
			}
		}

		internal CallContext.UserKind UserKindSource
		{
			get
			{
				return this.userKind;
			}
		}

		internal bool AllowUnthrottledBudget
		{
			get
			{
				return this.allowUnthrottledBudget;
			}
			set
			{
				this.allowUnthrottledBudget = value;
			}
		}

		internal string OwaExplicitLogonUser { get; set; }

		internal bool IsExternalUser
		{
			get
			{
				return this.userKind == CallContext.UserKind.External;
			}
		}

		internal LogonType LogonType
		{
			get
			{
				return this.requestedLogonType.LogonType;
			}
		}

		internal LogonTypeSource LogonTypeSource
		{
			get
			{
				return this.requestedLogonType.Source;
			}
		}

		internal bool RequirePrivilegedLogon
		{
			get
			{
				return this.LogonType == LogonType.Admin || this.LogonType == LogonType.SystemService;
			}
		}

		internal bool IsWSSecurityUser
		{
			get
			{
				return this.isWSSecurityUser;
			}
		}

		internal bool IsPartnerUser
		{
			get
			{
				return this.userKind == CallContext.UserKind.Partner;
			}
		}

		internal bool IsOAuthUser
		{
			get
			{
				return this.userKind == CallContext.UserKind.OAuth;
			}
		}

		internal bool IsMSAUser
		{
			get
			{
				return this.userKind == CallContext.UserKind.MSA;
			}
		}

		internal string UserAgent
		{
			get
			{
				return this.userAgent;
			}
		}

		internal AuthZClientInfo EffectiveCaller
		{
			get
			{
				return this.effectiveCallerAuthZClientInfo;
			}
		}

		internal ADRecipientSessionContext ADRecipientSessionContext
		{
			get
			{
				if (this.adRecipientSessionContext == null)
				{
					string message = string.Format("ADRecipientSessionContext is null. {0}", this.Description);
					throw new InvalidOperationException(message);
				}
				return this.adRecipientSessionContext;
			}
		}

		internal HttpContext HttpContext
		{
			get
			{
				return this.httpContext;
			}
			set
			{
				this.httpContext = value;
			}
		}

		internal EwsOperationContextBase OperationContext
		{
			get
			{
				return this.operationContext;
			}
		}

		internal RequestDetailsLogger ProtocolLog
		{
			get
			{
				return this.protocolLog;
			}
		}

		internal List<ClientStatistics> ClientRequestStatistics { get; set; }

		internal bool EncodeStringProperties { get; set; }

		internal bool UsingWcfDispatcher
		{
			get
			{
				return this.usingWcfDispatcher;
			}
			set
			{
				this.usingWcfDispatcher = value;
			}
		}

		internal bool IsLongRunningScenario { get; set; }

		internal bool IsDetachedFromRequest { get; set; }

		internal bool IsRequestProxiedFromDifferentResourceForest { get; private set; }

		internal bool IsHybridPublicFolderAccessRequest { get; set; }

		internal event EventHandler OnDisposed;

		internal List<IDisposable> ObjectsToDisposeList
		{
			get
			{
				if (this.objectToDisposeList == null)
				{
					this.objectToDisposeList = new List<IDisposable>();
				}
				return this.objectToDisposeList;
			}
		}

		internal MailboxLoggerHandler MailboxLogger
		{
			get
			{
				return this.mailboxLogger;
			}
		}

		internal MailboxIdServerInfo GetServerInfoForEffectiveCaller()
		{
			if (string.IsNullOrEmpty(this.EffectiveCaller.PrimarySmtpAddress))
			{
				return null;
			}
			return MailboxIdServerInfo.Create(this.EffectiveCaller.PrimarySmtpAddress);
		}

		internal void DisposeForExchangeService()
		{
			this.callerBudget = null;
			this.Dispose();
		}

		internal IDictionary<string, string> GetAccessingInformation()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("AccessingSmtpAddress", this.GetEffectiveAccessingSmtpAddress());
			if (this.AccessingPrincipal != null && this.AccessingPrincipal.MailboxInfo != null)
			{
				dictionary.Add("AccessingMailboxGuid", this.AccessingPrincipal.MailboxInfo.MailboxGuid.ToString());
				IUserPrincipal userPrincipal = this.AccessingPrincipal as IUserPrincipal;
				if (userPrincipal != null && userPrincipal.NetId != null)
				{
					dictionary.Add("AccessingNetId", userPrincipal.NetId.ToString());
				}
			}
			return dictionary;
		}

		protected virtual void Dispose(bool isDisposing)
		{
			ExTraceGlobals.CommonAlgorithmTracer.TraceDebug<int>((long)this.GetHashCode(), "CallContext.Dispose. Hashcode: {0}", this.GetHashCode());
			if (!this.isDisposed)
			{
				lock (this.instanceLock)
				{
					if (!this.isDisposed)
					{
						this.isDisposed = true;
						this.disposerThreadId = Thread.CurrentThread.ManagedThreadId;
						if (this.OnDisposed != null)
						{
							this.OnDisposed(this, EventArgs.Empty);
							this.OnDisposed = null;
						}
						if (this.objectToDisposeList != null)
						{
							foreach (IDisposable disposable in this.objectToDisposeList)
							{
								CallContext.DisposeObject(disposable);
							}
							this.objectToDisposeList = null;
						}
						if (this.effectiveCallerAuthZClientInfo != null)
						{
							this.effectiveCallerAuthZClientInfo.Dispose();
							this.effectiveCallerAuthZClientInfo = null;
						}
						this.DisposeOwaFields();
						if (this.sessionCache != null)
						{
							this.sessionCache.Dispose();
							this.sessionCache = null;
						}
						if (this.callerBudget != null)
						{
							try
							{
								FaultInjection.GenerateFault((FaultInjection.LIDs)3559271741U);
								if (this.ProtocolLog != null && this.ProtocolLog.ActivityScope != null)
								{
									ActivityContext.SetThreadScope(this.protocolLog.ActivityScope);
								}
								if (!this.IsDetachedFromRequest)
								{
									this.callerBudget.LogEndStateToIIS();
								}
							}
							catch (ADTransientException arg)
							{
								ExTraceGlobals.CommonAlgorithmTracer.TraceError<ADTransientException>(0L, "[CallContext::Dispose] Got ADTransientException {0} while disposing budget.", arg);
							}
							finally
							{
								ExTraceGlobals.UtilAlgorithmTracer.TraceDebug((long)this.GetHashCode(), "[CallContext::Dispose] Disposing of CallContext budget.");
								this.callerBudget.Dispose();
							}
							this.callerBudget = null;
						}
						if (isDisposing)
						{
							GC.SuppressFinalize(this);
						}
					}
				}
			}
		}

		public void Dispose()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Dispose();
			}
			this.Dispose(true);
		}

		public virtual DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<CallContext>(this);
		}

		public void SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
			}
		}

		public static CallContext Current
		{
			get
			{
				CallContext result = null;
				if (CallContext.current != null)
				{
					result = CallContext.current;
				}
				else if (HttpContext.Current != null)
				{
					result = (HttpContext.Current.Items["CallContext"] as CallContext);
				}
				else
				{
					ExTraceGlobals.CommonAlgorithmTracer.TraceDebug(0L, "CallContext.Current is null");
				}
				return result;
			}
		}

		protected virtual void AssertContexts(HttpContext httpContext, EwsOperationContextBase operationContext, RequestDetailsLogger requestDetailsLogger)
		{
			CallContext.AssertContextVariables(httpContext, requestDetailsLogger);
		}

		protected static void AssertContextVariables(HttpContext httpContext, RequestDetailsLogger requestDetailsLogger)
		{
		}

		public bool DuplicatedActionDetectionEnabled { get; private set; }

		public bool? IsDuplicatedAction { get; private set; }

		public string MobileDevicePolicyId
		{
			get
			{
				return this.mobileDevicePolicyId;
			}
		}

		public string OwaDeviceId
		{
			get
			{
				return this.owaDeviceId;
			}
		}

		public bool IsMowa
		{
			get
			{
				return this.owaProtocol == "MOWA";
			}
		}

		public bool IsOutlookService { get; private set; }

		public bool IsRemoteWipeRequested
		{
			get
			{
				if (this.IsMowa && this.HasDeviceHeaders)
				{
					GlobalInfo globalInfo = this.GetMowaSyncState();
					if (globalInfo != null)
					{
						return globalInfo.RemoteWipeRequestedTime != null;
					}
				}
				return false;
			}
		}

		public bool IsExplicitLogon
		{
			get
			{
				return this.isExplicitLogon;
			}
		}

		public string OwaDeviceType
		{
			get
			{
				return this.owaDeviceType;
			}
		}

		public string OwaProtocol
		{
			get
			{
				return this.owaProtocol;
			}
		}

		public bool ReturningSavedResult { get; private set; }

		public bool ResultSaved { get; private set; }

		public bool HasDeviceHeaders
		{
			get
			{
				return !string.IsNullOrWhiteSpace(this.owaDeviceId) && !string.IsNullOrWhiteSpace(this.owaDeviceType) && !string.IsNullOrWhiteSpace(this.owaProtocol);
			}
		}

		private bool DetectDuplicatedActions
		{
			get
			{
				return this.DuplicatedActionDetectionEnabled && this.HasDeviceHeaders && CallContext.IsQueuedActionId(this.owaActionId);
			}
		}

		public void MarkRemoteWipeAsSent()
		{
			GlobalInfo globalInfo = this.GetMowaSyncState();
			if (globalInfo != null)
			{
				globalInfo.RemoteWipeSentTime = new ExDateTime?(ExDateTime.UtcNow);
				this.SaveMowaSyncState();
			}
		}

		public void MarkRemoteWipeAsAcknowledged()
		{
			GlobalInfo globalInfo = this.GetMowaSyncState();
			if (globalInfo != null)
			{
				globalInfo.RemoteWipeAckTime = new ExDateTime?(ExDateTime.UtcNow);
				ProvisionCommand.SendRemoteWipeConfirmationMessage(globalInfo.RemoteWipeConfirmationAddresses, ExDateTime.Now, this.SessionCache.GetMailboxIdentityMailboxSession(), new DeviceIdentity(this.OwaDeviceId, this.OwaDeviceType, this.owaProtocol), this);
				this.SaveMowaSyncState();
			}
		}

		public void UpdateLastPolicyTime()
		{
			GlobalInfo globalInfo = this.GetMowaSyncState();
			if (globalInfo != null)
			{
				double value = 24.0;
				if (globalInfo.LastPolicyTime == null || globalInfo.DevicePolicyApplicationStatus == DevicePolicyApplicationStatus.NotApplied || ExDateTime.Compare(globalInfo.LastPolicyTime.Value, ExDateTime.UtcNow, TimeSpan.FromHours(value)) != 0)
				{
					globalInfo.LastPolicyTime = new ExDateTime?(ExDateTime.UtcNow);
					this.SaveMowaSyncState();
				}
			}
		}

		public void UpdatePolicyApplied(ADObjectId policy)
		{
			GlobalInfo globalInfo = this.GetMowaSyncState();
			if (globalInfo != null && ((globalInfo.DevicePolicyApplied == null && policy != null) || (globalInfo.DevicePolicyApplied != null && policy == null) || globalInfo.DevicePolicyApplied.ObjectGuid != policy.ObjectGuid))
			{
				globalInfo.DevicePolicyApplied = policy;
				this.SaveMowaSyncState();
			}
		}

		public void UpdateLastSyncAttemptTime()
		{
			if (this.IsMowa)
			{
				GlobalInfo globalInfo = this.GetMowaSyncState();
				if (globalInfo != null)
				{
					double value = 30.0;
					if (globalInfo.LastSyncAttemptTime == null || ExDateTime.Compare(globalInfo.LastSyncAttemptTime.Value, ExDateTime.UtcNow, TimeSpan.FromMinutes(value)) != 0)
					{
						globalInfo.LastSyncAttemptTime = new ExDateTime?(ExDateTime.UtcNow);
						this.SaveMowaSyncState();
					}
				}
			}
		}

		public void UpdateLastSyncSuccessTime()
		{
			if (this.IsMowa)
			{
				GlobalInfo globalInfo = this.GetMowaSyncState();
				if (globalInfo != null)
				{
					double value = 30.0;
					if (globalInfo.LastSyncSuccessTime == null || ExDateTime.Compare(globalInfo.LastSyncSuccessTime.Value, ExDateTime.UtcNow, TimeSpan.FromMinutes(value)) != 0)
					{
						globalInfo.LastSyncSuccessTime = new ExDateTime?(ExDateTime.UtcNow);
						this.SaveMowaSyncState();
					}
				}
			}
		}

		public void MarkDeviceAsBlockedByPolicy()
		{
			GlobalInfo globalInfo = this.GetMowaSyncState();
			if (globalInfo != null && globalInfo.DevicePolicyApplicationStatus != DevicePolicyApplicationStatus.NotApplied)
			{
				globalInfo.DevicePolicyApplicationStatus = DevicePolicyApplicationStatus.NotApplied;
				globalInfo.DeviceAccessState = DeviceAccessState.Blocked;
				globalInfo.DeviceAccessStateReason = DeviceAccessStateReason.Policy;
				this.SaveMowaSyncState();
			}
		}

		public void MarkDeviceAsAllowed()
		{
			GlobalInfo globalInfo = this.GetMowaSyncState();
			if (globalInfo != null && globalInfo.DevicePolicyApplicationStatus != DevicePolicyApplicationStatus.AppliedInFull)
			{
				globalInfo.DevicePolicyApplicationStatus = DevicePolicyApplicationStatus.AppliedInFull;
				globalInfo.DeviceAccessState = DeviceAccessState.Allowed;
				globalInfo.DeviceAccessStateReason = DeviceAccessStateReason.Global;
				this.SaveMowaSyncState();
			}
		}

		public static bool IsQueuedActionId(string actionId)
		{
			return !string.IsNullOrWhiteSpace(actionId) && actionId[0] != '-';
		}

		public bool IsDeviceIdProvisioned()
		{
			return string.IsNullOrWhiteSpace(this.owaDeviceId) || this.TryLoadOwaSyncStateStorage();
		}

		public bool TryGetResponse<T>(out T results)
		{
			if (this.DetectDuplicatedActions)
			{
				CallContext.OwaActionQueueState<T> owaActionQueueState = this.GetOwaActionQueueState<T>();
				try
				{
					this.IsDuplicatedAction = new bool?(owaActionQueueState.LastActionId == this.owaActionId);
					this.protocolLog.Set(ServiceCommonMetadata.IsDuplicatedAction, this.IsDuplicatedAction.Value ? "Y" : "N");
					if (this.IsDuplicatedAction.Value)
					{
						this.ReturningSavedResult = false;
						try
						{
							JsonFaultDetail lastActionError = owaActionQueueState.LastActionError;
							if (lastActionError != null)
							{
								this.ReturningSavedResult = true;
								throw new FaultException<JsonFaultDetail>(lastActionError, new FaultReason(owaActionQueueState.LastActionError.Message));
							}
							results = owaActionQueueState.LastActionResults;
							this.ReturningSavedResult = true;
							return true;
						}
						finally
						{
							if (!this.IsOutlookService)
							{
								this.httpContext.Response.Headers["X-OWA-ReturnedSavedResult"] = this.ReturningSavedResult.ToString();
							}
						}
					}
				}
				catch (CorruptSyncStateException exception)
				{
					RequestDetailsLogger.LogException(exception, "DuplicatedActionDetection is ignoring CorruptSyncStateException.", "TryGetResponse");
					this.IsDuplicatedAction = new bool?(false);
				}
				catch (CustomSerializationException exception2)
				{
					RequestDetailsLogger.LogException(exception2, "DuplicatedActionDetection is ignoring CustomSerializationException.", "TryGetResponse");
					this.IsDuplicatedAction = new bool?(false);
				}
			}
			results = default(T);
			return false;
		}

		public void SetResponse<T>(T result, Exception exception)
		{
			if (exception != null)
			{
				this.IsTransientErrorResponse = false;
			}
			if (this.DetectDuplicatedActions)
			{
				this.ResultSaved = false;
				try
				{
					if (!FaultExceptionUtilities.GetIsTransient(exception) && !this.IsTransientErrorResponse)
					{
						CallContext.OwaActionQueueState<T> owaActionQueueState = this.GetOwaActionQueueState<T>();
						owaActionQueueState.LastActionId = this.owaActionId;
						owaActionQueueState.LastActionResults = result;
						owaActionQueueState.LastActionError = CallContext.CreateJsonFaultDetail(exception);
						this.owaActionQueueSyncState.Commit();
						this.ResultSaved = true;
					}
				}
				finally
				{
					if (!this.IsOutlookService)
					{
						this.httpContext.Response.Headers["X-OWA-ResultSaved"] = this.ResultSaved.ToString();
					}
				}
			}
		}

		internal void SetCallContextFromActionInfo(string deviceId, string protocol, string deviceType, string actionId, bool IsOutlookService)
		{
			this.owaDeviceId = deviceId;
			this.owaProtocol = protocol;
			this.owaDeviceType = deviceType;
			this.owaActionId = actionId;
			this.IsOutlookService = IsOutlookService;
			this.DuplicatedActionDetectionEnabled = true;
			CallContext.SetCurrent(this);
		}

		internal void DisableDupDetection()
		{
			this.DuplicatedActionDetectionEnabled = false;
			CallContext.SetCurrent(null);
		}

		internal static void SetServiceUnavailableForTransientErrorResponse(CallContext callContext)
		{
			if (callContext != null && callContext.IsTransientErrorResponse && callContext.DetectDuplicatedActions)
			{
				IOutgoingWebResponseContext outgoingWebResponseContext = callContext.CreateWebResponseContext();
				if (outgoingWebResponseContext != null && outgoingWebResponseContext.StatusCode == HttpStatusCode.OK)
				{
					outgoingWebResponseContext.StatusCode = HttpStatusCode.ServiceUnavailable;
					outgoingWebResponseContext.SuppressContent = true;
					outgoingWebResponseContext.Headers["X-OWA-TransientErrorResponse"] = true.ToString();
				}
			}
		}

		private static JsonFaultDetail CreateJsonFaultDetail(Exception exception)
		{
			JsonFaultDetail jsonFaultDetail = null;
			if (exception != null)
			{
				jsonFaultDetail = new JsonFaultDetail();
				jsonFaultDetail.Message = exception.Message;
				jsonFaultDetail.StackTrace = exception.StackTrace;
				jsonFaultDetail.ExceptionType = exception.GetType().FullName;
				jsonFaultDetail.ExceptionDetail = new ExceptionDetail(exception);
			}
			return jsonFaultDetail;
		}

		private void InitializeOwaFields()
		{
			this.mobileDevicePolicyId = this.httpContext.Request.Headers["X-OWA-MobileDevicePolicyId"];
			this.owaProtocol = this.httpContext.Request.Headers["X-OWA-Protocol"];
			this.owaDeviceId = this.httpContext.Request.Headers["X-OWA-DeviceId"];
			this.owaDeviceType = this.httpContext.Request.Headers["X-OWA-DeviceType"];
			this.owaActionId = this.httpContext.Request.Headers["X-OWA-ActionId"];
			this.IsSmimeInstalled = (this.httpContext.Request.Headers["X-OWA-SmimeInstalled"] == "1");
			this.isExplicitLogon = !string.IsNullOrEmpty(this.httpContext.Request.Headers["X-OWA-ExplicitLogonUser"]);
			this.IsOutlookService = false;
		}

		private void DisposeOwaFields()
		{
			if (this.DetectDuplicatedActions && !this.IsOutlookService)
			{
				try
				{
					this.httpContext.Response.Headers["X-OWA-DuplicatedAction"] = ((this.IsDuplicatedAction != null) ? this.IsDuplicatedAction.Value.ToString() : "null");
				}
				catch (HttpException)
				{
				}
			}
			if (this.owaActionQueueSyncState != null)
			{
				this.owaActionQueueSyncState.Dispose();
				this.owaActionQueueSyncState = null;
			}
			if (this.mowaSyncState != null)
			{
				this.mowaSyncState.Dispose();
				this.mowaSyncState = null;
			}
			if (this.owaSyncStateStorage != null)
			{
				this.owaSyncStateStorage.Dispose();
				this.owaSyncStateStorage = null;
			}
		}

		private GlobalInfo GetMowaSyncState()
		{
			if (this.mowaSyncState == null)
			{
				this.EnsureSyncStateStorageIsLoaded();
				try
				{
					this.mowaSyncState = GlobalInfo.LoadFromMailbox(this.SessionCache.GetMailboxIdentityMailboxSession(), this.owaSyncStateStorage, null);
				}
				catch (AirSyncPermanentException ex)
				{
					RequestDetailsLoggerBase<RequestDetailsLogger>.SafeLogRequestException(this.ProtocolLog, ex, "GetMowaSyncState");
				}
			}
			return this.mowaSyncState;
		}

		private void SaveMowaSyncState()
		{
			this.mowaSyncState.SaveToMailbox();
			this.mowaSyncState.Dispose();
			this.mowaSyncState = null;
		}

		private CallContext.OwaActionQueueState<T> GetOwaActionQueueState<T>()
		{
			if (this.owaActionQueueSyncState == null)
			{
				this.EnsureSyncStateStorageIsLoaded();
				CallContext.OwaActionQueueStateInfo syncStateInfo = new CallContext.OwaActionQueueStateInfo();
				try
				{
					this.owaActionQueueSyncState = this.owaSyncStateStorage.GetCustomSyncState(syncStateInfo, new PropertyDefinition[0]);
				}
				catch (CorruptSyncStateException exception)
				{
					RequestDetailsLogger.LogException(exception, "DuplicatedActionDetection is ignoring CorruptSyncStateException.", "GetOwaActionQueueState");
				}
				catch (CustomSerializationException exception2)
				{
					RequestDetailsLogger.LogException(exception2, "DuplicatedActionDetection is ignoring CustomSerializationException.", "GetOwaActionQueueState");
				}
				if (this.owaActionQueueSyncState == null)
				{
					this.owaActionQueueSyncState = this.owaSyncStateStorage.CreateCustomSyncState(syncStateInfo);
				}
			}
			return new CallContext.OwaActionQueueState<T>(this.owaActionQueueSyncState);
		}

		private void EnsureSyncStateStorageIsLoaded()
		{
			if (!this.TryLoadOwaSyncStateStorage())
			{
				if (!this.IsOutlookService)
				{
					this.httpContext.Response.Headers["X-OWA-InvalidDeviceId"] = this.owaDeviceId;
				}
				NonProvisionedException ex = new NonProvisionedException(this.IsMowa);
				throw new FaultException<JsonFaultDetail>(CallContext.CreateJsonFaultDetail(ex), new FaultReason(ex.Message));
			}
		}

		private bool TryLoadOwaSyncStateStorage()
		{
			if (this.owaSyncStateStorage == null)
			{
				this.owaSyncStateStorage = SyncStateStorage.Bind(this.SessionCache.GetMailboxIdentityMailboxSession(), new DeviceIdentity(this.owaDeviceId, this.owaDeviceType, this.owaProtocol), null);
			}
			return this.owaSyncStateStorage != null;
		}

		internal ManagementRoleType ManagementRole { get; set; }

		internal static CallContext CreateFromRequest(MessageHeaderProcessor headerProcessor, Message request)
		{
			return CallContext.CreateFromRequest(headerProcessor, request, false);
		}

		internal static CallContext CreateFromRequest(MessageHeaderProcessor headerProcessor, Message request, bool duplicatedActionDetectionEnabled)
		{
			CallContext callContext = null;
			ExternalIdentity externalIdentity = HttpContext.Current.User.Identity as ExternalIdentity;
			if (externalIdentity != null)
			{
				UserWorkloadManager userWorkloadManager = CallContext.GetWorkloadManager(HttpContext.Current.ApplicationInstance);
				callContext = new ExternalCallContext(headerProcessor, request, externalIdentity, userWorkloadManager);
			}
			else
			{
				callContext = CallContext.CreateCallContext(headerProcessor, request);
			}
			if (callContext != null && callContext.AccessingPrincipal != null)
			{
				if (callContext.AccessingPrincipal.MailboxInfo != null && callContext.AccessingPrincipal.MailboxInfo.OrganizationId != null && callContext.AccessingPrincipal.MailboxInfo.OrganizationId.OrganizationalUnit != null)
				{
					CallContext.PushOrgInfoToHttpContext(callContext.AccessingPrincipal.MailboxInfo.OrganizationId.OrganizationalUnit.Name);
				}
				else if (callContext.AccessingPrincipal.ObjectId != null && callContext.AccessingPrincipal.ObjectId.DomainId != null)
				{
					CallContext.PushOrgInfoToHttpContext(callContext.AccessingPrincipal.ObjectId.DomainId.ToCanonicalName());
				}
			}
			if (callContext != null)
			{
				callContext.DuplicatedActionDetectionEnabled = duplicatedActionDetectionEnabled;
			}
			CallContext.UpdateActivity(callContext);
			try
			{
				callContext.soapAction = request.Headers.Action;
				callContext.callerBudget = EwsBudget.Acquire(callContext);
				EwsBudgetWrapper ewsBudgetWrapper = callContext.callerBudget as EwsBudgetWrapper;
				if (ewsBudgetWrapper != null)
				{
					ewsBudgetWrapper.StartConnection(CallContext.GetBudgetDescription(callContext));
				}
				ExTraceGlobals.UtilAlgorithmTracer.TraceDebug((long)callContext.GetHashCode(), "[CallContext::CreateFromRequest] CallContext budget acquired.");
				if (request.Headers.MessageId != null)
				{
					callContext.messageId = request.Headers.MessageId.ToString();
				}
			}
			catch
			{
				callContext.Dispose();
				throw;
			}
			return callContext;
		}

		internal static CallContext CreateForOData(HttpContext httpContext, string impersonatedUser)
		{
			HttpContext.Current = httpContext;
			HttpApplication applicationInstance = httpContext.ApplicationInstance;
			AuthZClientInfo authZClientInfo = null;
			CallContext callContext = null;
			CallContext result;
			try
			{
				AuthZClientInfo authZClientInfo2;
				authZClientInfo = (authZClientInfo2 = CallContext.GetCallerClientInfo());
				MailboxAccessType arg = MailboxAccessType.Normal;
				string callerApplicationId = null;
				if (authZClientInfo is AuthZClientInfo.ApplicationAttachedAuthZClientInfo)
				{
					arg = MailboxAccessType.ApplicationAction;
					callerApplicationId = (authZClientInfo as AuthZClientInfo.ApplicationAttachedAuthZClientInfo).OAuthIdentity.OAuthApplication.Id;
				}
				if (!string.IsNullOrEmpty(impersonatedUser))
				{
					SmtpAddressImpersonationProcessor smtpAddressImpersonationProcessor = new SmtpAddressImpersonationProcessor(impersonatedUser, false, authZClientInfo, HttpContext.Current.User.Identity);
					authZClientInfo2 = smtpAddressImpersonationProcessor.CreateAuthZClientInfo();
				}
				ADRecipientSessionContext adrecipientSessionContext = authZClientInfo2.GetADRecipientSessionContext();
				ExchangePrincipal exchangePrincipal = null;
				if (!ExchangePrincipalCache.TryGetFromCache(authZClientInfo2.ClientSecurityContext.UserSid, adrecipientSessionContext, out exchangePrincipal))
				{
					ExTraceGlobals.ServerToServerAuthZTracer.TraceDebug<SecurityIdentifier, MailboxAccessType>(0L, "Exchange principal cache gave back a null principal. AuthenticatedUser: {0}, MailboxAccessType: {1}", authZClientInfo2.ClientSecurityContext.UserSid, arg);
				}
				AppWideStoreSessionCache mailboxSessionBackingCache = (AppWideStoreSessionCache)applicationInstance.Application["WS_APPWideMailboxCacheKey"];
				UserWorkloadManager userWorkloadManager = CallContext.GetWorkloadManager(applicationInstance);
				AcceptedDomainCache acceptedDomainCache = (AcceptedDomainCache)applicationInstance.Application["WS_AcceptedDomainCacheKey"];
				callContext = new CallContext(mailboxSessionBackingCache, acceptedDomainCache, userWorkloadManager, ProxyCASStatus.InitialCASOrNoProxy, authZClientInfo2, exchangePrincipal, adrecipientSessionContext, arg, null, MessageHeaderProcessor.GetExchangeServerCulture(), MessageHeaderProcessor.GetExchangeServerCulture(), httpContext.Request.UserAgent, false, CallContext.UserKind.Uncategorized, OriginalCallerContext.FromAuthZClientInfo(authZClientInfo), RequestedLogonType.Default, WebMethodEntry.JsonWebMethodEntry, AuthZBehavior.DefaultBehavior);
				callContext.CallerApplicationId = callerApplicationId;
				callContext.callerBudget = EwsBudget.Acquire(callContext);
				ExTraceGlobals.UtilAlgorithmTracer.TraceDebug((long)callContext.GetHashCode(), "[CallContext::CreateForOData] CallContext budget acquired.");
				CallContext.UpdateActivity(callContext);
				httpContext.Items["CallContext"] = callContext;
				if (authZClientInfo != authZClientInfo2)
				{
					CallContext.DisposeObject(authZClientInfo);
				}
				result = callContext;
			}
			catch (Exception ex)
			{
				CallContext.DisposeObject(authZClientInfo);
				CallContext.DisposeObject(callContext);
				LocalizedException ex2 = ex as LocalizedException;
				if (ex2 != null)
				{
					throw FaultExceptionUtilities.CreateFault(ex2, FaultParty.Sender);
				}
				throw;
			}
			return result;
		}

		internal static CallContext CreateForExchangeService(HttpContext httpContext, AppWideStoreSessionCache mailboxSessionCache, AcceptedDomainCache acceptedDomainCache, UserWorkloadManager workloadManager, IEwsBudget budget, CultureInfo clientCulture)
		{
			HttpContext.Current = httpContext;
			MSAIdentity msaIdentity;
			CallContext callContext;
			if (CompositeIdentityBuilder.TryGetMsaNoAdUserIdentity(HttpContext.Current, out msaIdentity))
			{
				callContext = new MSACallContext(httpContext, mailboxSessionCache, acceptedDomainCache, msaIdentity, workloadManager, clientCulture);
			}
			else
			{
				AuthZClientInfo callerClientInfo = CallContext.GetCallerClientInfo();
				ADRecipientSessionContext adrecipientSessionContext = callerClientInfo.GetADRecipientSessionContext();
				ExchangePrincipal exchangePrincipal = null;
				if (!ExchangePrincipalCache.TryGetFromCache(callerClientInfo.ClientSecurityContext.UserSid, adrecipientSessionContext, out exchangePrincipal))
				{
					ExTraceGlobals.ServerToServerAuthZTracer.TraceDebug<string>(0L, "Exchange principal cache gave back a null principal for user {0}:", httpContext.User.Identity.GetSafeName(true));
				}
				callContext = new CallContext(mailboxSessionCache, acceptedDomainCache, workloadManager, ProxyCASStatus.InitialCASOrNoProxy, callerClientInfo, exchangePrincipal, adrecipientSessionContext, MailboxAccessType.Normal, null, MessageHeaderProcessor.GetExchangeServerCulture(), clientCulture, httpContext.Request.UserAgent, false, CallContext.UserKind.Uncategorized, OriginalCallerContext.Empty, RequestedLogonType.Default, WebMethodEntry.JsonWebMethodEntry, AuthZBehavior.DefaultBehavior);
			}
			callContext.callerBudget = budget;
			ExTraceGlobals.CommonAlgorithmTracer.TraceDebug((long)callContext.GetHashCode(), "CallContext budget assigned.");
			httpContext.Items["CallContext"] = callContext;
			ExchangeVersion.Current = ExchangeVersion.Latest;
			return callContext;
		}

		internal static CallContext CreateFromInternalRequestContext(MessageHeaderProcessor headerProcessor, Message request, bool duplicatedActionDetectionEnabled, IEWSPartnerRequestContext partnerRequestContext)
		{
			CallContext result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				using (AuthZClientInfo authZClientInfo = CallContext.CreateTemporaryAuthZForInternalRequest(partnerRequestContext))
				{
					HttpContext httpContext = HttpContext.Current;
					HttpApplication applicationInstance = httpContext.ApplicationInstance;
					AppWideStoreSessionCache mailboxSessionBackingCache = (AppWideStoreSessionCache)applicationInstance.Application["WS_APPWideMailboxCacheKey"];
					AcceptedDomainCache acceptedDomainCache = (AcceptedDomainCache)applicationInstance.Application["WS_AcceptedDomainCacheKey"];
					CallContext callContext = null;
					if (!CallContext.IsSameADUser(authZClientInfo, partnerRequestContext.ExchangePrincipal))
					{
						ExTraceGlobals.CommonAlgorithmTracer.TraceDebug(0L, "The partner's effectiveCaller is not the created principal.  Falling back to CreateFromRequest.");
						callContext = CallContext.CreateFromRequest(headerProcessor, request, duplicatedActionDetectionEnabled);
						disposeGuard.Add<CallContext>(callContext);
					}
					else if (!CallContext.IsSidBasedAuthZClient(authZClientInfo))
					{
						ExTraceGlobals.CommonAlgorithmTracer.TraceDebug(0L, "The partner's authz is not SID based. Falling back to CreateFromRequest.");
						callContext = CallContext.CreateFromRequest(headerProcessor, request, duplicatedActionDetectionEnabled);
						disposeGuard.Add<CallContext>(callContext);
					}
					else
					{
						ExTraceGlobals.CommonAlgorithmTracer.TraceDebug(0L, "Pre-requisites for fast-path internal request context creation have been met.");
						ADRecipientSessionContext adrecipientSessionContext = authZClientInfo.GetADRecipientSessionContext();
						UserWorkloadManager userWorkloadManager = CallContext.GetWorkloadManager(applicationInstance);
						headerProcessor.ProcessMailboxCultureHeader(request);
						headerProcessor.ProcessTimeZoneContextHeader(request);
						headerProcessor.ProcessDateTimePrecisionHeader(request);
						bool isServiceUnavailableOnTransientError = headerProcessor.ProcessServiceUnavailableOnTransientErrorHeader(request);
						ManagementRoleType managementRoleType = headerProcessor.ProcessManagementRoleHeader(request);
						WebMethodEntry webMethodEntry = (WebMethodEntry)request.Properties["WebMethodEntry"];
						AuthZBehavior authZBehavior = null;
						CallContext.ValidateRBACPermissions(webMethodEntry, authZClientInfo, managementRoleType, ref authZBehavior);
						using (DisposeGuard disposeGuard2 = default(DisposeGuard))
						{
							AuthZClientInfo authZClientInfo2 = authZClientInfo;
							authZClientInfo2.AddRef();
							disposeGuard2.Add<AuthZClientInfo>(authZClientInfo2);
							RequestedLogonType mailboxLogonType = authZBehavior.GetMailboxLogonType();
							ExTraceGlobals.AuthenticationTracer.TraceDebug<RequestedLogonType>(0L, "[CallContext.CreateFromInternalRequestContext] MailboxLogonType {0}", mailboxLogonType);
							callContext = new CallContext(mailboxSessionBackingCache, acceptedDomainCache, userWorkloadManager, ProxyCASStatus.InitialCASOrNoProxy, authZClientInfo, partnerRequestContext.ExchangePrincipal, adrecipientSessionContext, MailboxAccessType.Normal, null, EWSSettings.ServerCulture, EWSSettings.ClientCulture, partnerRequestContext.UserAgent, false, CallContext.UserKind.Uncategorized, OriginalCallerContext.Empty, mailboxLogonType, webMethodEntry, authZBehavior);
							disposeGuard.Add<CallContext>(callContext);
							disposeGuard2.Success();
						}
						callContext.MethodName = CallContext.GetMethodName(request);
						callContext.IsRequestTracingEnabled = CallContext.GetServerTracingEnabledFlag(httpContext.Request);
						callContext.DuplicatedActionDetectionEnabled = duplicatedActionDetectionEnabled;
						callContext.IsServiceUnavailableOnTransientError = isServiceUnavailableOnTransientError;
						CallContext.PerformPostCallContextCreationSteps(httpContext, request, callContext);
					}
					disposeGuard.Success();
					result = callContext;
				}
			}
			return result;
		}

		private static AuthZClientInfo CreateTemporaryAuthZForInternalRequest(IEWSPartnerRequestContext partnerRequestContext)
		{
			AuthZClientInfo callerClientInfo;
			if (partnerRequestContext.CallerClientInfo != null)
			{
				callerClientInfo = partnerRequestContext.CallerClientInfo;
				callerClientInfo.AddRef();
			}
			else
			{
				callerClientInfo = CallContext.GetCallerClientInfo();
			}
			return callerClientInfo;
		}

		private static void UpdateActivity(CallContext callContext)
		{
			if (callContext != null && (callContext.ProxyCASStatus == ProxyCASStatus.DestinationCAS || callContext.proxyCASStatus == ProxyCASStatus.DestinationCASFromCAFE))
			{
				IActivityScope currentActivityScope = ActivityContext.GetCurrentActivityScope();
				if (currentActivityScope != null)
				{
					object obj = null;
					if (EwsOperationContextBase.Current != null && EwsOperationContextBase.Current.IncomingMessageProperties.TryGetValue(HttpRequestMessageProperty.Name, out obj))
					{
						HttpRequestMessageProperty wcfMessage = (HttpRequestMessageProperty)obj;
						currentActivityScope.UpdateFromMessage(wcfMessage);
					}
				}
			}
		}

		protected static string GetMethodName(Message request)
		{
			string result = string.Empty;
			object obj;
			if (!string.IsNullOrEmpty(request.Headers.Action))
			{
				int num = request.Headers.Action.LastIndexOf('/');
				if (num >= 0)
				{
					result = request.Headers.Action.Substring(num + 1);
				}
				else
				{
					result = request.Headers.Action;
				}
			}
			else if (request.Properties.TryGetValue("HttpOperationName", out obj) && obj is string)
			{
				result = (string)obj;
			}
			return result;
		}

		private static bool GetServerTracingEnabledFlag(HttpRequest request)
		{
			return request != null && request.QueryString != null && request.QueryString.AllKeys.Contains("trace", StringComparer.OrdinalIgnoreCase);
		}

		private static CallContext CreateCallContext(MessageHeaderProcessor headerProcessor, Message request)
		{
			HttpContext httpContext = HttpContext.Current;
			HttpApplication applicationInstance = httpContext.ApplicationInstance;
			AuthZClientInfo authZClientInfo = null;
			AuthZClientInfo authZClientInfo2 = null;
			AuthZClientInfo authZClientInfo3 = null;
			AuthZClientInfo authZClientInfo4 = null;
			AuthZClientInfo authZClientInfo5 = null;
			AuthZBehavior authZBehavior = null;
			ProxyRequestType? proxy = null;
			CallContext callContext = null;
			bool flag = false;
			CallContext.UserKind userKind = CallContext.UserKind.Uncategorized;
			string callerApplicationId = null;
			CallContext result;
			try
			{
				List<ClientStatistics> clientRequestStatistics = HttpHeaderProcessor.ProcessClientStatisticsHttpHeader(httpContext);
				headerProcessor.ProcessMailboxCultureHeader(request);
				proxy = headerProcessor.ProcessRequestTypeHeader(request);
				headerProcessor.ProcessTimeZoneContextHeader(request);
				headerProcessor.ProcessDateTimePrecisionHeader(request);
				bool flag2 = headerProcessor.ProcessBackgroundLoadHeader(request);
				bool isServiceUnavailableOnTransientError = headerProcessor.ProcessServiceUnavailableOnTransientErrorHeader(request);
				ManagementRoleType managementRoleType = headerProcessor.ProcessManagementRoleHeader(request);
				bool flag3 = EWSSettings.UpnFromClaimSets != null;
				authZClientInfo = CallContext.GetCallerClientInfo();
				bool flag4 = false;
				if (authZClientInfo != null && authZClientInfo.UserIdentity != null && authZClientInfo.UserIdentity.ADUser != null && authZClientInfo.UserIdentity.ADUser.Database != null && !string.Equals(TopologyProvider.LocalForestFqdn, authZClientInfo.UserIdentity.ADUser.Database.PartitionFQDN, StringComparison.OrdinalIgnoreCase))
				{
					flag4 = true;
				}
				bool flag5 = httpContext.Request.Headers[WellKnownHeader.XIsFromBackend] == Global.BooleanTrue;
				if (!flag4 && !flag5)
				{
					authZClientInfo2 = headerProcessor.ProcessProxyHeaders(request, authZClientInfo);
				}
				bool flag6 = flag5 && flag4;
				ProxyCASStatus proxyCASStatus;
				AuthZClientInfo authZClientInfo6;
				if (authZClientInfo2 != null)
				{
					proxyCASStatus = (flag5 ? ProxyCASStatus.DestinationCASFromCAFE : ProxyCASStatus.DestinationCAS);
					authZClientInfo6 = authZClientInfo2;
				}
				else
				{
					proxyCASStatus = ((!string.IsNullOrEmpty(httpContext.Request.UserAgent) && httpContext.Request.UserAgent.StartsWith("ExchangeWebServicesProxy/CrossSite/EXCH/")) ? (flag5 ? ProxyCASStatus.DestinationCASFromCAFE : ProxyCASStatus.DestinationCAS) : ProxyCASStatus.InitialCASOrNoProxy);
					authZClientInfo6 = authZClientInfo;
				}
				authZClientInfo3 = headerProcessor.ProcessSerializedSecurityContextHeaders(request);
				AuthZClientInfo authZClientInfo7;
				if (authZClientInfo3 != null)
				{
					authZClientInfo7 = authZClientInfo3;
				}
				else
				{
					LogonType logonType;
					OpenAsAdminOrSystemServiceBudgetTypeType openAsAdminOrSystemServiceBudgetTypeType;
					authZClientInfo5 = CallContext.GetPrivilegedUserClientInfo(request, headerProcessor, authZClientInfo6, out logonType, out openAsAdminOrSystemServiceBudgetTypeType);
					if (authZClientInfo5 != null)
					{
						RequestedLogonType logonType2 = (logonType == LogonType.Admin) ? RequestedLogonType.AdminFromOpenAsAdminOrSystemServiceHeader : RequestedLogonType.SystemServiceFromOpenAsAdminOrSystemServiceHeader;
						authZClientInfo7 = authZClientInfo5;
						flag2 = (openAsAdminOrSystemServiceBudgetTypeType == OpenAsAdminOrSystemServiceBudgetTypeType.RunAsBackgroundLoad);
						flag = (openAsAdminOrSystemServiceBudgetTypeType == OpenAsAdminOrSystemServiceBudgetTypeType.Unthrottled);
						authZBehavior = new AuthZBehavior.PrivilegedSessionAuthZBehavior(logonType2);
					}
					else
					{
						authZClientInfo4 = CallContext.GetImpersonatedClientInfo(request, headerProcessor, authZClientInfo2, authZClientInfo6);
						if (authZClientInfo4 != null)
						{
							RequestDetailsLogger.Current.ActivityScope.UserEmail = authZClientInfo4.PrimarySmtpAddress;
							authZClientInfo7 = authZClientInfo4;
						}
						else
						{
							authZClientInfo7 = authZClientInfo6;
						}
					}
				}
				MailboxAccessType arg = MailboxAccessType.Normal;
				if (authZClientInfo3 != null || authZClientInfo5 != null)
				{
					arg = MailboxAccessType.ServerToServer;
				}
				else if (authZClientInfo4 != null)
				{
					arg = MailboxAccessType.ExchangeImpersonation;
				}
				if (authZClientInfo6 is PartnerAuthZClientInfo)
				{
					userKind = CallContext.UserKind.Partner;
				}
				else if (authZClientInfo6 is AuthZClientInfo.ApplicationAttachedAuthZClientInfo)
				{
					userKind = CallContext.UserKind.OAuth;
					arg = MailboxAccessType.ApplicationAction;
					callerApplicationId = (authZClientInfo6 as AuthZClientInfo.ApplicationAttachedAuthZClientInfo).OAuthIdentity.OAuthApplication.Id;
				}
				WebMethodEntry method = (WebMethodEntry)request.Properties["WebMethodEntry"];
				CallContext.ValidateRBACPermissions(method, authZClientInfo7, managementRoleType, ref authZBehavior);
				RequestedLogonType mailboxLogonType = authZBehavior.GetMailboxLogonType();
				ADRecipientSessionContext adrecipientSessionContext = authZClientInfo7.GetADRecipientSessionContext();
				ExchangePrincipal exchangePrincipal = null;
				bool flag7 = false;
				if (authZClientInfo7.ClientSecurityContext == null)
				{
					ExTraceGlobals.AuthorizationTracer.TraceDebug(0L, "The effective caller is null (app only OAuth token)");
				}
				else if (!CallContext.SkipInitializationOfEffectiveCallerPrincipal(method, proxy) && !flag6)
				{
					if (CallContext.HasPublicFolderMailboxHeader())
					{
						flag7 = ExchangePrincipalCache.TryGetExchangePrincipalForHybridPublicFolderAccess(authZClientInfo7.ClientSecurityContext.UserSid, adrecipientSessionContext, out exchangePrincipal, false);
					}
					if (!flag7 && !ExchangePrincipalCache.TryGetFromCache(authZClientInfo7.ClientSecurityContext.UserSid, adrecipientSessionContext, out exchangePrincipal))
					{
						ExTraceGlobals.ServerToServerAuthZTracer.TraceDebug<SecurityIdentifier, MailboxAccessType>(0L, "Exchange principal cache gave back a null principal.  AuthenticatedUser: {0}, MailboxAccessType: {1}", authZClientInfo7.ClientSecurityContext.UserSid, arg);
					}
				}
				ExTraceGlobals.FaultInjectionTracer.TraceTest<ExchangePrincipal>(3789958461U, ref exchangePrincipal);
				AppWideStoreSessionCache mailboxSessionBackingCache;
				if (!headerProcessor.IsAvailabilityServiceS2S(request) && authZClientInfo7.ObjectGuid != Guid.Empty && CallContext.CanUserAgentUseBackingCache(httpContext.Request.UserAgent))
				{
					mailboxSessionBackingCache = (AppWideStoreSessionCache)applicationInstance.Application["WS_APPWideMailboxCacheKey"];
				}
				else
				{
					ExTraceGlobals.ServerToServerAuthZTracer.TraceDebug(0L, "CallContext.CreateContext, Bypassing the backing cache.");
					mailboxSessionBackingCache = null;
				}
				AcceptedDomainCache acceptedDomainCache = (AcceptedDomainCache)applicationInstance.Application["WS_AcceptedDomainCacheKey"];
				UserWorkloadManager userWorkloadManager = CallContext.GetWorkloadManager(applicationInstance);
				CultureInfo cultureInfo = EWSSettings.ClientCulture;
				CultureInfo cultureInfo2 = EWSSettings.ServerCulture;
				callContext = new CallContext(mailboxSessionBackingCache, acceptedDomainCache, userWorkloadManager, proxyCASStatus, authZClientInfo7, exchangePrincipal, adrecipientSessionContext, arg, proxy, cultureInfo2, cultureInfo, httpContext.Request.UserAgent, flag3, userKind, OriginalCallerContext.FromAuthZClientInfo(authZClientInfo6), mailboxLogonType, method, authZBehavior);
				callContext.MethodName = CallContext.GetMethodName(request);
				callContext.IsRequestTracingEnabled = CallContext.GetServerTracingEnabledFlag(httpContext.Request);
				callContext.ClientRequestStatistics = clientRequestStatistics;
				callContext.EncodeStringProperties = Global.EncodeStringProperties;
				callContext.BackgroundLoad = (flag2 || (Global.BackgroundLoadedTasksEnabled && Global.BackgroundLoadedTasks.Contains(callContext.MethodName)));
				callContext.IsServiceUnavailableOnTransientError = isServiceUnavailableOnTransientError;
				callContext.AllowUnthrottledBudget = flag;
				callContext.ManagementRole = managementRoleType;
				callContext.CallerApplicationId = callerApplicationId;
				callContext.IsRequestProxiedFromDifferentResourceForest = flag6;
				callContext.IsHybridPublicFolderAccessRequest = flag7;
				if (authZClientInfo6 != null && authZClientInfo6.PrimarySmtpAddress != null)
				{
					CallContext.PushUserInfoToHttpContext(callContext.HttpContext, authZClientInfo6.PrimarySmtpAddress);
				}
				HttpContext.Current.Items["CallContext"] = callContext;
				headerProcessor.ValidateRights(callContext, authZClientInfo6, request);
				if (authZClientInfo4 != authZClientInfo7)
				{
					CallContext.DisposeObject(authZClientInfo4);
				}
				if (authZClientInfo3 != authZClientInfo7)
				{
					CallContext.DisposeObject(authZClientInfo3);
				}
				if (authZClientInfo2 != authZClientInfo7)
				{
					CallContext.DisposeObject(authZClientInfo2);
				}
				if (authZClientInfo != authZClientInfo7)
				{
					CallContext.DisposeObject(authZClientInfo);
				}
				if (authZClientInfo5 != authZClientInfo7)
				{
					CallContext.DisposeObject(authZClientInfo5);
				}
				result = callContext;
			}
			catch (Exception ex)
			{
				CallContext.DisposeObject(authZClientInfo);
				CallContext.DisposeObject(authZClientInfo2);
				CallContext.DisposeObject(authZClientInfo3);
				CallContext.DisposeObject(authZClientInfo4);
				CallContext.DisposeObject(authZClientInfo5);
				CallContext.DisposeObject(callContext);
				LocalizedException ex2 = ex as LocalizedException;
				if (ex2 != null)
				{
					throw FaultExceptionUtilities.CreateFault(ex2, FaultParty.Sender);
				}
				throw;
			}
			return result;
		}

		internal static AuthZClientInfo GetCallerClientInfo()
		{
			IIdentity userIdentity = CompositeIdentityBuilder.GetUserIdentity(HttpContext.Current);
			ExTraceGlobals.AuthenticationTracer.TraceDebug<IIdentity>(0L, "[AuthZClientInfo.GetCallerClientInfo] Original calling identity is {0}", userIdentity);
			return AuthZClientInfo.ResolveIdentity(userIdentity);
		}

		internal static void PushOrgInfoToHttpContext(string org)
		{
			if (HttpContext.Current != null && !string.IsNullOrEmpty(org))
			{
				HttpContext.Current.Items["AuthenticatedUserOrganization"] = org;
			}
		}

		internal static void PushUserInfoToHttpContext(HttpContext httpContext, string userName)
		{
			if (httpContext != null && !string.IsNullOrEmpty(userName) && httpContext.Items["AuthenticatedUser"] == null)
			{
				httpContext.Items["AuthenticatedUser"] = userName;
			}
		}

		internal static void PushUserPuidToHttpContext()
		{
			if (HttpContext.Current != null)
			{
				using (AuthZClientInfo callerClientInfo = CallContext.GetCallerClientInfo())
				{
					if (callerClientInfo != null && callerClientInfo.UserIdentity != null && callerClientInfo.UserIdentity.ADUser != null && callerClientInfo.UserIdentity.ADUser.NetID != null)
					{
						HttpContext.Current.Items["PassportUniqueId"] = callerClientInfo.UserIdentity.ADUser.NetID.ToString();
					}
				}
			}
		}

		internal static UserWorkloadManager GetWorkloadManager(HttpApplication httpApplication)
		{
			return (UserWorkloadManager)httpApplication.Application["WS_WorkloadManagerKey"];
		}

		internal static AuthZClientInfo GetImpersonatedClientInfo(Message request, MessageHeaderProcessor headerProcessor, AuthZClientInfo proxyClientInfo, AuthZClientInfo originalCallerClientInfo)
		{
			return headerProcessor.ProcessImpersonationHeaders(request, proxyClientInfo, originalCallerClientInfo);
		}

		internal static AuthZClientInfo GetPrivilegedUserClientInfo(Message request, MessageHeaderProcessor headerProcessor, AuthZClientInfo originalCallerClientInfo, out LogonType logonType, out OpenAsAdminOrSystemServiceBudgetTypeType budgetType)
		{
			logonType = LogonType.BestAccess;
			budgetType = OpenAsAdminOrSystemServiceBudgetTypeType.Default;
			SpecialLogonType? specialLogonType;
			int? num;
			AuthZClientInfo authZClientInfo = headerProcessor.ProcessOpenAsAdminOrSystemServiceHeader(request, originalCallerClientInfo, out specialLogonType, out num);
			if (authZClientInfo != null)
			{
				logonType = ((specialLogonType == SpecialLogonType.Admin) ? LogonType.Admin : LogonType.SystemService);
				budgetType = (OpenAsAdminOrSystemServiceBudgetTypeType)num.Value;
			}
			return authZClientInfo;
		}

		internal static string GetBudgetDescription(CallContext callContext)
		{
			string text = "EwsBudgetWrapper.EwsBudgetWrapper";
			if (callContext.HttpContext != null && callContext.HttpContext.Request != null)
			{
				HttpRequest request = callContext.HttpContext.Request;
				NameValueCollection queryString = request.QueryString;
				if (queryString != null)
				{
					text = text + "." + queryString.ToString();
				}
				if (request.Headers != null && !string.IsNullOrEmpty(request.Headers["X-OWA-CorrelationId"]))
				{
					text = text + "." + request.Headers["X-OWA-CorrelationId"];
				}
			}
			return text;
		}

		private static void PerformPostCallContextCreationSteps(HttpContext httpContext, Message request, CallContext callContext)
		{
			CallContext.UpdateActivity(callContext);
			httpContext.Items["CallContext"] = callContext;
			callContext.soapAction = request.Headers.Action;
			callContext.callerBudget = EwsBudget.Acquire(callContext);
			ExTraceGlobals.UtilAlgorithmTracer.TraceDebug((long)callContext.GetHashCode(), "[CallContext::PerformPostCallContextCreationSteps] CallContext budget acquired.");
			if (request.Headers.MessageId != null)
			{
				callContext.messageId = request.Headers.MessageId.ToString();
			}
		}

		private static bool SkipInitializationOfEffectiveCallerPrincipal(WebMethodEntry method, ProxyRequestType? proxy)
		{
			string name;
			return method != null && proxy != null && ((name = method.Name) != null && (name == "GetUserPhoto" || name == "GetUserPhoto:GET"));
		}

		internal static bool HasPublicFolderMailboxHeader()
		{
			HttpContext httpContext = HttpContext.Current;
			return httpContext != null && httpContext.Request != null && httpContext.Request.Headers != null && RemotePublicFolderOperations.CheckPublicFolderMailboxHeader(httpContext.Request.Headers);
		}

		private bool CallerAccessAllowed(EwsApplicationAccessPolicy ewsApplicationAccessPolicy, MultiValuedProperty<string> ewsExceptions)
		{
			if (ewsApplicationAccessPolicy == EwsApplicationAccessPolicy.EnforceAllowList)
			{
				return ewsExceptions == null || ewsExceptions.Find(new Predicate<string>(this.MatchesUserAgent)) != null;
			}
			return ewsExceptions == null || ewsExceptions.Find(new Predicate<string>(this.MatchesUserAgent)) == null;
		}

		private static void ValidateRBACPermissions(WebMethodEntry webMethodEntry, AuthZClientInfo effectiveCallerClientInfo, ManagementRoleType managementRoleType, ref AuthZBehavior authZBehavior)
		{
			effectiveCallerClientInfo.ApplyManagementRole(managementRoleType, webMethodEntry);
			if (authZBehavior == null)
			{
				authZBehavior = effectiveCallerClientInfo.GetAuthZBehavior();
			}
			if (!authZBehavior.IsAllowedToCallWebMethod(webMethodEntry))
			{
				throw new ServiceAccessDeniedException((CoreResources.IDs)2554577046U);
			}
		}

		private static bool IsSameADUser(AuthZClientInfo clientInfo, ExchangePrincipal exchangePrincipal)
		{
			ADObjectId adobjectId = null;
			if (clientInfo != null && clientInfo.UserIdentity != null && clientInfo.UserIdentity.ADUser != null)
			{
				adobjectId = clientInfo.UserIdentity.ADUser.ObjectId;
			}
			ADObjectId adobjectId2 = null;
			if (exchangePrincipal != null)
			{
				adobjectId2 = exchangePrincipal.ObjectId;
			}
			return adobjectId != null && adobjectId2 != null && adobjectId.Equals(adobjectId2);
		}

		private static bool IsSidBasedAuthZClient(AuthZClientInfo clientInfo)
		{
			return clientInfo != null && clientInfo.ClientSecurityContext != null && null != clientInfo.ClientSecurityContext.UserSid;
		}

		private bool ShouldBlockLyncBadLiveIdTokenAccess()
		{
			if (string.IsNullOrEmpty(this.UserAgent) || !this.UserAgent.StartsWith(CallContext.BlockLyncBadLiveIdTokenUserAgentPrefix.Value) || !this.UserAgent.EndsWith(CallContext.BlockLyncBadLiveIdTokenUserAgentSuffix.Value))
			{
				return false;
			}
			if (!object.Equals(this.HttpContext.Items["AuthType"], "LiveIdToken"))
			{
				return false;
			}
			string text = HttpContext.Current.Items["LiveIdTokenSmtpClaim"] as string;
			if (!string.IsNullOrEmpty(text))
			{
				ProxyAddress proxyAddress = ProxyAddress.Parse(text);
				if (proxyAddress is InvalidProxyAddress)
				{
					this.ProtocolLog.AppendGenericInfo("BlockLyncBadLiveIdTokenAccess", "BadSMTPClaim");
					if (CallContext.BlockLyncBadLiveIdTokenEnabled.Value)
					{
						return true;
					}
				}
				else if (!this.AccessingADUser.EmailAddresses.Contains(proxyAddress))
				{
					this.ProtocolLog.AppendGenericInfo("BlockLyncBadLiveIdTokenAccess_SmtpClaimMismatch", this.AccessingADUser.PrimarySmtpAddress + "-" + text);
					if (CallContext.BlockLyncBadLiveIdTokenEnabled.Value)
					{
						return true;
					}
				}
			}
			string text2 = this.HttpContext.Request.Headers[WellKnownHeader.AnchorMailbox];
			if (string.IsNullOrEmpty(text2))
			{
				return false;
			}
			ProxyAddress proxyAddress2 = ProxyAddress.Parse(text2);
			if (proxyAddress2 is InvalidProxyAddress)
			{
				return false;
			}
			if (!this.AccessingADUser.EmailAddresses.Contains(proxyAddress2))
			{
				this.ProtocolLog.AppendGenericInfo("BlockLyncBadLiveIdTokenAccess_AnchorMailboxMismatch", this.AccessingADUser.PrimarySmtpAddress + "-" + text2);
				if (CallContext.BlockLyncBadLiveIdTokenEnabled.Value)
				{
					return true;
				}
			}
			return false;
		}

		private bool CallerAccessAllowed()
		{
			if (this.OriginalCallerContext == null || this.originalCallerContext.Sid == null || this.AccessingADUser == null)
			{
				return true;
			}
			try
			{
				if (this.ShouldBlockLyncBadLiveIdTokenAccess())
				{
					return false;
				}
			}
			catch (Exception ex)
			{
				RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericInfo(RequestDetailsLogger.Current, "LyncBadLiveIdCheckException", ex.Message + ex.StackTrace);
			}
			ADUser accessingADUser = this.AccessingADUser;
			Organization organization = OrganizationCache.Singleton.Get(accessingADUser.OrganizationId);
			if (VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).Ews.EwsClientAccessRulesEnabled.Enabled)
			{
				bool flag = ClientAccessRulesUtils.ShouldBlockConnection(accessingADUser.OrganizationId, ClientAccessRulesUtils.GetUsernameFromIdInformation(accessingADUser.WindowsLiveID, accessingADUser.MasterAccountSid, accessingADUser.Sid, accessingADUser.Id), ClientAccessProtocol.ExchangeWebServices, ClientAccessRulesUtils.GetRemoteEndPointFromContext(this.HttpContext), ClientAccessAuthenticationMethod.BasicAuthentication, accessingADUser, delegate(ClientAccessRulesEvaluationContext context)
				{
					RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericInfo(RequestDetailsLogger.Current, ClientAccessRulesConstants.ClientAccessRuleName, context.CurrentRule.Name);
				}, delegate(double latency)
				{
					if (latency > 50.0)
					{
						RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericInfo(RequestDetailsLogger.Current, ClientAccessRulesConstants.ClientAccessRulesLatency, latency);
					}
				});
				if (flag)
				{
					return false;
				}
			}
			bool? ewsEnabled = accessingADUser.EwsEnabled;
			if (ewsEnabled == null)
			{
				ewsEnabled = organization.EwsEnabled;
			}
			if (ewsEnabled == null)
			{
				return true;
			}
			if (ewsEnabled == false)
			{
				return false;
			}
			if (this.UserAgent != null && (this.UserAgent.Contains("Microsoft Office Outlook") || this.UserAgent.Contains("Microsoft Outlook")))
			{
				bool? ewsAllowOutlook = accessingADUser.EwsAllowOutlook;
				if (ewsAllowOutlook == null)
				{
					ewsAllowOutlook = organization.EwsAllowOutlook;
				}
				if (ewsAllowOutlook != null)
				{
					return ewsAllowOutlook.Value;
				}
			}
			else if (this.UserAgent != null && this.UserAgent.Contains("MacOutlook"))
			{
				bool? ewsAllowMacOutlook = accessingADUser.EwsAllowMacOutlook;
				if (ewsAllowMacOutlook == null)
				{
					ewsAllowMacOutlook = organization.EwsAllowMacOutlook;
				}
				if (ewsAllowMacOutlook != null)
				{
					return ewsAllowMacOutlook.Value;
				}
			}
			else if (this.UserAgent != null && this.UserAgent.Contains("Microsoft-Entourage"))
			{
				bool? ewsAllowEntourage = accessingADUser.EwsAllowEntourage;
				if (ewsAllowEntourage == null)
				{
					ewsAllowEntourage = organization.EwsAllowEntourage;
				}
				if (ewsAllowEntourage != null)
				{
					return ewsAllowEntourage.Value;
				}
			}
			EwsApplicationAccessPolicy? ewsApplicationAccessPolicy = accessingADUser.EwsApplicationAccessPolicy;
			if (ewsApplicationAccessPolicy != null)
			{
				MultiValuedProperty<string> ewsExceptions = accessingADUser.EwsExceptions;
				return this.CallerAccessAllowed(ewsApplicationAccessPolicy.Value, ewsExceptions);
			}
			ewsApplicationAccessPolicy = organization.EwsApplicationAccessPolicy;
			if (ewsApplicationAccessPolicy != null)
			{
				MultiValuedProperty<string> ewsExceptions2 = organization.EwsExceptions;
				return this.CallerAccessAllowed(ewsApplicationAccessPolicy.Value, ewsExceptions2);
			}
			return true;
		}

		internal bool CallerHasAccess()
		{
			bool result;
			try
			{
				DateTime utcNow = DateTime.UtcNow;
				bool flag = this.mailboxAccessType == MailboxAccessType.ServerToServer || this.MailboxAccessType == MailboxAccessType.ExchangeImpersonation || this.CallerAccessAllowed();
				double totalMilliseconds = (DateTime.UtcNow - utcNow).TotalMilliseconds;
				if (totalMilliseconds > 50.0)
				{
					RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericInfo(RequestDetailsLogger.Current, "CallerHasAccessLatency", totalMilliseconds);
				}
				result = flag;
			}
			catch (TenantOrgContainerNotFoundException arg)
			{
				ExTraceGlobals.CommonAlgorithmTracer.TraceError<TenantOrgContainerNotFoundException>(0L, "[CallContext::CallerHasAccess] encounters TenantOrgContainerNotFoundException {0}.", arg);
				result = false;
			}
			return result;
		}

		internal IOutgoingWebResponseContext CreateWebResponseContext()
		{
			if (this.UsingWcfDispatcher)
			{
				return new OutgoingWebResponseContextWrapper(WebOperationContext.Current.OutgoingResponse);
			}
			return new OutgoingHttpResponseContextWrapper(this.HttpContext.Response);
		}

		public string MessageId
		{
			get
			{
				return this.messageId;
			}
		}

		public string MethodName
		{
			get
			{
				return this.methodName;
			}
			protected set
			{
				this.methodName = value;
			}
		}

		internal virtual string EffectiveCallerNetId
		{
			get
			{
				if (this.EffectiveCaller == null || this.EffectiveCaller.UserIdentity == null || this.EffectiveCaller.UserIdentity.ADUser == null)
				{
					return null;
				}
				return Convert.ToString(this.EffectiveCaller.UserIdentity.ADUser.NetID);
			}
		}

		internal bool IsClientConnected
		{
			get
			{
				return this.IsWebSocketRequest || this.HttpContext.Response.IsClientConnected;
			}
		}

		public const string CallContextKey = "CallContext";

		public const string ProxiedRequestUserAgentPrefix = "ExchangeWebServicesProxy/CrossSite/EXCH/";

		private const string MonitoringClientUserAgentTag = "MSEXCHMON";

		private const string AccessingSmtpAddressKey = "AccessingSmtpAddress";

		private const string AccessingMailboxGuidKey = "AccessingMailboxGuid";

		private const string AccessingNetIdKey = "AccessingNetId";

		public const string ActionIdHttpHeaderKey = "X-OWA-ActionId";

		private const string TraceEnabledQueryStringParameter = "trace";

		[ThreadStatic]
		private static CallContext current;

		protected ProxyRequestType? availabilityProxyRequestType;

		protected MailboxAccessType mailboxAccessType;

		protected ExchangePrincipal effectiveCallerExchangePrincipal;

		protected AuthZBehavior authZBehavior;

		protected AuthZClientInfo effectiveCallerAuthZClientInfo;

		protected ADRecipientSessionContext adRecipientSessionContext;

		protected SessionCache sessionCache;

		protected UserWorkloadManager workloadManager;

		protected AcceptedDomainCache acceptedDomainCache;

		protected string userAgent;

		protected bool isDisposed;

		protected int disposerThreadId;

		protected IEwsBudget callerBudget;

		protected CallContext.UserKind userKind;

		protected RequestedLogonType requestedLogonType;

		private ProxyCASStatus proxyCASStatus;

		private bool isWSSecurityUser;

		private string soapAction;

		private object instanceLock = new object();

		private HttpContext httpContext;

		private EwsOperationContextBase operationContext;

		private RequestDetailsLogger protocolLog;

		private string description;

		private WebMethodEntry webMethodEntry;

		private IOwaCallback owaCallback;

		private OriginalCallerContext originalCallerContext = OriginalCallerContext.Empty;

		private bool backgroundLoad;

		private bool allowUnthrottledBudget;

		private WorkloadType workloadType = WorkloadType.Ews;

		private CultureInfo owaCulture;

		private CultureInfo previousThreadCulture;

		private CultureInfo previousThreadUICulture;

		private bool isRequestTracingEnabled;

		private string owaUserContextKey;

		private bool isWebSocketRequest;

		private MailboxLoggerHandler mailboxLogger;

		private bool usingWcfDispatcher = true;

		private DisposeTracker disposeTracker;

		private List<IDisposable> objectToDisposeList;

		protected CultureInfo serverCulture;

		protected CultureInfo clientCulture;

		private string mobileDevicePolicyId;

		private string owaProtocol;

		private string owaDeviceId;

		private string owaDeviceType;

		private string owaActionId;

		private CustomSyncState owaActionQueueSyncState;

		private SyncStateStorage owaSyncStateStorage;

		private GlobalInfo mowaSyncState;

		private bool isExplicitLogon;

		private string methodName;

		private string messageId;

		public static readonly BoolAppSettingsEntry BlockLyncBadLiveIdTokenEnabled = new BoolAppSettingsEntry("BlockLyncBadLiveIdTokenEnabled", false, null);

		public static readonly StringAppSettingsEntry BlockLyncBadLiveIdTokenUserAgentPrefix = new StringAppSettingsEntry("BlockLyncBadLiveIdTokenUserAgentPrefix", "OC", null);

		public static readonly StringAppSettingsEntry BlockLyncBadLiveIdTokenUserAgentSuffix = new StringAppSettingsEntry("BlockLyncBadLiveIdTokenUserAgentSuffix", "Lync)", null);

		internal enum UserKind
		{
			Uncategorized,
			External,
			Partner,
			OAuth,
			MSA
		}

		private struct OwaActionQueueState<T>
		{
			public OwaActionQueueState(CustomSyncState state)
			{
				this.state = state;
			}

			public string LastActionId
			{
				get
				{
					StringData stringData = (StringData)this.state["LastActionId"];
					if (stringData == null)
					{
						return null;
					}
					return stringData.Data;
				}
				set
				{
					this.state["LastActionId"] = new StringData(value);
				}
			}

			public T LastActionResults
			{
				get
				{
					return this.GetObject<T>("LastActionResults");
				}
				set
				{
					this.SetObject<T>("LastActionResults", value);
					if (value != null)
					{
						this.EnsureLastActionResultsCanBeDeserialized();
					}
				}
			}

			public JsonFaultDetail LastActionError
			{
				get
				{
					return this.GetObject<JsonFaultDetail>("LastActionException");
				}
				set
				{
					this.SetObject<JsonFaultDetail>("LastActionException", value);
				}
			}

			private static string Serialize<D>(D data)
			{
				DataContractJsonSerializer dataContractJsonSerializer = new DataContractJsonSerializer(typeof(D));
				string result;
				using (MemoryStream memoryStream = new MemoryStream())
				{
					dataContractJsonSerializer.WriteObject(memoryStream, data);
					memoryStream.Seek(0L, SeekOrigin.Begin);
					using (StreamReader streamReader = new StreamReader(memoryStream))
					{
						result = streamReader.ReadToEnd();
					}
				}
				return result;
			}

			private static D Deserialize<D>(string data)
			{
				DataContractJsonSerializer dataContractJsonSerializer = new DataContractJsonSerializer(typeof(D));
				D result;
				using (MemoryStream memoryStream = new MemoryStream(Encoding.Unicode.GetBytes(data)))
				{
					object obj = dataContractJsonSerializer.ReadObject(memoryStream);
					result = (D)((object)obj);
				}
				return result;
			}

			private void SetObject<D>(string key, D data)
			{
				string data2 = CallContext.OwaActionQueueState<T>.Serialize<D>(data);
				this.state[key] = new StringData(data2);
			}

			private D GetObject<D>(string key)
			{
				StringData stringData = (StringData)this.state[key];
				if (stringData == null)
				{
					return default(D);
				}
				D result;
				try
				{
					result = CallContext.OwaActionQueueState<T>.Deserialize<D>(stringData.Data);
				}
				catch (SerializationException ex)
				{
					throw new CustomSerializationException(CoreResources.ActionQueueDeserializationError(key, stringData.Data, typeof(D).FullName, ex.Message), ex);
				}
				return result;
			}

			private void EnsureLastActionResultsCanBeDeserialized()
			{
				T lastActionResults = this.LastActionResults;
			}

			private CustomSyncState state;

			private static class PropertyNames
			{
				public const string LastActionId = "LastActionId";

				public const string LastActionResults = "LastActionResults";

				public const string LastActionError = "LastActionException";
			}
		}

		private class OwaActionQueueStateInfo : SyncStateInfo
		{
			public override string UniqueName
			{
				get
				{
					return "ActionQueue";
				}
				set
				{
					throw new InvalidOperationException();
				}
			}

			public override int Version
			{
				get
				{
					return 1;
				}
			}
		}
	}
}
