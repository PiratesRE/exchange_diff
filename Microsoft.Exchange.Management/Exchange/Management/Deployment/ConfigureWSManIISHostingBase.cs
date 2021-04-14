using System;
using System.IO;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Net;
using Microsoft.Web.Administration;

namespace Microsoft.Exchange.Management.Deployment
{
	public abstract class ConfigureWSManIISHostingBase : Task
	{
		protected string System32Path
		{
			get
			{
				return this.system32Path;
			}
		}

		protected string WinrmFilePath
		{
			get
			{
				return this.winrmFilePath;
			}
		}

		protected string NetFilePath
		{
			get
			{
				return this.netFilePath;
			}
		}

		protected string PSVdirPhysicalPath
		{
			get
			{
				return this.psVdirPhysicalPath;
			}
		}

		protected string PSDCCasVdirPhysicalPath
		{
			get
			{
				return this.psDCCasVdirPhysicalPath;
			}
		}

		protected string InetsrvPath
		{
			get
			{
				return this.inetsrvPath;
			}
		}

		protected string AppcmdPath
		{
			get
			{
				return this.appcmdPath;
			}
		}

		protected string IISConfigFilePath
		{
			get
			{
				return this.iisConfigFilePath;
			}
		}

		protected string WSManCfgFilePath
		{
			get
			{
				return this.wsmanCfgFilePath;
			}
		}

		protected string WSManDCCasCfgFilePath
		{
			get
			{
				return this.wsmanDCCasCfgFilePath;
			}
		}

		protected string WSManCfgSchemaFileOriginalPath
		{
			get
			{
				return this.wsmanCfgSchemaFileOriginalPath;
			}
		}

		protected string WSManCfgSchemaFilePath
		{
			get
			{
				return this.wsmanCfgSchemaFilePath;
			}
		}

		protected string WSManModuleFilePath
		{
			get
			{
				return this.wsmanModuleFilePath;
			}
		}

		protected string KerberosAuthModuleFilePath
		{
			get
			{
				return this.kerbModuleFilePath;
			}
		}

		protected string IISBackupFileName
		{
			get
			{
				return this.iisBackupFileName;
			}
		}

		protected string DefaultSiteName
		{
			get
			{
				return this.defaultSiteName;
			}
			set
			{
				this.defaultSiteName = value;
			}
		}

