using System;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Web;
using Microsoft.Exchange.Clients.Common;
using Microsoft.Exchange.Clients.EventLogs;
using Microsoft.Exchange.Clients.Owa.Core.Transcoding;
using Microsoft.Exchange.CommonHelpProvider;
using Microsoft.Exchange.Data.Search;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.Security;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal abstract class OwaApplicationBase
	{
		internal abstract OWAVDirType OwaVDirType { get; }

		internal void ExecuteApplicationStart(object sender, EventArgs e)
		{
			ExTraceGlobals.CoreCallTracer.TraceDebug(0L, "Global.Application_Start");
			try
			{
				if (OwaApplicationBase.IsRunningDfpowa)
				{
					string localPath = new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath;
					DirectoryInfo[] directories = Directory.GetParent(Directory.GetParent(localPath).FullName).GetDirectories("Config");
					if (directories.Length > 0)
					{
						VariantConfiguration.Initialize(directories[0].FullName);
					}
				}
				Globals.Initialize(this.OwaVDirType);
				Kerberos.FlushTicketCache();
				SmallIconManager.Initialize();
				ThemeManager.Initialize();
				FormsRegistryManager.Initialize(HttpRuntime.AppDomainAppPath);
				ADCustomPropertyParser.Initialize(HttpRuntime.AppDomainAppPath);
				PerformanceCounterManager.InitializePerformanceCounters();
				ProxyEventHandler.Initialize();
				ExRpcModule.Bind();
				UIExtensionManager.Initialize();
				HelpProvider.Initialize(HelpProvider.HelpAppName.Owa);
				this.ExecuteApplicationSpecificStart();
			}
			catch (OwaSmallIconManagerInitializationException initializationError)
			{
				ExTraceGlobals.CoreTracer.TraceDebug(0L, "Application initialization failed");
				Globals.InitializationError = initializationError;
				return;
			}
			catch (OwaThemeManagerInitializationException initializationError2)
			{
				ExTraceGlobals.CoreTracer.TraceDebug(0L, "Application initialization failed");
				Globals.InitializationError = initializationError2;
				return;
			}
			catch (OwaFormsRegistryInitializationException initializationError3)
			{
				ExTraceGlobals.CoreTracer.TraceDebug(0L, "Application initialization failed");
				Globals.InitializationError = initializationError3;
				return;
			}
			catch (OwaInvalidConfigurationException ex)
			{
				ExTraceGlobals.CoreTracer.TraceDebug(0L, "Application initialization failed because of a problem with configuration: " + ex.Message);
				Globals.InitializationError = ex;
				return;
			}
			catch (OwaWin32Exception ex2)
			{
				ExTraceGlobals.CoreTracer.TraceDebug<int>(0L, "Application initialization failed with a win32 error: {0}", ex2.LastError);
				Globals.InitializationError = ex2;
				return;
			}
			catch (Exception initializationError4)
			{
				ExTraceGlobals.CoreTracer.TraceDebug(0L, "Application initialization failed");
				Globals.InitializationError = initializationError4;
				throw;
			}
			Globals.IsInitialized = true;
			ExTraceGlobals.CoreTracer.TraceDebug(0L, "Application initialization succeeded");
			if (Globals.IsPreCheckinApp)
			{
				OwaDiagnostics.Logger.LogEvent(ClientsEventLogConstants.Tuple_DfpOwaStartedSuccessfully, string.Empty, new object[0]);
				return;
			}
			OwaDiagnostics.Logger.LogEvent(ClientsEventLogConstants.Tuple_OwaStartedSuccessfully, string.Empty, new object[0]);
		}

		protected abstract void ExecuteApplicationSpecificStart();

		internal void ExecuteApplicationEnd(object sender, EventArgs e)
		{
			Tokenizer.ReleaseWordBreakers();
			InstantMessageOCSProvider.DisposeEndpointManager();
			if (TranscodingTaskManager.IsInitialized)
			{
				TranscodingTaskManager.StopTranscoding();
			}
			OwaConfigurationManager.ShutDownConfigurationManager();
			PerformanceCounterManager.ArePerfCountersEnabled = false;
			Globals.UnloadOwaSettings();
		}

		private static readonly bool IsRunningDfpowa = !string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["IsPreCheckinApp"]) && StringComparer.OrdinalIgnoreCase.Equals("true", ConfigurationManager.AppSettings["IsPreCheckinApp"]);
	}
}
