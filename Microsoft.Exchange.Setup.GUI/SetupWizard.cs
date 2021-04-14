using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Microsoft.Exchange.Configuration.MonadDataProvider;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Deployment;
using Microsoft.Exchange.Management.Deployment.HybridConfigurationDetection;
using Microsoft.Exchange.Setup.Common;
using Microsoft.Exchange.Setup.CommonBase;
using Microsoft.Exchange.Setup.ExSetupUI;

namespace Microsoft.Exchange.Setup.GUI
{
	internal class SetupWizard : LauncherBase
	{
		public SetupWizard()
		{
		}

		public SetupWizard(ISetupContext context, MonadConnection connection) : base(context, connection)
		{
		}

		internal IHybridConfigurationDetection HybridConfigurationDetectionProvider
		{
			get
			{
				if (this.hybridConfigurationDetectionProvider == null)
				{
					BridgeLogger logger = new BridgeLogger(SetupLogger.Logger);
					this.hybridConfigurationDetectionProvider = new HybridConfigurationDetection(logger);
				}
				return this.hybridConfigurationDetectionProvider;
			}
		}

		public int Main(IList<SetupWizardPage> pages, SetupBase theApp)
		{
			this.pages = pages;
			return base.MainCore(SetupBase.SetupArgs, theApp.ParsedArguments, theApp);
		}

		internal int Main(IList<SetupWizardPage> pages, string[] parsedArguments, SetupBase theApp)
		{
			this.pages = pages;
			return this.ProcessRunInternal(parsedArguments, theApp);
		}

		protected override int ProcessRunInternal(string[] args, SetupBase theApp)
		{
			if (args.Length == 0)
			{
				if (base.RootDataHandler.IsCleanMachine)
				{
					SetupLogger.LogError(new LocalizedException(new LocalizedString(Strings.CannotRunWithoutParameter)));
					theApp.WriteError(Strings.CannotRunWithoutParameter);
				}
				else
				{
					SetupLogger.LogError(new LocalizedException(new LocalizedString(Strings.CannotRunInstalled)));
					theApp.WriteError(Strings.CannotRunInstalled);
				}
				return 1;
			}
			base.RootDataHandler.UpdateWorkUnits();
			base.RootDataHandler.ModeDatahandler.IgnoreValidatingRoleChanges = true;
			List<ValidationError> list = new List<ValidationError>();
			list.AddRange(base.RootDataHandler.Validate());
			base.RootDataHandler.ModeDatahandler.IgnoreValidatingRoleChanges = false;
			if (list.Count > 0)
			{
				foreach (ValidationError validationError in list)
				{
					theApp.ReportError(validationError.Description);
				}
				ExSetupUI.ExitApplication(ExitCode.Error);
				return 1;
			}
			try
			{
				this.PopulateWizard(base.RootDataHandler);
			}
			catch (LanguageBundleAlreadyInstalledException ex)
			{
				SetupLogger.LogError(ex);
				MessageBoxHelper.ShowError(ex.Message);
				ExSetupUI.ExitApplication(ExitCode.Error);
				return 1;
			}
			catch (UMLanguagePackAlreadyInstalledException ex2)
			{
				SetupLogger.LogError(ex2);
				MessageBoxHelper.ShowError(ex2.Message);
				ExSetupUI.ExitApplication(ExitCode.Error);
				return 1;
			}
			catch (PartiallyConfiguredException ex3)
			{
				SetupLogger.LogError(ex3);
				MessageBoxHelper.ShowError(ex3.Message);
				ExSetupUI.ExitApplication(ExitCode.Error);
				return 1;
			}
			return 0;
		}

		protected override void CreateRootDataHandler(Dictionary<string, object> arguments, MonadConnection connection)
		{
			base.CreateRootDataHandler(arguments, connection);
			base.RootDataHandler.ReadDataFinished += delegate(object param0, EventArgs param1)
			{
				if (base.RootDataHandler.IsCleanMachine && base.RootDataHandler.Mode == InstallationModes.Install && !base.RootDataHandler.IsUmLanguagePackOperation && !base.RootDataHandler.IsLanguagePackOperation)
				{
					base.RootDataHandler.RequestedRoles.Clear();
				}
				base.RootDataHandler.SuppressValidateSourceDir = true;
			};
		}

