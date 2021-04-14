using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.ServiceModel;
using System.Web;
using Microsoft.Exchange.Clients.EventLogs;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Search;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.Security;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Mapi;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal abstract class BaseApplication
	{
		protected BaseApplication()
		{
			this.applicationStopwatch.Start();
			OwaRegistryKeys.Initialize();
		}

		public string ApplicationVersion
		{
			get
			{
				return this.applicationVersion;
			}
		}

		public long ApplicationTime
		{
			get
			{
				return this.applicationStopwatch.ElapsedMilliseconds;
			}
		}

		public bool ArePerfCountersEnabled
		{
			get
			{
				bool result = true;
				try
				{
					result = BaseApplication.GetAppSetting<bool>("ArePerfCountersEnabled", true);
				}
				catch (Exception ex)
				{
					ExTraceGlobals.CoreTracer.TraceError<string, string>(0L, "Failed to get value for ArePerfCountersEnabled from AppSetting. Error: {0}. Stack: {1}.", ex.Message, ex.StackTrace);
				}
				return result;
			}
		}

		public bool SendWatsonReports
		{
			get
			{
				return BaseApplication.GetAppSetting<bool>("SendWatsonReports", true);
			}
		}

		public bool DisableBreadcrumbs
		{
			get
			{
				return BaseApplication.GetAppSetting<bool>("DisableBreadcrumbs", false);
			}
		}

		public bool CheckForForgottenAttachmentsEnabled
		{
			get
			{
				return BaseApplication.GetAppSetting<bool>("CheckForForgottenAttachmentsEnabled", true);
			}
		}

		public string BlockedQueryStringValues
		{
			get
			{
				return BaseApplication.GetAppSetting<string>("BlockedQueryStringValues", null);
			}
		}

		public string ContentDeliveryNetworkEndpoint
		{
			get
			{
				return BaseApplication.GetAppSetting<string>("ContentDeliveryNetworkEndpoint", null);
			}
		}

		public bool ControlTasksQueueDisabled
		{
			get
			{
				return BaseApplication.GetAppSetting<bool>("ControlTasksQueueDisabled", false);
			}
		}

		public bool IsPreCheckinApp
		{
			get
			{
				return BaseApplication.GetAppSetting<bool>("IsPreCheckinApp", false);
			}
		}

		public bool IsFirstReleaseFlightingEnabled
		{
			get
			{
				return BaseApplication.GetAppSetting<bool>("FirstReleaseFlightingEnabled", false);
			}
		}

		public bool OwaIsNoRecycleEnabled
		{
			get
			{
				return BaseApplication.GetAppSetting<bool>("OwaIsNoRecycleEnabled", false);
			}
		}

		public double OwaVersionReadingInterval
		{
			get
			{
				return BaseApplication.GetAppSetting<double>("OwaVersionReadingInterval", OwaVersionId.OwaVersionReadingInterval);
			}
		}

		public abstract int ActivityBasedPresenceDuration { get; }

		public abstract int MaxBreadcrumbs { get; }

		public abstract bool LogVerboseNotifications { get; }

		public abstract TroubleshootingContext TroubleshootingContext { get; }

		public abstract bool LogErrorDetails { get; }

		public abstract bool LogErrorTraces { get; }

		public abstract HttpClientCredentialType ServiceAuthenticationType { get; }

		internal static BaseApplication CreateInstance()
		{
			BaseApplication result;
			if (!string.IsNullOrEmpty(HttpRuntime.AppDomainAppId) && HttpRuntime.AppDomainAppId.EndsWith("calendar", StringComparison.CurrentCultureIgnoreCase))
			{
				result = new OwaAnonymousApplication();
			}
			else
			{
				result = new OwaApplication();
			}
			return result;
		}

		internal static TimeSpan GetTimeSpanAppSetting(string key, TimeSpan defaultValue)
		{
			string text = ConfigurationManager.AppSettings.Get(key);
			TimeSpan result = defaultValue;
			if (!string.IsNullOrWhiteSpace(text) && !TimeSpan.TryParse(text, out result))
			{
				result = defaultValue;
			}
			return result;
		}

		internal static T GetAppSetting<T>(string key, T defaultValue)
		{
			try
			{
				string value = ConfigurationManager.AppSettings.Get(key);
				if (!string.IsNullOrWhiteSpace(value))
				{
					return (T)((object)Convert.ChangeType(value, typeof(T)));
				}
			}
			catch (ConfigurationErrorsException ex)
			{
				ExTraceGlobals.CoreCallTracer.TraceDebug<string, string>(0L, "Failed to load setting {0}. Error: {1}", key, ex.Message);
			}
			catch (FormatException ex2)
			{
				ExTraceGlobals.CoreCallTracer.TraceDebug<string, string>(0L, "Failed to load setting {0}. Error: {1}", key, ex2.Message);
			}
			catch (InvalidOperationException ex3)
			{
				ExTraceGlobals.CoreCallTracer.TraceDebug<string, string>(0L, "Failed to load setting {0}. Error: {1}", key, ex3.Message);
			}
			return defaultValue;
		}

		internal void Dispose()
		{
			this.InternalDispose();
		}

		internal void ExecuteApplicationStart(object sender, EventArgs e)
		{
			ExTraceGlobals.CoreCallTracer.TraceDebug(0L, "Global.Application_Start");
			try
			{
				int num = Privileges.RemoveAllExcept(new string[]
				{
					"SeAuditPrivilege",
					"SeChangeNotifyPrivilege",
					"SeCreateGlobalPrivilege",
					"SeImpersonatePrivilege",
					"SeIncreaseQuotaPrivilege",
					"SeAssignPrimaryTokenPrivilege"
				}, "MSExchangeOWAAppPool");
				if (num != 0)
				{
					throw new OwaWin32Exception(num, "Failed to remove unnecessary privileges");
				}
				if (BaseApplication.IsRunningDfpowa)
				{
					string localPath = new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath;
					DirectoryInfo[] directories = Directory.GetParent(Directory.GetParent(localPath).FullName).GetDirectories("Config");
					if (directories.Length > 0)
					{
						VariantConfiguration.Initialize(directories[0].FullName);
					}
				}
				string applicationName = this.IsPreCheckinApp ? "PCDFOWA" : "OWA";
				Globals.InitializeMultiPerfCounterInstance(applicationName);
				Globals.Initialize();
				Kerberos.FlushTicketCache();
				ExRpcModule.Bind();
				this.ExecuteApplicationSpecificStart();
			}
			catch (OwaWin32Exception ex)
			{
				ExTraceGlobals.CoreTracer.TraceDebug<int>(0L, "Application initialization failed with a win32 error: {0}", ex.LastError);
				Globals.InitializationError = ex;
				return;
			}
			catch (Exception initializationError)
			{
				ExTraceGlobals.CoreTracer.TraceDebug(0L, "Application initialization failed");
				Globals.InitializationError = initializationError;
				throw;
			}
			Globals.IsInitialized = true;
			ExTraceGlobals.CoreTracer.TraceDebug(0L, "Application initialization succeeded");
			int id = Process.GetCurrentProcess().Id;
			if (Globals.IsPreCheckinApp)
			{
				OwaDiagnostics.Logger.LogEvent(ClientsEventLogConstants.Tuple_DfpOwaStartedSuccessfully, string.Empty, new object[]
				{
					id
				});
				return;
			}
			OwaDiagnostics.Logger.LogEvent(ClientsEventLogConstants.Tuple_OwaStartedSuccessfully, string.Empty, new object[]
			{
				id
			});
			OwaDiagnostics.PublishMonitoringEventNotification(ExchangeComponent.Owa.Name, "OwaWebAppStarted", "Outlook Web App started successfully", ResultSeverityLevel.Error);
		}

		internal void ExecuteApplicationEnd(object sender, EventArgs e)
		{
			Tokenizer.ReleaseWordBreakers();
			InstantMessageOCSProvider.DisposeEndpointManager();
		}

		internal virtual void ExecuteApplicationBeginRequest(object sender, EventArgs e)
		{
		}

		internal virtual void ExecuteApplicationEndRequest(object sender, EventArgs e)
		{
		}

		internal abstract void UpdateErrorTracingConfiguration();

		internal abstract void Initialize();

		protected abstract void InternalDispose();

		protected abstract void ExecuteApplicationSpecificStart();

		protected const int FailoverServerLcid = 1033;

		private static readonly bool IsRunningDfpowa = !string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["IsPreCheckinApp"]) && StringComparer.OrdinalIgnoreCase.Equals("true", ConfigurationManager.AppSettings["IsPreCheckinApp"]);

		private readonly string applicationVersion = typeof(Globals).GetApplicationVersion();

		private Stopwatch applicationStopwatch = new Stopwatch();
	}
}
