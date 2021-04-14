using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.IO;
using System.Management.Automation;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.AccessControl;
using System.Security.Principal;
using System.ServiceProcess;
using System.Threading;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Security.WindowsFirewall;
using Microsoft.Win32;

namespace Microsoft.Exchange.Configuration.Tasks
{
	public abstract class ManageService : ConfigureService
	{
		protected ManageService()
		{
			TaskLogger.LogEnter();
			InstallContext installContext = new InstallContext();
			installContext.Parameters["logtoconsole"] = "false";
			installContext.Parameters["assemblypath"] = base.GetType().Assembly.Location;
			this.serviceInstaller = new ServiceInstaller();
			this.serviceInstaller.Context = installContext;
			this.serviceInstaller.ServiceName = this.Name;
			this.serviceInstaller.StartType = ServiceStartMode.Manual;
			this.serviceProcessInstaller = new ServiceProcessInstaller();
			this.serviceProcessInstaller.Context = installContext;
			this.serviceProcessInstaller.Account = ServiceAccount.NetworkService;
			this.serviceProcessInstaller.Installers.Add(this.serviceInstaller);
			this.ServicesDependedOn = new string[]
			{
				ManagedServiceName.ActiveDirectoryTopologyService
			};
			this.serviceFirewallRules = new List<ExchangeFirewallRule>(2);
			TaskLogger.LogExit();
		}

		protected string DisplayName
		{
			get
			{
				return this.serviceInstaller.DisplayName;
			}
			set
			{
				this.serviceInstaller.DisplayName = value;
			}
		}

		protected string Description
		{
			get
			{
				return this.description;
			}
			set
			{
				this.description = value;
			}
		}

		protected ServiceAccount Account
		{
			get
			{
				return this.serviceProcessInstaller.Account;
			}
			set
			{
				this.serviceProcessInstaller.Account = value;
			}
		}

		protected ServiceStartMode StartMode
		{
			get
			{
				return this.serviceInstaller.StartType;
			}
			set
			{
				this.serviceInstaller.StartType = value;
			}
		}

		protected string[] ServicesDependedOn
		{
			get
			{
				return this.serviceInstaller.ServicesDependedOn;
			}
			set
			{
				this.serviceInstaller.ServicesDependedOn = value;
			}
		}

		protected InstallContext ServiceInstallContext
		{
			get
			{
				return this.serviceProcessInstaller.Context;
			}
			set
			{
				this.serviceProcessInstaller.Context = value;
			}
		}

		protected string EventMessageFile
		{
			get
			{
				return this.eventMessageFile;
			}
			set
			{
				this.eventMessageFile = value;
			}
		}

		protected int CategoryCount
		{
			get
			{
				return this.categoryCount;
			}
			set
			{
				this.categoryCount = value;
			}
		}

		protected ServiceInstaller ServiceInstaller
		{
			get
			{
				return this.serviceInstaller;
			}
		}

		internal static void AddAssemblyToFirewallExceptions(string name, string fullPath, Task.TaskErrorLoggingDelegate errorHandler)
		{
			if (string.IsNullOrEmpty(fullPath) || string.IsNullOrEmpty(name))
			{
				return;
			}
			if (ConfigurationContext.Setup.IsLonghornServer)
			{
				string args = string.Format("advfirewall firewall add rule name=\"{0}\" dir=in action=allow program=\"{1}\" localip=any remoteip=any profile=any Enable=yes", name, fullPath);
				ManageService.RunNetShProcess(args, errorHandler);
			}
		}

		internal static void RemoveAssemblyFromFirewallExceptions(string name, string fullPath, Task.TaskErrorLoggingDelegate errorHandler)
		{
			if (string.IsNullOrEmpty(fullPath))
			{
				return;
			}
			if (ConfigurationContext.Setup.IsLonghornServer)
			{
				string args = string.Format("advfirewall firewall delete rule name=\"{0}\" program=\"{1}\"", name, fullPath);
				ManageService.RunNetShProcess(args, errorHandler);
			}
		}

