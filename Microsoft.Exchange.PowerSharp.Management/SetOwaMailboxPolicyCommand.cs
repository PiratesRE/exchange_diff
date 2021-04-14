using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class SetOwaMailboxPolicyCommand : SyntheticCommandWithPipelineInputNoOutput<OwaMailboxPolicy>
	{
		private SetOwaMailboxPolicyCommand() : base("Set-OwaMailboxPolicy")
		{
		}

		public SetOwaMailboxPolicyCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual SetOwaMailboxPolicyCommand SetParameters(SetOwaMailboxPolicyCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual SetOwaMailboxPolicyCommand SetParameters(SetOwaMailboxPolicyCommand.IdentityParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual SwitchParameter IsDefault
			{
				set
				{
					base.PowerSharpParameters["IsDefault"] = value;
				}
			}

			public virtual SwitchParameter DisableFacebook
			{
				set
				{
					base.PowerSharpParameters["DisableFacebook"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual bool DirectFileAccessOnPublicComputersEnabled
			{
				set
				{
					base.PowerSharpParameters["DirectFileAccessOnPublicComputersEnabled"] = value;
				}
			}

			public virtual bool DirectFileAccessOnPrivateComputersEnabled
			{
				set
				{
					base.PowerSharpParameters["DirectFileAccessOnPrivateComputersEnabled"] = value;
				}
			}

			public virtual bool WebReadyDocumentViewingOnPublicComputersEnabled
			{
				set
				{
					base.PowerSharpParameters["WebReadyDocumentViewingOnPublicComputersEnabled"] = value;
				}
			}

			public virtual bool WebReadyDocumentViewingOnPrivateComputersEnabled
			{
				set
				{
					base.PowerSharpParameters["WebReadyDocumentViewingOnPrivateComputersEnabled"] = value;
				}
			}

			public virtual bool ForceWebReadyDocumentViewingFirstOnPublicComputers
			{
				set
				{
					base.PowerSharpParameters["ForceWebReadyDocumentViewingFirstOnPublicComputers"] = value;
				}
			}

			public virtual bool ForceWebReadyDocumentViewingFirstOnPrivateComputers
			{
				set
				{
					base.PowerSharpParameters["ForceWebReadyDocumentViewingFirstOnPrivateComputers"] = value;
				}
			}

			public virtual bool WacViewingOnPublicComputersEnabled
			{
				set
				{
					base.PowerSharpParameters["WacViewingOnPublicComputersEnabled"] = value;
				}
			}

			public virtual bool WacViewingOnPrivateComputersEnabled
			{
				set
				{
					base.PowerSharpParameters["WacViewingOnPrivateComputersEnabled"] = value;
				}
			}

			public virtual bool ForceWacViewingFirstOnPublicComputers
			{
				set
				{
					base.PowerSharpParameters["ForceWacViewingFirstOnPublicComputers"] = value;
				}
			}

			public virtual bool ForceWacViewingFirstOnPrivateComputers
			{
				set
				{
					base.PowerSharpParameters["ForceWacViewingFirstOnPrivateComputers"] = value;
				}
			}

			public virtual AttachmentBlockingActions ActionForUnknownFileAndMIMETypes
			{
				set
				{
					base.PowerSharpParameters["ActionForUnknownFileAndMIMETypes"] = value;
				}
			}

			public virtual MultiValuedProperty<string> WebReadyFileTypes
			{
				set
				{
					base.PowerSharpParameters["WebReadyFileTypes"] = value;
				}
			}

			public virtual MultiValuedProperty<string> WebReadyMimeTypes
			{
				set
				{
					base.PowerSharpParameters["WebReadyMimeTypes"] = value;
				}
			}

			public virtual bool WebReadyDocumentViewingForAllSupportedTypes
			{
				set
				{
					base.PowerSharpParameters["WebReadyDocumentViewingForAllSupportedTypes"] = value;
				}
			}

			public virtual MultiValuedProperty<string> WebReadyDocumentViewingSupportedMimeTypes
			{
				set
				{
					base.PowerSharpParameters["WebReadyDocumentViewingSupportedMimeTypes"] = value;
				}
			}

			public virtual MultiValuedProperty<string> WebReadyDocumentViewingSupportedFileTypes
			{
				set
				{
					base.PowerSharpParameters["WebReadyDocumentViewingSupportedFileTypes"] = value;
				}
			}

			public virtual MultiValuedProperty<string> AllowedFileTypes
			{
				set
				{
					base.PowerSharpParameters["AllowedFileTypes"] = value;
				}
			}

			public virtual MultiValuedProperty<string> AllowedMimeTypes
			{
				set
				{
					base.PowerSharpParameters["AllowedMimeTypes"] = value;
				}
			}

			public virtual MultiValuedProperty<string> ForceSaveFileTypes
			{
				set
				{
					base.PowerSharpParameters["ForceSaveFileTypes"] = value;
				}
			}

			public virtual MultiValuedProperty<string> ForceSaveMimeTypes
			{
				set
				{
					base.PowerSharpParameters["ForceSaveMimeTypes"] = value;
				}
			}

			public virtual MultiValuedProperty<string> BlockedFileTypes
			{
				set
				{
					base.PowerSharpParameters["BlockedFileTypes"] = value;
				}
			}

			public virtual MultiValuedProperty<string> BlockedMimeTypes
			{
				set
				{
					base.PowerSharpParameters["BlockedMimeTypes"] = value;
				}
			}

			public virtual bool PhoneticSupportEnabled
			{
				set
				{
					base.PowerSharpParameters["PhoneticSupportEnabled"] = value;
				}
			}

			public virtual string DefaultTheme
			{
				set
				{
					base.PowerSharpParameters["DefaultTheme"] = value;
				}
			}

			public virtual int DefaultClientLanguage
			{
				set
				{
					base.PowerSharpParameters["DefaultClientLanguage"] = value;
				}
			}

			public virtual int LogonAndErrorLanguage
			{
				set
				{
					base.PowerSharpParameters["LogonAndErrorLanguage"] = value;
				}
			}

			public virtual bool UseGB18030
			{
				set
				{
					base.PowerSharpParameters["UseGB18030"] = value;
				}
			}

			public virtual bool UseISO885915
			{
				set
				{
					base.PowerSharpParameters["UseISO885915"] = value;
				}
			}

			public virtual OutboundCharsetOptions OutboundCharset
			{
				set
				{
					base.PowerSharpParameters["OutboundCharset"] = value;
				}
			}

			public virtual bool GlobalAddressListEnabled
			{
				set
				{
					base.PowerSharpParameters["GlobalAddressListEnabled"] = value;
				}
			}

			public virtual bool OrganizationEnabled
			{
				set
				{
					base.PowerSharpParameters["OrganizationEnabled"] = value;
				}
			}

			public virtual bool ExplicitLogonEnabled
			{
				set
				{
					base.PowerSharpParameters["ExplicitLogonEnabled"] = value;
				}
			}

			public virtual bool OWALightEnabled
			{
				set
				{
					base.PowerSharpParameters["OWALightEnabled"] = value;
				}
			}

			public virtual bool DelegateAccessEnabled
			{
				set
				{
					base.PowerSharpParameters["DelegateAccessEnabled"] = value;
				}
			}

			public virtual bool IRMEnabled
			{
				set
				{
					base.PowerSharpParameters["IRMEnabled"] = value;
				}
			}

			public virtual bool CalendarEnabled
			{
				set
				{
					base.PowerSharpParameters["CalendarEnabled"] = value;
				}
			}

			public virtual bool ContactsEnabled
			{
				set
				{
					base.PowerSharpParameters["ContactsEnabled"] = value;
				}
			}

			public virtual bool TasksEnabled
			{
				set
				{
					base.PowerSharpParameters["TasksEnabled"] = value;
				}
			}

			public virtual bool JournalEnabled
			{
				set
				{
					base.PowerSharpParameters["JournalEnabled"] = value;
				}
			}

			public virtual bool NotesEnabled
			{
				set
				{
					base.PowerSharpParameters["NotesEnabled"] = value;
				}
			}

			public virtual bool RemindersAndNotificationsEnabled
			{
				set
				{
					base.PowerSharpParameters["RemindersAndNotificationsEnabled"] = value;
				}
			}

			public virtual bool PremiumClientEnabled
			{
				set
				{
					base.PowerSharpParameters["PremiumClientEnabled"] = value;
				}
			}

			public virtual bool SpellCheckerEnabled
			{
				set
				{
					base.PowerSharpParameters["SpellCheckerEnabled"] = value;
				}
			}

			public virtual bool SearchFoldersEnabled
			{
				set
				{
					base.PowerSharpParameters["SearchFoldersEnabled"] = value;
				}
			}

			public virtual bool SignaturesEnabled
			{
				set
				{
					base.PowerSharpParameters["SignaturesEnabled"] = value;
				}
			}

			public virtual bool ThemeSelectionEnabled
			{
				set
				{
					base.PowerSharpParameters["ThemeSelectionEnabled"] = value;
				}
			}

			public virtual bool JunkEmailEnabled
			{
				set
				{
					base.PowerSharpParameters["JunkEmailEnabled"] = value;
				}
			}

			public virtual bool UMIntegrationEnabled
			{
				set
				{
					base.PowerSharpParameters["UMIntegrationEnabled"] = value;
				}
			}

			public virtual bool WSSAccessOnPublicComputersEnabled
			{
				set
				{
					base.PowerSharpParameters["WSSAccessOnPublicComputersEnabled"] = value;
				}
			}

			public virtual bool WSSAccessOnPrivateComputersEnabled
			{
				set
				{
					base.PowerSharpParameters["WSSAccessOnPrivateComputersEnabled"] = value;
				}
			}

			public virtual bool ChangePasswordEnabled
			{
				set
				{
					base.PowerSharpParameters["ChangePasswordEnabled"] = value;
				}
			}

			public virtual bool UNCAccessOnPublicComputersEnabled
			{
				set
				{
					base.PowerSharpParameters["UNCAccessOnPublicComputersEnabled"] = value;
				}
			}

			public virtual bool UNCAccessOnPrivateComputersEnabled
			{
				set
				{
					base.PowerSharpParameters["UNCAccessOnPrivateComputersEnabled"] = value;
				}
			}

			public virtual bool ActiveSyncIntegrationEnabled
			{
				set
				{
					base.PowerSharpParameters["ActiveSyncIntegrationEnabled"] = value;
				}
			}

			public virtual bool AllAddressListsEnabled
			{
				set
				{
					base.PowerSharpParameters["AllAddressListsEnabled"] = value;
				}
			}

			public virtual bool RulesEnabled
			{
				set
				{
					base.PowerSharpParameters["RulesEnabled"] = value;
				}
			}

			public virtual bool PublicFoldersEnabled
			{
				set
				{
					base.PowerSharpParameters["PublicFoldersEnabled"] = value;
				}
			}

			public virtual bool SMimeEnabled
			{
				set
				{
					base.PowerSharpParameters["SMimeEnabled"] = value;
				}
			}

			public virtual bool RecoverDeletedItemsEnabled
			{
				set
				{
					base.PowerSharpParameters["RecoverDeletedItemsEnabled"] = value;
				}
			}

			public virtual bool InstantMessagingEnabled
			{
				set
				{
					base.PowerSharpParameters["InstantMessagingEnabled"] = value;
				}
			}

			public virtual bool TextMessagingEnabled
			{
				set
				{
					base.PowerSharpParameters["TextMessagingEnabled"] = value;
				}
			}

			public virtual bool ForceSaveAttachmentFilteringEnabled
			{
				set
				{
					base.PowerSharpParameters["ForceSaveAttachmentFilteringEnabled"] = value;
				}
			}

			public virtual bool SilverlightEnabled
			{
				set
				{
					base.PowerSharpParameters["SilverlightEnabled"] = value;
				}
			}

			public virtual InstantMessagingTypeOptions? InstantMessagingType
			{
				set
				{
					base.PowerSharpParameters["InstantMessagingType"] = value;
				}
			}

			public virtual bool DisplayPhotosEnabled
			{
				set
				{
					base.PowerSharpParameters["DisplayPhotosEnabled"] = value;
				}
			}

			public virtual bool SetPhotoEnabled
			{
				set
				{
					base.PowerSharpParameters["SetPhotoEnabled"] = value;
				}
			}

			public virtual AllowOfflineOnEnum AllowOfflineOn
			{
				set
				{
					base.PowerSharpParameters["AllowOfflineOn"] = value;
				}
			}

			public virtual string SetPhotoURL
			{
				set
				{
					base.PowerSharpParameters["SetPhotoURL"] = value;
				}
			}

			public virtual bool PlacesEnabled
			{
				set
				{
					base.PowerSharpParameters["PlacesEnabled"] = value;
				}
			}

			public virtual bool WeatherEnabled
			{
				set
				{
					base.PowerSharpParameters["WeatherEnabled"] = value;
				}
			}

			public virtual bool AllowCopyContactsToDeviceAddressBook
			{
				set
				{
					base.PowerSharpParameters["AllowCopyContactsToDeviceAddressBook"] = value;
				}
			}

			public virtual bool PredictedActionsEnabled
			{
				set
				{
					base.PowerSharpParameters["PredictedActionsEnabled"] = value;
				}
			}

			public virtual bool UserDiagnosticEnabled
			{
				set
				{
					base.PowerSharpParameters["UserDiagnosticEnabled"] = value;
				}
			}

			public virtual bool FacebookEnabled
			{
				set
				{
					base.PowerSharpParameters["FacebookEnabled"] = value;
				}
			}

			public virtual bool LinkedInEnabled
			{
				set
				{
					base.PowerSharpParameters["LinkedInEnabled"] = value;
				}
			}

			public virtual bool WacExternalServicesEnabled
			{
				set
				{
					base.PowerSharpParameters["WacExternalServicesEnabled"] = value;
				}
			}

			public virtual bool WacOMEXEnabled
			{
				set
				{
					base.PowerSharpParameters["WacOMEXEnabled"] = value;
				}
			}

			public virtual bool ReportJunkEmailEnabled
			{
				set
				{
					base.PowerSharpParameters["ReportJunkEmailEnabled"] = value;
				}
			}

			public virtual bool GroupCreationEnabled
			{
				set
				{
					base.PowerSharpParameters["GroupCreationEnabled"] = value;
				}
			}

			public virtual bool SkipCreateUnifiedGroupCustomSharepointClassification
			{
				set
				{
					base.PowerSharpParameters["SkipCreateUnifiedGroupCustomSharepointClassification"] = value;
				}
			}

			public virtual WebPartsFrameOptions WebPartsFrameOptionsType
			{
				set
				{
					base.PowerSharpParameters["WebPartsFrameOptionsType"] = value;
				}
			}

			public virtual string Name
			{
				set
				{
					base.PowerSharpParameters["Name"] = value;
				}
			}

			public virtual SwitchParameter Verbose
			{
				set
				{
					base.PowerSharpParameters["Verbose"] = value;
				}
			}

			public virtual SwitchParameter Debug
			{
				set
				{
					base.PowerSharpParameters["Debug"] = value;
				}
			}

			public virtual ActionPreference ErrorAction
			{
				set
				{
					base.PowerSharpParameters["ErrorAction"] = value;
				}
			}

			public virtual ActionPreference WarningAction
			{
				set
				{
					base.PowerSharpParameters["WarningAction"] = value;
				}
			}

			public virtual SwitchParameter WhatIf
			{
				set
				{
					base.PowerSharpParameters["WhatIf"] = value;
				}
			}
		}

		public class IdentityParameters : ParametersBase
		{
			public virtual string Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = ((value != null) ? new MailboxPolicyIdParameter(value) : null);
				}
			}

			public virtual SwitchParameter IsDefault
			{
				set
				{
					base.PowerSharpParameters["IsDefault"] = value;
				}
			}

			public virtual SwitchParameter DisableFacebook
			{
				set
				{
					base.PowerSharpParameters["DisableFacebook"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual bool DirectFileAccessOnPublicComputersEnabled
			{
				set
				{
					base.PowerSharpParameters["DirectFileAccessOnPublicComputersEnabled"] = value;
				}
			}

			public virtual bool DirectFileAccessOnPrivateComputersEnabled
			{
				set
				{
					base.PowerSharpParameters["DirectFileAccessOnPrivateComputersEnabled"] = value;
				}
			}

			public virtual bool WebReadyDocumentViewingOnPublicComputersEnabled
			{
				set
				{
					base.PowerSharpParameters["WebReadyDocumentViewingOnPublicComputersEnabled"] = value;
				}
			}

			public virtual bool WebReadyDocumentViewingOnPrivateComputersEnabled
			{
				set
				{
					base.PowerSharpParameters["WebReadyDocumentViewingOnPrivateComputersEnabled"] = value;
				}
			}

			public virtual bool ForceWebReadyDocumentViewingFirstOnPublicComputers
			{
				set
				{
					base.PowerSharpParameters["ForceWebReadyDocumentViewingFirstOnPublicComputers"] = value;
				}
			}

			public virtual bool ForceWebReadyDocumentViewingFirstOnPrivateComputers
			{
				set
				{
					base.PowerSharpParameters["ForceWebReadyDocumentViewingFirstOnPrivateComputers"] = value;
				}
			}

			public virtual bool WacViewingOnPublicComputersEnabled
			{
				set
				{
					base.PowerSharpParameters["WacViewingOnPublicComputersEnabled"] = value;
				}
			}

			public virtual bool WacViewingOnPrivateComputersEnabled
			{
				set
				{
					base.PowerSharpParameters["WacViewingOnPrivateComputersEnabled"] = value;
				}
			}

			public virtual bool ForceWacViewingFirstOnPublicComputers
			{
				set
				{
					base.PowerSharpParameters["ForceWacViewingFirstOnPublicComputers"] = value;
				}
			}

			public virtual bool ForceWacViewingFirstOnPrivateComputers
			{
				set
				{
					base.PowerSharpParameters["ForceWacViewingFirstOnPrivateComputers"] = value;
				}
			}

			public virtual AttachmentBlockingActions ActionForUnknownFileAndMIMETypes
			{
				set
				{
					base.PowerSharpParameters["ActionForUnknownFileAndMIMETypes"] = value;
				}
			}

			public virtual MultiValuedProperty<string> WebReadyFileTypes
			{
				set
				{
					base.PowerSharpParameters["WebReadyFileTypes"] = value;
				}
			}

			public virtual MultiValuedProperty<string> WebReadyMimeTypes
			{
				set
				{
					base.PowerSharpParameters["WebReadyMimeTypes"] = value;
				}
			}

			public virtual bool WebReadyDocumentViewingForAllSupportedTypes
			{
				set
				{
					base.PowerSharpParameters["WebReadyDocumentViewingForAllSupportedTypes"] = value;
				}
			}

			public virtual MultiValuedProperty<string> WebReadyDocumentViewingSupportedMimeTypes
			{
				set
				{
					base.PowerSharpParameters["WebReadyDocumentViewingSupportedMimeTypes"] = value;
				}
			}

			public virtual MultiValuedProperty<string> WebReadyDocumentViewingSupportedFileTypes
			{
				set
				{
					base.PowerSharpParameters["WebReadyDocumentViewingSupportedFileTypes"] = value;
				}
			}

			public virtual MultiValuedProperty<string> AllowedFileTypes
			{
				set
				{
					base.PowerSharpParameters["AllowedFileTypes"] = value;
				}
			}

			public virtual MultiValuedProperty<string> AllowedMimeTypes
			{
				set
				{
					base.PowerSharpParameters["AllowedMimeTypes"] = value;
				}
			}

			public virtual MultiValuedProperty<string> ForceSaveFileTypes
			{
				set
				{
					base.PowerSharpParameters["ForceSaveFileTypes"] = value;
				}
			}

			public virtual MultiValuedProperty<string> ForceSaveMimeTypes
			{
				set
				{
					base.PowerSharpParameters["ForceSaveMimeTypes"] = value;
				}
			}

			public virtual MultiValuedProperty<string> BlockedFileTypes
			{
				set
				{
					base.PowerSharpParameters["BlockedFileTypes"] = value;
				}
			}

			public virtual MultiValuedProperty<string> BlockedMimeTypes
			{
				set
				{
					base.PowerSharpParameters["BlockedMimeTypes"] = value;
				}
			}

			public virtual bool PhoneticSupportEnabled
			{
				set
				{
					base.PowerSharpParameters["PhoneticSupportEnabled"] = value;
				}
			}

			public virtual string DefaultTheme
			{
				set
				{
					base.PowerSharpParameters["DefaultTheme"] = value;
				}
			}

			public virtual int DefaultClientLanguage
			{
				set
				{
					base.PowerSharpParameters["DefaultClientLanguage"] = value;
				}
			}

			public virtual int LogonAndErrorLanguage
			{
				set
				{
					base.PowerSharpParameters["LogonAndErrorLanguage"] = value;
				}
			}

			public virtual bool UseGB18030
			{
				set
				{
					base.PowerSharpParameters["UseGB18030"] = value;
				}
			}

			public virtual bool UseISO885915
			{
				set
				{
					base.PowerSharpParameters["UseISO885915"] = value;
				}
			}

			public virtual OutboundCharsetOptions OutboundCharset
			{
				set
				{
					base.PowerSharpParameters["OutboundCharset"] = value;
				}
			}

			public virtual bool GlobalAddressListEnabled
			{
				set
				{
					base.PowerSharpParameters["GlobalAddressListEnabled"] = value;
				}
			}

			public virtual bool OrganizationEnabled
			{
				set
				{
					base.PowerSharpParameters["OrganizationEnabled"] = value;
				}
			}

			public virtual bool ExplicitLogonEnabled
			{
				set
				{
					base.PowerSharpParameters["ExplicitLogonEnabled"] = value;
				}
			}

			public virtual bool OWALightEnabled
			{
				set
				{
					base.PowerSharpParameters["OWALightEnabled"] = value;
				}
			}

			public virtual bool DelegateAccessEnabled
			{
				set
				{
					base.PowerSharpParameters["DelegateAccessEnabled"] = value;
				}
			}

			public virtual bool IRMEnabled
			{
				set
				{
					base.PowerSharpParameters["IRMEnabled"] = value;
				}
			}

			public virtual bool CalendarEnabled
			{
				set
				{
					base.PowerSharpParameters["CalendarEnabled"] = value;
				}
			}

			public virtual bool ContactsEnabled
			{
				set
				{
					base.PowerSharpParameters["ContactsEnabled"] = value;
				}
			}

			public virtual bool TasksEnabled
			{
				set
				{
					base.PowerSharpParameters["TasksEnabled"] = value;
				}
			}

			public virtual bool JournalEnabled
			{
				set
				{
					base.PowerSharpParameters["JournalEnabled"] = value;
				}
			}

			public virtual bool NotesEnabled
			{
				set
				{
					base.PowerSharpParameters["NotesEnabled"] = value;
				}
			}

			public virtual bool RemindersAndNotificationsEnabled
			{
				set
				{
					base.PowerSharpParameters["RemindersAndNotificationsEnabled"] = value;
				}
			}

			public virtual bool PremiumClientEnabled
			{
				set
				{
					base.PowerSharpParameters["PremiumClientEnabled"] = value;
				}
			}

			public virtual bool SpellCheckerEnabled
			{
				set
				{
					base.PowerSharpParameters["SpellCheckerEnabled"] = value;
				}
			}

			public virtual bool SearchFoldersEnabled
			{
				set
				{
					base.PowerSharpParameters["SearchFoldersEnabled"] = value;
				}
			}

			public virtual bool SignaturesEnabled
			{
				set
				{
					base.PowerSharpParameters["SignaturesEnabled"] = value;
				}
			}

			public virtual bool ThemeSelectionEnabled
			{
				set
				{
					base.PowerSharpParameters["ThemeSelectionEnabled"] = value;
				}
			}

			public virtual bool JunkEmailEnabled
			{
				set
				{
					base.PowerSharpParameters["JunkEmailEnabled"] = value;
				}
			}

			public virtual bool UMIntegrationEnabled
			{
				set
				{
					base.PowerSharpParameters["UMIntegrationEnabled"] = value;
				}
			}

			public virtual bool WSSAccessOnPublicComputersEnabled
			{
				set
				{
					base.PowerSharpParameters["WSSAccessOnPublicComputersEnabled"] = value;
				}
			}

			public virtual bool WSSAccessOnPrivateComputersEnabled
			{
				set
				{
					base.PowerSharpParameters["WSSAccessOnPrivateComputersEnabled"] = value;
				}
			}

			public virtual bool ChangePasswordEnabled
			{
				set
				{
					base.PowerSharpParameters["ChangePasswordEnabled"] = value;
				}
			}

			public virtual bool UNCAccessOnPublicComputersEnabled
			{
				set
				{
					base.PowerSharpParameters["UNCAccessOnPublicComputersEnabled"] = value;
				}
			}

			public virtual bool UNCAccessOnPrivateComputersEnabled
			{
				set
				{
					base.PowerSharpParameters["UNCAccessOnPrivateComputersEnabled"] = value;
				}
			}

			public virtual bool ActiveSyncIntegrationEnabled
			{
				set
				{
					base.PowerSharpParameters["ActiveSyncIntegrationEnabled"] = value;
				}
			}

			public virtual bool AllAddressListsEnabled
			{
				set
				{
					base.PowerSharpParameters["AllAddressListsEnabled"] = value;
				}
			}

			public virtual bool RulesEnabled
			{
				set
				{
					base.PowerSharpParameters["RulesEnabled"] = value;
				}
			}

			public virtual bool PublicFoldersEnabled
			{
				set
				{
					base.PowerSharpParameters["PublicFoldersEnabled"] = value;
				}
			}

			public virtual bool SMimeEnabled
			{
				set
				{
					base.PowerSharpParameters["SMimeEnabled"] = value;
				}
			}

			public virtual bool RecoverDeletedItemsEnabled
			{
				set
				{
					base.PowerSharpParameters["RecoverDeletedItemsEnabled"] = value;
				}
			}

			public virtual bool InstantMessagingEnabled
			{
				set
				{
					base.PowerSharpParameters["InstantMessagingEnabled"] = value;
				}
			}

			public virtual bool TextMessagingEnabled
			{
				set
				{
					base.PowerSharpParameters["TextMessagingEnabled"] = value;
				}
			}

			public virtual bool ForceSaveAttachmentFilteringEnabled
			{
				set
				{
					base.PowerSharpParameters["ForceSaveAttachmentFilteringEnabled"] = value;
				}
			}

			public virtual bool SilverlightEnabled
			{
				set
				{
					base.PowerSharpParameters["SilverlightEnabled"] = value;
				}
			}

			public virtual InstantMessagingTypeOptions? InstantMessagingType
			{
				set
				{
					base.PowerSharpParameters["InstantMessagingType"] = value;
				}
			}

			public virtual bool DisplayPhotosEnabled
			{
				set
				{
					base.PowerSharpParameters["DisplayPhotosEnabled"] = value;
				}
			}

			public virtual bool SetPhotoEnabled
			{
				set
				{
					base.PowerSharpParameters["SetPhotoEnabled"] = value;
				}
			}

			public virtual AllowOfflineOnEnum AllowOfflineOn
			{
				set
				{
					base.PowerSharpParameters["AllowOfflineOn"] = value;
				}
			}

			public virtual string SetPhotoURL
			{
				set
				{
					base.PowerSharpParameters["SetPhotoURL"] = value;
				}
			}

			public virtual bool PlacesEnabled
			{
				set
				{
					base.PowerSharpParameters["PlacesEnabled"] = value;
				}
			}

			public virtual bool WeatherEnabled
			{
				set
				{
					base.PowerSharpParameters["WeatherEnabled"] = value;
				}
			}

			public virtual bool AllowCopyContactsToDeviceAddressBook
			{
				set
				{
					base.PowerSharpParameters["AllowCopyContactsToDeviceAddressBook"] = value;
				}
			}

			public virtual bool PredictedActionsEnabled
			{
				set
				{
					base.PowerSharpParameters["PredictedActionsEnabled"] = value;
				}
			}

			public virtual bool UserDiagnosticEnabled
			{
				set
				{
					base.PowerSharpParameters["UserDiagnosticEnabled"] = value;
				}
			}

			public virtual bool FacebookEnabled
			{
				set
				{
					base.PowerSharpParameters["FacebookEnabled"] = value;
				}
			}

			public virtual bool LinkedInEnabled
			{
				set
				{
					base.PowerSharpParameters["LinkedInEnabled"] = value;
				}
			}

			public virtual bool WacExternalServicesEnabled
			{
				set
				{
					base.PowerSharpParameters["WacExternalServicesEnabled"] = value;
				}
			}

			public virtual bool WacOMEXEnabled
			{
				set
				{
					base.PowerSharpParameters["WacOMEXEnabled"] = value;
				}
			}

			public virtual bool ReportJunkEmailEnabled
			{
				set
				{
					base.PowerSharpParameters["ReportJunkEmailEnabled"] = value;
				}
			}

			public virtual bool GroupCreationEnabled
			{
				set
				{
					base.PowerSharpParameters["GroupCreationEnabled"] = value;
				}
			}

			public virtual bool SkipCreateUnifiedGroupCustomSharepointClassification
			{
				set
				{
					base.PowerSharpParameters["SkipCreateUnifiedGroupCustomSharepointClassification"] = value;
				}
			}

			public virtual WebPartsFrameOptions WebPartsFrameOptionsType
			{
				set
				{
					base.PowerSharpParameters["WebPartsFrameOptionsType"] = value;
				}
			}

			public virtual string Name
			{
				set
				{
					base.PowerSharpParameters["Name"] = value;
				}
			}

			public virtual SwitchParameter Verbose
			{
				set
				{
					base.PowerSharpParameters["Verbose"] = value;
				}
			}

			public virtual SwitchParameter Debug
			{
				set
				{
					base.PowerSharpParameters["Debug"] = value;
				}
			}

			public virtual ActionPreference ErrorAction
			{
				set
				{
					base.PowerSharpParameters["ErrorAction"] = value;
				}
			}

			public virtual ActionPreference WarningAction
			{
				set
				{
					base.PowerSharpParameters["WarningAction"] = value;
				}
			}

			public virtual SwitchParameter WhatIf
			{
				set
				{
					base.PowerSharpParameters["WhatIf"] = value;
				}
			}
		}
	}
}
