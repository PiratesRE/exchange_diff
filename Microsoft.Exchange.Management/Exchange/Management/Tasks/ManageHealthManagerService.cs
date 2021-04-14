using System;
using System.Configuration.Install;
using System.IO;
using System.Management.Automation;
using System.Security;
using System.Security.AccessControl;
using System.Security.Principal;
using System.ServiceProcess;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Win32;

namespace Microsoft.Exchange.Management.Tasks
{
	public abstract class ManageHealthManagerService : ManageService
	{
		public ManageHealthManagerService()
		{
			base.Account = ServiceAccount.LocalSystem;
			base.StartMode = ServiceStartMode.Automatic;
			base.DisplayName = Strings.HealthManagerServiceDisplayName;
			base.Description = Strings.HealthManagerServiceDescription;
			InstallContext installContext = new InstallContext();
			installContext.Parameters["logtoconsole"] = "false";
			installContext.Parameters["assemblypath"] = Path.Combine(ConfigurationContext.Setup.BinPath, "MSExchangeHMHost.exe");
			base.FirstFailureActionType = ServiceActionType.Restart;
			base.FirstFailureActionDelay = 5000U;
			base.SecondFailureActionType = ServiceActionType.Restart;
			base.SecondFailureActionDelay = 5000U;
			base.AllOtherFailuresActionType = ServiceActionType.Restart;
			base.AllOtherFailuresActionDelay = 5000U;
			base.FailureResetPeriod = 0U;
			base.ServiceInstallContext = installContext;
			base.ServicesDependedOn = new string[]
			{
				"eventlog",
				"MSExchangeADTopology"
			};
			base.EventMessageFile = Path.Combine(ConfigurationContext.Setup.ResPath, "Microsoft.Exchange.ActiveMonitoring.EventLog.dll");
			base.CategoryCount = 2;
		}

		protected override string Name
		{
			get
			{
				return "MSExchangeHM";
			}
		}

		protected void RegisterProcessManagerEventLog()
		{
			RegistryKey registryKey = null;
			string text = ManageService.eventLogRegPath + "MSExchangeHMHost";
			string value = Path.Combine(ConfigurationContext.Setup.BinPath, "Microsoft.Exchange.Common.ProcessManagerMsg.dll");
			try
			{
				registryKey = Registry.LocalMachine.OpenSubKey(text, true);
				if (registryKey == null)
				{
					registryKey = Registry.LocalMachine.CreateSubKey(text, RegistryKeyPermissionCheck.ReadWriteSubTree);
				}
				registryKey.SetValue(ManageService.eventMessageFileSubKeyName, value);
				registryKey.SetValue(ManageService.categoryMessageFileSubKeyName, value);
				registryKey.SetValue(ManageService.categoryCountSubKeyName, 1);
				registryKey.SetValue(ManageService.typesSupportedSubKeyName, 7);
			}
			catch (SecurityException inner)
			{
				base.WriteError(new SecurityException(Strings.ErrorOpenKeyDeniedForWrite(text), inner), ErrorCategory.WriteError, null);
			}
			finally
			{
				if (registryKey != null)
				{
					registryKey.Close();
					registryKey = null;
				}
			}
		}

		protected void PersistManagedAvailabilityServersUsgSid()
		{
			SecurityIdentifier securityIdentifier = null;
			try
			{
				IRootOrganizationRecipientSession rootOrganizationRecipientSession = DirectorySessionFactory.Default.CreateRootOrgRecipientSession(true, ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 153, "PersistManagedAvailabilityServersUsgSid", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Monitoring\\Service\\ManageHealthManagerService.cs");
				securityIdentifier = rootOrganizationRecipientSession.GetWellKnownExchangeGroupSid(WellKnownGuid.MaSWkGuid);
			}
			catch (Exception)
			{
			}
			if (securityIdentifier != null)
			{
				using (RegistryKey registryKey = Registry.LocalMachine.CreateSubKey(ManageHealthManagerService.RegistryPathBase))
				{
					registryKey.SetValue("ManagedAvailabilityServersUsgSid", securityIdentifier.ToString());
				}
				string name = "SOFTWARE\\Microsoft\\ExchangeServer";
				using (RegistryKey registryKey2 = Registry.LocalMachine.OpenSubKey(name, true))
				{
					if (registryKey2 != null)
					{
						RegistrySecurity accessControl = registryKey2.GetAccessControl();
						accessControl.AddAccessRule(new RegistryAccessRule(securityIdentifier, RegistryRights.FullControl, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.InheritOnly, AccessControlType.Allow));
						registryKey2.SetAccessControl(accessControl);
						using (RegistryKey registryKey3 = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Control\\SecurePipeServers\\winreg", true))
						{
							RegistrySecurity accessControl2 = registryKey3.GetAccessControl();
							accessControl2.AddAccessRule(new RegistryAccessRule(securityIdentifier, RegistryRights.ExecuteKey, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.InheritOnly, AccessControlType.Allow));
							registryKey3.SetAccessControl(accessControl2);
						}
					}
				}
			}
		}

		protected void RemoveManagedAvailabilityServersUsgSidCache()
		{
			using (RegistryKey registryKey = Registry.LocalMachine.CreateSubKey(ManageHealthManagerService.RegistryPathBase))
			{
				registryKey.DeleteValue("ManagedAvailabilityServersUsgSid", false);
			}
		}

		private const string ManagedAvailabilityServersUsgSidValueName = "ManagedAvailabilityServersUsgSid";

		private const string ServiceShortName = "MSExchangeHM";

		private const string ServiceBinaryName = "MSExchangeHMHost.exe";

		private const string EventLogBinaryName = "Microsoft.Exchange.ActiveMonitoring.EventLog.dll";

		private const string HostShortName = "MSExchangeHMHost";

		private const string HostEventLogBinaryName = "Microsoft.Exchange.Common.ProcessManagerMsg.dll";

		private static readonly string RegistryPathBase = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\ActiveMonitoring\\";
	}
}
