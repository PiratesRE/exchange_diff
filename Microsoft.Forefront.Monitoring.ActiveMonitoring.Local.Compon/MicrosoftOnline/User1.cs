using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.MicrosoftOnline
{
	[GeneratedCode("wsdl", "2.0.50727.1432")]
	[XmlType(TypeName = "User", Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[Serializable]
	public class User1 : DirectoryObject
	{
		public DirectoryPropertyBooleanSingle AccountEnabled
		{
			get
			{
				return this.accountEnabledField;
			}
			set
			{
				this.accountEnabledField = value;
			}
		}

		public DirectoryPropertyXmlAlternativeSecurityId AlternativeSecurityId
		{
			get
			{
				return this.alternativeSecurityIdField;
			}
			set
			{
				this.alternativeSecurityIdField = value;
			}
		}

		public DirectoryPropertyXmlAssignedPlan AssignedPlan
		{
			get
			{
				return this.assignedPlanField;
			}
			set
			{
				this.assignedPlanField = value;
			}
		}

		public DirectoryPropertyXmlAssignedLicense AssignedLicense
		{
			get
			{
				return this.assignedLicenseField;
			}
			set
			{
				this.assignedLicenseField = value;
			}
		}

		public DirectoryPropertyReferenceAddressListSingle Assistant
		{
			get
			{
				return this.assistantField;
			}
			set
			{
				this.assistantField = value;
			}
		}

		public DirectoryPropertyBooleanSingle BelongsToFirstLoginObjectSet
		{
			get
			{
				return this.belongsToFirstLoginObjectSetField;
			}
			set
			{
				this.belongsToFirstLoginObjectSetField = value;
			}
		}

		public DirectoryPropertyStringSingleLength1To256 BesServiceInstance
		{
			get
			{
				return this.besServiceInstanceField;
			}
			set
			{
				this.besServiceInstanceField = value;
			}
		}

		public DirectoryPropertyStringSingleLength1To3 C
		{
			get
			{
				return this.cField;
			}
			set
			{
				this.cField = value;
			}
		}

		public DirectoryPropertyStringSingleLength1To2048 CloudLegacyExchangeDN
		{
			get
			{
				return this.cloudLegacyExchangeDNField;
			}
			set
			{
				this.cloudLegacyExchangeDNField = value;
			}
		}

		public DirectoryPropertyInt32Single CloudMSExchArchiveStatus
		{
			get
			{
				return this.cloudMSExchArchiveStatusField;
			}
			set
			{
				this.cloudMSExchArchiveStatusField = value;
			}
		}

		public DirectoryPropertyBinarySingleLength1To4000 CloudMSExchBlockedSendersHash
		{
			get
			{
				return this.cloudMSExchBlockedSendersHashField;
			}
			set
			{
				this.cloudMSExchBlockedSendersHashField = value;
			}
		}

		public DirectoryPropertyGuidSingle CloudMSExchMailboxGuid
		{
			get
			{
				return this.cloudMSExchMailboxGuidField;
			}
			set
			{
				this.cloudMSExchMailboxGuidField = value;
			}
		}

		public DirectoryPropertyInt32Single CloudMSExchRecipientDisplayType
		{
			get
			{
				return this.cloudMSExchRecipientDisplayTypeField;
			}
			set
			{
				this.cloudMSExchRecipientDisplayTypeField = value;
			}
		}

		public DirectoryPropertyBinarySingleLength1To12000 CloudMSExchSafeRecipientsHash
		{
			get
			{
				return this.cloudMSExchSafeRecipientsHashField;
			}
			set
			{
				this.cloudMSExchSafeRecipientsHashField = value;
			}
		}

		public DirectoryPropertyBinarySingleLength1To32000 CloudMSExchSafeSendersHash
		{
			get
			{
				return this.cloudMSExchSafeSendersHashField;
			}
			set
			{
				this.cloudMSExchSafeSendersHashField = value;
			}
		}

		public DirectoryPropertyStringLength1To1123 CloudMSExchUCVoiceMailSettings
		{
			get
			{
				return this.cloudMSExchUCVoiceMailSettingsField;
			}
			set
			{
				this.cloudMSExchUCVoiceMailSettingsField = value;
			}
		}

		public DirectoryPropertyStringSingleLength1To454 CloudSipProxyAddress
		{
			get
			{
				return this.cloudSipProxyAddressField;
			}
			set
			{
				this.cloudSipProxyAddressField = value;
			}
		}

		public DirectoryPropertyStringSingleLength1To128 Co
		{
			get
			{
				return this.coField;
			}
			set
			{
				this.coField = value;
			}
		}

		public DirectoryPropertyStringSingleLength1To64 Company
		{
			get
			{
				return this.companyField;
			}
			set
			{
				this.companyField = value;
			}
		}

		public DirectoryPropertyInt32SingleMin0Max65535 CountryCode
		{
			get
			{
				return this.countryCodeField;
			}
			set
			{
				this.countryCodeField = value;
			}
		}

		public DirectoryPropertyStringSingleLength1To128 DefaultGeography
		{
			get
			{
				return this.defaultGeographyField;
			}
			set
			{
				this.defaultGeographyField = value;
			}
		}

		public DirectoryPropertyBooleanSingle DeliverAndRedirect
		{
			get
			{
				return this.deliverAndRedirectField;
			}
			set
			{
				this.deliverAndRedirectField = value;
			}
		}

		public DirectoryPropertyStringSingleLength1To64 Department
		{
			get
			{
				return this.departmentField;
			}
			set
			{
				this.departmentField = value;
			}
		}

		public DirectoryPropertyStringSingleLength1To1024 Description
		{
			get
			{
				return this.descriptionField;
			}
			set
			{
				this.descriptionField = value;
			}
		}

		public DirectoryPropertyBooleanSingle DirSyncEnabled
		{
			get
			{
				return this.dirSyncEnabledField;
			}
			set
			{
				this.dirSyncEnabledField = value;
			}
		}

		public DirectoryPropertyInt32SingleMin0 DirSyncOverrides
		{
			get
			{
				return this.dirSyncOverridesField;
			}
			set
			{
				this.dirSyncOverridesField = value;
			}
		}

		public DirectoryPropertyStringSingleLength1To256 DisplayName
		{
			get
			{
				return this.displayNameField;
			}
			set
			{
				this.displayNameField = value;
			}
		}

		public DirectoryPropertyStringSingleLength1To16 EmployeeId
		{
			get
			{
				return this.employeeIdField;
			}
			set
			{
				this.employeeIdField = value;
			}
		}

		public DirectoryPropertyStringSingleLength1To1024 ExtensionAttribute1
		{
			get
			{
				return this.extensionAttribute1Field;
			}
			set
			{
				this.extensionAttribute1Field = value;
			}
		}

		public DirectoryPropertyStringSingleLength1To1024 ExtensionAttribute10
		{
			get
			{
				return this.extensionAttribute10Field;
			}
			set
			{
				this.extensionAttribute10Field = value;
			}
		}

		public DirectoryPropertyStringSingleLength1To2048 ExtensionAttribute11
		{
			get
			{
				return this.extensionAttribute11Field;
			}
			set
			{
				this.extensionAttribute11Field = value;
			}
		}

		public DirectoryPropertyStringSingleLength1To2048 ExtensionAttribute12
		{
			get
			{
				return this.extensionAttribute12Field;
			}
			set
			{
				this.extensionAttribute12Field = value;
			}
		}

		public DirectoryPropertyStringSingleLength1To2048 ExtensionAttribute13
		{
			get
			{
				return this.extensionAttribute13Field;
			}
			set
			{
				this.extensionAttribute13Field = value;
			}
		}

		public DirectoryPropertyStringSingleLength1To2048 ExtensionAttribute14
		{
			get
			{
				return this.extensionAttribute14Field;
			}
			set
			{
				this.extensionAttribute14Field = value;
			}
		}

		public DirectoryPropertyStringSingleLength1To2048 ExtensionAttribute15
		{
			get
			{
				return this.extensionAttribute15Field;
			}
			set
			{
				this.extensionAttribute15Field = value;
			}
		}

		public DirectoryPropertyStringSingleLength1To1024 ExtensionAttribute2
		{
			get
			{
				return this.extensionAttribute2Field;
			}
			set
			{
				this.extensionAttribute2Field = value;
			}
		}

		public DirectoryPropertyStringSingleLength1To1024 ExtensionAttribute3
		{
			get
			{
				return this.extensionAttribute3Field;
			}
			set
			{
				this.extensionAttribute3Field = value;
			}
		}

		public DirectoryPropertyStringSingleLength1To1024 ExtensionAttribute4
		{
			get
			{
				return this.extensionAttribute4Field;
			}
			set
			{
				this.extensionAttribute4Field = value;
			}
		}

		public DirectoryPropertyStringSingleLength1To1024 ExtensionAttribute5
		{
			get
			{
				return this.extensionAttribute5Field;
			}
			set
			{
				this.extensionAttribute5Field = value;
			}
		}

		public DirectoryPropertyStringSingleLength1To1024 ExtensionAttribute6
		{
			get
			{
				return this.extensionAttribute6Field;
			}
			set
			{
				this.extensionAttribute6Field = value;
			}
		}

		public DirectoryPropertyStringSingleLength1To1024 ExtensionAttribute7
		{
			get
			{
				return this.extensionAttribute7Field;
			}
			set
			{
				this.extensionAttribute7Field = value;
			}
		}

		public DirectoryPropertyStringSingleLength1To1024 ExtensionAttribute8
		{
			get
			{
				return this.extensionAttribute8Field;
			}
			set
			{
				this.extensionAttribute8Field = value;
			}
		}

		public DirectoryPropertyStringSingleLength1To1024 ExtensionAttribute9
		{
			get
			{
				return this.extensionAttribute9Field;
			}
			set
			{
				this.extensionAttribute9Field = value;
			}
		}

		public DirectoryPropertyStringSingleLength1To64 FacsimileTelephoneNumber
		{
			get
			{
				return this.facsimileTelephoneNumberField;
			}
			set
			{
				this.facsimileTelephoneNumberField = value;
			}
		}

		public DirectoryPropertyStringSingleLength1To64 GivenName
		{
			get
			{
				return this.givenNameField;
			}
			set
			{
				this.givenNameField = value;
			}
		}

		public DirectoryPropertyStringSingleLength1To64 HomePhone
		{
			get
			{
				return this.homePhoneField;
			}
			set
			{
				this.homePhoneField = value;
			}
		}

		public DirectoryPropertyStringSingleLength1To1024 Info
		{
			get
			{
				return this.infoField;
			}
			set
			{
				this.infoField = value;
			}
		}

		public DirectoryPropertyStringSingleLength1To6 Initials
		{
			get
			{
				return this.initialsField;
			}
			set
			{
				this.initialsField = value;
			}
		}

		public DirectoryPropertyInt32Single InternetEncoding
		{
			get
			{
				return this.internetEncodingField;
			}
			set
			{
				this.internetEncodingField = value;
			}
		}

		public DirectoryPropertyStringSingleLength1To64 IPPhone
		{
			get
			{
				return this.iPPhoneField;
			}
			set
			{
				this.iPPhoneField = value;
			}
		}

		public DirectoryPropertyStringSingleLength1To128 L
		{
			get
			{
				return this.lField;
			}
			set
			{
				this.lField = value;
			}
		}

		public DirectoryPropertyDateTimeSingle LastRestorationTimestamp
		{
			get
			{
				return this.lastRestorationTimestampField;
			}
			set
			{
				this.lastRestorationTimestampField = value;
			}
		}

		public DirectoryPropertyDateTimeSingle LastDirSyncTime
		{
			get
			{
				return this.lastDirSyncTimeField;
			}
			set
			{
				this.lastDirSyncTimeField = value;
			}
		}

		public DirectoryPropertyStringSingleLength1To256 Mail
		{
			get
			{
				return this.mailField;
			}
			set
			{
				this.mailField = value;
			}
		}

		public DirectoryPropertyStringSingleMailNickname MailNickname
		{
			get
			{
				return this.mailNicknameField;
			}
			set
			{
				this.mailNicknameField = value;
			}
		}

		public DirectoryPropertyStringSingleLength1To64 MiddleName
		{
			get
			{
				return this.middleNameField;
			}
			set
			{
				this.middleNameField = value;
			}
		}

		public DirectoryPropertyXmlMigrationDetail MigrationDetail
		{
			get
			{
				return this.migrationDetailField;
			}
			set
			{
				this.migrationDetailField = value;
			}
		}

		public DirectoryPropertyStringSingleLength1To256 MigrationSourceAnchor
		{
			get
			{
				return this.migrationSourceAnchorField;
			}
			set
			{
				this.migrationSourceAnchorField = value;
			}
		}

		public DirectoryPropertyInt32SingleMin0 MigrationState
		{
			get
			{
				return this.migrationStateField;
			}
			set
			{
				this.migrationStateField = value;
			}
		}

		public DirectoryPropertyStringSingleLength1To64 Mobile
		{
			get
			{
				return this.mobileField;
			}
			set
			{
				this.mobileField = value;
			}
		}

		public DirectoryPropertyInt32Single MSDSHABSeniorityIndex
		{
			get
			{
				return this.mSDSHABSeniorityIndexField;
			}
			set
			{
				this.mSDSHABSeniorityIndexField = value;
			}
		}

		public DirectoryPropertyStringSingleLength1To256 MSDSPhoneticDisplayName
		{
			get
			{
				return this.mSDSPhoneticDisplayNameField;
			}
			set
			{
				this.mSDSPhoneticDisplayNameField = value;
			}
		}

		public DirectoryPropertyGuidSingle MSExchArchiveGuid
		{
			get
			{
				return this.mSExchArchiveGuidField;
			}
			set
			{
				this.mSExchArchiveGuidField = value;
			}
		}

		public DirectoryPropertyStringLength1To512 MSExchArchiveName
		{
			get
			{
				return this.mSExchArchiveNameField;
			}
			set
			{
				this.mSExchArchiveNameField = value;
			}
		}

		public DirectoryPropertyStringSingleLength1To256 MSExchAssistantName
		{
			get
			{
				return this.mSExchAssistantNameField;
			}
			set
			{
				this.mSExchAssistantNameField = value;
			}
		}

		public DirectoryPropertyInt32Single MSExchAuditAdmin
		{
			get
			{
				return this.mSExchAuditAdminField;
			}
			set
			{
				this.mSExchAuditAdminField = value;
			}
		}

		public DirectoryPropertyInt32Single MSExchAuditDelegate
		{
			get
			{
				return this.mSExchAuditDelegateField;
			}
			set
			{
				this.mSExchAuditDelegateField = value;
			}
		}

		public DirectoryPropertyInt32Single MSExchAuditDelegateAdmin
		{
			get
			{
				return this.mSExchAuditDelegateAdminField;
			}
			set
			{
				this.mSExchAuditDelegateAdminField = value;
			}
		}

		public DirectoryPropertyInt32Single MSExchAuditOwner
		{
			get
			{
				return this.mSExchAuditOwnerField;
			}
			set
			{
				this.mSExchAuditOwnerField = value;
			}
		}

		public DirectoryPropertyBinarySingleLength1To4000 MSExchBlockedSendersHash
		{
			get
			{
				return this.mSExchBlockedSendersHashField;
			}
			set
			{
				this.mSExchBlockedSendersHashField = value;
			}
		}

		public DirectoryPropertyBooleanSingle MSExchBypassAudit
		{
			get
			{
				return this.mSExchBypassAuditField;
			}
			set
			{
				this.mSExchBypassAuditField = value;
			}
		}

		public DirectoryPropertyDateTimeSingle MSExchElcExpirySuspensionEnd
		{
			get
			{
				return this.mSExchElcExpirySuspensionEndField;
			}
			set
			{
				this.mSExchElcExpirySuspensionEndField = value;
			}
		}

		public DirectoryPropertyDateTimeSingle MSExchElcExpirySuspensionStart
		{
			get
			{
				return this.mSExchElcExpirySuspensionStartField;
			}
			set
			{
				this.mSExchElcExpirySuspensionStartField = value;
			}
		}

		public DirectoryPropertyInt32Single MSExchElcMailboxFlags
		{
			get
			{
				return this.mSExchElcMailboxFlagsField;
			}
			set
			{
				this.mSExchElcMailboxFlagsField = value;
			}
		}

		public DirectoryPropertyBooleanSingle MSExchEnableModeration
		{
			get
			{
				return this.mSExchEnableModerationField;
			}
			set
			{
				this.mSExchEnableModerationField = value;
			}
		}

		public DirectoryPropertyStringLength1To2048 MSExchExtensionCustomAttribute1
		{
			get
			{
				return this.mSExchExtensionCustomAttribute1Field;
			}
			set
			{
				this.mSExchExtensionCustomAttribute1Field = value;
			}
		}

		public DirectoryPropertyStringLength1To2048 MSExchExtensionCustomAttribute2
		{
			get
			{
				return this.mSExchExtensionCustomAttribute2Field;
			}
			set
			{
				this.mSExchExtensionCustomAttribute2Field = value;
			}
		}

		public DirectoryPropertyStringLength1To2048 MSExchExtensionCustomAttribute3
		{
			get
			{
				return this.mSExchExtensionCustomAttribute3Field;
			}
			set
			{
				this.mSExchExtensionCustomAttribute3Field = value;
			}
		}

		public DirectoryPropertyStringLength1To2048 MSExchExtensionCustomAttribute4
		{
			get
			{
				return this.mSExchExtensionCustomAttribute4Field;
			}
			set
			{
				this.mSExchExtensionCustomAttribute4Field = value;
			}
		}

		public DirectoryPropertyStringLength1To2048 MSExchExtensionCustomAttribute5
		{
			get
			{
				return this.mSExchExtensionCustomAttribute5Field;
			}
			set
			{
				this.mSExchExtensionCustomAttribute5Field = value;
			}
		}

		public DirectoryPropertyBooleanSingle MSExchHideFromAddressLists
		{
			get
			{
				return this.mSExchHideFromAddressListsField;
			}
			set
			{
				this.mSExchHideFromAddressListsField = value;
			}
		}

		public DirectoryPropertyStringSingleLength1To256 MSExchImmutableId
		{
			get
			{
				return this.mSExchImmutableIdField;
			}
			set
			{
				this.mSExchImmutableIdField = value;
			}
		}

		public DirectoryPropertyDateTimeSingle MSExchLitigationHoldDate
		{
			get
			{
				return this.mSExchLitigationHoldDateField;
			}
			set
			{
				this.mSExchLitigationHoldDateField = value;
			}
		}

		public DirectoryPropertyStringSingleLength1To1024 MSExchLitigationHoldOwner
		{
			get
			{
				return this.mSExchLitigationHoldOwnerField;
			}
			set
			{
				this.mSExchLitigationHoldOwnerField = value;
			}
		}

		public DirectoryPropertyBooleanSingle MSExchMailboxAuditEnable
		{
			get
			{
				return this.mSExchMailboxAuditEnableField;
			}
			set
			{
				this.mSExchMailboxAuditEnableField = value;
			}
		}

		public DirectoryPropertyInt32Single MSExchMailboxAuditLogAgeLimit
		{
			get
			{
				return this.mSExchMailboxAuditLogAgeLimitField;
			}
			set
			{
				this.mSExchMailboxAuditLogAgeLimitField = value;
			}
		}

		public DirectoryPropertyGuidSingle MSExchMailboxGuid
		{
			get
			{
				return this.mSExchMailboxGuidField;
			}
			set
			{
				this.mSExchMailboxGuidField = value;
			}
		}

		public DirectoryPropertyInt32Single MSExchModerationFlags
		{
			get
			{
				return this.mSExchModerationFlagsField;
			}
			set
			{
				this.mSExchModerationFlagsField = value;
			}
		}

		public DirectoryPropertyInt32Single MSExchRecipientDisplayType
		{
			get
			{
				return this.mSExchRecipientDisplayTypeField;
			}
			set
			{
				this.mSExchRecipientDisplayTypeField = value;
			}
		}

		public DirectoryPropertyInt64Single MSExchRecipientTypeDetails
		{
			get
			{
				return this.mSExchRecipientTypeDetailsField;
			}
			set
			{
				this.mSExchRecipientTypeDetailsField = value;
			}
		}

		public DirectoryPropertyInt64Single MSExchRemoteRecipientType
		{
			get
			{
				return this.mSExchRemoteRecipientTypeField;
			}
			set
			{
				this.mSExchRemoteRecipientTypeField = value;
			}
		}

		public DirectoryPropertyBooleanSingle MSExchRequireAuthToSendTo
		{
			get
			{
				return this.mSExchRequireAuthToSendToField;
			}
			set
			{
				this.mSExchRequireAuthToSendToField = value;
			}
		}

		public DirectoryPropertyInt32Single MSExchResourceCapacity
		{
			get
			{
				return this.mSExchResourceCapacityField;
			}
			set
			{
				this.mSExchResourceCapacityField = value;
			}
		}

		public DirectoryPropertyStringSingleLength1To1024 MSExchResourceDisplay
		{
			get
			{
				return this.mSExchResourceDisplayField;
			}
			set
			{
				this.mSExchResourceDisplayField = value;
			}
		}

		public DirectoryPropertyStringLength1To1024 MSExchResourceMetadata
		{
			get
			{
				return this.mSExchResourceMetadataField;
			}
			set
			{
				this.mSExchResourceMetadataField = value;
			}
		}

		public DirectoryPropertyStringLength1To1024 MSExchResourceSearchProperties
		{
			get
			{
				return this.mSExchResourceSearchPropertiesField;
			}
			set
			{
				this.mSExchResourceSearchPropertiesField = value;
			}
		}

		public DirectoryPropertyStringSingleLength1To1024 MSExchRetentionComment
		{
			get
			{
				return this.mSExchRetentionCommentField;
			}
			set
			{
				this.mSExchRetentionCommentField = value;
			}
		}

		public DirectoryPropertyStringSingleLength1To2048 MSExchRetentionUrl
		{
			get
			{
				return this.mSExchRetentionUrlField;
			}
			set
			{
				this.mSExchRetentionUrlField = value;
			}
		}

		public DirectoryPropertyBinarySingleLength1To12000 MSExchSafeRecipientsHash
		{
			get
			{
				return this.mSExchSafeRecipientsHashField;
			}
			set
			{
				this.mSExchSafeRecipientsHashField = value;
			}
		}

		public DirectoryPropertyBinarySingleLength1To32000 MSExchSafeSendersHash
		{
			get
			{
				return this.mSExchSafeSendersHashField;
			}
			set
			{
				this.mSExchSafeSendersHashField = value;
			}
		}

		public DirectoryPropertyStringLength2To500 MSExchSenderHintTranslations
		{
			get
			{
				return this.mSExchSenderHintTranslationsField;
			}
			set
			{
				this.mSExchSenderHintTranslationsField = value;
			}
		}

		public DirectoryPropertyDateTimeSingle MSExchTeamMailboxExpiration
		{
			get
			{
				return this.mSExchTeamMailboxExpirationField;
			}
			set
			{
				this.mSExchTeamMailboxExpirationField = value;
			}
		}

		public DirectoryPropertyReferenceAddressList MSExchTeamMailboxOwners
		{
			get
			{
				return this.mSExchTeamMailboxOwnersField;
			}
			set
			{
				this.mSExchTeamMailboxOwnersField = value;
			}
		}

		public DirectoryPropertyReferenceAddressListSingle MSExchTeamMailboxSharePointLinkedBy
		{
			get
			{
				return this.mSExchTeamMailboxSharePointLinkedByField;
			}
			set
			{
				this.mSExchTeamMailboxSharePointLinkedByField = value;
			}
		}

		public DirectoryPropertyStringSingleLength1To2048 MSExchTeamMailboxSharePointUrl
		{
			get
			{
				return this.mSExchTeamMailboxSharePointUrlField;
			}
			set
			{
				this.mSExchTeamMailboxSharePointUrlField = value;
			}
		}

		public DirectoryPropertyStringLength1To40 MSExchUserHoldPolicies
		{
			get
			{
				return this.mSExchUserHoldPoliciesField;
			}
			set
			{
				this.mSExchUserHoldPoliciesField = value;
			}
		}

		public DirectoryPropertyInt32SingleMin0 MSRtcSipApplicationOptions
		{
			get
			{
				return this.mSRtcSipApplicationOptionsField;
			}
			set
			{
				this.mSRtcSipApplicationOptionsField = value;
			}
		}

		public DirectoryPropertyStringSingleLength1To256 MSRtcSipDeploymentLocator
		{
			get
			{
				return this.mSRtcSipDeploymentLocatorField;
			}
			set
			{
				this.mSRtcSipDeploymentLocatorField = value;
			}
		}

		public DirectoryPropertyStringSingleLength1To500 MSRtcSipLine
		{
			get
			{
				return this.mSRtcSipLineField;
			}
			set
			{
				this.mSRtcSipLineField = value;
			}
		}

		public DirectoryPropertyInt32Single MSRtcSipOptionFlags
		{
			get
			{
				return this.mSRtcSipOptionFlagsField;
			}
			set
			{
				this.mSRtcSipOptionFlagsField = value;
			}
		}

		public DirectoryPropertyStringSingleLength1To512 MSRtcSipOwnerUrn
		{
			get
			{
				return this.mSRtcSipOwnerUrnField;
			}
			set
			{
				this.mSRtcSipOwnerUrnField = value;
			}
		}

		public DirectoryPropertyStringSingleLength1To454 MSRtcSipPrimaryUserAddress
		{
			get
			{
				return this.mSRtcSipPrimaryUserAddressField;
			}
			set
			{
				this.mSRtcSipPrimaryUserAddressField = value;
			}
		}

		public DirectoryPropertyBooleanSingle MSRtcSipUserEnabled
		{
			get
			{
				return this.mSRtcSipUserEnabledField;
			}
			set
			{
				this.mSRtcSipUserEnabledField = value;
			}
		}

		public DirectoryPropertyBinarySingleLength1To128 OnPremiseSecurityIdentifier
		{
			get
			{
				return this.onPremiseSecurityIdentifierField;
			}
			set
			{
				this.onPremiseSecurityIdentifierField = value;
			}
		}

		public DirectoryPropertyStringLength1To64 OtherFacsimileTelephoneNumber
		{
			get
			{
				return this.otherFacsimileTelephoneNumberField;
			}
			set
			{
				this.otherFacsimileTelephoneNumberField = value;
			}
		}

		public DirectoryPropertyStringLength1To64 OtherHomePhone
		{
			get
			{
				return this.otherHomePhoneField;
			}
			set
			{
				this.otherHomePhoneField = value;
			}
		}

		public DirectoryPropertyStringLength1To512 OtherIPPhone
		{
			get
			{
				return this.otherIPPhoneField;
			}
			set
			{
				this.otherIPPhoneField = value;
			}
		}

		public DirectoryPropertyStringLength1To256 OtherMail
		{
			get
			{
				return this.otherMailField;
			}
			set
			{
				this.otherMailField = value;
			}
		}

		public DirectoryPropertyStringLength1To64 OtherMobile
		{
			get
			{
				return this.otherMobileField;
			}
			set
			{
				this.otherMobileField = value;
			}
		}

		public DirectoryPropertyStringLength1To64 OtherPager
		{
			get
			{
				return this.otherPagerField;
			}
			set
			{
				this.otherPagerField = value;
			}
		}

		public DirectoryPropertyStringLength1To64 OtherTelephone
		{
			get
			{
				return this.otherTelephoneField;
			}
			set
			{
				this.otherTelephoneField = value;
			}
		}

		public DirectoryPropertyInt32SingleMin0 PasswordPolicies
		{
			get
			{
				return this.passwordPoliciesField;
			}
			set
			{
				this.passwordPoliciesField = value;
			}
		}

		public DirectoryPropertyStringSingleLength1To64 Pager
		{
			get
			{
				return this.pagerField;
			}
			set
			{
				this.pagerField = value;
			}
		}

		public DirectoryPropertyStringSingleLength1To128 PhysicalDeliveryOfficeName
		{
			get
			{
				return this.physicalDeliveryOfficeNameField;
			}
			set
			{
				this.physicalDeliveryOfficeNameField = value;
			}
		}

		public DirectoryPropertyXmlAnySingle PortalSetting
		{
			get
			{
				return this.portalSettingField;
			}
			set
			{
				this.portalSettingField = value;
			}
		}

		public DirectoryPropertyStringSingleLength1To40 PostalCode
		{
			get
			{
				return this.postalCodeField;
			}
			set
			{
				this.postalCodeField = value;
			}
		}

		public DirectoryPropertyStringLength1To40 PostOfficeBox
		{
			get
			{
				return this.postOfficeBoxField;
			}
			set
			{
				this.postOfficeBoxField = value;
			}
		}

		public DirectoryPropertyStringSingleLength1To64 PreferredLanguage
		{
			get
			{
				return this.preferredLanguageField;
			}
			set
			{
				this.preferredLanguageField = value;
			}
		}

		public DirectoryPropertyXmlProvisionedPlan ProvisionedPlan
		{
			get
			{
				return this.provisionedPlanField;
			}
			set
			{
				this.provisionedPlanField = value;
			}
		}

		public DirectoryPropertyProxyAddresses ProxyAddresses
		{
			get
			{
				return this.proxyAddressesField;
			}
			set
			{
				this.proxyAddressesField = value;
			}
		}

		public DirectoryPropertyXmlRightsManagementUserKeySingle RightsManagementUserKey
		{
			get
			{
				return this.rightsManagementUserKeyField;
			}
			set
			{
				this.rightsManagementUserKeyField = value;
			}
		}

		public DirectoryPropertyXmlServiceInfo ServiceInfo
		{
			get
			{
				return this.serviceInfoField;
			}
			set
			{
				this.serviceInfoField = value;
			}
		}

		public DirectoryPropertyXmlServiceOriginatedResource ServiceOriginatedResource
		{
			get
			{
				return this.serviceOriginatedResourceField;
			}
			set
			{
				this.serviceOriginatedResourceField = value;
			}
		}

		public DirectoryPropertyStringSingleLength1To64 ShadowAlias
		{
			get
			{
				return this.shadowAliasField;
			}
			set
			{
				this.shadowAliasField = value;
			}
		}

		public DirectoryPropertyStringSingleLength1To64 ShadowCommonName
		{
			get
			{
				return this.shadowCommonNameField;
			}
			set
			{
				this.shadowCommonNameField = value;
			}
		}

		public DirectoryPropertyStringSingleLength1To256 ShadowDisplayName
		{
			get
			{
				return this.shadowDisplayNameField;
			}
			set
			{
				this.shadowDisplayNameField = value;
			}
		}

		public DirectoryPropertyStringSingleLength1To2048 ShadowLegacyExchangeDN
		{
			get
			{
				return this.shadowLegacyExchangeDNField;
			}
			set
			{
				this.shadowLegacyExchangeDNField = value;
			}
		}

		public DirectoryPropertyStringSingleLength1To256 ShadowMail
		{
			get
			{
				return this.shadowMailField;
			}
			set
			{
				this.shadowMailField = value;
			}
		}

		public DirectoryPropertyStringSingleLength1To64 ShadowMobile
		{
			get
			{
				return this.shadowMobileField;
			}
			set
			{
				this.shadowMobileField = value;
			}
		}

		public DirectoryPropertyStringLength1To1123 ShadowProxyAddresses
		{
			get
			{
				return this.shadowProxyAddressesField;
			}
			set
			{
				this.shadowProxyAddressesField = value;
			}
		}

		public DirectoryPropertyStringSingleLength1To1123 ShadowTargetAddress
		{
			get
			{
				return this.shadowTargetAddressField;
			}
			set
			{
				this.shadowTargetAddressField = value;
			}
		}

		public DirectoryPropertyStringSingleLength1To1024 ShadowUserPrincipalName
		{
			get
			{
				return this.shadowUserPrincipalNameField;
			}
			set
			{
				this.shadowUserPrincipalNameField = value;
			}
		}

		public DirectoryPropertyStringSingleLength1To454 SipProxyAddress
		{
			get
			{
				return this.sipProxyAddressField;
			}
			set
			{
				this.sipProxyAddressField = value;
			}
		}

		public DirectoryPropertyStringSingleLength1To64 Sn
		{
			get
			{
				return this.snField;
			}
			set
			{
				this.snField = value;
			}
		}

		public DirectoryPropertyDateTimeSingle SoftDeletionTimestamp
		{
			get
			{
				return this.softDeletionTimestampField;
			}
			set
			{
				this.softDeletionTimestampField = value;
			}
		}

		public DirectoryPropertyStringSingleLength1To256 SourceAnchor
		{
			get
			{
				return this.sourceAnchorField;
			}
			set
			{
				this.sourceAnchorField = value;
			}
		}

		public DirectoryPropertyStringSingleLength1To128 St
		{
			get
			{
				return this.stField;
			}
			set
			{
				this.stField = value;
			}
		}

		public DirectoryPropertyStringSingleLength1To1024 Street
		{
			get
			{
				return this.streetField;
			}
			set
			{
				this.streetField = value;
			}
		}

		public DirectoryPropertyStringSingleLength1To1024 StreetAddress
		{
			get
			{
				return this.streetAddressField;
			}
			set
			{
				this.streetAddressField = value;
			}
		}

		public DirectoryPropertyXmlStrongAuthenticationMethod StrongAuthenticationMethod
		{
			get
			{
				return this.strongAuthenticationMethodField;
			}
			set
			{
				this.strongAuthenticationMethodField = value;
			}
		}

		public DirectoryPropertyTargetAddress TargetAddress
		{
			get
			{
				return this.targetAddressField;
			}
			set
			{
				this.targetAddressField = value;
			}
		}

		public DirectoryPropertyStringSingleLength1To64 TelephoneAssistant
		{
			get
			{
				return this.telephoneAssistantField;
			}
			set
			{
				this.telephoneAssistantField = value;
			}
		}

		public DirectoryPropertyStringSingleLength1To64 TelephoneNumber
		{
			get
			{
				return this.telephoneNumberField;
			}
			set
			{
				this.telephoneNumberField = value;
			}
		}

		public DirectoryPropertyBinarySingleLength1To102400 ThumbnailPhoto
		{
			get
			{
				return this.thumbnailPhotoField;
			}
			set
			{
				this.thumbnailPhotoField = value;
			}
		}

		public DirectoryPropertyStringSingleLength1To128 Title
		{
			get
			{
				return this.titleField;
			}
			set
			{
				this.titleField = value;
			}
		}

		public DirectoryPropertyStringLength1To1123 Url
		{
			get
			{
				return this.urlField;
			}
			set
			{
				this.urlField = value;
			}
		}

		public DirectoryPropertyStringSingleLength1To3 UsageLocation
		{
			get
			{
				return this.usageLocationField;
			}
			set
			{
				this.usageLocationField = value;
			}
		}

		public DirectoryPropertyStringSingleLength1To1024 UserPrincipalName
		{
			get
			{
				return this.userPrincipalNameField;
			}
			set
			{
				this.userPrincipalNameField = value;
			}
		}

		public DirectoryPropertyXmlValidationError ValidationError
		{
			get
			{
				return this.validationErrorField;
			}
			set
			{
				this.validationErrorField = value;
			}
		}

		public DirectoryPropertyBinarySingleLength8 WindowsLiveNetId
		{
			get
			{
				return this.windowsLiveNetIdField;
			}
			set
			{
				this.windowsLiveNetIdField = value;
			}
		}

		public DirectoryPropertyStringSingleLength1To2048 WwwHomepage
		{
			get
			{
				return this.wwwHomepageField;
			}
			set
			{
				this.wwwHomepageField = value;
			}
		}

		[XmlArray(Namespace = "http://schemas.microsoft.com/online/directoryservices/sync/metadata/2010/01")]
		[XmlArrayItem("AttributeSet", IsNullable = false)]
		public AttributeSet[] SingleAuthorityMetadata
		{
			get
			{
				return this.singleAuthorityMetadataField;
			}
			set
			{
				this.singleAuthorityMetadataField = value;
			}
		}

		[XmlAnyAttribute]
		public XmlAttribute[] AnyAttr
		{
			get
			{
				return this.anyAttrField;
			}
			set
			{
				this.anyAttrField = value;
			}
		}

		private DirectoryPropertyBooleanSingle accountEnabledField;

		private DirectoryPropertyXmlAlternativeSecurityId alternativeSecurityIdField;

		private DirectoryPropertyXmlAssignedPlan assignedPlanField;

		private DirectoryPropertyXmlAssignedLicense assignedLicenseField;

		private DirectoryPropertyReferenceAddressListSingle assistantField;

		private DirectoryPropertyBooleanSingle belongsToFirstLoginObjectSetField;

		private DirectoryPropertyStringSingleLength1To256 besServiceInstanceField;

		private DirectoryPropertyStringSingleLength1To3 cField;

		private DirectoryPropertyStringSingleLength1To2048 cloudLegacyExchangeDNField;

		private DirectoryPropertyInt32Single cloudMSExchArchiveStatusField;

		private DirectoryPropertyBinarySingleLength1To4000 cloudMSExchBlockedSendersHashField;

		private DirectoryPropertyGuidSingle cloudMSExchMailboxGuidField;

		private DirectoryPropertyInt32Single cloudMSExchRecipientDisplayTypeField;

		private DirectoryPropertyBinarySingleLength1To12000 cloudMSExchSafeRecipientsHashField;

		private DirectoryPropertyBinarySingleLength1To32000 cloudMSExchSafeSendersHashField;

		private DirectoryPropertyStringLength1To1123 cloudMSExchUCVoiceMailSettingsField;

		private DirectoryPropertyStringSingleLength1To454 cloudSipProxyAddressField;

		private DirectoryPropertyStringSingleLength1To128 coField;

		private DirectoryPropertyStringSingleLength1To64 companyField;

		private DirectoryPropertyInt32SingleMin0Max65535 countryCodeField;

		private DirectoryPropertyStringSingleLength1To128 defaultGeographyField;

		private DirectoryPropertyBooleanSingle deliverAndRedirectField;

		private DirectoryPropertyStringSingleLength1To64 departmentField;

		private DirectoryPropertyStringSingleLength1To1024 descriptionField;

		private DirectoryPropertyBooleanSingle dirSyncEnabledField;

		private DirectoryPropertyInt32SingleMin0 dirSyncOverridesField;

		private DirectoryPropertyStringSingleLength1To256 displayNameField;

		private DirectoryPropertyStringSingleLength1To16 employeeIdField;

		private DirectoryPropertyStringSingleLength1To1024 extensionAttribute1Field;

		private DirectoryPropertyStringSingleLength1To1024 extensionAttribute10Field;

		private DirectoryPropertyStringSingleLength1To2048 extensionAttribute11Field;

		private DirectoryPropertyStringSingleLength1To2048 extensionAttribute12Field;

		private DirectoryPropertyStringSingleLength1To2048 extensionAttribute13Field;

		private DirectoryPropertyStringSingleLength1To2048 extensionAttribute14Field;

		private DirectoryPropertyStringSingleLength1To2048 extensionAttribute15Field;

		private DirectoryPropertyStringSingleLength1To1024 extensionAttribute2Field;

		private DirectoryPropertyStringSingleLength1To1024 extensionAttribute3Field;

		private DirectoryPropertyStringSingleLength1To1024 extensionAttribute4Field;

		private DirectoryPropertyStringSingleLength1To1024 extensionAttribute5Field;

		private DirectoryPropertyStringSingleLength1To1024 extensionAttribute6Field;

		private DirectoryPropertyStringSingleLength1To1024 extensionAttribute7Field;

		private DirectoryPropertyStringSingleLength1To1024 extensionAttribute8Field;

		private DirectoryPropertyStringSingleLength1To1024 extensionAttribute9Field;

		private DirectoryPropertyStringSingleLength1To64 facsimileTelephoneNumberField;

		private DirectoryPropertyStringSingleLength1To64 givenNameField;

		private DirectoryPropertyStringSingleLength1To64 homePhoneField;

		private DirectoryPropertyStringSingleLength1To1024 infoField;

		private DirectoryPropertyStringSingleLength1To6 initialsField;

		private DirectoryPropertyInt32Single internetEncodingField;

		private DirectoryPropertyStringSingleLength1To64 iPPhoneField;

		private DirectoryPropertyStringSingleLength1To128 lField;

		private DirectoryPropertyDateTimeSingle lastRestorationTimestampField;

		private DirectoryPropertyDateTimeSingle lastDirSyncTimeField;

		private DirectoryPropertyStringSingleLength1To256 mailField;

		private DirectoryPropertyStringSingleMailNickname mailNicknameField;

		private DirectoryPropertyStringSingleLength1To64 middleNameField;

		private DirectoryPropertyXmlMigrationDetail migrationDetailField;

		private DirectoryPropertyStringSingleLength1To256 migrationSourceAnchorField;

		private DirectoryPropertyInt32SingleMin0 migrationStateField;

		private DirectoryPropertyStringSingleLength1To64 mobileField;

		private DirectoryPropertyInt32Single mSDSHABSeniorityIndexField;

		private DirectoryPropertyStringSingleLength1To256 mSDSPhoneticDisplayNameField;

		private DirectoryPropertyGuidSingle mSExchArchiveGuidField;

		private DirectoryPropertyStringLength1To512 mSExchArchiveNameField;

		private DirectoryPropertyStringSingleLength1To256 mSExchAssistantNameField;

		private DirectoryPropertyInt32Single mSExchAuditAdminField;

		private DirectoryPropertyInt32Single mSExchAuditDelegateField;

		private DirectoryPropertyInt32Single mSExchAuditDelegateAdminField;

		private DirectoryPropertyInt32Single mSExchAuditOwnerField;

		private DirectoryPropertyBinarySingleLength1To4000 mSExchBlockedSendersHashField;

		private DirectoryPropertyBooleanSingle mSExchBypassAuditField;

		private DirectoryPropertyDateTimeSingle mSExchElcExpirySuspensionEndField;

		private DirectoryPropertyDateTimeSingle mSExchElcExpirySuspensionStartField;

		private DirectoryPropertyInt32Single mSExchElcMailboxFlagsField;

		private DirectoryPropertyBooleanSingle mSExchEnableModerationField;

		private DirectoryPropertyStringLength1To2048 mSExchExtensionCustomAttribute1Field;

		private DirectoryPropertyStringLength1To2048 mSExchExtensionCustomAttribute2Field;

		private DirectoryPropertyStringLength1To2048 mSExchExtensionCustomAttribute3Field;

		private DirectoryPropertyStringLength1To2048 mSExchExtensionCustomAttribute4Field;

		private DirectoryPropertyStringLength1To2048 mSExchExtensionCustomAttribute5Field;

		private DirectoryPropertyBooleanSingle mSExchHideFromAddressListsField;

		private DirectoryPropertyStringSingleLength1To256 mSExchImmutableIdField;

		private DirectoryPropertyDateTimeSingle mSExchLitigationHoldDateField;

		private DirectoryPropertyStringSingleLength1To1024 mSExchLitigationHoldOwnerField;

		private DirectoryPropertyBooleanSingle mSExchMailboxAuditEnableField;

		private DirectoryPropertyInt32Single mSExchMailboxAuditLogAgeLimitField;

		private DirectoryPropertyGuidSingle mSExchMailboxGuidField;

		private DirectoryPropertyInt32Single mSExchModerationFlagsField;

		private DirectoryPropertyInt32Single mSExchRecipientDisplayTypeField;

		private DirectoryPropertyInt64Single mSExchRecipientTypeDetailsField;

		private DirectoryPropertyInt64Single mSExchRemoteRecipientTypeField;

		private DirectoryPropertyBooleanSingle mSExchRequireAuthToSendToField;

		private DirectoryPropertyInt32Single mSExchResourceCapacityField;

		private DirectoryPropertyStringSingleLength1To1024 mSExchResourceDisplayField;

		private DirectoryPropertyStringLength1To1024 mSExchResourceMetadataField;

		private DirectoryPropertyStringLength1To1024 mSExchResourceSearchPropertiesField;

		private DirectoryPropertyStringSingleLength1To1024 mSExchRetentionCommentField;

		private DirectoryPropertyStringSingleLength1To2048 mSExchRetentionUrlField;

		private DirectoryPropertyBinarySingleLength1To12000 mSExchSafeRecipientsHashField;

		private DirectoryPropertyBinarySingleLength1To32000 mSExchSafeSendersHashField;

		private DirectoryPropertyStringLength2To500 mSExchSenderHintTranslationsField;

		private DirectoryPropertyDateTimeSingle mSExchTeamMailboxExpirationField;

		private DirectoryPropertyReferenceAddressList mSExchTeamMailboxOwnersField;

		private DirectoryPropertyReferenceAddressListSingle mSExchTeamMailboxSharePointLinkedByField;

		private DirectoryPropertyStringSingleLength1To2048 mSExchTeamMailboxSharePointUrlField;

		private DirectoryPropertyStringLength1To40 mSExchUserHoldPoliciesField;

		private DirectoryPropertyInt32SingleMin0 mSRtcSipApplicationOptionsField;

		private DirectoryPropertyStringSingleLength1To256 mSRtcSipDeploymentLocatorField;

		private DirectoryPropertyStringSingleLength1To500 mSRtcSipLineField;

		private DirectoryPropertyInt32Single mSRtcSipOptionFlagsField;

		private DirectoryPropertyStringSingleLength1To512 mSRtcSipOwnerUrnField;

		private DirectoryPropertyStringSingleLength1To454 mSRtcSipPrimaryUserAddressField;

		private DirectoryPropertyBooleanSingle mSRtcSipUserEnabledField;

		private DirectoryPropertyBinarySingleLength1To128 onPremiseSecurityIdentifierField;

		private DirectoryPropertyStringLength1To64 otherFacsimileTelephoneNumberField;

		private DirectoryPropertyStringLength1To64 otherHomePhoneField;

		private DirectoryPropertyStringLength1To512 otherIPPhoneField;

		private DirectoryPropertyStringLength1To256 otherMailField;

		private DirectoryPropertyStringLength1To64 otherMobileField;

		private DirectoryPropertyStringLength1To64 otherPagerField;

		private DirectoryPropertyStringLength1To64 otherTelephoneField;

		private DirectoryPropertyInt32SingleMin0 passwordPoliciesField;

		private DirectoryPropertyStringSingleLength1To64 pagerField;

		private DirectoryPropertyStringSingleLength1To128 physicalDeliveryOfficeNameField;

		private DirectoryPropertyXmlAnySingle portalSettingField;

		private DirectoryPropertyStringSingleLength1To40 postalCodeField;

		private DirectoryPropertyStringLength1To40 postOfficeBoxField;

		private DirectoryPropertyStringSingleLength1To64 preferredLanguageField;

		private DirectoryPropertyXmlProvisionedPlan provisionedPlanField;

		private DirectoryPropertyProxyAddresses proxyAddressesField;

		private DirectoryPropertyXmlRightsManagementUserKeySingle rightsManagementUserKeyField;

		private DirectoryPropertyXmlServiceInfo serviceInfoField;

		private DirectoryPropertyXmlServiceOriginatedResource serviceOriginatedResourceField;

		private DirectoryPropertyStringSingleLength1To64 shadowAliasField;

		private DirectoryPropertyStringSingleLength1To64 shadowCommonNameField;

		private DirectoryPropertyStringSingleLength1To256 shadowDisplayNameField;

		private DirectoryPropertyStringSingleLength1To2048 shadowLegacyExchangeDNField;

		private DirectoryPropertyStringSingleLength1To256 shadowMailField;

		private DirectoryPropertyStringSingleLength1To64 shadowMobileField;

		private DirectoryPropertyStringLength1To1123 shadowProxyAddressesField;

		private DirectoryPropertyStringSingleLength1To1123 shadowTargetAddressField;

		private DirectoryPropertyStringSingleLength1To1024 shadowUserPrincipalNameField;

		private DirectoryPropertyStringSingleLength1To454 sipProxyAddressField;

		private DirectoryPropertyStringSingleLength1To64 snField;

		private DirectoryPropertyDateTimeSingle softDeletionTimestampField;

		private DirectoryPropertyStringSingleLength1To256 sourceAnchorField;

		private DirectoryPropertyStringSingleLength1To128 stField;

		private DirectoryPropertyStringSingleLength1To1024 streetField;

		private DirectoryPropertyStringSingleLength1To1024 streetAddressField;

		private DirectoryPropertyXmlStrongAuthenticationMethod strongAuthenticationMethodField;

		private DirectoryPropertyTargetAddress targetAddressField;

		private DirectoryPropertyStringSingleLength1To64 telephoneAssistantField;

		private DirectoryPropertyStringSingleLength1To64 telephoneNumberField;

		private DirectoryPropertyBinarySingleLength1To102400 thumbnailPhotoField;

		private DirectoryPropertyStringSingleLength1To128 titleField;

		private DirectoryPropertyStringLength1To1123 urlField;

		private DirectoryPropertyStringSingleLength1To3 usageLocationField;

		private DirectoryPropertyStringSingleLength1To1024 userPrincipalNameField;

		private DirectoryPropertyXmlValidationError validationErrorField;

		private DirectoryPropertyBinarySingleLength8 windowsLiveNetIdField;

		private DirectoryPropertyStringSingleLength1To2048 wwwHomepageField;

		private AttributeSet[] singleAuthorityMetadataField;

		private XmlAttribute[] anyAttrField;
	}
}
