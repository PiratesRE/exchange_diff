using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Configuration;
using System.Xml;
using Microsoft.Exchange.Data.ApplicationLogic;
using Microsoft.Exchange.Data.ApplicationLogic.Extension;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Diagnostics.FaultInjection;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Net.Protocols;
using Microsoft.Exchange.Security;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.Services.Core;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.EventLogs;
using Microsoft.Exchange.Services.OData;
using Microsoft.Exchange.Services.Wcf;
using Microsoft.Exchange.UM.ClientAccess;
using Microsoft.Exchange.UM.UMCommon.MessageContent;
using Microsoft.Exchange.WorkloadManagement;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Services
{
	public class Global : HttpApplication
	{
		public static IUnifiedContactStoreConfiguration UnifiedContactStoreConfiguration
		{
			get
			{
				return Global.unifiedContactStoreConfiguration;
			}
		}

		public static int FindCountLimit
		{
			get
			{
				return Global.findCountLimit;
			}
		}

		public static int HangingConnectionLimit
		{
			get
			{
				return Global.hangingConnectionLimit;
			}
		}

		public static bool? EnableSchemaValidationOverride
		{
			get
			{
				return Global.enableSchemaValidationOverride;
			}
		}

		public static bool UseGcCollect
		{
			get
			{
				return Global.useGcCollect;
			}
		}

		public static int CreateItemRequestSizeThreshold
		{
			get
			{
				return Global.createItemRequestSizeThreshold;
			}
		}

		public static int CollectIntervalInMilliseconds
		{
			get
			{
				return Global.collectIntervalInMilliseconds;
			}
		}

		public static int PrivateWorkingSetThreshold
		{
			get
			{
				return Global.privateWorkingSetThreshold;
			}
		}

		public static int AccessingPrincipalCacheSize
		{
			get
			{
				return Global.accessingPrincipalCacheSize;
			}
		}

		public static bool AllowCommandOptimization(string name)
		{
			return !Global.disableAllCommandOptimizations && !Global.disableCommandOptimizationSet.Contains(name.ToLowerInvariant());
		}

		public static int MaxAttachmentNestLevel
		{
			get
			{
				return Global.maxAttachmentNestLevel;
			}
		}

		public static bool EncodeStringProperties
		{
			get
			{
				return Global.GetAppSettingAsBool("EncodeStringProperties", false);
			}
		}

		public static int SearchTimeoutInMilliseconds
		{
			get
			{
				int result = Global.searchTimeoutInMilliseconds;
				ExTraceGlobals.FaultInjectionTracer.TraceTest<int>(3200658749U, ref result);
				return result;
			}
		}

		public static int MaximumTemporaryFilteredViewPerUser
		{
			get
			{
				return Global.maximumTemporaryFilteredViewPerUser;
			}
		}

		internal static int OrganizationWideAccessPolicyTimeoutInSeconds
		{
			get
			{
				return Global.organizationWideAccessPolicyTimeoutInSeconds;
			}
		}

		internal static int ExchangePrincipalCacheTimeoutInMinutes
		{
			get
			{
				return Global.exchangePrincipalCacheTimeoutInMinutes;
			}
		}

		internal static int ExchangePrincipalCacheTimeoutInSecondsOnError
		{
			get
			{
				return Global.exchangePrincipalCacheTimeoutInSecondsOnError;
			}
		}

		public static bool WriteStackTraceOnISE
		{
			get
			{
				return Global.writeStackTraceOnISE;
			}
		}

		public static string JunkMailReportingAddress
		{
			get
			{
				return Global.junkMailReportingAddress;
			}
		}

		public static string NotJunkMailReportingAddress
		{
			get
			{
				return Global.notJunkMailReportingAddress;
			}
		}

		public static bool SendXBEServerExceptionHeaderToCafe
		{
			get
			{
				return Global.sendXBEServerExceptionHeaderToCafe;
			}
		}

		public static bool EnableMailboxLogger
		{
			get
			{
				return Global.enableMailboxLogger;
			}
		}

		public static bool SendXBEServerExceptionHeaderToCafeOnFailover { get; set; }

		public static bool FastParticipantResolveEnabled { get; private set; }

		public static bool EnableFaultInjection { get; private set; }

		public static List<Dictionary<string, string>> FaultsList { get; private set; }

		public static HashSet<string> WriteStackTraceToProtocolLogForErrorTypes
		{
			get
			{
				return Global.writeStackTraceToProtocolLogForErrorTypes;
			}
		}

		public static bool WriteThrottlingDiagnostics
		{
			get
			{
				return Global.writeThrottlingDiagnostics;
			}
		}

		public static int GetAttachmentSizeLimit
		{
			get
			{
				int num = Global.MaxMaxRequestSizeForEWS.Member;
				if (num == 0)
				{
					num = Global.getAttachmentSizeLimit;
				}
				int num2 = 0;
				ExTraceGlobals.FaultInjectionTracer.TraceTest<int>(2659593533U, ref num2);
				if (num2 != 0)
				{
					num = num2;
				}
				return num;
			}
		}

		internal static IResponseShapeResolver ResponseShapeResolver
		{
			get
			{
				return Global.responseShapeResolver;
			}
			set
			{
				Global.responseShapeResolver = value;
			}
		}

		internal static EwsClientMailboxSessionCloningHandler EwsClientMailboxSessionCloningHandler
		{
			get
			{
				return Global.ewsClientMailboxSessionCloningHandler;
			}
			set
			{
				Global.ewsClientMailboxSessionCloningHandler = value;
			}
		}

		internal static string DefaultMapiClientType
		{
			get
			{
				return Global.defaultMapiClientType;
			}
			set
			{
				Global.defaultMapiClientType = value;
				ExTraceGlobals.CommonAlgorithmTracer.TraceDebug<string>(0L, "Default MAPI client type set to {0}", Global.defaultMapiClientType);
			}
		}

		internal static bool SafeHtmlLoaded
		{
			get
			{
				return Global.safeHtmlLoaded;
			}
			set
			{
				Global.safeHtmlLoaded = value;
			}
		}

		internal static bool ODataStackTraceInErrorResponse
		{
			get
			{
				return false || Global.oDataStackTraceInErrorResponse;
			}
		}

		internal static int GetAppSettingAsInt(string key, int defaultValue)
		{
			string s = ConfigurationManager.AppSettings[key];
			int result;
			if (int.TryParse(s, out result))
			{
				return result;
			}
			return defaultValue;
		}

		internal static bool GetAppSettingAsBool(string key, bool defaultValue)
		{
			string value = ConfigurationManager.AppSettings[key];
			bool result;
			if (!string.IsNullOrEmpty(value) && bool.TryParse(value, out result))
			{
				return result;
			}
			return defaultValue;
		}

		private static MailboxSession DefaultEwsClientMailboxSessionCloningHandler(Guid mailboxGuid, CultureInfo userCulture, LogonType logonType, string userContextKey, ExchangePrincipal mailboxToAccess, IADOrgPerson person, ClientSecurityContext callerSecurityContext, GenericIdentity genericIdentity, bool unifiedLogon)
		{
			return null;
		}

		private static void InitializeThrottlingPerfCounters()
		{
			int appSettingAsInt = Global.GetAppSettingAsInt("MassUserOverBudgetPercent", 0);
			if (appSettingAsInt <= 0 || appSettingAsInt > 100)
			{
				ExTraceGlobals.ThrottlingTracer.TraceError<int>(0L, "[Global::InitializeThrottlingPerfCounters] Invalid MassUserOverBudgetPercent value in web.config: {0}.  Must be >0 and <=100.", appSettingAsInt);
			}
			ThrottlingPerfCounterWrapper.Initialize(BudgetType.Ews, (appSettingAsInt > 0 && appSettingAsInt <= 100) ? new int?(appSettingAsInt) : null);
			ResourceHealthMonitorManager.Initialize(ResourceHealthComponent.EWS);
		}

		private static void InitializeProxyTimeout()
		{
			Global.proxyTimeout = Global.GetAppSettingAsInt("ProxyTimeout", Global.defaultProxyTimeout);
			if (Global.proxyTimeout < Global.minimumProxyTimeout)
			{
				Global.proxyTimeout = Global.minimumProxyTimeout;
			}
		}

		private static void InitializeSearchTimeout()
		{
			Global.searchTimeoutInMilliseconds = Global.GetAppSettingAsInt("SearchTimeoutInMilliseconds", 60000);
			if (Global.searchTimeoutInMilliseconds < Global.minimumSearchTimeout && Global.searchTimeoutInMilliseconds != -1)
			{
				Global.searchTimeoutInMilliseconds = Global.minimumSearchTimeout;
			}
		}

		private static void InitializeMaximumTemporaryFilteredViewPerUser()
		{
			int num = Global.GetAppSettingAsInt("MaximumTemporaryFilteredViewPerUser", 20);
			if (num < Global.minimumTemporaryFilteredViewPerUser)
			{
				num = Global.minimumTemporaryFilteredViewPerUser;
			}
			else if (num > 20)
			{
				num = 20;
			}
			Global.maximumTemporaryFilteredViewPerUser = num;
		}

		private static void InitializeEventQueuePollingInterval()
		{
			int num = Global.GetAppSettingAsInt("EventQueuePollingInterval", 5);
			if (num < 1)
			{
				ExTraceGlobals.ThrottlingTracer.TraceError<int, int>(0L, "[Global::InitializeEventQueuePollingInterval] Invalid EventQueuePollingInterval value in web.config: {0}.  Must be >= {1}.", num, 1);
				num = 1;
			}
			else if (num > 60)
			{
				ExTraceGlobals.ThrottlingTracer.TraceError<int, int>(0L, "[Global::InitializeEventQueuePollingInterval] Invalid EventQueuePollingInterval value in web.config: {0}.  Must be <= {1}.", num, 60);
				num = 60;
			}
			Global.eventQueuePollingInterval = num;
		}

		private static void InitializeAccessPolicyTimeout()
		{
			Global.organizationWideAccessPolicyTimeoutInSeconds = Global.GetAppSettingAsInt("OrganizationWideAccessPolicyTimeoutInSeconds", 10800);
			if (Global.organizationWideAccessPolicyTimeoutInSeconds < 0)
			{
				Global.organizationWideAccessPolicyTimeoutInSeconds = 0;
			}
		}

		private static UserWorkloadManager CreateWorkloadManager()
		{
			int workloadManagerMaxThreadCount = Global.GetWorkloadManagerMaxThreadCount();
			int workloadManagerMaxTasksQueued = Global.GetWorkloadManagerMaxTasksQueued();
			int appSettingAsInt = Global.GetAppSettingAsInt("DelayTimeThreshold", 0);
			if (appSettingAsInt <= 0)
			{
				ExTraceGlobals.ThrottlingTracer.TraceError<int>(0L, "[Global::InitializeThrottlingPerfCounters] Invalid DelayTimeThreshold value in web.config :{0}.  Must be > 0.", appSettingAsInt);
			}
			UserWorkloadManager.Initialize(workloadManagerMaxThreadCount, workloadManagerMaxTasksQueued, workloadManagerMaxTasksQueued, TimeSpan.FromMinutes(5.0), (appSettingAsInt > 0) ? new int?(appSettingAsInt) : null);
			return UserWorkloadManager.Singleton;
		}

		private static int GetWorkloadManagerMaxThreadCount()
		{
			int appSettingAsInt = Global.GetAppSettingAsInt("MaxWorkerThreadsPerProcessor", Global.defaultMaxWorkerThreadsPerProcessor);
			int processorCount = Environment.ProcessorCount;
			if (appSettingAsInt < Global.minimumMaxWorkerThreadsPerProcessor)
			{
				ExTraceGlobals.CommonAlgorithmTracer.TraceError<int, int>(0L, "[Global.GetWorkloadManagerMaxThreadCount Invalid MaxWorkerThreadsPerProcessor value in web.config: {0}.  Must be >= {1}.", appSettingAsInt, Global.minimumMaxWorkerThreadsPerProcessor);
				appSettingAsInt = Global.minimumMaxWorkerThreadsPerProcessor;
			}
			int num = appSettingAsInt * processorCount;
			int num2 = UserWorkloadManager.GetAvailableThreads() / 2;
			int num3 = Global.defaultMaxWorkerThreadsPerProcessor * processorCount;
			int num4 = Math.Max(num2, num3);
			if (num > num4)
			{
				ExTraceGlobals.CommonAlgorithmTracer.TraceError<int, int, int>(0L, "Global.GetWorkloadManagerMaxThreadCount Invalid MaxWorkerThreadsPerProcessor value in web.config: {0}. Product of maximum worker threads and processor count ({1}) must be <= available threads ceiling ({2}).", appSettingAsInt, processorCount, num4);
				num = num4;
			}
			ExTraceGlobals.CommonAlgorithmTracer.TraceDebug(0L, "maxThreadCount: {0}. availableThreads: {1}. defaultMaxThreadCount: {2}. processor count: {3}.", new object[]
			{
				num,
				num2,
				num3,
				processorCount
			});
			return num;
		}

		private static int GetWorkloadManagerMaxTasksQueued()
		{
			int appSettingAsInt = Global.GetAppSettingAsInt("MaxTasksQueued", Global.defaultMaxTasksQueued);
			if (appSettingAsInt < Global.minimumMaxTasksQueued)
			{
				ExTraceGlobals.CommonAlgorithmTracer.TraceError<int, int>(0L, "Global.GetWorkloadManagerMaxTasksQueued Invalid MaxTasksQueued value in web.config: {0}.  Must be >= {1}.", appSettingAsInt, Global.minimumMaxTasksQueued);
				appSettingAsInt = Global.minimumMaxTasksQueued;
			}
			else if (appSettingAsInt > Global.maximumMaxTasksQueued)
			{
				ExTraceGlobals.CommonAlgorithmTracer.TraceError<int, int>(0L, "Global.GetWorkloadManagerMaxTasksQueued Invalid MaxTasksQueued value in web.config: {0}.  Must be <= {1}.", appSettingAsInt, Global.maximumMaxTasksQueued);
				appSettingAsInt = Global.maximumMaxTasksQueued;
			}
			return appSettingAsInt;
		}

		private void Application_BeginRequest(object sender, EventArgs e)
		{
			RequestDetailsLoggerBase<RequestDetailsLogger>.InitializeRequestLogger();
			RequestDetailsLogger.Current.ActivityScope.UpdateFromMessage(HttpContext.Current.Request);
			RequestDetailsLogger.Current.ActivityScope.SerializeTo(HttpContext.Current.Response);
			NameValueCollection headers = HttpContext.Current.Request.Headers;
			if (headers != null)
			{
				string text = headers.Get("X-EWS-TargetVersion");
				RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendColumn(RequestDetailsLogger.Current, EwsMetadata.VersionInfo, "Target", string.IsNullOrEmpty(text) ? "None" : text);
				if (headers.Get("SOAPAction") != null && headers.Get("Authorization") != null && headers.Get("Authorization").StartsWith("Negotiate") && headers.Get("SOAPAction").ToLowerInvariant().Contains("wssecurity") && HttpContext.Current.Request.Url.PathAndQuery.ToLowerInvariant().Contains("wssecurity"))
				{
					string userAgent = HttpContext.Current.Request.UserAgent;
					ExTraceGlobals.CommonAlgorithmTracer.TraceError<string>(0L, "Bad request from {0}. Returning BadRequest to workaround WCF crash.", userAgent);
					Global.SetHttpResponse(HttpContext.Current, HttpStatusCode.BadRequest);
				}
			}
			string sourceCafeServer = CafeHelper.GetSourceCafeServer(HttpContext.Current.Request);
			if (!string.IsNullOrEmpty(sourceCafeServer))
			{
				RequestDetailsLogger.Current.Set(EwsMetadata.FrontEndServer, sourceCafeServer);
			}
			UserWorkloadManager userWorkloadManager = (UserWorkloadManager)base.Application["WS_WorkloadManagerKey"];
			if (UserWorkloadManager.Singleton.IsQueueFull)
			{
				ExTraceGlobals.CommonAlgorithmTracer.TraceError(0L, "Global.Application_BeginRequest WorkloadManager queue is full. Returning ServiceUnavailable.");
				PerformanceMonitor.UpdateTotalRequestRejectionsCount();
				Global.SetHttpResponse(HttpContext.Current, HttpStatusCode.ServiceUnavailable);
			}
			Stopwatch stopwatch = new Stopwatch();
			HttpContext.Current.Items["ServicesStopwatch"] = stopwatch;
			stopwatch.Start();
			if (Global.issueEwsCookie)
			{
				HttpResponse response = HttpContext.Current.Response;
				HttpRequest request = HttpContext.Current.Request;
				if (request.Cookies["exchangecookie"] == null)
				{
					HttpCookie httpCookie = new HttpCookie("exchangecookie");
					httpCookie.Expires = (DateTime)ExDateTime.Now.AddYears(1);
					httpCookie.HttpOnly = true;
					httpCookie.Path = "/";
					httpCookie.Value = Guid.NewGuid().ToString("N");
					response.Cookies.Add(httpCookie);
					return;
				}
				HttpCookie httpCookie2 = request.Cookies["exchangecookie"];
				if (httpCookie2.Value.Length == 32)
				{
					response.Cookies.Add(httpCookie2);
				}
			}
		}

		private void Application_EndRequest(object sender, EventArgs e)
		{
			try
			{
				if (base.Response.StatusCode == 404 && base.Response.SubStatusCode == 13)
				{
					base.Response.StatusCode = 507;
				}
				double wcfDispatchLatency = EWSSettings.WcfDispatchLatency;
				if (wcfDispatchLatency > 100.0)
				{
					RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericInfo(RequestDetailsLogger.Current, "WcfLatency", wcfDispatchLatency);
				}
				Stopwatch stopwatch = HttpContext.Current.Items["ServicesStopwatch"] as Stopwatch;
				if (stopwatch != null)
				{
					stopwatch.Stop();
					long elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
					PerformanceMonitor.UpdateResponseTimePerformanceCounter(elapsedMilliseconds);
				}
			}
			finally
			{
				Global.CleanUpRequestObjects();
			}
		}

		internal static void CleanUpRequestObjects()
		{
			RequestDetailsLogger requestDetailsLogger = RequestDetailsLoggerBase<RequestDetailsLogger>.Current;
			try
			{
				int num = (int)EwsCpuBasedSleeper.Singleton.GetDelayTime();
				if (num > 0)
				{
					requestDetailsLogger.AppendGenericInfo("Delay", num);
				}
				try
				{
					Global.DisposeServiceCommand();
					Global.DisposeCallContext();
					Global.DisposeRequestMessage();
				}
				finally
				{
					Global.ResetServerVersion();
				}
				try
				{
					if (requestDetailsLogger != null)
					{
						ActivityContext.SetThreadScope(requestDetailsLogger.ActivityScope);
					}
				}
				finally
				{
					if (requestDetailsLogger != null && !requestDetailsLogger.IsDisposed)
					{
						requestDetailsLogger.Commit();
					}
					if (num > 0 && HttpContext.Current != null && HttpContext.Current.Response != null && HttpContext.Current.Response.IsClientConnected)
					{
						Thread.Sleep(num);
					}
				}
			}
			finally
			{
				BufferedRegionStream bufferedRegionStream = EWSSettings.MessageStream as BufferedRegionStream;
				if (bufferedRegionStream != null)
				{
					bufferedRegionStream.Dispose();
					EWSSettings.MessageStream = null;
				}
			}
		}

		private static void DisposeRequestMessage()
		{
			Message message = HttpContext.Current.Items["EwsHttpContextMessage"] as Message;
			if (message != null)
			{
				message.Close();
				HttpContext.Current.Items.Remove("EwsHttpContextMessage");
			}
		}

		internal static void DisposeCallContext()
		{
			CallContext callContext = HttpContext.Current.Items["CallContext"] as CallContext;
			if (callContext != null)
			{
				callContext.Dispose();
				HttpContext.Current.Items["CallContext"] = null;
			}
		}

		internal static void ResetServerVersion()
		{
			HttpContext.Current.Items["WS_ServerVersionKey"] = null;
			ExchangeVersion.Current = null;
		}

		internal static void DisposeServiceCommand()
		{
			ServiceCommandBase serviceCommandBase = HttpContext.Current.Items["WS_ServiceCommandKey"] as ServiceCommandBase;
			if (serviceCommandBase != null)
			{
				IDictionary items = HttpContext.Current.Items;
				if (items.Contains("WS_ServiceProviderRequestIdKey"))
				{
					Global.TraceCasStop(serviceCommandBase.GetType(), serviceCommandBase.CallContext, (Guid)items["WS_ServiceProviderRequestIdKey"]);
				}
				IDisposable disposable = serviceCommandBase as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
				HttpContext.Current.Items["WS_ServiceCommandKey"] = null;
			}
		}

		private void InitializeIdentityCache()
		{
			int num = Global.GetAppSettingAsInt("IdentityCacheSizeLimit", 4000);
			if (num <= 0)
			{
				num = 4000;
			}
			int appSettingAsInt = Global.GetAppSettingAsInt("PerUserAccessPolicyTimeoutInSeconds", -1);
			if (appSettingAsInt < 0)
			{
				ADIdentityInformationCache.Initialize(num);
				return;
			}
			TimeSpan absoluteTimeout = TimeSpan.FromSeconds((double)appSettingAsInt);
			TimeSpan slidingTimeout = TimeSpan.FromSeconds((double)(appSettingAsInt / 2));
			ADIdentityInformationCache.Initialize(num, absoluteTimeout, slidingTimeout);
		}

		private void Application_Start(object sender, EventArgs e)
		{
			string[] privilegesToKeep = new string[]
			{
				"SeAssignPrimaryTokenPrivilege",
				"SeAuditPrivilege",
				"SeChangeNotifyPrivilege",
				"SeCreateGlobalPrivilege",
				"SeImpersonatePrivilege",
				"SeIncreaseQuotaPrivilege",
				"SeTcbPrivilege"
			};
			int num = Privileges.RemoveAllExcept(privilegesToKeep, "MSExchangeServicesAppPool");
			if (num != 0)
			{
				Environment.Exit(num);
			}
			this.InitializeWatsonReporting();
			ODataConfig.Initialize();
			int appSettingAsInt = Global.GetAppSettingAsInt("AppSettingCertificateExpirationCheckerIntervalInMinutes", 4320);
			ExchangeCertificateChecker.Initialize(appSettingAsInt);
			this.InitializeIdentityCache();
			Global.InitializeEventQueuePollingInterval();
			Globals.InitializeMultiPerfCounterInstance("EWS");
			ExchangeDiagnosticsHelper.RegisterDiagnosticsComponents();
			Global.InitializeThrottlingPerfCounters();
			PerformanceMonitor.Initialize();
			Subscriptions.Initialize();
			base.Application["WS_APPWideMailboxCacheKey"] = new AppWideStoreSessionCache();
			base.Application["WS_AcceptedDomainCacheKey"] = new AcceptedDomainCache();
			UserWorkloadManager userWorkloadManager = Global.CreateWorkloadManager();
			base.Application["WS_WorkloadManagerKey"] = userWorkloadManager;
			ShutdownHandler.Singleton.Add(userWorkloadManager);
			Global.InitializeSettingsFromWebConfig();
			Global.InitializeProxyTimeout();
			Global.InitializeSearchTimeout();
			Global.InitializeAccessPolicyTimeout();
			Global.writeRequestDetailsToLog = Global.GetAppSettingAsBool("WriteRequestsToLog", true);
			ExRpcModule.Bind();
			StoreSession.AbandonNotificationsDuringShutdown(false);
			UMClientCommonBase.InitializePerformanceCounters(true);
			ExTraceGlobals.FaultInjectionTracer.RegisterExceptionInjectionCallback(new ExceptionInjectionCallback(FaultInjection.Callback));
			KillBitTimer.Singleton.Start();
			KillbitWatcher.TryWatch(new KillbitWatcher.ReadKillBitFromFileCallback(KillBitHelper.ReadKillBitFromFile));
			ServiceDiagnostics.LogEventWithTrace(ServicesEventLogConstants.Tuple_StartedSuccessfully, null, ExTraceGlobals.CommonAlgorithmTracer, this, "Application started successfully.", null);
		}

		internal static void InitializeSettingsFromWebConfig()
		{
			Global.proxyToSelf = Global.GetAppSettingAsBool("ProxyToSelf", false);
			Global.writeProxyHopHeaders = Global.GetAppSettingAsBool("WriteProxyHopHeaders", false);
			Global.writeFailoverTypeHeader = Global.GetAppSettingAsBool("WriteFailoverTypeHeader", false);
			Global.writeStackTraceOnISE = Global.GetAppSettingAsBool("WriteStackTraceOnISE", false);
			Global.writeStackTraceToProtocolLogForErrorTypes = Global.GetAppSettingAsHashSet("WriteStackTraceToProtocolLogForErrorTypes", null);
			Global.sendDebugResponseHeaders = Global.GetAppSettingAsBool("SendDebugResponseHeaders", false);
			Global.writeThrottlingDiagnostics = Global.GetAppSettingAsBool("WriteThrottlingDiagnostics", false);
			Global.issueEwsCookie = Global.GetAppSettingAsBool("IssueEwsCookie", true);
			Global.chargePreExecuteToBudgetEnabled = Global.GetAppSettingAsBool("ChargePreExecuteToBudgetEnabled", true);
			Global.accessingPrincipalCacheSize = Global.GetAppSettingAsInt("AccessingPrincipalCacheSize", 4);
			Global.maxAttachmentNestLevel = Global.GetAppSettingAsInt("MaxAttachmentNestingDepth", 10);
			Global.findCountLimit = Global.GetAppSettingAsInt("FindCountLimit", 1000);
			Global.hangingConnectionLimit = Global.GetAppSettingAsInt("HangingConnectionLimit", 10);
			Global.createItemRequestSizeThreshold = Global.GetAppSettingAsInt("CreateItemRequestSizeThreshold", 5120000);
			Global.privateWorkingSetThreshold = Global.GetAppSettingAsInt("PrivateWorkingSetThreshold", 716800000);
			Global.collectIntervalInMilliseconds = Global.GetAppSettingAsInt("CollectIntervalInMilliseconds", 300000);
			Global.sendXBEServerExceptionHeaderToCafe = Global.GetAppSettingAsBool("SendXBEServerExceptionHeaderToCafe", true);
			Global.enableMailboxLogger = Global.GetAppSettingAsBool("EnableMailboxLogger", false);
			Global.FastParticipantResolveEnabled = Global.GetAppSettingAsBool("FastParticipantResolveEnabled", true);
			Global.enableSchemaValidationOverride = null;
			Global.useGcCollect = Global.GetAppSettingAsBool("UseGcCollect", false);
			Global.exchangePrincipalCacheTimeoutInMinutes = Global.GetAppSettingAsInt("ExchangePrincipalCacheTimeoutInMinutes", 5);
			Global.exchangePrincipalCacheTimeoutInSecondsOnError = Global.GetAppSettingAsInt("ExchangePrincipalCacheTimeoutInSecondsOnError", 30);
			string value = ConfigurationManager.AppSettings["EnableSchemaValidationOverride"];
			bool value2;
			if (!string.IsNullOrEmpty(value) && bool.TryParse(value, out value2))
			{
				Global.enableSchemaValidationOverride = new bool?(value2);
			}
			Global.bulkOperationThrottlingEnabled = Global.GetAppSettingAsBool("BulkOperationThrottlingEnabled", Global.bulkOperationThrottlingEnabled);
			Global.bulkOperationConcurrencyCap = Global.GetAppSettingAsInt("BulkOperationConcurrencyCap", Global.bulkOperationConcurrencyCap);
			Global.bulkOperationMethods = Global.GetAppSettingAsHashSet("BulkOperationMethods", Global.bulkOperationMethods);
			SingleComponentThrottlingPolicy.BulkOperationConcurrencyCap = Convert.ToUInt32(Global.bulkOperationConcurrencyCap);
			Global.nonInteractiveOperationThrottlingEnabled = Global.GetAppSettingAsBool("NonInteractiveOperationThrottlingEnabled", Global.nonInteractiveOperationThrottlingEnabled);
			Global.nonInteractiveOperationConcurrencyCap = Global.GetAppSettingAsInt("NonInteractiveOperationConcurrencyCap", Global.nonInteractiveOperationConcurrencyCap);
			Global.nonInteractiveOperationMethods = Global.GetAppSettingAsHashSet("NonInteractiveOperationMethods", null);
			SingleComponentThrottlingPolicy.NonInteractiveOperationConcurrencyCap = Convert.ToUInt32(Global.nonInteractiveOperationConcurrencyCap);
			Global.backgroundLoadedTasksEnabled = Global.GetAppSettingAsBool("BackgroundLoadedTasksEnabled", Global.backgroundLoadedTasksEnabled);
			Global.backgroundLoadedTasks = Global.GetAppSettingAsHashSet("BackgroundLoadedTasks", Global.backgroundLoadedTasks);
			Global.backgroundSyncTasksForWellKnownClientsEnabled = Global.GetAppSettingAsBool("BackgroundSyncTasksForWellKnownClientsEnabled", Global.backgroundSyncTasksForWellKnownClientsEnabled);
			Global.backgroundSyncTasksForWellKnownClients = Global.GetAppSettingAsHashSet("BackgroundSyncTasksForWellKnownClients", Global.backgroundSyncTasksForWellKnownClients);
			Global.wellKnownClientsForBackgroundSync = Global.GetAppSettingAsHashSet("WellKnownClientsForBackgroundSync", Global.wellKnownClientsForBackgroundSync);
			Global.disableCommandOptimizationSet = Global.GetAppSettingAsHashSet("DisableCommandOptimizations", null);
			Global.disableAllCommandOptimizations = Global.disableCommandOptimizationSet.Contains("all");
			Global.SendXBEServerExceptionHeaderToCafeOnFailover = Global.GetAppSettingAsBool("SendXBEServerExceptionHeaderToCafeOnFailover", true);
			Global.EnableFaultInjection = Global.GetAppSettingAsBool("EnableFaultInjection", false);
			Global.httpHandlerDisabledMethods = Global.GetAppSettingAsHashSet("HttpHandlerDisabledMethods", Global.httpHandlerDisabledMethods);
			Global.oDataStackTraceInErrorResponse = Global.GetAppSettingAsBool("OData.StackTraceInErrorResponse", false);
			Global.InitializeMaximumTemporaryFilteredViewPerUser();
			if (Global.EnableFaultInjection)
			{
				Global.GetFaultInjectionConfig();
			}
		}

		public static bool ProxyToSelf
		{
			get
			{
				return Global.proxyToSelf;
			}
		}

		public static int EventQueuePollingInterval
		{
			get
			{
				return Global.eventQueuePollingInterval;
			}
		}

		public static bool ShowDebugInformation
		{
			get
			{
				return Global.GetAppSettingAsBool("ShowDebugInformation", false);
			}
		}

		public static bool ChargePreExecuteToBudgetEnabled
		{
			get
			{
				return Global.chargePreExecuteToBudgetEnabled;
			}
		}

		internal static void TraceCasStop(object serviceProviderOperation, CallContext callContext, Guid serviceRequestId)
		{
			if (ETWTrace.ShouldTraceCasStop(serviceRequestId))
			{
				object obj = callContext.EffectiveCaller.PrimarySmtpAddress;
				obj = (obj ?? callContext.EffectiveCallerSid);
				int bytesIn = 0;
				HttpContext httpContext = HttpContext.Current;
				string serverAddress = string.Empty;
				if (httpContext != null)
				{
					HttpRequest request = httpContext.Request;
					if (request != null)
					{
						bytesIn = request.TotalBytes;
						Uri url = request.Url;
						serverAddress = ((url != null) ? url.Host : string.Empty);
					}
				}
				Microsoft.Exchange.Diagnostics.Trace.TraceCasStop(CasTraceEventType.Ews, serviceRequestId, bytesIn, 0, serverAddress, obj, serviceProviderOperation, string.Empty, string.Empty);
			}
		}

		public static int ProxyTimeout
		{
			get
			{
				return Global.proxyTimeout;
			}
		}

		public static bool WriteProxyHopHeaders
		{
			get
			{
				return Global.writeProxyHopHeaders;
			}
		}

		public static bool WriteFailoverTypeHeader
		{
			get
			{
				return Global.writeFailoverTypeHeader;
			}
		}

		public static HashSet<string> BulkOperationMethods
		{
			get
			{
				return Global.bulkOperationMethods;
			}
		}

		public static HashSet<string> NonInteractiveOperationMethods
		{
			get
			{
				return Global.nonInteractiveOperationMethods;
			}
			set
			{
				Global.nonInteractiveOperationMethods = value;
			}
		}

		public static HashSet<string> BackgroundLoadedTasks
		{
			get
			{
				return Global.backgroundLoadedTasks;
			}
			set
			{
				Global.backgroundLoadedTasks = value;
			}
		}

		public static HashSet<string> LongRunningScenarioTasks
		{
			get
			{
				return Global.longRunningScenarioTasks;
			}
		}

		public static HashSet<string> LongRunningScenarioNonBackgroundTasks
		{
			get
			{
				return Global.longRunningScenarioNonBackgroundTasks;
			}
		}

		public static HashSet<RoleType> LongRunningScenarioEnabledRoleTypes
		{
			get
			{
				return Global.longRunningScenarioEnabledRoleTypes;
			}
		}

		public static Regex LongRunningScenarioEnabledUserAgents
		{
			get
			{
				return Global.longRunningScenarioEnabledUserAgents;
			}
		}

		public static bool SendDebugResponseHeaders
		{
			get
			{
				return Global.sendDebugResponseHeaders;
			}
		}

		public static bool WriteRequestDetailsToLog
		{
			get
			{
				return Global.writeRequestDetailsToLog;
			}
		}

		public static HashSet<string> HttpHandleDisabledMethods
		{
			get
			{
				return Global.httpHandlerDisabledMethods;
			}
		}

		public static bool BulkOperationThrottlingEnabled
		{
			get
			{
				return Global.bulkOperationThrottlingEnabled;
			}
		}

		public static bool NonInteractiveThrottlingEnabled
		{
			get
			{
				return Global.nonInteractiveOperationThrottlingEnabled;
			}
			set
			{
				Global.nonInteractiveOperationThrottlingEnabled = value;
			}
		}

		public static bool BackgroundLoadedTasksEnabled
		{
			get
			{
				return Global.backgroundLoadedTasksEnabled;
			}
		}

		public static bool BackgroundSyncTasksForWellKnownClientsEnabled
		{
			get
			{
				return Global.backgroundSyncTasksForWellKnownClientsEnabled;
			}
		}

		public static HashSet<string> BackgroundSyncTasksForWellKnownClients
		{
			get
			{
				return Global.backgroundSyncTasksForWellKnownClients;
			}
		}

		public static HashSet<string> WellKnownClientsForBackgroundSync
		{
			get
			{
				return Global.wellKnownClientsForBackgroundSync;
			}
		}

		internal static FileVersionInfo BuildVersionInfo
		{
			get
			{
				if (Global.buildVersionInfo == null)
				{
					FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
					Interlocked.Exchange<FileVersionInfo>(ref Global.buildVersionInfo, versionInfo);
				}
				return Global.buildVersionInfo;
			}
		}

		internal static BudgetType BudgetType
		{
			get
			{
				return Global.budgetType;
			}
			set
			{
				Global.budgetType = value;
			}
		}

		internal static BudgetType BulkOperationBudgetType
		{
			get
			{
				return Global.bulkOperationBudgetType;
			}
			set
			{
				Global.bulkOperationBudgetType = value;
			}
		}

		internal static BudgetType NonInteractiveOperationBudgetType
		{
			get
			{
				return Global.nonInteractiveOperationBudgetType;
			}
			set
			{
				Global.nonInteractiveOperationBudgetType = value;
			}
		}

		internal static void SetHttpResponse(HttpContext context, HttpStatusCode statusCode)
		{
			context.Response.StatusCode = (int)statusCode;
			context.Response.SuppressContent = true;
			context.ApplicationInstance.CompleteRequest();
		}

		internal static HashSet<string> GetAppSettingAsHashSet(string key, HashSet<string> existingHashSet = null)
		{
			string text = ConfigurationManager.AppSettings[key];
			HashSet<string> hashSet = (existingHashSet != null) ? existingHashSet : new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
			if (!string.IsNullOrEmpty(text))
			{
				foreach (string text2 in text.Split(new char[]
				{
					',',
					';'
				}))
				{
					hashSet.Add(text2.ToLowerInvariant());
				}
			}
			return hashSet;
		}

		private void InitializeWatsonReporting()
		{
			bool appSettingAsBool = Global.GetAppSettingAsBool("SendWatsonReport", true);
			bool appSettingAsBool2 = Global.GetAppSettingAsBool("FilterExceptionsFromWatsonReport", true);
			ServiceDiagnostics.InitializeWatsonReporting(appSettingAsBool, appSettingAsBool2);
		}

		private void Application_Error(object sender, EventArgs e)
		{
			HttpApplication httpApplication = (HttpApplication)sender;
			Exception lastError = httpApplication.Server.GetLastError();
			if (Global.IsKnownHttpException(lastError) || Global.IsKnownWcfException(lastError))
			{
				return;
			}
			if (Global.IsDocumentationError(lastError, httpApplication.Context, "/ews/UM2007Legacy.asmx"))
			{
				httpApplication.Server.ClearError();
				HttpContext.Current.Response.StatusCode = 200;
				HttpContext.Current.Response.Write(string.Format("<HTML><HEAD><TITLE>{0}</TITLE></HEAD><BODY><B>{0}</B></BODY></HTML>", Strings.UMWebServicePage));
				HttpContext.Current.Response.Flush();
				HttpContext.Current.Response.Close();
				return;
			}
			if (Global.IsDocumentationError(lastError, httpApplication.Context, "/ews/exchange.asmx"))
			{
				httpApplication.Server.ClearError();
				HttpContext.Current.Response.Redirect("Services.wsdl", false);
				HttpContext.Current.Response.Flush();
				HttpContext.Current.Response.Close();
				return;
			}
			ServiceDiagnostics.ReportException(lastError, ServicesEventLogConstants.Tuple_InternalServerError, null, "Exception from Application_Error event: {0}");
		}

		internal static bool IsDocumentationError(Exception exception, HttpContext httpContext, string path)
		{
			if (!(exception is InvalidOperationException))
			{
				return false;
			}
			if (httpContext == null)
			{
				return false;
			}
			HttpRequest request = httpContext.Request;
			return !(request.RequestType != "GET") && string.Equals(request.Path, path, StringComparison.OrdinalIgnoreCase);
		}

		internal static bool IsKnownWcfException(Exception exception)
		{
			return exception is ServiceActivationException;
		}

		internal static bool IsKnownHttpException(Exception exception)
		{
			bool result = false;
			HttpException ex = exception as HttpException;
			if (ex != null)
			{
				int httpCode = ex.GetHttpCode();
				if (httpCode >= 400 && httpCode <= 599 && httpCode != 500)
				{
					result = true;
				}
			}
			return result;
		}

		private void Application_End(object sender, EventArgs e)
		{
			RequestDetailsLogger.FlushQueuedFileWrites();
			StoreSession.AbandonNotificationsDuringShutdown(true);
			if (Subscriptions.Singleton != null)
			{
				Subscriptions.Singleton.Dispose();
			}
			ExchangeCertificateChecker.Terminate();
			this.DisposeApplicationObject("WS_APPWideMailboxCacheKey");
			this.DisposeApplicationObject("WS_AcceptedDomainCacheKey");
			ExchangeDiagnosticsHelper.UnRegisterDiagnosticsComponents();
			ServiceDiagnostics.LogEventWithTrace(ServicesEventLogConstants.Tuple_StoppedSuccessfully, null, ExTraceGlobals.CommonAlgorithmTracer, this, "Application stopped successfully.", null);
		}

		private void DisposeApplicationObject(string applicationCacheKey)
		{
			using (base.Application[applicationCacheKey] as IDisposable)
			{
				base.Application.Remove(applicationCacheKey);
			}
		}

		private static void GetFaultInjectionConfig()
		{
			Configuration configuration = WebConfigurationManager.OpenWebConfiguration("/EWS");
			AppSettingsSection appSettingsSection = (AppSettingsSection)configuration.GetSection("appSettings");
			string rawXml = appSettingsSection.SectionInformation.GetRawXml();
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(rawXml);
			XmlNodeList xmlNodeList = xmlDocument.SelectNodes("//appSettings/add[@key = 'FaultInjectionFault']/@value");
			if (xmlNodeList == null)
			{
				Global.EnableFaultInjection = false;
				return;
			}
			List<string> list = (from XmlNode node in xmlNodeList
			select node.Value).ToList<string>();
			Global.FaultsList = new List<Dictionary<string, string>>();
			foreach (string text in list)
			{
				string[] fault = text.Split(new char[]
				{
					';'
				});
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				string valueForKey = Global.GetValueForKey("HttpStatus", fault);
				string valueForKey2 = Global.GetValueForKey("ErrorCode", fault);
				if ((string.IsNullOrEmpty(valueForKey) || valueForKey == "200") && string.IsNullOrEmpty(valueForKey2))
				{
					Global.EnableFaultInjection = false;
					break;
				}
				dictionary.Add("HttpStatus", valueForKey);
				dictionary.Add("ErrorCode", valueForKey2);
				dictionary.Add("SoapAction", Global.GetValueForKey("SoapAction", fault));
				string text2 = Global.GetValueForKey("UserName", fault);
				if (!string.IsNullOrEmpty(text2))
				{
					text2 = text2.ToLower();
				}
				dictionary.Add("UserName", text2);
				Global.FaultsList.Add(dictionary);
			}
		}

		public static string GetValueForKey(string key, string[] fault)
		{
			foreach (string text in fault)
			{
				if (text.StartsWith(key))
				{
					return text.Substring(key.Length + 1);
				}
			}
			return string.Empty;
		}

		internal const string ServiceCommandKey = "WS_ServiceCommandKey";

		internal const string ServiceProviderRequestIdKey = "WS_ServiceProviderRequestIdKey";

		internal const string ServerVersionKey = "WS_ServerVersionKey";

		internal const string AnchorMailboxHintKey = "AnchorMailboxHintKey";

		internal const CasTraceEventType EwsTraceEventType = CasTraceEventType.Ews;

		private const string AppSettingEncodeStringProperties = "EncodeStringProperties";

		private const int DefaultFindCountLimit = 1000;

		private const int DefaultSearchTimeoutInMilliseconds = 60000;

		private const int DefaultMaximumTemporaryFilteredViewPerUser = 20;

		private const int DefaultHangingConnectionLimit = 10;

		private const string DefaultJunkMailReportingAddress = "junk@office365.microsoft.com";

		private const string DefaultNotJunkMailReportingAddress = "not_junk@office365.microsoft.com";

		private const int DefaultCreateItemRequestSizeThreshold = 5120000;

		private const int DefaultPrivateWorkingSetThreshold = 716800000;

		private const int DefaultCollectIntervalInMilliseconds = 300000;

		private const int DefaultOrganizationWideAccessPolicyTimeoutInSeconds = 10800;

		internal const string TargetVersionHeaderName = "X-EWS-TargetVersion";

		internal const string AppWideMailboxCacheKey = "WS_APPWideMailboxCacheKey";

		internal const string AcceptedDomainCacheKey = "WS_AcceptedDomainCacheKey";

		internal const string WorkloadManagerKey = "WS_WorkloadManagerKey";

		internal const string ExchangeUM12LegacyAsmx = "/ews/UM2007Legacy.asmx";

		internal const string ServicesRequestDetailsLoggerKey = "WS_RequestDetailsLoggerKey";

		private const string HttpGet = "GET";

		private const string ExchangeAsmx = "/ews/exchange.asmx";

		private const string ServicesWsdl = "Services.wsdl";

		internal const string CertificateValidationComponentId = "EWS";

		internal const string ProxyCertificateValidationComponentId = "EwsProxy";

		private const string ServicesStopwatch = "ServicesStopwatch";

		private const string AppSettingSendWatsonReport = "SendWatsonReport";

		private const string AppSettingFilterExceptionsFromWatsonReport = "FilterExceptionsFromWatsonReport";

		private const string AppSettingProxyToSelf = "ProxyToSelf";

		private const string AppSettingWriteProxyHopHeaders = "WriteProxyHopHeaders";

		private const string AppSettingProxyTimeout = "ProxyTimeout";

		private const string AppSettingWriteRequestsToLog = "WriteRequestsToLog";

		private const string AppSettingWriteFailoverTypeHeader = "WriteFailoverTypeHeader";

		private const string AppSettingWriteStackTraceOnISE = "WriteStackTraceOnISE";

		private const string AppSettingWriteStackTraceToProtocolLogForErrorTypes = "WriteStackTraceToProtocolLogForErrorTypes";

		private const string AppSettingSendDebugResponseHeaders = "SendDebugResponseHeaders";

		private const string AppSettingSearchTimeout = "SearchTimeoutInMilliseconds";

		private const string AppSettingEDiscoverySearchTimeout = "EDiscoverySearchTimeoutInMilliseconds";

		private const string AppSettingMassUserOverBudgetPercent = "MassUserOverBudgetPercent";

		private const string AppSettingDelayTimeThreshold = "DelayTimeThreshold";

		private const string AppSettingIssueEwsCookie = "IssueEwsCookie";

		private const string AppSettingAccessingPrincipalCacheSize = "AccessingPrincipalCacheSize";

		private const string AppSettingMaxWorkerThreadsPerProcessor = "MaxWorkerThreadsPerProcessor";

		private const string AppSettingMaxTasksQueued = "MaxTasksQueued";

		private const string AppSettingEventQueuePollingInterval = "EventQueuePollingInterval";

		private const string AppSettingWriteThrottlingDiagnostics = "WriteThrottlingDiagnostics";

		private const string AppSettingPerUserAccessPolicyTimeoutInSeconds = "PerUserAccessPolicyTimeoutInSeconds";

		private const string AppSettingOrganizationWideAccessPolicyTimeoutInSeconds = "OrganizationWideAccessPolicyTimeoutInSeconds";

		private const string AppSettingCertificateExpirationCheckerIntervalInMinutes = "AppSettingCertificateExpirationCheckerIntervalInMinutes";

		private const string AppSettingDisableCommandOptimizations = "DisableCommandOptimizations";

		private const string AppSettingIdentityCacheSizeLimit = "IdentityCacheSizeLimit";

		private const string AppSettingMaxAttachmentNestingDepth = "MaxAttachmentNestingDepth";

		private const string AppSettingFindCountLimit = "FindCountLimit";

		private const string AppSettingShowDebugInformation = "ShowDebugInformation";

		private const string AppSettingMaximumTemporaryFilteredViewPerUser = "MaximumTemporaryFilteredViewPerUser";

		private const string AppSettingBulkOperationThrottlingEnabled = "BulkOperationThrottlingEnabled";

		private const string AppSettingBulkOperationMethods = "BulkOperationMethods";

		private const string AppSettingBulkOperationConcurrencyCap = "BulkOperationConcurrencyCap";

		private const string AppSettingNonInteractiveOperationThrottlingEnabled = "NonInteractiveOperationThrottlingEnabled";

		private const string AppSettingNonInteractiveOperationMethods = "NonInteractiveOperationMethods";

		private const string AppSettingNonInteractiveOperationConcurrencyCap = "NonInteractiveOperationConcurrencyCap";

		private const string AppSettingBackgroundLoadedTasksEnabled = "BackgroundLoadedTasksEnabled";

		private const string AppSettingBackgroundLoadedTasks = "BackgroundLoadedTasks";

		private const string AppSettingBackgroundSyncTasksForWellKnownClientsEnabled = "BackgroundSyncTasksForWellKnownClientsEnabled";

		private const string AppSettingBackgroundSyncTasksForWellKnownClients = "BackgroundSyncTasksForWellKnownClients";

		private const string AppSettingWellKnownClientsForBackgroundSync = "WellKnownClientsForBackgroundSync";

		private const string AppSettingChargePreExecuteToBudgetEnabled = "ChargePreExecuteToBudgetEnabled";

		private const string AppSettingHangingConnectionLimit = "HangingConnectionLimit";

		private const string AppSettingEnableSchemaValidationOverride = "EnableSchemaValidationOverride";

		private const string AppSettingUseGcCollect = "UseGcCollect";

		private const string AppSettingCreateItemRequestSizeThreshold = "CreateItemRequestSizeThreshold";

		private const string AppSettingPrivateWorkingSetThreshold = "PrivateWorkingSetThreshold";

		private const string AppSettingCollectIntervalInMilliseconds = "CollectIntervalInMilliseconds";

		private const string AppSettingSendXBEServerExceptionHeaderToCafe = "SendXBEServerExceptionHeaderToCafe";

		private const string AppSettingEnableMailboxLogger = "EnableMailboxLogger";

		private const string AppSettingSendXBEServerExceptionHeaderToCafeOnFailover = "SendXBEServerExceptionHeaderToCafeOnFailover";

		private const string AppSettingExchangePrincipalCacheTimeoutInMinutes = "ExchangePrincipalCacheTimeoutInMinutes";

		private const string AppSettingFastParticipantResolveEnabled = "FastParticipantResolveEnabled";

		private const string AppSettingExchangePrincipalCacheTimeoutInSecondsOnError = "ExchangePrincipalCacheTimeoutInSecondsOnError";

		private const string AppSettingHttpHandlerDisabledMethods = "HttpHandlerDisabledMethods";

		private const int DefaultMassUserOverBudgetPercent = 0;

		private const int DefaultDelayTimeThresholdInMilliseconds = 0;

		private const int DefaultIdentityCacheSize = 4000;

		private const string AppSettingEnableFaultInjection = "EnableFaultInjection";

		private const string AppSettingFaultInjectionFaults = "FaultInjectionFault";

		private const string AppSettingODataStackTraceInErrorResponse = "OData.StackTraceInErrorResponse";

		private const string TrackingCookieName = "exchangecookie";

		private const int DefaultEventQueuePollingInterval = 5;

		private const int MinEventQueuePollingInterval = 1;

		private const int MaxEventQueuePollingInterval = 60;

		private const int DefaultCertificateCheckerTimerIntervalInMinutes = 4320;

		private const int MinimumPerUserAccessPolicyTimeoutInSeconds = 0;

		private const int MinimumOrganizationWideAccessPolicyTimeoutInSeconds = 0;

		internal static readonly string BooleanTrue = true.ToString();

		private static readonly UnifiedContactStoreConfiguration unifiedContactStoreConfiguration = new UnifiedContactStoreConfiguration();

		private static int maxAttachmentNestLevel = 10;

		private static int findCountLimit = 1000;

		private static int searchTimeoutInMilliseconds = 60000;

		private static int maximumTemporaryFilteredViewPerUser = 20;

		private static int hangingConnectionLimit = 10;

		private static bool? enableSchemaValidationOverride = null;

		private static bool useGcCollect = false;

		private static int createItemRequestSizeThreshold = 5120000;

		private static int privateWorkingSetThreshold = 716800000;

		private static int collectIntervalInMilliseconds = 300000;

		private static int organizationWideAccessPolicyTimeoutInSeconds = 10800;

		private static int exchangePrincipalCacheTimeoutInMinutes = 5;

		private static int exchangePrincipalCacheTimeoutInSecondsOnError = 30;

		private static bool writeStackTraceOnISE = false;

		private static HashSet<string> writeStackTraceToProtocolLogForErrorTypes = new HashSet<string>();

		private static bool writeThrottlingDiagnostics = false;

		private static string junkMailReportingAddress = "junk@office365.microsoft.com";

		private static string notJunkMailReportingAddress = "not_junk@office365.microsoft.com";

		private static bool sendXBEServerExceptionHeaderToCafe = true;

		private static bool enableMailboxLogger = false;

		private static int accessingPrincipalCacheSize = 4;

		private static HashSet<string> disableCommandOptimizationSet = new HashSet<string>();

		private static bool disableAllCommandOptimizations = false;

		private static bool oDataStackTraceInErrorResponse = false;

		private static IResponseShapeResolver responseShapeResolver = new DefaultResponseShapeResolver();

		private static EwsClientMailboxSessionCloningHandler ewsClientMailboxSessionCloningHandler = new EwsClientMailboxSessionCloningHandler(Global.DefaultEwsClientMailboxSessionCloningHandler);

		private static bool safeHtmlLoaded;

		private static string defaultMapiClientType = "Client=WebServices";

		private static int getAttachmentSizeLimit = 38797312;

		internal static readonly RemoteCertificateValidationCallback RemoteCertificateValidationCallback = (object obj, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors) => EWSSettings.AllowInternalUntrustedCerts || errors == SslPolicyErrors.None;

		private static readonly int defaultProxyTimeout = 59000;

		private static readonly int minimumProxyTimeout = 10000;

		private static readonly int minimumSearchTimeout = 10;

		private static readonly int minimumTemporaryFilteredViewPerUser = 5;

		private static readonly int defaultMaxWorkerThreadsPerProcessor = 10;

		private static readonly int minimumMaxWorkerThreadsPerProcessor = 1;

		private static readonly int defaultMaxTasksQueued = 500;

		private static readonly int minimumMaxTasksQueued = 50;

		private static readonly int maximumMaxTasksQueued = 750;

		private static bool proxyToSelf = false;

		private static bool writeProxyHopHeaders = false;

		private static bool writeFailoverTypeHeader = false;

		private static bool writeRequestDetailsToLog = true;

		private static int proxyTimeout = Global.defaultProxyTimeout;

		private static bool sendDebugResponseHeaders = false;

		private static bool issueEwsCookie = true;

		private static bool chargePreExecuteToBudgetEnabled = true;

		private static bool bulkOperationThrottlingEnabled = true;

		private static int bulkOperationConcurrencyCap = 2;

		private static HashSet<string> bulkOperationMethods = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase)
		{
			typeof(MarkAllItemsAsRead).Name,
			typeof(EmptyFolder).Name
		};

		private static HashSet<string> httpHandlerDisabledMethods = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);

		private static bool nonInteractiveOperationThrottlingEnabled = false;

		private static int nonInteractiveOperationConcurrencyCap = 10;

		private static HashSet<string> nonInteractiveOperationMethods = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);

		private static bool backgroundLoadedTasksEnabled = true;

		private static HashSet<string> backgroundLoadedTasks = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase)
		{
			typeof(ExportItems).Name,
			typeof(GetImItemList).Name,
			typeof(UploadItems).Name
		};

		private static bool backgroundSyncTasksForWellKnownClientsEnabled = false;

		private static HashSet<string> backgroundSyncTasksForWellKnownClients = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase)
		{
			typeof(GetItem).Name,
			typeof(GetAttachment).Name
		};

		private static HashSet<string> wellKnownClientsForBackgroundSync = new HashSet<string>(StringComparer.InvariantCulture)
		{
			"MacOutlook".ToLowerInvariant()
		};

		private static Regex longRunningScenarioEnabledUserAgents = new Regex("EDiscovery", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);

		private static HashSet<RoleType> longRunningScenarioEnabledRoleTypes = new HashSet<RoleType>
		{
			RoleType.MailboxSearch
		};

		private static HashSet<string> longRunningScenarioTasks = new HashSet<string>
		{
			typeof(SearchMailboxes).Name,
			typeof(GetNonIndexableItemStatistics).Name,
			typeof(GetNonIndexableItemDetails).Name,
			typeof(GetDiscoverySearchConfiguration).Name,
			typeof(GetFolder).Name,
			typeof(FindFolder).Name,
			typeof(ExportItems).Name,
			typeof(UploadItems).Name,
			typeof(GetItem).Name,
			typeof(CreateFolder).Name,
			typeof(DeleteFolder).Name,
			typeof(MoveFolder).Name,
			typeof(FindItem).Name,
			typeof(CreateItem).Name,
			typeof(UpdateItem).Name,
			typeof(DeleteItem).Name,
			typeof(GetAttachment).Name,
			typeof(CreateAttachment).Name,
			typeof(DeleteAttachment).Name
		};

		private static HashSet<string> longRunningScenarioNonBackgroundTasks = new HashSet<string>
		{
			typeof(SearchMailboxes).Name
		};

		private static int eventQueuePollingInterval = 5;

		private static BudgetType budgetType = BudgetType.Ews;

		private static BudgetType bulkOperationBudgetType = BudgetType.EwsBulkOperation;

		private static BudgetType nonInteractiveOperationBudgetType = BudgetType.EwsBulkOperation;

		private static FileVersionInfo buildVersionInfo;

		internal static LazyMember<int> MaxMaxRequestSizeForEWS = new LazyMember<int>(delegate()
		{
			int num = 0;
			try
			{
				List<CustomBindingElement> list = new List<CustomBindingElement>(MessageEncoderWithXmlDeclaration.EwsBindingNames.Length);
				Configuration configuration = WebConfigurationManager.OpenWebConfiguration("~/web.config");
				BindingsSection bindingsSection = (BindingsSection)configuration.GetSection("system.serviceModel/bindings");
				foreach (string text in MessageEncoderWithXmlDeclaration.EwsBindingNames)
				{
					if (bindingsSection.CustomBinding.Bindings.ContainsKey(text))
					{
						list.Add(bindingsSection.CustomBinding.Bindings[text]);
					}
					else
					{
						ExTraceGlobals.CommonAlgorithmTracer.TraceDebug<string>(0L, "Binding {0} was not found in web.config file.", text);
					}
				}
				foreach (CustomBindingElement customBindingElement in list)
				{
					TransportElement transportElement = (TransportElement)customBindingElement[typeof(HttpsTransportElement)];
					if (transportElement == null)
					{
						transportElement = (TransportElement)customBindingElement[typeof(HttpTransportElement)];
					}
					if (transportElement != null)
					{
						if (transportElement.MaxReceivedMessageSize > 0L)
						{
							num = Math.Max(num, (int)transportElement.MaxReceivedMessageSize);
						}
					}
					else
					{
						ExTraceGlobals.CommonAlgorithmTracer.TraceDebug<string>(0L, "No transport element found for binding {0} in web.config file.", customBindingElement.Name);
					}
				}
			}
			catch (Exception)
			{
				num = 0;
			}
			return num;
		});

		internal static LazyMember<bool> UseBufferRequestChannelListener = new LazyMember<bool>(delegate()
		{
			bool result = false;
			List<CustomBindingElement> list = new List<CustomBindingElement>(MessageEncoderWithXmlDeclaration.EwsBindingNames.Length);
			Configuration configuration = WebConfigurationManager.OpenWebConfiguration("~/web.config");
			BindingsSection bindingsSection = (BindingsSection)configuration.GetSection("system.serviceModel/bindings");
			foreach (string text in MessageEncoderWithXmlDeclaration.EwsBindingNames)
			{
				if (bindingsSection.CustomBinding.Bindings.ContainsKey(text))
				{
					list.Add(bindingsSection.CustomBinding.Bindings[text]);
				}
				else
				{
					ExTraceGlobals.CommonAlgorithmTracer.TraceDebug<string>(0L, "Binding {0} was not found in web.config file.", text);
				}
			}
			foreach (CustomBindingElement customBindingElement in list)
			{
				HttpsTransportElement httpsTransportElement = (HttpsTransportElement)customBindingElement[typeof(HttpsTransportElement)];
				if (httpsTransportElement != null)
				{
					if (httpsTransportElement.TransferMode == TransferMode.Streamed)
					{
						result = true;
					}
				}
				else
				{
					HttpTransportElement httpTransportElement = (HttpTransportElement)customBindingElement[typeof(HttpTransportElement)];
					if (httpTransportElement != null && httpTransportElement.TransferMode == TransferMode.Streamed)
					{
						result = true;
					}
				}
			}
			return result;
		});
	}
}