		protected void Install()
		{
			TaskLogger.LogEnter(new object[]
			{
				this.Name
			});
			Hashtable hashtable = new Hashtable();
			if (!ServiceControllerUtils.IsInstalled(this.Name))
			{
				try
				{
					TaskLogger.Trace("Installing service", new object[0]);
					this.serviceProcessInstaller.Install(hashtable);
				}
				catch (Win32Exception ex)
				{
					if (1072 == ex.NativeErrorCode)
					{
						Thread.Sleep(10000);
						hashtable = new Hashtable();
						this.serviceProcessInstaller.Install(hashtable);
					}
					else
					{
						base.WriteError(new TaskWin32Exception(ex), ErrorCategory.WriteError, null);
					}
				}
				base.ConfigureServiceSidType();
				if (this.serviceFirewallRules.Count > 0)
				{
					foreach (ExchangeFirewallRule exchangeFirewallRule in this.serviceFirewallRules)
					{
						TaskLogger.Trace("Adding Windows Firewall Rule for Service {0}", new object[]
						{
							this.Name
						});
						exchangeFirewallRule.Add();
					}
				}
				this.serviceProcessInstaller.Commit(hashtable);
			}
			else
			{
				TaskLogger.Trace("Service is already installed.", new object[0]);
			}
			if (this.Description != null)
			{
				try
				{
					using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(ManageService.serviceRegPath + this.Name, true))
					{
						registryKey.SetValue(ManageService.descriptionSubKeyName, this.Description);
					}
					goto IL_197;
				}
				catch (SecurityException inner)
				{
					base.WriteError(new SecurityException(Strings.ErrorOpenKeyDeniedForWrite(ManageService.serviceRegPath + this.Name), inner), ErrorCategory.WriteError, null);
					goto IL_197;
				}
			}
			TaskLogger.Trace("No service description", new object[0]);
			IL_197:
			if (this.EventMessageFile != null)
			{
				RegistryKey registryKey2 = null;
				try
				{
					try
					{
						registryKey2 = Registry.LocalMachine.OpenSubKey(ManageService.eventLogRegPath + this.Name, true);
						if (registryKey2 == null)
						{
							registryKey2 = Registry.LocalMachine.CreateSubKey(ManageService.eventLogRegPath + this.Name, RegistryKeyPermissionCheck.ReadWriteSubTree);
						}
						registryKey2.SetValue(ManageService.eventMessageFileSubKeyName, this.EventMessageFile);
						registryKey2.SetValue(ManageService.categoryMessageFileSubKeyName, this.EventMessageFile);
						registryKey2.SetValue(ManageService.categoryCountSubKeyName, this.CategoryCount);
						registryKey2.SetValue(ManageService.typesSupportedSubKeyName, 7);
					}
					catch (SecurityException inner2)
					{
						base.WriteError(new SecurityException(Strings.ErrorOpenKeyDeniedForWrite(ManageService.serviceRegPath + this.Name), inner2), ErrorCategory.WriteError, null);
					}
					goto IL_281;
				}
				finally
				{
					if (registryKey2 != null)
					{
						registryKey2.Close();
						registryKey2 = null;
					}
				}
			}
			TaskLogger.Trace("No event message file", new object[0]);
			IL_281:
			if (base.FirstFailureActionType != ServiceActionType.None)
			{
				base.ConfigureFailureActions();
				base.ConfigureFailureActionsFlag();
			}
			else
			{
				TaskLogger.Trace("No failure actions", new object[0]);
			}
			TaskLogger.LogExit();
		}

