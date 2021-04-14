using System;
using System.ComponentModel;
using System.IO;
using System.Management.Automation;
using System.Security;
using System.ServiceProcess;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Web.Administration;
using Microsoft.Win32;

namespace Microsoft.Exchange.Management.Deployment
{
	[Cmdlet("Configure", "WSManIISHosting", SupportsShouldProcess = true)]
	[ClassAccessLevel(AccessLevel.Consumer)]
	public sealed class ConfigureWSManIISHosting : ConfigureWSManIISHostingBase
	{
		[Parameter]
		public SwitchParameter DataCenterCAS
		{
			get
			{
				return (SwitchParameter)(base.Fields["DataCenterCAS"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["DataCenterCAS"] = value;
			}
		}

		[Parameter]
		public SwitchParameter EnableKerberosModule
		{
			get
			{
				return (SwitchParameter)(base.Fields["EnableKerbAuth"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["EnableKerbAuth"] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageConfigureWSManIISHosting;
			}
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.InternalProcessRecord();
			if (base.HasErrors)
			{
				return;
			}
			this.ChangeWinrmServiceSettings();
			base.WriteVerbose(Strings.VerboseCheckRequiredFiles);
			this.CheckConfigurationFilePaths();
			if (this.EnableKerberosModule)
			{
				this.InstallKerberosAuthenticationModule();
			}
			if (!this.isWSManInstalled)
			{
				return;
			}
			base.WriteVerbose(Strings.VerboseCheckRequiredRegistryKeys);
			this.CheckRequiredRegistryKeys();
			if (!this.isWSManInstalled)
			{
				return;
			}
			base.WriteVerbose(Strings.VerboseCheckIISConfiguration);
			this.CheckIISConfigurationFile();
			if (this.needToRestartWSMan)
			{
				base.RestartWSManService();
			}
			base.OpenHttpPortsOnFirewall();
			if (this.DataCenterCAS)
			{
				base.EnableBasicAuthForWSMan();
			}
			TaskLogger.LogExit();
		}

		private void ChangeWinrmServiceSettings()
		{
			base.WriteVerbose(Strings.VerboseChangeWinrmStartType);
			try
			{
				string keyName = "HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Services\\WinRM";
				Registry.SetValue(keyName, "Start", ServiceStartMode.Automatic, RegistryValueKind.DWord);
				Registry.SetValue(keyName, "DelayedAutoStart", 1, RegistryValueKind.DWord);
			}
			catch (SecurityException ex)
			{
				base.WriteError(new InvalidOperationException(Strings.ErrorChangeWinrmStartType(ex.ToString()), ex), ErrorCategory.InvalidOperation, null);
			}
			catch (IOException ex2)
			{
				base.WriteError(new InvalidOperationException(Strings.ErrorChangeWinrmStartType(ex2.ToString()), ex2), ErrorCategory.InvalidOperation, null);
			}
			base.WriteVerbose(Strings.VerboseStartWinrm);
			try
			{
				using (ServiceController serviceController = new ServiceController("winrm"))
				{
					if (serviceController.Status == ServiceControllerStatus.Stopped)
					{
						serviceController.Start();
					}
				}
			}
			catch (InvalidOperationException ex3)
			{
				base.WriteError(new InvalidOperationException(Strings.ErrorStartWinrm(ex3.ToString()), ex3), ErrorCategory.InvalidOperation, null);
			}
			catch (Win32Exception ex4)
			{
				base.WriteError(new InvalidOperationException(Strings.ErrorStartWinrm(ex4.ToString()), ex4), ErrorCategory.InvalidOperation, null);
			}
		}

		private void CheckConfigurationFilePaths()
		{
			if (!File.Exists(base.AppcmdPath))
			{
				base.WriteError(new FileNotFoundException(Strings.ErrorAppCmdNotExist(base.AppcmdPath)), ErrorCategory.ObjectNotFound, null);
			}
			if (!File.Exists(base.IISConfigFilePath))
			{
				base.WriteError(new FileNotFoundException(Strings.ErrorAppCmdNotExist(base.IISConfigFilePath)), ErrorCategory.ObjectNotFound, null);
			}
			if (!File.Exists(base.WSManCfgSchemaFilePath))
			{
				if (!File.Exists(base.WSManCfgSchemaFileOriginalPath))
				{
					this.WriteWarning(Strings.ErrorWSManConfigSchemaFileNotFound(base.WSManCfgSchemaFilePath));
					this.isWSManInstalled = false;
					return;
				}
				File.Copy(base.WSManCfgSchemaFileOriginalPath, base.WSManCfgSchemaFilePath, true);
			}
			if (!File.Exists(base.WSManModuleFilePath))
			{
				this.WriteWarning(Strings.ErrorWSManModuleFileNotFound(base.WSManModuleFilePath));
				this.isWSManInstalled = false;
				return;
			}
			if (this.EnableKerberosModule && !File.Exists(base.KerberosAuthModuleFilePath))
			{
				this.WriteWarning(Strings.ErrorKerbauthModuleFileNotFound(base.KerberosAuthModuleFilePath));
				return;
			}
			if (!File.Exists(base.WinrmFilePath))
			{
				this.WriteWarning(Strings.ErrorWinRMCmdNotFound(base.WinrmFilePath));
				this.isWSManInstalled = false;
				return;
			}
			if (!File.Exists(base.NetFilePath))
			{
				base.WriteError(new FileNotFoundException(Strings.ErrorSystemFileNotFound(base.NetFilePath)), ErrorCategory.ObjectNotFound, null);
			}
			using (ServerManager serverManager = new ServerManager())
			{
				foreach (Site site in serverManager.Sites)
				{
					if (site.Id == 1L)
					{
						base.DefaultSiteName = site.Name;
						break;
					}
				}
			}
			if (base.DefaultSiteName == null)
			{
				base.WriteError(new InvalidOperationException(Strings.ErrorDefaultWebSiteNotExist(base.IISConfigFilePath)), ErrorCategory.InvalidOperation, null);
			}
		}

		private void CheckRequiredRegistryKeys()
		{
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\WSMAN"))
			{
				if (registryKey == null || registryKey.GetValue("StackVersion") == null)
				{
					this.WriteWarning(Strings.ErrorWSManRegistryCorrupted);
					this.isWSManInstalled = false;
				}
				else
				{
					if (registryKey.GetValue("UpdatedConfig") == null)
					{
						base.RebuildWSManRegistry();
						this.needToRestartWSMan = true;
					}
					using (RegistryKey registryKey2 = registryKey.OpenSubKey("Listener"))
					{
						if (registryKey2 == null)
						{
							base.RebuildWSManRegistry();
							this.needToRestartWSMan = true;
						}
					}
					using (RegistryKey registryKey3 = registryKey.OpenSubKey("Plugin"))
					{
						if (registryKey3 == null || registryKey3.SubKeyCount < 3)
						{
							base.RebuildWSManRegistry();
							this.needToRestartWSMan = true;
						}
					}
				}
			}
		}

		private void CheckIISConfigurationFile()
		{
			bool flag = false;
			bool flag2 = false;
			using (ServerManager serverManager = new ServerManager())
			{
				Site site = serverManager.Sites[base.DefaultSiteName];
				if (serverManager.ApplicationPools["MSExchangePowerShellAppPool"] == null)
				{
					base.WriteError(new InvalidOperationException(Strings.ErrorPowerShellVdirAppPoolNotExist("MSExchangePowerShellAppPool", base.IISConfigFilePath)), ErrorCategory.InvalidOperation, null);
				}
				if (site.Applications["/PowerShell"] == null)
				{
					base.WriteError(new InvalidOperationException(Strings.ErrorApplicationNotExist("PowerShell", base.IISConfigFilePath)), ErrorCategory.InvalidOperation, null);
				}
				if (site.Applications["/PowerShell"].VirtualDirectories["/"] == null)
				{
					base.WriteError(new InvalidOperationException(Strings.ErrorVdirNotExisted("PowerShell", base.IISConfigFilePath)), ErrorCategory.InvalidOperation, null);
				}
				if (this.DataCenterCAS)
				{
					if (site.Applications["/PowerShell-LiveID"] == null)
					{
						base.WriteError(new InvalidOperationException(Strings.ErrorApplicationNotExist("PowerShell-LiveID", base.IISConfigFilePath)), ErrorCategory.InvalidOperation, null);
					}
					if (site.Applications["/PowerShell-LiveID"].VirtualDirectories["/"] == null)
					{
						base.WriteError(new InvalidOperationException(Strings.ErrorVdirNotExisted("PowerShell-LiveID", base.IISConfigFilePath)), ErrorCategory.InvalidOperation, null);
					}
				}
				bool flag3 = false;
				for (int i = 0; i < site.Bindings.Count; i++)
				{
					Binding binding = site.Bindings[i];
					if (string.Equals("https", binding.Protocol, StringComparison.InvariantCultureIgnoreCase))
					{
						flag3 = true;
						break;
					}
				}
				if (!flag3)
				{
					base.WriteError(new InvalidOperationException(Strings.ErrorHttpsBindingNotExist(base.IISConfigFilePath)), ErrorCategory.InvalidOperation, null);
				}
				SectionGroup sectionGroup = serverManager.GetApplicationHostConfiguration().RootSectionGroup.SectionGroups["system.webServer"];
				SectionDefinition sectionDefinition = sectionGroup.Sections["system.management.wsmanagement.config"];
				if (sectionDefinition == null)
				{
					base.BackupIISConfig();
					base.WriteVerbose(Strings.VerboseAddWSManConfigSection(base.IISConfigFilePath));
					sectionDefinition = sectionGroup.Sections.Add("system.management.wsmanagement.config");
					sectionDefinition.OverrideModeDefault = "Allow";
				}
				else if (!string.Equals(sectionDefinition.OverrideModeDefault, "Allow", StringComparison.InvariantCultureIgnoreCase))
				{
					base.BackupIISConfig();
					sectionDefinition.OverrideModeDefault = "Allow";
				}
				ConfigurationElementCollection collection = serverManager.GetApplicationHostConfiguration().GetSection("system.webServer/globalModules").GetCollection();
				ConfigurationElement configurationElement = null;
				for (int j = 0; j < collection.Count; j++)
				{
					ConfigurationElement configurationElement2 = collection[j];
					object attributeValue = configurationElement2.GetAttributeValue("name");
					if (attributeValue != null && string.Equals("WSMan", attributeValue.ToString(), StringComparison.InvariantCultureIgnoreCase))
					{
						configurationElement = configurationElement2;
						break;
					}
				}
				if (configurationElement == null)
				{
					flag2 = true;
				}
				else
				{
					object value = configurationElement.Attributes["image"].Value;
					if (value == null || !string.Equals(value.ToString(), base.WSManModuleFilePath, StringComparison.InvariantCultureIgnoreCase))
					{
						flag = true;
					}
				}
				if (base.IISBackupFileName != null)
				{
					serverManager.CommitChanges();
				}
			}
			if (flag || flag2)
			{
				base.BackupIISConfig();
			}
			if (flag)
			{
				string arguments = "uninstall module WSMan";
				base.WriteVerbose(Strings.VerboseUninstallWSManModule("WSMan"));
				base.ExecuteCmd(base.AppcmdPath, arguments, base.InetsrvPath, true, true);
			}
			if (flag2)
			{
				string arguments2 = "unlock config -section:system.webServer/modules";
				base.WriteVerbose(Strings.VerboseUnlockingModulesSection);
				base.ExecuteCmd(base.AppcmdPath, arguments2, base.InetsrvPath, true, true);
				arguments2 = "install module /name:WSMan /image:" + base.WSManModuleFilePath + " /add:false";
				base.WriteVerbose(Strings.VerboseInstallWSManModule("WSMan"));
				base.ExecuteCmd(base.AppcmdPath, arguments2, base.InetsrvPath, true, true);
			}
		}

		private void InstallKerberosAuthenticationModule()
		{
			bool flag = true;
			using (ServerManager serverManager = new ServerManager())
			{
				ConfigurationElementCollection collection = serverManager.GetApplicationHostConfiguration().GetSection("system.webServer/globalModules").GetCollection();
				ConfigurationElement configurationElement = null;
				for (int i = 0; i < collection.Count; i++)
				{
					ConfigurationElement configurationElement2 = collection[i];
					object attributeValue = configurationElement2.GetAttributeValue("name");
					if (attributeValue != null && string.Equals("Kerbauth", attributeValue.ToString(), StringComparison.OrdinalIgnoreCase))
					{
						configurationElement = configurationElement2;
						break;
					}
				}
				if (configurationElement != null)
				{
					flag = false;
				}
			}
			if (flag)
			{
				string arguments = "install module /name:kerbauth /image:\"" + base.KerberosAuthModuleFilePath + "\" /add:false";
				base.WriteVerbose(Strings.VerboseInstallKerberosAuthenticationModule(base.KerberosAuthModuleFilePath));
				base.ExecuteCmd(base.AppcmdPath, arguments, base.InetsrvPath, true, false);
			}
		}

		private const string strWSManRegistryRoot = "Software\\Microsoft\\Windows\\CurrentVersion\\WSMAN";

		private const string strWSManStackVersion = "StackVersion";

		private const string strWSManUpdatedConfig = "UpdatedConfig";

		private const string strWSManListener = "Listener";

		private const string strWSManPlugin = "Plugin";

		private const string paramDataCenterCAS = "DataCenterCAS";

		private const string paramEnableKerbAuth = "EnableKerbAuth";

		private const string strAllow = "Allow";

		private const string strWSManAuthPlugin = "Microsoft.Exchange.AuthorizationPlugin.dll";

		private bool needToRestartWSMan;

		private bool isWSManInstalled = true;
	}
}
