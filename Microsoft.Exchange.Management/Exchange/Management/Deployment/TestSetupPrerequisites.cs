using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Analysis;
using Microsoft.Exchange.Management.Analysis.Features;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Deployment
{
	[Cmdlet("test", "SetupPrerequisites")]
	public class TestSetupPrerequisites : Task
	{
		public TestSetupPrerequisites()
		{
			this.IsDatacenter = false;
		}

		[Parameter(Mandatory = true)]
		public string[] Roles
		{
			get
			{
				return (string[])base.Fields["Roles"];
			}
			set
			{
				base.Fields["Roles"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string ScanType
		{
			get
			{
				return (string)base.Fields["ScanType"];
			}
			set
			{
				base.Fields["ScanType"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter IsDatacenter
		{
			get
			{
				return (SwitchParameter)base.Fields["IsDatacenter"];
			}
			set
			{
				base.Fields["IsDatacenter"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public LocalLongFullPath TargetDir
		{
			get
			{
				return (LocalLongFullPath)base.Fields["TargetDir"];
			}
			set
			{
				base.Fields["TargetDir"] = value;
			}
		}

		[Parameter(Mandatory = true)]
		public string ExchangeVersion
		{
			get
			{
				return (string)base.Fields["ExchangeVersion"];
			}
			set
			{
				base.Fields["ExchangeVersion"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string[] SetupRoles
		{
			get
			{
				return (string[])base.Fields["SetupRoles"];
			}
			set
			{
				base.Fields["SetupRoles"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int ADAMPort
		{
			get
			{
				return (int)base.Fields["ADAMPort"];
			}
			set
			{
				base.Fields["ADAMPort"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int ADAMSSLPort
		{
			get
			{
				return (int)base.Fields["ADAMSSLPort"];
			}
			set
			{
				base.Fields["ADAMSSLPort"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool CreatePublicDB
		{
			get
			{
				return (bool)base.Fields["CreatePublicDB"];
			}
			set
			{
				base.Fields["CreatePublicDB"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool? CustomerFeedbackEnabled
		{
			get
			{
				return (bool?)base.Fields["CustomerFeedbackEnabled"];
			}
			set
			{
				base.Fields["CustomerFeedbackEnabled"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string NewProvisionedServerName
		{
			get
			{
				return (string)base.Fields["NewProvisionedServerName"];
			}
			set
			{
				base.Fields["NewProvisionedServerName"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string RemoveProvisionedServerName
		{
			get
			{
				return (string)base.Fields["RemoveProvisionedServerName"];
			}
			set
			{
				base.Fields["RemoveProvisionedServerName"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string PrepareDomain
		{
			get
			{
				return (string)base.Fields["PrepareDomain"];
			}
			set
			{
				base.Fields["PrepareDomain"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool PrepareSCT
		{
			get
			{
				return (bool)base.Fields["PrepareSCT"];
			}
			set
			{
				base.Fields["PrepareSCT"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool PrepareOrganization
		{
			get
			{
				return (bool)base.Fields["PrepareOrganization"];
			}
			set
			{
				base.Fields["PrepareOrganization"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool PrepareSchema
		{
			get
			{
				return (bool)base.Fields["PrepareSchema"];
			}
			set
			{
				base.Fields["PrepareSchema"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool PrepareAllDomains
		{
			get
			{
				return (bool)base.Fields["PrepareAllDomains"];
			}
			set
			{
				base.Fields["PrepareAllDomains"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string ADInitError
		{
			get
			{
				return (string)base.Fields["ADInitError"];
			}
			set
			{
				base.Fields["ADInitError"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string LanguagePackDir
		{
			get
			{
				return (string)base.Fields["LanguagePackDir"];
			}
			set
			{
				base.Fields["LanguagePackDir"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool LanguagesAvailableToInstall
		{
			get
			{
				return (bool)base.Fields["LanguagesAvailableToInstall"];
			}
			set
			{
				base.Fields["LanguagesAvailableToInstall"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool LanguagePackVersioning
		{
			get
			{
				return (bool)base.Fields["LanguagePackVersioning"];
			}
			set
			{
				base.Fields["LanguagePackVersioning"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool SufficientLanguagePackDiskSpace
		{
			get
			{
				return (bool)base.Fields["SufficientLanguagePackDiskSpace"];
			}
			set
			{
				base.Fields["SufficientLanguagePackDiskSpace"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool LanguagePacksInstalled
		{
			get
			{
				return (bool)base.Fields["LanguagePacksInstalled"];
			}
			set
			{
				base.Fields["LanguagePacksInstalled"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string AlreadyInstallUMLanguages
		{
			get
			{
				return (string)base.Fields["AlreadyInstallUMLanguages"];
			}
			set
			{
				base.Fields["AlreadyInstallUMLanguages"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool? ActiveDirectorySplitPermissions
		{
			get
			{
				return (bool?)base.Fields["ActiveDirectorySplitPermissions"];
			}
			set
			{
				base.Fields["ActiveDirectorySplitPermissions"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Fqdn DomainController
		{
			get
			{
				return (Fqdn)base.Fields["DomainController"];
			}
			set
			{
				base.Fields["DomainController"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool HostingDeploymentEnabled
		{
			get
			{
				return (bool)base.Fields["HostingDeploymentEnabled"];
			}
			set
			{
				base.Fields["HostingDeploymentEnabled"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string PathToDCHybridConfigFile
		{
			get
			{
				return (string)base.Fields["PathToDCHybridConfigFile"];
			}
			set
			{
				base.Fields["PathToDCHybridConfigFile"] = value;
			}
		}

		internal SetupRole PrereqSetupRoles
		{
			get
			{
				SetupRole setupRole = SetupRole.None;
				if (this.Roles != null)
				{
					foreach (string value in this.Roles)
					{
						if (!string.IsNullOrEmpty(value))
						{
							try
							{
								setupRole |= (SetupRole)Enum.Parse(typeof(SetupRole), value, true);
							}
							catch (ArgumentException)
							{
							}
							catch (OverflowException)
							{
							}
						}
					}
				}
				return setupRole;
			}
		}

		internal SetupMode PrereqSetupMode
		{
			get
			{
				SetupMode result = SetupMode.None;
				string a;
				if (!string.IsNullOrEmpty(this.ScanType) && (a = this.ScanType.ToLower()) != null)
				{
					if (!(a == "precheckdr"))
					{
						if (!(a == "precheckinstall"))
						{
							if (!(a == "precheckuninstall"))
							{
								if (a == "precheckupgrade")
								{
									result = SetupMode.Upgrade;
								}
							}
							else
							{
								result = SetupMode.Uninstall;
							}
						}
						else
						{
							result = SetupMode.Install;
						}
					}
					else
					{
						result = SetupMode.DisasterRecovery;
					}
				}
				return result;
			}
		}

		protected override void InternalBeginProcessing()
		{
			if (!base.Fields.Contains("TargetDir"))
			{
				this.TargetDir = null;
			}
			if (!base.Fields.Contains("ExchangeVersion"))
			{
				this.ExchangeVersion = "1.0.0.0";
			}
			if (!base.Fields.Contains("SetupRoles"))
			{
				this.SetupRoles = null;
			}
			if (!base.Fields.Contains("ADAMPort"))
			{
				this.ADAMPort = 50389;
			}
			if (!base.Fields.Contains("ADAMSSLPort"))
			{
				this.ADAMSSLPort = 50636;
			}
			if (!base.Fields.Contains("CreatePublicDB"))
			{
				this.CreatePublicDB = false;
			}
			if (!base.Fields.Contains("CustomerFeedbackEnabled"))
			{
				this.CustomerFeedbackEnabled = null;
			}
			if (!base.Fields.Contains("NewProvisionedServerName"))
			{
				this.NewProvisionedServerName = "";
			}
			if (!base.Fields.Contains("RemoveProvisionedServerName"))
			{
				this.RemoveProvisionedServerName = "";
			}
			if (!base.Fields.Contains("PrepareDomain"))
			{
				this.PrepareDomain = "";
			}
			if (!base.Fields.Contains("PrepareSCT"))
			{
				this.PrepareSCT = false;
			}
			if (!base.Fields.Contains("PrepareOrganization"))
			{
				this.PrepareOrganization = false;
			}
			if (!base.Fields.Contains("PrepareSchema"))
			{
				this.PrepareSchema = false;
			}
			if (!base.Fields.Contains("PrepareAllDomains"))
			{
				this.PrepareAllDomains = false;
			}
			if (!base.Fields.Contains("ADInitError"))
			{
				this.ADInitError = "";
			}
			if (!base.Fields.Contains("LanguagePackDir"))
			{
				this.LanguagePackDir = "";
			}
			if (!base.Fields.Contains("LanguagesAvailableToInstall"))
			{
				this.LanguagesAvailableToInstall = false;
			}
			if (!base.Fields.Contains("SufficientLanguagePackDiskSpace"))
			{
				this.SufficientLanguagePackDiskSpace = false;
			}
			if (!base.Fields.Contains("LanguagePacksInstalled"))
			{
				this.LanguagePacksInstalled = false;
			}
			if (!base.Fields.Contains("AlreadyInstallUMLanguages"))
			{
				this.AlreadyInstallUMLanguages = "";
			}
			if (!base.Fields.Contains("LanguagePackVersioning"))
			{
				this.LanguagePackVersioning = true;
			}
			if (!base.Fields.Contains("ActiveDirectorySplitPermissions"))
			{
				this.ActiveDirectorySplitPermissions = null;
			}
			if (!base.Fields.Contains("HostingDeploymentEnabled"))
			{
				this.HostingDeploymentEnabled = false;
			}
			if (!base.Fields.Contains("PathToDCHybridConfigFile"))
			{
				this.PathToDCHybridConfigFile = "";
			}
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.FullyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 468, "InternalProcessRecord", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Deployment\\TestSetupPrerequisites.cs");
			string targetDir = (this.TargetDir != null) ? this.TargetDir.ToString() : null;
			Version exchangeVersion = new Version(this.ExchangeVersion);
			int adamport = this.ADAMPort;
			int adamsslport = this.ADAMSSLPort;
			bool createPublicDB = this.CreatePublicDB;
			bool customerFeedbackEnabled = this.CustomerFeedbackEnabled != null && this.CustomerFeedbackEnabled.Value;
			string newProvisionedServerName = this.NewProvisionedServerName;
			string removeProvisionedServerName = this.RemoveProvisionedServerName;
			Fqdn fqdn = topologyConfigurationSession.ServerSettings.PreferredGlobalCatalog(TopologyProvider.LocalForestFqdn);
			GlobalParameters globalParameters = new GlobalParameters(targetDir, exchangeVersion, adamport, adamsslport, createPublicDB, customerFeedbackEnabled, newProvisionedServerName, removeProvisionedServerName, (fqdn != null) ? fqdn : "", (this.DomainController != null) ? this.DomainController.ToString() : "", this.PrepareDomain, this.PrepareSCT, this.PrepareOrganization, this.PrepareSchema, this.PrepareAllDomains, this.ADInitError, this.LanguagePackDir, this.LanguagesAvailableToInstall, this.SufficientLanguagePackDiskSpace, this.LanguagePacksInstalled, this.AlreadyInstallUMLanguages, this.LanguagePackVersioning, this.ActiveDirectorySplitPermissions != null && this.ActiveDirectorySplitPermissions.Value, this.SetupRoles, this.GetIgnoreFilesInUseFlag(), this.HostingDeploymentEnabled, this.PathToDCHybridConfigFile, this.IsDatacenter);
			try
			{
				TaskLogger.LogAllAsInfo = true;
				SetupPrereqChecks setupPrereqChecks = new SetupPrereqChecks(this.PrereqSetupMode, this.PrereqSetupRoles, globalParameters);
				setupPrereqChecks.DoCheckPrereqs(new Action<int>(this.WriteProgressRecord), this);
			}
			finally
			{
				TaskLogger.LogAllAsInfo = false;
			}
			TaskLogger.LogExit();
		}

		private bool GetIgnoreFilesInUseFlag()
		{
			if (this.IsDatacenter)
			{
				ParameterCollection parameterCollection = RolesUtility.ReadSetupParameters(this.IsDatacenter);
				foreach (Parameter parameter in parameterCollection)
				{
					if (string.Equals(parameter.Name, "IgnoreFilesInUse", StringComparison.InvariantCultureIgnoreCase))
					{
						return (bool)parameter.EffectiveValue;
					}
				}
				return false;
			}
			return false;
		}

		private void WriteProgressRecord(int percentage)
		{
			base.WriteProgress(Strings.SetupPrereqAnalysis, (percentage >= 100) ? Strings.SetupPrereqAnalysisCompleted : Strings.SetupPrereqAnalysisInProgress, percentage);
		}
	}
}
