using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Configuration.MonadDataProvider;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Deployment;

namespace Microsoft.Exchange.Setup.Common
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class RootDataHandler : SetupSingleTaskDataHandler
	{
		public LongPath SetupSourceDir
		{
			get
			{
				return base.SetupContext.SourceDir;
			}
		}

		public bool IsCleanMachine
		{
			get
			{
				return base.SetupContext.IsCleanMachine;
			}
		}

		public RootDataHandler(Dictionary<string, object> parsedArguments, MonadConnection connection) : base(null, "", connection)
		{
			this.SuppressValidateSourceDir = false;
			this.hasReadData = false;
			this.parsedArguments = parsedArguments;
		}

		public RootDataHandler(ISetupContext context, MonadConnection connection) : base(context, "", connection)
		{
			this.SuppressValidateSourceDir = false;
			this.hasReadData = false;
		}

		public void LoadState(UserChoiceState userChoiceState)
		{
			if (userChoiceState != null)
			{
				userChoiceState.WriteToContext(base.SetupContext, this.ModeDatahandler);
			}
		}

		protected override void OnSaveData()
		{
			UserChoiceState.DeleteFile();
			base.OnSaveData();
		}

		protected override void OnReadData()
		{
			SetupLogger.TraceEnter(new object[0]);
			if (!this.hasReadData)
			{
				if (base.SetupContext == null)
				{
					base.SetupContext = new SetupContext(this.parsedArguments);
				}
				this.hasReadData = true;
				this.OnReadDataFinished();
			}
			SetupLogger.TraceExit();
		}

		public event EventHandler ReadDataFinished;

		private void OnReadDataFinished()
		{
			if (this.ReadDataFinished != null)
			{
				this.ReadDataFinished(this, EventArgs.Empty);
			}
		}

		public string CurrentWizardPageName
		{
			get
			{
				return base.SetupContext.CurrentWizardPageName;
			}
			set
			{
				base.SetupContext.CurrentWizardPageName = value;
			}
		}

		public bool IsRestoredFromPreviousState
		{
			get
			{
				return base.SetupContext.IsRestoredFromPreviousState;
			}
			set
			{
				base.SetupContext.IsRestoredFromPreviousState = value;
			}
		}

		public bool IsUmLanguagePackOperation
		{
			get
			{
				return base.SetupContext.IsUmLanguagePackOperation;
			}
			set
			{
				base.SetupContext.IsUmLanguagePackOperation = value;
			}
		}

		public List<CultureInfo> InstalledUMLanguagePacks
		{
			get
			{
				return base.SetupContext.InstalledUMLanguagePacks;
			}
		}

		public List<CultureInfo> SelectedCultures
		{
			get
			{
				return base.SetupContext.SelectedCultures;
			}
		}

		public RoleCollection RequestedRoles
		{
			get
			{
				return base.SetupContext.RequestedRoles;
			}
		}

		public List<string> RequestedInstallableUnits
		{
			get
			{
				return this.ModeDatahandler.SelectedInstallableUnits;
			}
		}

		public InstallationModes Mode
		{
			get
			{
				return base.SetupContext.InstallationMode;
			}
			set
			{
				base.SetupContext.InstallationMode = value;
			}
		}

		public bool IsLanguagePackOperation
		{
			get
			{
				return base.SetupContext.IsLanguagePackOperation;
			}
			set
			{
				base.SetupContext.IsLanguagePackOperation = value;
			}
		}

		public LongPath LanguagePackPath
		{
			get
			{
				return base.SetupContext.LanguagePackPath;
			}
			set
			{
				base.SetupContext.LanguagePackPath = value;
			}
		}

		public ModeDataHandler ModeDatahandler
		{
			get
			{
				ModeDataHandler result = null;
				switch (this.Mode)
				{
				case InstallationModes.Install:
					if (base.SetupContext.IsUmLanguagePackOperation)
					{
						if (this.addUmLanguagePackModeDataHandler == null)
						{
							this.addUmLanguagePackModeDataHandler = new AddUmLanguagePackModeDataHandler(base.SetupContext, base.Connection);
						}
						result = this.addUmLanguagePackModeDataHandler;
					}
					else
					{
						if (this.installModeDataHandler == null)
						{
							this.installModeDataHandler = new InstallModeDataHandler(base.SetupContext, base.Connection);
						}
						result = this.installModeDataHandler;
					}
					break;
				case InstallationModes.BuildToBuildUpgrade:
					if (this.upgradeModeDataHandler == null)
					{
						this.upgradeModeDataHandler = new UpgradeModeDataHandler(base.SetupContext, base.Connection);
					}
					result = this.upgradeModeDataHandler;
					break;
				case InstallationModes.DisasterRecovery:
					if (this.disasterRecoveryModeDataHandler == null)
					{
						this.disasterRecoveryModeDataHandler = new DisasterRecoveryModeDataHandler(base.SetupContext, base.Connection);
					}
					result = this.disasterRecoveryModeDataHandler;
					break;
				case InstallationModes.Uninstall:
					if (base.SetupContext.IsUmLanguagePackOperation)
					{
						if (this.removeUmLanguagePackModeDataHandler == null)
						{
							this.removeUmLanguagePackModeDataHandler = new RemoveUmLanguagePackModeDataHandler(base.SetupContext, base.Connection);
						}
						result = this.removeUmLanguagePackModeDataHandler;
					}
					else
					{
						if (this.uninstallModeDataHandler == null)
						{
							this.uninstallModeDataHandler = new UninstallModeDataHandler(base.SetupContext, base.Connection);
						}
						result = this.uninstallModeDataHandler;
					}
					break;
				}
				return result;
			}
		}

		public bool TreatPreReqErrorsAsWarningsInDC
		{
			get
			{
				return base.SetupContext.IsDatacenter && (!base.SetupContext.IsDatacenter || base.SetupContext.TreatPreReqErrorsAsWarnings);
			}
		}

		public string IntroTitle
		{
			get
			{
				string result = string.Empty;
				switch (this.Mode)
				{
				case InstallationModes.Install:
				case InstallationModes.Uninstall:
					result = (this.IsCleanMachine ? Strings.FreshIntroduction : Strings.MaintenanceIntroduction);
					break;
				case InstallationModes.BuildToBuildUpgrade:
					result = Strings.UpgradeIntroduction;
					break;
				}
				return result;
			}
		}

		public string IntroDescription
		{
			get
			{
				string result = string.Empty;
				switch (this.Mode)
				{
				case InstallationModes.Install:
					result = (this.IsCleanMachine ? Strings.FreshIntroductionText : Strings.AddIntroductionText);
					break;
				case InstallationModes.BuildToBuildUpgrade:
					result = Strings.UpgradeIntroductionText;
					break;
				case InstallationModes.Uninstall:
					result = ((this.InstalledUMLanguagePacks.Count > 1) ? Strings.RemoveUnifiedMessagingServerDescription : Strings.RemoveIntroductionText);
					break;
				}
				return result;
			}
		}

		public string LicenseAgreementShortDescription
		{
			get
			{
				string result = string.Empty;
				switch (this.Mode)
				{
				case InstallationModes.Install:
					result = Strings.InstallationLicenseAgreementShortDescription;
					break;
				case InstallationModes.BuildToBuildUpgrade:
					result = Strings.UpgradeLicenseAgreementShortDescription;
					break;
				}
				return result;
			}
		}

		public string ProgressDescription
		{
			get
			{
				string result = string.Empty;
				switch (this.Mode)
				{
				case InstallationModes.Install:
					result = Strings.AddProgressText;
					break;
				case InstallationModes.BuildToBuildUpgrade:
					result = Strings.UpgradeProgressText;
					break;
				case InstallationModes.Uninstall:
					result = Strings.RemoveProgressText;
					break;
				}
				return result;
			}
		}

		public bool SuppressValidateSourceDir { get; set; }

		public override ValidationError[] Validate()
		{
			SetupLogger.TraceEnter(new object[0]);
			List<ValidationError> list = new List<ValidationError>();
			if (this.IsCleanMachine)
			{
				if (!base.SetupContext.HasPrepareADParameters)
				{
					if (base.SetupContext.InstalledRolesLocal.Count == 0 && base.SetupContext.InstalledRolesAD.Count == 0)
					{
						if (this.Mode != InstallationModes.Install && !base.SetupContext.HasRemoveProvisionedServerParameters)
						{
							list.Add(new SetupValidationError(Strings.ModeErrorForFreshInstall));
						}
					}
					else if (base.SetupContext.IsBackupKeyPresent)
					{
						if (this.Mode != InstallationModes.BuildToBuildUpgrade)
						{
							list.Add(new SetupValidationError(Strings.ModeErrorForUpgrade));
						}
					}
					else if (this.Mode != InstallationModes.DisasterRecovery)
					{
						list.Add(new SetupValidationError(Strings.ModeErrorForDisasterRecovery));
					}
				}
			}
			else
			{
				bool flag = true;
				foreach (Role role in base.SetupContext.InstalledRolesLocal)
				{
					if (!base.SetupContext.IsUnpacked(role.RoleName))
					{
						flag = false;
						break;
					}
				}
				if (flag && base.SetupContext.UnpackedRoles.Count > 0 && !base.SetupContext.IsUnpacked("AdminToolsRole"))
				{
					flag = false;
				}
				if (!flag && this.Mode != InstallationModes.DisasterRecovery)
				{
					list.Add(new SetupValidationError(Strings.ModeErrorForDisasterRecovery));
				}
			}
			if (base.SetupContext.ParsedArguments.ContainsKey("domaincontroller") && base.SetupContext.IsInstalledLocal("GatewayRole"))
			{
				list.Add(new SetupValidationError(Strings.ParameterNotValidForCurrentRoles("domaincontroller")));
			}
			if (base.SetupContext.ExchangeOrganizationExists && base.SetupContext.ParsedArguments.ContainsKey("organizationname") && !base.SetupContext.OrganizationName.EscapedName.Equals(base.SetupContext.OrganizationNameFoundInAD.EscapedName, StringComparison.InvariantCultureIgnoreCase))
			{
				list.Add(new SetupValidationError(Strings.OrganizationAlreadyExists(base.SetupContext.OrganizationNameFoundInAD.EscapedName)));
			}
			if (base.SetupContext.RegistryError != null)
			{
				list.Add(new SetupValidationError(base.SetupContext.RegistryError.LocalizedString));
			}
			if (base.SetupContext.OrganizationNameValidationException != null)
			{
				list.Add(new SetupValidationError(base.SetupContext.OrganizationNameValidationException.LocalizedString));
			}
			if (base.SetupContext.DomainController != null)
			{
				list.AddRange(this.ModeDatahandler.CheckForADErrors(false));
			}
			list.AddRange(base.Validate());
			SetupLogger.TraceExit();
			return list.ToArray();
		}

		public override void UpdateWorkUnits()
		{
			SetupLogger.TraceEnter(new object[0]);
			base.DataHandlers.Clear();
			base.DataHandlers.Add(this.ModeDatahandler);
			base.UpdateWorkUnits();
			foreach (WorkUnit workUnit in base.WorkUnits)
			{
				workUnit.CanShowExecutedCommand = false;
			}
			SetupLogger.Log(Strings.RootDataHandlerCount(base.DataHandlers.Count));
			SetupLogger.TraceExit();
		}

		public override string CompletionDescription
		{
			get
			{
				return this.ModeDatahandler.CompletionDescription;
			}
		}

		public override string CompletionStatus
		{
			get
			{
				return this.ModeDatahandler.CompletionStatus;
			}
		}

		private bool hasReadData;

		private Dictionary<string, object> parsedArguments;

		private InstallModeDataHandler installModeDataHandler;

		private UninstallModeDataHandler uninstallModeDataHandler;

		private DisasterRecoveryModeDataHandler disasterRecoveryModeDataHandler;

		private UpgradeModeDataHandler upgradeModeDataHandler;

		private AddUmLanguagePackModeDataHandler addUmLanguagePackModeDataHandler;

		private RemoveUmLanguagePackModeDataHandler removeUmLanguagePackModeDataHandler;
	}
}