		protected string PSVdirName
		{
			get
			{
				if (this.psVdirName == null)
				{
					this.psVdirName = this.DefaultSiteName + "/PowerShell";
				}
				return this.psVdirName;
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
			this.ResolveConfigurationFilePaths();
			TaskLogger.LogExit();
		}

		private void ResolveConfigurationFilePaths()
		{
			this.system32Path = Path.Combine(Environment.GetEnvironmentVariable("windir"), "system32");
			this.winrmFilePath = Path.Combine(this.system32Path, "winrm.cmd");
			this.netFilePath = Path.Combine(this.system32Path, "net.exe");
			this.inetsrvPath = Path.Combine(this.system32Path, "inetsrv");
			this.appcmdPath = Path.Combine(this.inetsrvPath, "appcmd.exe");
			string path = Path.Combine(this.inetsrvPath, "config");
			this.iisConfigFilePath = Path.Combine(path, "ApplicationHost.Config");
			this.wsmanCfgSchemaFilePath = Path.Combine(path, "schema");
			this.wsmanCfgSchemaFilePath = Path.Combine(this.wsmanCfgSchemaFilePath, "wsmanconfig_schema.xml");
			string path2 = Path.Combine(ConfigurationContext.Setup.InstallPath, "ClientAccess");
			this.psVdirPhysicalPath = Path.Combine(path2, "PowerShell");
			this.psDCCasVdirPhysicalPath = Path.Combine(path2, "PowerShell-LiveID");
			this.wsmanCfgFilePath = Path.Combine(this.psVdirPhysicalPath, "web.config");
			this.wsmanDCCasCfgFilePath = Path.Combine(this.psDCCasVdirPhysicalPath, "web.config");
			this.wsmanCfgSchemaFileOriginalPath = Path.Combine(this.system32Path, "wsmanconfig_schema.xml");
			this.wsmanModuleFilePath = Path.Combine(this.system32Path, "wsmsvc.dll");
			this.kerbModuleFilePath = Path.Combine(ConfigurationContext.Setup.BinPath, "kerbauth.dll");
		}

		protected void RebuildWSManRegistry()
		{
			this.RestartWSManService();
			string arguments = "i restore winrm/config";
			base.WriteVerbose(Strings.VerboseRebuildWSManRegistry);
			this.ExecuteCmd(this.WinrmFilePath, arguments, this.InetsrvPath, false, false);
		}

		protected void RestartWSManService()
		{
			base.WriteVerbose(Strings.VerboseRestartWSManService);
			this.ExecuteCmd(this.NetFilePath, "stop winrm", this.InetsrvPath, false, false);
			this.ExecuteCmd(this.NetFilePath, "start winrm", this.InetsrvPath, true, false);
		}

		protected void RestartDefaultWebSite()
		{
			base.WriteVerbose(Strings.VerboseRestartDefaultWebSite);
			using (ServerManager serverManager = new ServerManager())
			{
				Site site = serverManager.Sites[this.DefaultSiteName];
				try
				{
					site.Stop();
					site.Start();
				}
				catch (ServerManagerException ex)
				{
					base.WriteWarning(ex.Message);
				}
			}
		}

		protected void RestartWSManAppPool(string appPool)
		{
			base.WriteVerbose(Strings.VerboseRestartWSManAppPool(appPool));
			string arguments = "stop apppool /apppool.name:" + appPool;
			string arguments2 = "start apppool /apppool.name:" + appPool;
			this.ExecuteCmd(this.AppcmdPath, arguments, this.InetsrvPath, false, false);
			this.ExecuteCmd(this.AppcmdPath, arguments2, this.InetsrvPath, true, true);
		}

		protected void BackupIISConfig()
		{
			if (this.iisBackupFileName == null)
			{
				this.iisBackupFileName = "IISBackup" + ExDateTime.Now.ToFileTimeUtc().ToString();
				base.WriteVerbose(Strings.VerboseBackupIISConfig(this.iisBackupFileName));
				string arguments = "add backup " + this.iisBackupFileName;
				this.ExecuteCmd(this.AppcmdPath, arguments, this.InetsrvPath, true, false);
			}
		}

		protected void RestoreOriginalIISConfig()
		{
			base.WriteVerbose(Strings.VerboseRestoreIISConfig(this.iisBackupFileName));
			string arguments = "restore backup " + this.iisBackupFileName;
			this.ExecuteCmd(this.AppcmdPath, arguments, this.InetsrvPath, false, false);
		}

		protected void OpenHttpPortsOnFirewall()
		{
			string text = Path.Combine(this.System32Path, "netsh.exe");
			if (File.Exists(text))
			{
				string arguments = "firewall set portopening TCP 80 HTTP";
				base.WriteVerbose(Strings.VerboseOpenFirewallPort("80", "HTTP"));
				this.ExecuteCmd(text, arguments, this.InetsrvPath, false, false);
				string arguments2 = "firewall set portopening TCP 443 HTTPS";
				base.WriteVerbose(Strings.VerboseOpenFirewallPort("443", "HTTPS"));
				this.ExecuteCmd(text, arguments2, this.InetsrvPath, false, false);
				return;
			}
			this.WriteWarning(Strings.ErrorSystemFileNotFound(text));
		}

		protected void EnableBasicAuthForWSMan()
		{
			string arguments = "p winrm/config/client/auth @{Basic=\"true\"}";
			base.WriteVerbose(Strings.VerboseEnableBasicAuthForWSMan);
			this.ExecuteCmd(this.WinrmFilePath, arguments, this.InetsrvPath, false, false);
		}

		protected void ExecuteCmd(string appPath, string arguments, string executionPath, bool writeError, bool needToRestoreIIS)
		{
			string output;
			string errors;
			int num = ProcessRunner.Run(appPath, arguments, -1, executionPath, out output, out errors);
			if (num == 0)
			{
				base.WriteVerbose(Strings.VerboseExecuteCmd(appPath, arguments, output));
				return;
			}
			if (!writeError)
			{
				this.WriteWarning(Strings.ErrorCmdExecutionFailed(appPath, arguments, output, errors, num.ToString()));
				return;
			}
			if (needToRestoreIIS && this.iisBackupFileName != null)
			{
				this.RestoreOriginalIISConfig();
			}
			base.WriteError(new InvalidOperationException(Strings.ErrorCmdExecutionFailed(appPath, arguments, output, errors, num.ToString())), ErrorCategory.InvalidOperation, null);
		}

		protected const string strPSVdirName = "PowerShell";

		protected const string strPSDCCasVdirName = "PowerShell-LiveID";

		protected const string strPSVdirAppPool = "MSExchangePowerShellAppPool";

		protected const string strPSDCCasVdirAppPool = "MSExchangePowerShellLiveIDAppPool";

		protected const string strWSManModuleName = "WSMan";

		protected const string strKerbAuthModuleName = "Kerbauth";

		protected const string strWSManConfigSectionName = "system.management.wsmanagement.config";

		private const string strClientAccess = "ClientAccess";

		private const string strBin = "Bin";

		private const string strWSManCfgFileName = "web.config";

		private const string strWSManCfgSchemaFileName = "wsmanconfig_schema.xml";

		private const string strWSManModuleFileName = "wsmsvc.dll";

		private const string strKerbauthModuleFileName = "kerbauth.dll";

		private const string strApplicationHostFileName = "ApplicationHost.Config";

		private const string strHttpPort = "80";

		private const string strHttpsPort = "443";

		private string system32Path;

		private string winrmFilePath;

		private string netFilePath;

		private string psVdirPhysicalPath;

		private string psDCCasVdirPhysicalPath;

		private string inetsrvPath;

		private string appcmdPath;

		private string iisConfigFilePath;

		private string wsmanCfgFilePath;

		private string wsmanDCCasCfgFilePath;

		private string wsmanCfgSchemaFileOriginalPath;

		private string wsmanCfgSchemaFilePath;

		private string wsmanModuleFilePath;

		private string kerbModuleFilePath;

		private string iisBackupFileName;

		private string defaultSiteName;

		private string psVdirName;
	}
}