		private void PopulateWizard(RootDataHandler rootDataHandler)
		{
			if (SetupWizard.IsPartiallyConfigured && (base.RootDataHandler.Mode == InstallationModes.Install || base.RootDataHandler.Mode == InstallationModes.BuildToBuildUpgrade))
			{
				this.pages.Add(new IncompleteInstallationDetectedPage());
				this.pages.Add(new SetupProgressPage(base.RootDataHandler));
				return;
			}
			if (base.RootDataHandler == null || base.RootDataHandler.Mode == InstallationModes.BuildToBuildUpgrade)
			{
				this.PopulateWizardForInstallAndUpgrade(base.RootDataHandler);
			}
			if (base.RootDataHandler.Mode == InstallationModes.Install)
			{
				if (base.RootDataHandler.IsCleanMachine)
				{
					this.PopulateWizardForInstallAndUpgrade(base.RootDataHandler);
				}
				else
				{
					if (!base.RootDataHandler.IsUmLanguagePackOperation && !base.RootDataHandler.IsLanguagePackOperation)
					{
						this.pages.Add(new AddRemoveServerRolePage(base.RootDataHandler));
					}
					if (base.RootDataHandler.IsUmLanguagePackOperation || base.RootDataHandler.IsLanguagePackOperation)
					{
						if (base.RootDataHandler.IsLanguagePackOperation)
						{
							if (!base.RootDataHandler.RequestedInstallableUnits.Contains("LanguagePacks"))
							{
								throw new LanguageBundleAlreadyInstalledException();
							}
						}
						else
						{
							for (int i = 0; i < base.RootDataHandler.SelectedCultures.Count; i++)
							{
								StringBuilder stringBuilder = new StringBuilder();
								CultureInfo cultureInfo = base.RootDataHandler.SelectedCultures[i];
								stringBuilder.Append(cultureInfo.DisplayName);
								if (i < base.RootDataHandler.SelectedCultures.Count - 1)
								{
									stringBuilder.Append(" ");
								}
								if (base.RootDataHandler.InstalledUMLanguagePacks.Contains(cultureInfo))
								{
									throw new UMLanguagePackAlreadyInstalledException(cultureInfo.DisplayName);
								}
								SetupLogger.Log(Strings.UnifiedMessagingInstallSummaryText(stringBuilder.ToString()));
							}
						}
						this.pages.Add(new EULAPage(base.RootDataHandler));
					}
				}
			}
			if (base.RootDataHandler.Mode == InstallationModes.Uninstall)
			{
				if (SetupWizard.IsPartiallyConfigured)
				{
					throw new PartiallyConfiguredException();
				}
				this.pages.Add(new UninstallSummaryPage());
			}
			if (base.RootDataHandler != null && this.pages.Count > 0)
			{
				if (base.RootDataHandler.Mode != InstallationModes.Uninstall || !base.RootDataHandler.IsUmLanguagePackOperation)
				{
					this.pages.Add(new PreCheckPage(base.RootDataHandler));
				}
				this.pages.Add(new SetupProgressPage(base.RootDataHandler));
				if ((base.RootDataHandler.Mode == InstallationModes.Install || base.RootDataHandler.Mode == InstallationModes.BuildToBuildUpgrade) && !base.RootDataHandler.IsUmLanguagePackOperation && !base.RootDataHandler.IsLanguagePackOperation)
				{
					this.pages.Add(new SetupCompletedPage(base.RootDataHandler));
				}
			}
		}

		private void PopulateWizardForInstallAndUpgrade(RootDataHandler rootDataHandler)
		{
			this.pages.Add(new WelcomePage(base.RootDataHandler));
			ModeDataHandler modeDataHandler = (rootDataHandler == null) ? null : rootDataHandler.ModeDatahandler;
			if (modeDataHandler != null)
			{
				modeDataHandler.IgnoreValidatingRoleChanges = true;
			}
			InstallModeDataHandler installModeDataHandler = modeDataHandler as InstallModeDataHandler;
			if (base.RootDataHandler == null || (base.RootDataHandler.IsCleanMachine && base.RootDataHandler.Mode == InstallationModes.Install))
			{
				this.pages.Add(new EULAPage(base.RootDataHandler));
				bool flag = false;
				if (modeDataHandler.IsADPrepTasksRequired)
				{
					try
					{
						flag = this.HybridConfigurationDetectionProvider.RunOnPremisesHybridTest();
					}
					catch (Exception e)
					{
						SetupLogger.LogError(e);
						MessageBoxHelper.ShowError(Strings.SetupWillNotContinue);
						ExSetupUI.ExitApplication(ExitCode.Error);
					}
					if (flag)
					{
						this.pages.Add(new HybridConfigurationCredentialPage());
						this.pages.Add(new HybridConfigurationStatusPage(this.HybridConfigurationDetectionProvider));
					}
				}
				this.pages.Add(new RecommendedSettingsPage(installModeDataHandler));
				this.pages.Add(new RoleSelectionPage(base.RootDataHandler));
				this.pages.Add(new InstallationSpaceAndLocationPage(base.RootDataHandler));
				if (modeDataHandler != null)
				{
					if (!installModeDataHandler.ExchangeOrganizationExists)
					{
						installModeDataHandler.InstallOrgCfgDataHandler.OrganizationName = new OrganizationName("First Organization");
						this.pages.Add(new ExchangeOrganizationPage(installModeDataHandler));
					}
					if (installModeDataHandler.SelectedInstallableUnits.Contains("BridgeheadRole"))
					{
						this.pages.Add(new ProtectionSettingsPage(installModeDataHandler));
						return;
					}
				}
			}
			else
			{
				this.pages.Add(new EULAPage(base.RootDataHandler));
			}
		}

		private IList<SetupWizardPage> pages;

		private HybridConfigurationDetection hybridConfigurationDetectionProvider;
	}
}
