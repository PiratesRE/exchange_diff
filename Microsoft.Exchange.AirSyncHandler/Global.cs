using System;
using System.Diagnostics;
using System.Globalization;
using System.Security.Permissions;
using System.Security.Principal;
using System.Threading;
using System.Web;
using Microsoft.Exchange.AirSync;
using Microsoft.Exchange.Data.ApplicationLogic;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.AirSync;
using Microsoft.Exchange.Security;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Mapi;

namespace Microsoft.Exchange.AirSyncHandler
{
	public class Global : HttpApplication
	{
		[PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
		public override void Init()
		{
			base.Init();
		}

		private static void HandledExceptionProxy(object sender, UnhandledExceptionEventArgs e)
		{
			if (GlobalSettings.SendWatsonReport)
			{
				AirSyncFatalException ex = e.ExceptionObject as AirSyncFatalException;
				if (ex == null || ex.WatsonReportEnabled)
				{
					Exception ex2 = e.ExceptionObject as Exception;
					if (ex2 != null)
					{
						AirSyncDiagnostics.SendWatson(ex2);
						return;
					}
					ExWatson.HandleException(sender, e);
				}
			}
		}

		private static void ResetAutoBlockedDeviceCounter(object state)
		{
			AirSyncCounters.AutoBlockedDevices.RawValue = 0L;
		}

		private void Application_End(object sender, EventArgs e)
		{
			this.ExecuteApplicationEnd(sender, e);
		}

		private void Application_Start(object sender, EventArgs e)
		{
			this.ExecuteApplicationStart(sender, e);
		}

		private void ExecuteApplicationStart(object sender, EventArgs e)
		{
			int num = 0;
			num = Privileges.RemoveAllExcept(new string[]
			{
				"SeAuditPrivilege",
				"SeChangeNotifyPrivilege",
				"SeCreateGlobalPrivilege",
				"SeImpersonatePrivilege",
				"SeIncreaseQuotaPrivilege",
				"SeAssignPrimaryTokenPrivilege",
				"SeTcbPrivilege"
			}, "MSExchangeServicesSyncPool");
			if (num != 0)
			{
				string name;
				using (WindowsIdentity current = WindowsIdentity.GetCurrent())
				{
					name = current.Name;
				}
				AirSyncDiagnostics.LogPeriodicEvent(AirSyncEventLogConstants.Tuple_SetPrivilegesFailure, "SetPrivilegesFailure: " + name, new string[]
				{
					name,
					num.ToString(CultureInfo.InvariantCulture)
				});
				Environment.Exit(num);
			}
			ExWatson.Init("E12");
			AppDomain.CurrentDomain.UnhandledException += Global.HandledExceptionProxy;
			Globals.InitializeMultiPerfCounterInstance("AirSync");
			ExRpcModule.Bind();
			AirSyncDiagnostics.TraceInfo<ResourceManagerHandle>(ExTraceGlobals.RequestsTracer, null, "AuthzAuthorization.ResourceManagerHandle static instance loaded", AuthzAuthorization.ResourceManagerHandle);
			AirSyncSyncStateTypeFactory.EnsureSyncStateTypesRegistered();
			FolderSyncState.RegisterCustomDataVersioningHandler(new FolderSyncState.HandleCustomDataVersioningDelegate(FolderSyncStateCustomDataInfo.HandlerCustomDataVersioning));
			FolderHierarchySyncState.RegisterCustomDataVersioningHandler(new FolderHierarchySyncState.HandleCustomDataVersioningDelegate(FolderHierarchySyncStateCustomDataInfo.HandlerCustomDataVersioning));
			EventQueue.PollingInterval = GlobalSettings.EventQueuePollingInterval;
			this.resetAutoBlockedDeviceCounterTimer = new Timer(new TimerCallback(Global.ResetAutoBlockedDeviceCounter), null, 86400000, 86400000);
		}

		private void ExecuteApplicationEnd(object sender, EventArgs e)
		{
			ADNotificationManager.Stop();
			DeviceClassCache.Instance.Stop();
			ADUserCache.Stop();
			MailboxSessionCache.Stop();
			DeviceBehaviorCache.Stop();
			ExchangeDiagnosticsHelper.UnRegisterDiagnosticsComponents();
			this.resetAutoBlockedDeviceCounterTimer.Dispose();
			GlobalSettings.SyncLog.Dispose();
			using (Process currentProcess = Process.GetCurrentProcess())
			{
				AirSyncDiagnostics.LogEvent(AirSyncEventLogConstants.Tuple_AirSyncUnloaded, new string[]
				{
					currentProcess.Id.ToString(CultureInfo.InvariantCulture)
				});
			}
		}

		private Timer resetAutoBlockedDeviceCounterTimer;
	}
}
