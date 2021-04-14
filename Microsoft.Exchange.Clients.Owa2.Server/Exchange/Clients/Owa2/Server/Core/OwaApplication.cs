using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Security;
using System.ServiceModel;
using System.Threading;
using System.Web;
using Microsoft.Exchange.Clients.Common;
using Microsoft.Exchange.Clients.Owa2.Server.Diagnostics;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data.ApplicationLogic.Extension;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.ProvisioningCache;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.CmdletInfra;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.Net.Protocols;
using Microsoft.Exchange.SafeHtml;
using Microsoft.Exchange.Services;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf;
using Microsoft.Exchange.UM.ClientAccess;
using Microsoft.Exchange.WorkloadManagement;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class OwaApplication : BaseApplication
	{
		public override int MaxBreadcrumbs
		{
			get
			{
				return BaseApplication.GetAppSetting<int>("MaxBreadcrumbs", 20);
			}
		}

		public override bool LogVerboseNotifications
		{
			get
			{
				return BaseApplication.GetAppSetting<bool>("LogVerboseNotifications", true);
			}
		}

		public override int ActivityBasedPresenceDuration
		{
			get
			{
				return BaseApplication.GetAppSetting<int>("ActivityBasedPresenceDuration", 300000);
			}
		}

		public override HttpClientCredentialType ServiceAuthenticationType
		{
			get
			{
				return HttpClientCredentialType.None;
			}
		}

		public override TroubleshootingContext TroubleshootingContext
		{
			get
			{
				return this.troubleshootingContext;
			}
		}

		public override bool LogErrorDetails
		{
			get
			{
				return BaseApplication.GetAppSetting<bool>("LogErrorDetails", false);
			}
		}

		public override bool LogErrorTraces
		{
			get
			{
				return this.logErrorTraces ?? false;
			}
		}

		internal static RequestDetailsLogger GetRequestDetailsLogger
		{
			get
			{
				return RequestDetailsLoggerBase<RequestDetailsLogger>.Current;
			}
		}

		internal override void UpdateErrorTracingConfiguration()
		{
			bool appSetting = BaseApplication.GetAppSetting<bool>("LogErrorTraces", false);
			if (appSetting == this.logErrorTraces)
			{
				return;
			}
			this.logErrorTraces = new bool?(appSetting);
			Exception ex = null;
			try
			{
				if (File.Exists(ConfigFiles.InMemory.ConfigFilePath))
				{
					ExTraceGlobals.ConfigurationManagerTracer.TraceDebug(0L, "Globals.UpdateErrorTracingConfiguration: Not changing settings, in-memory tracing configuration file is present.");
					return;
				}
			}
			catch (IOException ex2)
			{
				ex = ex2;
			}
			catch (UnauthorizedAccessException ex3)
			{
				ex = ex3;
			}
			catch (SecurityException ex4)
			{
				ex = ex4;
			}
			if (ex == null)
			{
				bool enable = this.logErrorTraces ?? false;
				Guid componentGuid = new Guid("9041df24-db8f-4561-9ce6-75ee8dc21732");
				ExTraceConfiguration.Instance.EnableInMemoryTracing(componentGuid, enable);
				Guid componentGuid2 = new Guid("1758fd24-1153-4624-96f6-742b18fc0372");
				ExTraceConfiguration.Instance.EnableInMemoryTracing(componentGuid2, enable);
				Guid componentGuid3 = new Guid("6d031d1d-5908-457a-a6c4-cdd0f6e74d81");
				ExTraceConfiguration.Instance.EnableInMemoryTracing(componentGuid3, enable);
				return;
			}
			ExTraceGlobals.ConfigurationManagerTracer.TraceDebug<Exception>(0L, "Globals.UpdateErrorTracingConfiguration: Not changing settings, unable to verify presence of in-memory tracing configuration file: {0}", ex);
		}

		internal override void Initialize()
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			ExTraceGlobals.ConfigurationManagerTracer.TraceDebug(0L, "OwaApplication.Initialize: SafeHtml loading begin");
			IntPtr value = NativeMethods.LoadLibrary(Path.GetFullPath(Path.Combine(ExchangeSetupContext.BinPath, "SafeHtmlNativeWrapper.dll")));
			if (value == IntPtr.Zero)
			{
				ExTraceGlobals.ConfigurationManagerTracer.TraceError(0L, "OwaApplication.Initialize: Failed to load SafeHtmlNativeWrapper.");
				Global.SafeHtmlLoaded = false;
			}
			else
			{
				ExTraceGlobals.ConfigurationManagerTracer.TraceDebug(0L, "OwaApplication.Initialize: SafeHtmlNativeWrapper loaded successfully.");
				Global.SafeHtmlLoaded = true;
			}
			SafeHtml.Initialize(ExchangeSetupContext.BinPath + Path.DirectorySeparatorChar);
			ExTraceGlobals.ConfigurationManagerTracer.TraceDebug(0L, "OwaApplication.Initialize: SafeHtml loading finished");
			Global.InitializeSettingsFromWebConfig();
			int workerThreads;
			int num;
			ThreadPool.GetMinThreads(out workerThreads, out num);
			int configIntValue = AppConfigLoader.GetConfigIntValue("ThreadPoolMinIOCPThreads", 0, int.MaxValue, 3 * Environment.ProcessorCount);
			ThreadPool.SetMinThreads(workerThreads, configIntValue);
			OwaApplication.InitializeApplicationCaches();
			RequestDetailsLogger.ApplicationType = LoggerApplicationType.Owa;
			OwaClientLogger.Initialize();
			OwaClientTraceLogger.Initialize();
			OwaServerLogger.Initialize();
			OwaServerTraceLogger.Initialize();
			SettingOverrideSync.Instance.Start(true);
			LoggerSettings.MaxAppendableColumnLength = null;
			LoggerSettings.ErrorMessageLengthThreshold = null;
			Global.ResponseShapeResolver = new OwaResponseShapeResolver();
			Global.EwsClientMailboxSessionCloningHandler = new EwsClientMailboxSessionCloningHandler(UserContextManager.GetClonedMailboxSession);
			Global.DefaultMapiClientType = "Client=OWA";
			MailboxSession.DefaultFoldersToForceInit = OwaApplication.foldersToForceInitialize;
			UserContextManager.Initialize();
			if (Globals.OwaIsNoRecycleEnabled)
			{
				OwaVersionId.InitializeOwaVersionReadingTimer();
			}
			KillBitTimer.Singleton.Start();
			KillbitWatcher.TryWatch(new KillbitWatcher.ReadKillBitFromFileCallback(KillBitHelper.ReadKillBitFromFile));
			OwaServerLogger.AppendToLog(new OwaAppStartLogEvent((double)stopwatch.ElapsedMilliseconds));
		}

		internal override void ExecuteApplicationBeginRequest(object sender, EventArgs e)
		{
			try
			{
				OwaDiagnostics.SendWatsonReportsForGrayExceptions(delegate()
				{
					RequestDetailsLoggerBase<RequestDetailsLogger>.InitializeRequestLogger();
					RequestDetailsLogger getRequestDetailsLogger = OwaApplication.GetRequestDetailsLogger;
					getRequestDetailsLogger.ActivityScope.UpdateFromMessage(HttpContext.Current.Request);
					getRequestDetailsLogger.ActivityScope.SerializeTo(HttpContext.Current.Response);
					getRequestDetailsLogger.ActivityScope.SetProperty(OwaServerLogger.LoggerData.IsRequest, "1");
					string requestCorrelationId = HttpUtilities.GetRequestCorrelationId(HttpContext.Current);
					getRequestDetailsLogger.Set(OwaServerLogger.LoggerData.CorrelationId, requestCorrelationId);
					getRequestDetailsLogger.Set(OwaServerLogger.LoggerData.RequestStartTime, DateTime.UtcNow);
					HttpContext.Current.Response.AppendToLog("&ActID=" + getRequestDetailsLogger.ActivityId);
					HttpContext.Current.Response.AppendToLog("&CorrelationID=" + requestCorrelationId);
				});
			}
			catch (GrayException arg)
			{
				ExTraceGlobals.ConfigurationManagerTracer.TraceDebug<GrayException>(0L, "OwaApplication.ExecuteApplicationBeginRequest: GrayException: {0}", arg);
				throw;
			}
		}

		internal override void ExecuteApplicationEndRequest(object sender, EventArgs e)
		{
			try
			{
				OwaDiagnostics.SendWatsonReportsForGrayExceptions(delegate()
				{
					try
					{
						RequestDetailsLogger getRequestDetailsLogger = OwaApplication.GetRequestDetailsLogger;
						if (getRequestDetailsLogger != null)
						{
							HttpApplication httpApplication = (HttpApplication)sender;
							OwaServerLogger.LogHttpContextData(httpApplication.Context, getRequestDetailsLogger);
							RequestDetailsLogger.LogEvent(getRequestDetailsLogger, OwaServerLogger.LoggerData.RequestEndTime);
						}
						OwaApplication.FinalizeResponse();
					}
					finally
					{
						Global.CleanUpRequestObjects();
					}
				});
			}
			catch (GrayException arg)
			{
				ExTraceGlobals.ConfigurationManagerTracer.TraceDebug<GrayException>(0L, "OwaApplication.ExecuteApplicationEndRequest: GrayException: {0}", arg);
				throw;
			}
		}

		protected override void InternalDispose()
		{
			InstantMessageProvider.DisposeProvider();
			BrokerGateway.Shutdown();
			ClientWatsonParametersFactory.Shutdown();
			SettingOverrideSync.Instance.Stop();
			Global.EwsClientMailboxSessionCloningHandler = null;
		}

		protected override void ExecuteApplicationSpecificStart()
		{
			ErrorHandlerUtilities.RegisterForUnhandledExceptions();
			StoreSession.UseRPCContextPool = true;
			UMClientCommonBase.InitializePerformanceCounters(false);
		}

		private static void InitializeApplicationCaches()
		{
			HttpContext httpContext = HttpContext.Current;
			HttpApplication applicationInstance = httpContext.ApplicationInstance;
			ADIdentityInformationCache.Initialize(4000);
			applicationInstance.Application["WS_APPWideMailboxCacheKey"] = new AppWideStoreSessionCache();
			applicationInstance.Application["WS_AcceptedDomainCacheKey"] = new AcceptedDomainCache();
			int maxThreadCount = 10 * Environment.ProcessorCount;
			Global.BudgetType = BudgetType.Owa;
			Global.BulkOperationBudgetType = BudgetType.OwaBulkOperation;
			Global.NonInteractiveOperationBudgetType = BudgetType.OwaNonInteractiveOperation;
			Global.NonInteractiveThrottlingEnabled = true;
			Global.NonInteractiveOperationMethods = OwaApplication.owaNonInteractiveMethodNames;
			UserWorkloadManager.Initialize(maxThreadCount, 500, 500, TimeSpan.FromMinutes(5.0), null);
			applicationInstance.Application["WS_WorkloadManagerKey"] = UserWorkloadManager.Singleton;
			OwaEventRegistry owaEventRegistry = new OwaEventRegistry();
			owaEventRegistry.RegisterHandler(typeof(PendingRequestEventHandler));
			applicationInstance.Application["OwaEventRegistry"] = owaEventRegistry;
			string identification = ConfigurationManager.AppSettings["ProvisioningCacheIdentification"];
			ProvisioningCache.InitializeAppRegistrySettings(identification);
		}

		private static void FinalizeResponse()
		{
			HttpRequest request = HttpContext.Current.Request;
			HttpResponse response = HttpContext.Current.Response;
			RequestDetailsLogger getRequestDetailsLogger = OwaApplication.GetRequestDetailsLogger;
			if (getRequestDetailsLogger != null && getRequestDetailsLogger.ActivityScope != null)
			{
				string property = getRequestDetailsLogger.ActivityScope.GetProperty(ServiceCommonMetadata.ExceptionName);
				double totalMilliseconds = getRequestDetailsLogger.ActivityScope.TotalMilliseconds;
				string property2 = getRequestDetailsLogger.ActivityScope.GetProperty(ServiceCommonMetadata.CorrectBEServerToUse);
				AggregatedOperationStatistics aggregatedOperationStatistics = getRequestDetailsLogger.ActivityScope.TakeStatisticsSnapshot(AggregatedOperationType.ADCalls);
				AggregatedOperationStatistics aggregatedOperationStatistics2 = getRequestDetailsLogger.ActivityScope.TakeStatisticsSnapshot(AggregatedOperationType.StoreRPCs);
				string value = string.Format(CultureInfo.InvariantCulture, "{0:F0};{1:F0};{2:F0}", new object[]
				{
					totalMilliseconds,
					aggregatedOperationStatistics.TotalMilliseconds,
					aggregatedOperationStatistics2.TotalMilliseconds
				});
				try
				{
					response.Headers.Add("X-OWA-DiagnosticsInfo", value);
					if (!string.IsNullOrEmpty(property))
					{
						response.Headers["X-OWA-Error"] = property;
					}
					if (!string.IsNullOrWhiteSpace(property2))
					{
						response.Headers[WellKnownHeader.XDBMountedOnServer] = property2;
					}
					if (UserAgentUtilities.IsMonitoringRequest(request.UserAgent))
					{
						response.Headers["X-DiagInfoLdapLatency"] = aggregatedOperationStatistics.TotalMilliseconds.ToString("F0");
						response.Headers["X-DiagInfoRpcLatency"] = aggregatedOperationStatistics2.TotalMilliseconds.ToString("F0");
						response.Headers["X-DiagInfoIisLatency"] = totalMilliseconds.ToString("F0");
					}
				}
				catch (Exception arg)
				{
					ExTraceGlobals.ConfigurationManagerTracer.TraceDebug<Exception>(0L, "Exception occurred while attempting to append diagnostic headers, exception will be ignored: {0}", arg);
				}
			}
		}

		private const int DefaultMaxRequestsQueued = 500;

		private const int DefaultMaxWorkerThreadsPerProc = 10;

		internal const string ActivityIdKeyForIISLogs = "&ActID=";

		internal const string CorrelationIdKeyForIISLogs = "&CorrelationID=";

		private const int DefaultIdentityCacheSize = 4000;

		private static readonly HashSet<DefaultFolderType> foldersToForceInitialize = new HashSet<DefaultFolderType>
		{
			DefaultFolderType.ToDoSearch,
			DefaultFolderType.RecoverableItemsRoot,
			DefaultFolderType.RecoverableItemsVersions,
			DefaultFolderType.RecoverableItemsDeletions,
			DefaultFolderType.RecoverableItemsPurges,
			DefaultFolderType.RecipientCache,
			DefaultFolderType.QuickContacts,
			DefaultFolderType.MyContacts,
			DefaultFolderType.ImContactList
		};

		private static readonly HashSet<string> owaNonInteractiveMethodNames = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase)
		{
			typeof(InstantMessageSignIn).Name,
			typeof(LogDatapoint).Name,
			typeof(ResetPresence).Name,
			typeof(SubscribeToNotification).Name,
			typeof(GetPersonaPhoto).Name,
			typeof(GetPeopleIKnowGraphCommand).Name
		};

		private TroubleshootingContext troubleshootingContext = new TroubleshootingContext("OwaServer");

		private bool? logErrorTraces;
	}
}
