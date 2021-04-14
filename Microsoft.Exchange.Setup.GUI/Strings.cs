using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Setup.GUI
{
	internal static class Strings
	{
		static Strings()
		{
			Strings.stringIDs.Add(2116473977U, "DiskSpaceAllocationTitle");
			Strings.stringIDs.Add(3281626719U, "ExchangeOrganizationPageTitle");
			Strings.stringIDs.Add(3919765759U, "ExchangeOrganizationName");
			Strings.stringIDs.Add(846084549U, "AddRemoveServerRolePageTitle");
			Strings.stringIDs.Add(3060531531U, "ActiveDirectorySplitPermissions");
			Strings.stringIDs.Add(1689769030U, "HybridConfigurationEnterCredentialLabelText");
			Strings.stringIDs.Add(3411794799U, "LanguageBundleCannotRunInstall");
			Strings.stringIDs.Add(2014064605U, "SetupWillNotContinue");
			Strings.stringIDs.Add(4058204109U, "InvalidCredentials");
			Strings.stringIDs.Add(4099293073U, "ProtectionSettingsPageTitle");
			Strings.stringIDs.Add(3750498566U, "HybridConfigurationCredentialsFinished");
			Strings.stringIDs.Add(2235382510U, "MailboxRole");
			Strings.stringIDs.Add(479704857U, "NotAcceptEULAText");
			Strings.stringIDs.Add(1732413395U, "SetupCompletedPageEACText");
			Strings.stringIDs.Add(1256637832U, "NoEndUserLicenseAgreement");
			Strings.stringIDs.Add(3534072816U, "ReadMoreAboutUsageLinkText");
			Strings.stringIDs.Add(3606962380U, "AdminToolsRole");
			Strings.stringIDs.Add(3537994597U, "CannotRunWithoutParameter");
			Strings.stringIDs.Add(3619730788U, "AlreadyInstalled");
			Strings.stringIDs.Add(2079091583U, "ProtectionSettingsYesNoLabelText");
			Strings.stringIDs.Add(3057699019U, "DiskSpaceCapacityUnit");
			Strings.stringIDs.Add(4252207216U, "BrowseInstallationPathButtonText");
			Strings.stringIDs.Add(2549121878U, "UninstallWelcomeTitle");
			Strings.stringIDs.Add(1785456672U, "CannotRunInstalled");
			Strings.stringIDs.Add(2998280118U, "Introduction");
			Strings.stringIDs.Add(2886682374U, "DisableMalwareNoRadioButtonText");
			Strings.stringIDs.Add(1954427109U, "OpenExchangeAdminCenterCheckBoxText");
			Strings.stringIDs.Add(3767526541U, "SetupProgressPageTitle");
			Strings.stringIDs.Add(3093934206U, "PlanDeploymentLinkLabel2Link");
			Strings.stringIDs.Add(593134813U, "SetupCompletedPageTitle");
			Strings.stringIDs.Add(3055213040U, "PreCheckPageTitle");
			Strings.stringIDs.Add(1873628080U, "SetupCompletedPageText");
			Strings.stringIDs.Add(3109201548U, "AcceptEULAText");
			Strings.stringIDs.Add(894991490U, "PreCheckDescriptionText");
			Strings.stringIDs.Add(1883156336U, "MicrosoftExchangeServer");
			Strings.stringIDs.Add(4102307169U, "btnPrint");
			Strings.stringIDs.Add(1286725797U, "InstallationPathTitle");
			Strings.stringIDs.Add(3088556892U, "HybridConfigurationEnterCredentialForAccountLabelText");
			Strings.stringIDs.Add(1923571563U, "EULAPageText");
			Strings.stringIDs.Add(1608263701U, "UseRecommendedSettingsDescription");
			Strings.stringIDs.Add(574235583U, "RoleSelectionPageTitle");
			Strings.stringIDs.Add(544429720U, "PlanDeploymentLabel");
			Strings.stringIDs.Add(3708323460U, "DisableMalwareYesRadioButtonText");
			Strings.stringIDs.Add(516668141U, "PlanDeploymentLinkLabel1Link");
			Strings.stringIDs.Add(1290564665U, "InstallWindowsPrereqCheckBoxText");
			Strings.stringIDs.Add(1269906618U, "DoNotUseSettingsRadioButtonText");
			Strings.stringIDs.Add(3706612651U, "ExchangeOrganizationNameError");
			Strings.stringIDs.Add(2419732849U, "UninstallPageTitle");
			Strings.stringIDs.Add(4050885606U, "FatalError");
			Strings.stringIDs.Add(1499387377U, "ClientAccessRole");
			Strings.stringIDs.Add(257944843U, "RoleSelectionLabelText");
			Strings.stringIDs.Add(1180534834U, "InvalidInstallationLocation");
			Strings.stringIDs.Add(367349671U, "RequiredDiskSpaceDescriptionText");
			Strings.stringIDs.Add(614788730U, "PartiallyConfiguredCannotRunUnInstall");
			Strings.stringIDs.Add(1319431375U, "PlanDeploymentLinkLabel2Text");
			Strings.stringIDs.Add(3712487120U, "SetupCompleted");
			Strings.stringIDs.Add(2106877831U, "PreCheckFail");
			Strings.stringIDs.Add(2352415574U, "DescriptionTitle");
			Strings.stringIDs.Add(3771677870U, "PrereqNoteText");
			Strings.stringIDs.Add(3256873468U, "EulaLabelText");
			Strings.stringIDs.Add(3418450723U, "HybridConfigurationSystemExceptionText");
			Strings.stringIDs.Add(1319299077U, "FolderBrowserDialogDescriptionText");
			Strings.stringIDs.Add(267801009U, "HybridConfigurationCredentialsChecks");
			Strings.stringIDs.Add(4176057518U, "NotUseRecommendedSettingsDescription");
			Strings.stringIDs.Add(2263082167U, "CancelMessageBoxMessage");
			Strings.stringIDs.Add(1634809916U, "UseSettingsRadioButtonText");
			Strings.stringIDs.Add(4109048229U, "ActiveDirectorySplitPermissionsDescription");
			Strings.stringIDs.Add(3379112507U, "IncompleteInstallationDetectedPageTitle");
			Strings.stringIDs.Add(2442879412U, "PasswordLabelText");
			Strings.stringIDs.Add(163021091U, "PlanDeploymentLinkLabel3Link");
			Strings.stringIDs.Add(3553989528U, "PreCheckSuccess");
			Strings.stringIDs.Add(4171294242U, "UninstallWelcomeDiscription");
			Strings.stringIDs.Add(1850975354U, "HybridConfigurationCredentialPageTitle");
			Strings.stringIDs.Add(1938529548U, "SetupFailedPrintEULA");
			Strings.stringIDs.Add(4101971563U, "HybridConfigurationCredentialsFailed");
			Strings.stringIDs.Add(2026922639U, "IncompleteInstallationDetectedSummaryLabelText");
			Strings.stringIDs.Add(396014875U, "HybridConfigurationStatusPageTitle");
			Strings.stringIDs.Add(851625806U, "RecommendedSettingsTitle");
			Strings.stringIDs.Add(3846284970U, "PlanDeploymentLinkLabel3Text");
			Strings.stringIDs.Add(1612790765U, "ProtectionSettingsLabelText");
			Strings.stringIDs.Add(3004026682U, "SetupWizardCaption");
			Strings.stringIDs.Add(2339675629U, "UserNameLabelText");
			Strings.stringIDs.Add(1635017634U, "ReadMoreAboutCheckingForErrorSolutionsLinkText");
			Strings.stringIDs.Add(2230563552U, "PlanDeploymentLinkLabel1Text");
			Strings.stringIDs.Add(622196391U, "AvailableDiskSpaceDescriptionText");
			Strings.stringIDs.Add(307954797U, "InstallationSpaceAndLocationPageTitle");
			Strings.stringIDs.Add(2343429092U, "SetupCompletedPageLinkText");
			Strings.stringIDs.Add(3664928351U, "EdgeRole");
		}

		public static LocalizedString DiskSpaceAllocationTitle
		{
			get
			{
				return new LocalizedString("DiskSpaceAllocationTitle", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExchangeOrganizationPageTitle
		{
			get
			{
				return new LocalizedString("ExchangeOrganizationPageTitle", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExchangeOrganizationName
		{
			get
			{
				return new LocalizedString("ExchangeOrganizationName", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AddRemoveServerRolePageTitle
		{
			get
			{
				return new LocalizedString("AddRemoveServerRolePageTitle", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ActiveDirectorySplitPermissions
		{
			get
			{
				return new LocalizedString("ActiveDirectorySplitPermissions", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HybridConfigurationEnterCredentialLabelText
		{
			get
			{
				return new LocalizedString("HybridConfigurationEnterCredentialLabelText", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LanguageBundleCannotRunInstall
		{
			get
			{
				return new LocalizedString("LanguageBundleCannotRunInstall", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SetupWillNotContinue
		{
			get
			{
				return new LocalizedString("SetupWillNotContinue", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidCredentials
		{
			get
			{
				return new LocalizedString("InvalidCredentials", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ProtectionSettingsPageTitle
		{
			get
			{
				return new LocalizedString("ProtectionSettingsPageTitle", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UnifiedMessagingInstallSummaryText(string culture)
		{
			return new LocalizedString("UnifiedMessagingInstallSummaryText", Strings.ResourceManager, new object[]
			{
				culture
			});
		}

		public static LocalizedString HybridConfigurationCredentialsFinished
		{
			get
			{
				return new LocalizedString("HybridConfigurationCredentialsFinished", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MailboxRole
		{
			get
			{
				return new LocalizedString("MailboxRole", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NotAcceptEULAText
		{
			get
			{
				return new LocalizedString("NotAcceptEULAText", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SetupCompletedPageEACText
		{
			get
			{
				return new LocalizedString("SetupCompletedPageEACText", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NoEndUserLicenseAgreement
		{
			get
			{
				return new LocalizedString("NoEndUserLicenseAgreement", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReadMoreAboutUsageLinkText
		{
			get
			{
				return new LocalizedString("ReadMoreAboutUsageLinkText", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AdminToolsRole
		{
			get
			{
				return new LocalizedString("AdminToolsRole", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CannotRunWithoutParameter
		{
			get
			{
				return new LocalizedString("CannotRunWithoutParameter", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AlreadyInstalled
		{
			get
			{
				return new LocalizedString("AlreadyInstalled", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ProtectionSettingsYesNoLabelText
		{
			get
			{
				return new LocalizedString("ProtectionSettingsYesNoLabelText", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DiskSpaceCapacityUnit
		{
			get
			{
				return new LocalizedString("DiskSpaceCapacityUnit", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString BrowseInstallationPathButtonText
		{
			get
			{
				return new LocalizedString("BrowseInstallationPathButtonText", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UninstallWelcomeTitle
		{
			get
			{
				return new LocalizedString("UninstallWelcomeTitle", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CannotRunInstalled
		{
			get
			{
				return new LocalizedString("CannotRunInstalled", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Introduction
		{
			get
			{
				return new LocalizedString("Introduction", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DisableMalwareNoRadioButtonText
		{
			get
			{
				return new LocalizedString("DisableMalwareNoRadioButtonText", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString OpenExchangeAdminCenterCheckBoxText
		{
			get
			{
				return new LocalizedString("OpenExchangeAdminCenterCheckBoxText", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SetupProgressPageTitle
		{
			get
			{
				return new LocalizedString("SetupProgressPageTitle", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PlanDeploymentLinkLabel2Link
		{
			get
			{
				return new LocalizedString("PlanDeploymentLinkLabel2Link", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SetupCompletedPageTitle
		{
			get
			{
				return new LocalizedString("SetupCompletedPageTitle", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PreCheckPageTitle
		{
			get
			{
				return new LocalizedString("PreCheckPageTitle", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SetupCompletedPageText
		{
			get
			{
				return new LocalizedString("SetupCompletedPageText", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AcceptEULAText
		{
			get
			{
				return new LocalizedString("AcceptEULAText", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PreCheckDescriptionText
		{
			get
			{
				return new LocalizedString("PreCheckDescriptionText", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MicrosoftExchangeServer
		{
			get
			{
				return new LocalizedString("MicrosoftExchangeServer", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString btnPrint
		{
			get
			{
				return new LocalizedString("btnPrint", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InstallationPathTitle
		{
			get
			{
				return new LocalizedString("InstallationPathTitle", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HybridConfigurationEnterCredentialForAccountLabelText
		{
			get
			{
				return new LocalizedString("HybridConfigurationEnterCredentialForAccountLabelText", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EULAPageText
		{
			get
			{
				return new LocalizedString("EULAPageText", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CurrentStep(string currentStep, string totalSteps, string currentTask)
		{
			return new LocalizedString("CurrentStep", Strings.ResourceManager, new object[]
			{
				currentStep,
				totalSteps,
				currentTask
			});
		}

		public static LocalizedString UseRecommendedSettingsDescription
		{
			get
			{
				return new LocalizedString("UseRecommendedSettingsDescription", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoleSelectionPageTitle
		{
			get
			{
				return new LocalizedString("RoleSelectionPageTitle", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PlanDeploymentLabel
		{
			get
			{
				return new LocalizedString("PlanDeploymentLabel", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DisableMalwareYesRadioButtonText
		{
			get
			{
				return new LocalizedString("DisableMalwareYesRadioButtonText", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PlanDeploymentLinkLabel1Link
		{
			get
			{
				return new LocalizedString("PlanDeploymentLinkLabel1Link", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InstallWindowsPrereqCheckBoxText
		{
			get
			{
				return new LocalizedString("InstallWindowsPrereqCheckBoxText", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DoNotUseSettingsRadioButtonText
		{
			get
			{
				return new LocalizedString("DoNotUseSettingsRadioButtonText", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExchangeOrganizationNameError
		{
			get
			{
				return new LocalizedString("ExchangeOrganizationNameError", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UninstallPageTitle
		{
			get
			{
				return new LocalizedString("UninstallPageTitle", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FatalError
		{
			get
			{
				return new LocalizedString("FatalError", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClientAccessRole
		{
			get
			{
				return new LocalizedString("ClientAccessRole", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoleSelectionLabelText
		{
			get
			{
				return new LocalizedString("RoleSelectionLabelText", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidInstallationLocation
		{
			get
			{
				return new LocalizedString("InvalidInstallationLocation", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RequiredDiskSpaceDescriptionText
		{
			get
			{
				return new LocalizedString("RequiredDiskSpaceDescriptionText", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PartiallyConfiguredCannotRunUnInstall
		{
			get
			{
				return new LocalizedString("PartiallyConfiguredCannotRunUnInstall", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PlanDeploymentLinkLabel2Text
		{
			get
			{
				return new LocalizedString("PlanDeploymentLinkLabel2Text", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SetupCompleted
		{
			get
			{
				return new LocalizedString("SetupCompleted", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PreCheckFail
		{
			get
			{
				return new LocalizedString("PreCheckFail", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DescriptionTitle
		{
			get
			{
				return new LocalizedString("DescriptionTitle", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PrereqNoteText
		{
			get
			{
				return new LocalizedString("PrereqNoteText", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EulaLabelText
		{
			get
			{
				return new LocalizedString("EulaLabelText", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HybridConfigurationSystemExceptionText
		{
			get
			{
				return new LocalizedString("HybridConfigurationSystemExceptionText", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FolderBrowserDialogDescriptionText
		{
			get
			{
				return new LocalizedString("FolderBrowserDialogDescriptionText", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UnifiedMessagingCannotRunInstall(string culture)
		{
			return new LocalizedString("UnifiedMessagingCannotRunInstall", Strings.ResourceManager, new object[]
			{
				culture
			});
		}

		public static LocalizedString HybridConfigurationCredentialsChecks
		{
			get
			{
				return new LocalizedString("HybridConfigurationCredentialsChecks", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NotUseRecommendedSettingsDescription
		{
			get
			{
				return new LocalizedString("NotUseRecommendedSettingsDescription", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CancelMessageBoxMessage
		{
			get
			{
				return new LocalizedString("CancelMessageBoxMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UseSettingsRadioButtonText
		{
			get
			{
				return new LocalizedString("UseSettingsRadioButtonText", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ActiveDirectorySplitPermissionsDescription
		{
			get
			{
				return new LocalizedString("ActiveDirectorySplitPermissionsDescription", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IncompleteInstallationDetectedPageTitle
		{
			get
			{
				return new LocalizedString("IncompleteInstallationDetectedPageTitle", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PasswordLabelText
		{
			get
			{
				return new LocalizedString("PasswordLabelText", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PlanDeploymentLinkLabel3Link
		{
			get
			{
				return new LocalizedString("PlanDeploymentLinkLabel3Link", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PreCheckSuccess
		{
			get
			{
				return new LocalizedString("PreCheckSuccess", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UninstallWelcomeDiscription
		{
			get
			{
				return new LocalizedString("UninstallWelcomeDiscription", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HybridConfigurationCredentialPageTitle
		{
			get
			{
				return new LocalizedString("HybridConfigurationCredentialPageTitle", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SetupFailedPrintEULA
		{
			get
			{
				return new LocalizedString("SetupFailedPrintEULA", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HybridConfigurationCredentialsFailed
		{
			get
			{
				return new LocalizedString("HybridConfigurationCredentialsFailed", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IncompleteInstallationDetectedSummaryLabelText
		{
			get
			{
				return new LocalizedString("IncompleteInstallationDetectedSummaryLabelText", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HybridConfigurationStatusPageTitle
		{
			get
			{
				return new LocalizedString("HybridConfigurationStatusPageTitle", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RecommendedSettingsTitle
		{
			get
			{
				return new LocalizedString("RecommendedSettingsTitle", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PlanDeploymentLinkLabel3Text
		{
			get
			{
				return new LocalizedString("PlanDeploymentLinkLabel3Text", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ProtectionSettingsLabelText
		{
			get
			{
				return new LocalizedString("ProtectionSettingsLabelText", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SetupWizardCaption
		{
			get
			{
				return new LocalizedString("SetupWizardCaption", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UserNameLabelText
		{
			get
			{
				return new LocalizedString("UserNameLabelText", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReadMoreAboutCheckingForErrorSolutionsLinkText
		{
			get
			{
				return new LocalizedString("ReadMoreAboutCheckingForErrorSolutionsLinkText", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PlanDeploymentLinkLabel1Text
		{
			get
			{
				return new LocalizedString("PlanDeploymentLinkLabel1Text", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PageLoaded(string pageName)
		{
			return new LocalizedString("PageLoaded", Strings.ResourceManager, new object[]
			{
				pageName
			});
		}

		public static LocalizedString AvailableDiskSpaceDescriptionText
		{
			get
			{
				return new LocalizedString("AvailableDiskSpaceDescriptionText", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SetupNotFoundInSourceDirError(string fileName)
		{
			return new LocalizedString("SetupNotFoundInSourceDirError", Strings.ResourceManager, new object[]
			{
				fileName
			});
		}

		public static LocalizedString InstallationSpaceAndLocationPageTitle
		{
			get
			{
				return new LocalizedString("InstallationSpaceAndLocationPageTitle", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SetupCompletedPageLinkText
		{
			get
			{
				return new LocalizedString("SetupCompletedPageLinkText", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EdgeRole
		{
			get
			{
				return new LocalizedString("EdgeRole", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GetLocalizedString(Strings.IDs key)
		{
			return new LocalizedString(Strings.stringIDs[(uint)key], Strings.ResourceManager, new object[0]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(88);

		private static ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.Setup.GUI.Strings", typeof(Strings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			DiskSpaceAllocationTitle = 2116473977U,
			ExchangeOrganizationPageTitle = 3281626719U,
			ExchangeOrganizationName = 3919765759U,
			AddRemoveServerRolePageTitle = 846084549U,
			ActiveDirectorySplitPermissions = 3060531531U,
			HybridConfigurationEnterCredentialLabelText = 1689769030U,
			LanguageBundleCannotRunInstall = 3411794799U,
			SetupWillNotContinue = 2014064605U,
			InvalidCredentials = 4058204109U,
			ProtectionSettingsPageTitle = 4099293073U,
			HybridConfigurationCredentialsFinished = 3750498566U,
			MailboxRole = 2235382510U,
			NotAcceptEULAText = 479704857U,
			SetupCompletedPageEACText = 1732413395U,
			NoEndUserLicenseAgreement = 1256637832U,
			ReadMoreAboutUsageLinkText = 3534072816U,
			AdminToolsRole = 3606962380U,
			CannotRunWithoutParameter = 3537994597U,
			AlreadyInstalled = 3619730788U,
			ProtectionSettingsYesNoLabelText = 2079091583U,
			DiskSpaceCapacityUnit = 3057699019U,
			BrowseInstallationPathButtonText = 4252207216U,
			UninstallWelcomeTitle = 2549121878U,
			CannotRunInstalled = 1785456672U,
			Introduction = 2998280118U,
			DisableMalwareNoRadioButtonText = 2886682374U,
			OpenExchangeAdminCenterCheckBoxText = 1954427109U,
			SetupProgressPageTitle = 3767526541U,
			PlanDeploymentLinkLabel2Link = 3093934206U,
			SetupCompletedPageTitle = 593134813U,
			PreCheckPageTitle = 3055213040U,
			SetupCompletedPageText = 1873628080U,
			AcceptEULAText = 3109201548U,
			PreCheckDescriptionText = 894991490U,
			MicrosoftExchangeServer = 1883156336U,
			btnPrint = 4102307169U,
			InstallationPathTitle = 1286725797U,
			HybridConfigurationEnterCredentialForAccountLabelText = 3088556892U,
			EULAPageText = 1923571563U,
			UseRecommendedSettingsDescription = 1608263701U,
			RoleSelectionPageTitle = 574235583U,
			PlanDeploymentLabel = 544429720U,
			DisableMalwareYesRadioButtonText = 3708323460U,
			PlanDeploymentLinkLabel1Link = 516668141U,
			InstallWindowsPrereqCheckBoxText = 1290564665U,
			DoNotUseSettingsRadioButtonText = 1269906618U,
			ExchangeOrganizationNameError = 3706612651U,
			UninstallPageTitle = 2419732849U,
			FatalError = 4050885606U,
			ClientAccessRole = 1499387377U,
			RoleSelectionLabelText = 257944843U,
			InvalidInstallationLocation = 1180534834U,
			RequiredDiskSpaceDescriptionText = 367349671U,
			PartiallyConfiguredCannotRunUnInstall = 614788730U,
			PlanDeploymentLinkLabel2Text = 1319431375U,
			SetupCompleted = 3712487120U,
			PreCheckFail = 2106877831U,
			DescriptionTitle = 2352415574U,
			PrereqNoteText = 3771677870U,
			EulaLabelText = 3256873468U,
			HybridConfigurationSystemExceptionText = 3418450723U,
			FolderBrowserDialogDescriptionText = 1319299077U,
			HybridConfigurationCredentialsChecks = 267801009U,
			NotUseRecommendedSettingsDescription = 4176057518U,
			CancelMessageBoxMessage = 2263082167U,
			UseSettingsRadioButtonText = 1634809916U,
			ActiveDirectorySplitPermissionsDescription = 4109048229U,
			IncompleteInstallationDetectedPageTitle = 3379112507U,
			PasswordLabelText = 2442879412U,
			PlanDeploymentLinkLabel3Link = 163021091U,
			PreCheckSuccess = 3553989528U,
			UninstallWelcomeDiscription = 4171294242U,
			HybridConfigurationCredentialPageTitle = 1850975354U,
			SetupFailedPrintEULA = 1938529548U,
			HybridConfigurationCredentialsFailed = 4101971563U,
			IncompleteInstallationDetectedSummaryLabelText = 2026922639U,
			HybridConfigurationStatusPageTitle = 396014875U,
			RecommendedSettingsTitle = 851625806U,
			PlanDeploymentLinkLabel3Text = 3846284970U,
			ProtectionSettingsLabelText = 1612790765U,
			SetupWizardCaption = 3004026682U,
			UserNameLabelText = 2339675629U,
			ReadMoreAboutCheckingForErrorSolutionsLinkText = 1635017634U,
			PlanDeploymentLinkLabel1Text = 2230563552U,
			AvailableDiskSpaceDescriptionText = 622196391U,
			InstallationSpaceAndLocationPageTitle = 307954797U,
			SetupCompletedPageLinkText = 2343429092U,
			EdgeRole = 3664928351U
		}

		private enum ParamIDs
		{
			UnifiedMessagingInstallSummaryText,
			CurrentStep,
			UnifiedMessagingCannotRunInstall,
			PageLoaded,
			SetupNotFoundInSourceDirError
		}
	}
}