		protected void Uninstall()
		{
			TaskLogger.LogEnter();
			if (!ServiceControllerUtils.IsInstalled(this.Name))
			{
				base.WriteVerbose(Strings.ServiceNotInstalled(this.Name));
				return;
			}
			base.WriteVerbose(Strings.WillUninstallInstalledService(this.Name));
			try
			{
				this.serviceProcessInstaller.Uninstall(null);
			}
			catch (Win32Exception ex)
			{
				if (ex.NativeErrorCode == 1060)
				{
					this.WriteWarning(Strings.ServiceAlreadyNotInstalled(this.Name));
				}
				else
				{
					base.WriteError(new ServiceUninstallFailureException(this.Name, ex.Message, ex), ErrorCategory.InvalidOperation, null);
				}
			}
			catch (InstallException ex2)
			{
				base.WriteError(new ServiceUninstallFailureException(this.Name, ex2.Message, ex2), ErrorCategory.InvalidOperation, null);
			}
			if (this.serviceFirewallRules.Count > 0)
			{
				using (List<ExchangeFirewallRule>.Enumerator enumerator = this.serviceFirewallRules.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						ExchangeFirewallRule exchangeFirewallRule = enumerator.Current;
						TaskLogger.Trace("Removing Windows Firewall Rule for Service {0}", new object[]
						{
							this.Name
						});
						exchangeFirewallRule.Remove();
					}
					return;
				}
			}
			string fullPath = this.serviceProcessInstaller.Context.Parameters["assemblypath"];
			TaskLogger.Trace("Removing Service {0} from windows firewall exception", new object[]
			{
				this.Name
			});
			ManageService.RemoveAssemblyFromFirewallExceptions(this.Name, fullPath, new Task.TaskErrorLoggingDelegate(base.WriteError));
			TaskLogger.LogExit();
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
		}

		protected void LockdownServiceAccess()
		{
			TaskLogger.Trace("Modifying service ACL to remove Network Logon ACE.", new object[0]);
			ServiceAccessFlags serviceAccessFlags = ServiceAccessFlags.ReadControl | ServiceAccessFlags.WriteDac;
			base.DoNativeServiceTask(this.Name, serviceAccessFlags, delegate(IntPtr service)
			{
				string name = this.Name;
				IntPtr intPtr = IntPtr.Zero;
				IntPtr intPtr2 = IntPtr.Zero;
				try
				{
					int num = 65536;
					intPtr = Marshal.AllocHGlobal(num);
					int num2;
					if (!NativeMethods.QueryServiceObjectSecurity(service, SecurityInfos.DiscretionaryAcl, intPtr, num, out num2))
					{
						base.WriteError(TaskWin32Exception.FromErrorCodeAndVerbose(Marshal.GetLastWin32Error(), Strings.ErrorQueryServiceObjectSecurity(name)), ErrorCategory.InvalidOperation, null);
					}
					byte[] array = new byte[num2];
					Marshal.Copy(intPtr, array, 0, num2);
					RawSecurityDescriptor rawSecurityDescriptor = new RawSecurityDescriptor(array, 0);
					CommonSecurityDescriptor commonSecurityDescriptor = new CommonSecurityDescriptor(false, false, rawSecurityDescriptor);
					CommonAce commonAce = null;
					SecurityIdentifier right = new SecurityIdentifier("S-1-5-11");
					for (int i = 0; i < commonSecurityDescriptor.DiscretionaryAcl.Count; i++)
					{
						CommonAce commonAce2 = (CommonAce)commonSecurityDescriptor.DiscretionaryAcl[i];
						if (commonAce2.SecurityIdentifier == right)
						{
							commonAce = commonAce2;
							break;
						}
					}
					if (commonAce == null)
					{
						TaskLogger.Trace("Service ACL was not modified as Network Logon SID is not found.", new object[0]);
					}
					else
					{
						commonSecurityDescriptor.DiscretionaryAcl.RemoveAccess(AccessControlType.Allow, commonAce.SecurityIdentifier, commonAce.AccessMask, commonAce.InheritanceFlags, commonAce.PropagationFlags);
						int binaryLength = commonSecurityDescriptor.BinaryLength;
						byte[] array2 = new byte[binaryLength];
						commonSecurityDescriptor.GetBinaryForm(array2, 0);
						intPtr2 = Marshal.AllocHGlobal(binaryLength);
						Marshal.Copy(array2, 0, intPtr2, binaryLength);
						if (!NativeMethods.SetServiceObjectSecurity(service, SecurityInfos.DiscretionaryAcl, intPtr2))
						{
							base.WriteError(TaskWin32Exception.FromErrorCodeAndVerbose(Marshal.GetLastWin32Error(), Strings.ErrorSetServiceObjectSecurity(name)), ErrorCategory.InvalidOperation, null);
						}
						TaskLogger.Trace("Service ACL modified - Network Logon ACE removed.", new object[0]);
					}
				}
				finally
				{
					if (IntPtr.Zero != intPtr)
					{
						Marshal.FreeHGlobal(intPtr);
					}
					if (IntPtr.Zero != intPtr2)
					{
						Marshal.FreeHGlobal(intPtr2);
					}
				}
			});
		}

		protected void AddFirewallRule(ExchangeFirewallRule firewallRule)
		{
			this.serviceFirewallRules.Add(firewallRule);
		}

		private static void RunNetShProcess(string args, Task.TaskErrorLoggingDelegate errorHandler)
		{
			string text = null;
			string text2 = null;
			try
			{
				ProcessRunner.Run(ManageService.NetshExe, args, -1, null, out text, out text2);
			}
			catch (Win32Exception exception)
			{
				errorHandler(exception, ErrorCategory.InvalidOperation, null);
			}
			catch (System.TimeoutException exception2)
			{
				errorHandler(exception2, ErrorCategory.OperationTimeout, null);
			}
			catch (InvalidOperationException exception3)
			{
				errorHandler(exception3, ErrorCategory.InvalidOperation, null);
			}
		}

		protected static readonly string descriptionSubKeyName = "Description";

		protected static readonly string eventMessageFileSubKeyName = "EventMessageFile";

		protected static readonly string categoryMessageFileSubKeyName = "CategoryMessageFile";

		protected static readonly string categoryCountSubKeyName = "CategoryCount";

		protected static readonly string typesSupportedSubKeyName = "TypesSupported";

		protected static readonly string serviceRegPath = "SYSTEM\\CurrentControlSet\\Services\\";

		protected static readonly string eventLogRegPath = "SYSTEM\\CurrentControlSet\\Services\\EventLog\\Application\\";

		private readonly List<ExchangeFirewallRule> serviceFirewallRules;

		private static string NetshExe = Path.Combine(Environment.SystemDirectory, "netsh.exe");

		private string description;

		private string eventMessageFile;

		private int categoryCount;

		private ServiceInstaller serviceInstaller;

		private ServiceProcessInstaller serviceProcessInstaller;
	}
}
