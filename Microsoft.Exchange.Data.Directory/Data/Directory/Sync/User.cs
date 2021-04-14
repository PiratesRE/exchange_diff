using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	[DesignerCategory("code")]
	[GeneratedCode("svcutil", "4.0.30319.17627")]
	[XmlType(Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11")]
	[DebuggerStepThrough]
	[Serializable]
	public class User : DirectoryObject, IValidationErrorSupport
	{
		internal override void ForEachProperty(IPropertyProcessor processor)
		{
			processor.Process<DirectoryPropertyXmlAssignedPlan>(SyncUserSchema.AssignedPlan, ref this.assignedPlanField);
			processor.Process<DirectoryPropertyStringSingleLength1To3>(SyncOrgPersonSchema.C, ref this.cField);
			processor.Process<DirectoryPropertyStringSingleLength1To2048>(SyncRecipientSchema.CloudLegacyExchangeDN, ref this.cloudLegacyExchangeDNField);
			processor.Process<DirectoryPropertyInt32Single>(SyncUserSchema.CloudMsExchArchiveStatus, ref this.cloudMSExchArchiveStatusField);
			processor.Process<DirectoryPropertyBinarySingleLength1To4000>(SyncUserSchema.CloudMsExchBlockedSendersHash, ref this.cloudMSExchBlockedSendersHashField);
			processor.Process<DirectoryPropertyInt32Single>(SyncRecipientSchema.CloudMsExchRecipientDisplayType, ref this.cloudMSExchRecipientDisplayTypeField);
			processor.Process<DirectoryPropertyBinarySingleLength1To12000>(SyncUserSchema.CloudMsExchSafeRecipientsHash, ref this.cloudMSExchSafeRecipientsHashField);
			processor.Process<DirectoryPropertyBinarySingleLength1To32000>(SyncUserSchema.CloudMsExchSafeSendersHash, ref this.cloudMSExchSafeSendersHashField);
			processor.Process<DirectoryPropertyStringLength1To1123>(SyncUserSchema.CloudMsExchUCVoiceMailSettings, ref this.cloudMSExchUCVoiceMailSettingsField);
			processor.Process<DirectoryPropertyStringLength1To40>(SyncUserSchema.CloudMsExchUserHoldPolicies, ref this.cloudMSExchUserHoldPoliciesField);
			processor.Process<DirectoryPropertyReferenceAddressList>(SyncUserSchema.CloudSiteMailboxOwners, ref this.cloudMSExchTeamMailboxOwnersField);
			processor.Process<DirectoryPropertyDateTimeSingle>(SyncUserSchema.CloudSiteMailboxClosedTime, ref this.cloudMSExchTeamMailboxExpirationField);
			processor.Process<DirectoryPropertyStringSingleLength1To2048>(SyncUserSchema.CloudSharePointUrl, ref this.cloudMSExchTeamMailboxSharePointUrlField);
			processor.Process<DirectoryPropertyStringSingleLength1To128>(SyncOrgPersonSchema.Co, ref this.coField);
			processor.Process<DirectoryPropertyStringSingleLength1To64>(SyncOrgPersonSchema.Company, ref this.companyField);
			processor.Process<DirectoryPropertyInt32SingleMin0Max65535>(SyncOrgPersonSchema.CountryCode, ref this.countryCodeField);
			processor.Process<DirectoryPropertyStringSingleLength1To64>(SyncOrgPersonSchema.Department, ref this.departmentField);
			processor.Process<DirectoryPropertyBooleanSingle>(SyncUserSchema.DeliverToMailboxAndForward, ref this.deliverAndRedirectField);
			processor.Process<DirectoryPropertyStringSingleLength1To256>(SyncRecipientSchema.DisplayName, ref this.displayNameField);
			processor.Process<DirectoryPropertyBooleanSingle>(SyncRecipientSchema.IsDirSynced, ref this.dirSyncEnabledField);
			DirectoryPropertyAttributeSet directoryPropertyAttributeSet = (DirectoryPropertyAttributeSet)DirectoryObject.GetDirectoryProperty(this.singleAuthorityMetadataField);
			processor.Process<DirectoryPropertyAttributeSet>(SyncRecipientSchema.DirSyncAuthorityMetadata, ref directoryPropertyAttributeSet);
			processor.Process<DirectoryPropertyStringSingleLength1To1024>(SyncRecipientSchema.CustomAttribute1, ref this.extensionAttribute1Field);
			processor.Process<DirectoryPropertyStringSingleLength1To1024>(SyncRecipientSchema.CustomAttribute10, ref this.extensionAttribute10Field);
			processor.Process<DirectoryPropertyStringSingleLength1To2048>(SyncRecipientSchema.CustomAttribute11, ref this.extensionAttribute11Field);
			processor.Process<DirectoryPropertyStringSingleLength1To2048>(SyncRecipientSchema.CustomAttribute12, ref this.extensionAttribute12Field);
			processor.Process<DirectoryPropertyStringSingleLength1To2048>(SyncRecipientSchema.CustomAttribute13, ref this.extensionAttribute13Field);
			processor.Process<DirectoryPropertyStringSingleLength1To2048>(SyncRecipientSchema.CustomAttribute14, ref this.extensionAttribute14Field);
			processor.Process<DirectoryPropertyStringSingleLength1To2048>(SyncRecipientSchema.CustomAttribute15, ref this.extensionAttribute15Field);
			processor.Process<DirectoryPropertyStringSingleLength1To1024>(SyncRecipientSchema.CustomAttribute2, ref this.extensionAttribute2Field);
			processor.Process<DirectoryPropertyStringSingleLength1To1024>(SyncRecipientSchema.CustomAttribute3, ref this.extensionAttribute3Field);
			processor.Process<DirectoryPropertyStringSingleLength1To1024>(SyncRecipientSchema.CustomAttribute4, ref this.extensionAttribute4Field);
			processor.Process<DirectoryPropertyStringSingleLength1To1024>(SyncRecipientSchema.CustomAttribute5, ref this.extensionAttribute5Field);
			processor.Process<DirectoryPropertyStringSingleLength1To1024>(SyncRecipientSchema.CustomAttribute6, ref this.extensionAttribute6Field);
			processor.Process<DirectoryPropertyStringSingleLength1To1024>(SyncRecipientSchema.CustomAttribute7, ref this.extensionAttribute7Field);
			processor.Process<DirectoryPropertyStringSingleLength1To1024>(SyncRecipientSchema.CustomAttribute8, ref this.extensionAttribute8Field);
			processor.Process<DirectoryPropertyStringSingleLength1To1024>(SyncRecipientSchema.CustomAttribute9, ref this.extensionAttribute9Field);
			processor.Process<DirectoryPropertyStringLength1To2048>(SyncRecipientSchema.ExtensionCustomAttribute1, ref this.mSExchExtensionCustomAttribute1Field);
			processor.Process<DirectoryPropertyStringLength1To2048>(SyncRecipientSchema.ExtensionCustomAttribute2, ref this.mSExchExtensionCustomAttribute2Field);
			processor.Process<DirectoryPropertyStringLength1To2048>(SyncRecipientSchema.ExtensionCustomAttribute3, ref this.mSExchExtensionCustomAttribute3Field);
			processor.Process<DirectoryPropertyStringLength1To2048>(SyncRecipientSchema.ExtensionCustomAttribute4, ref this.mSExchExtensionCustomAttribute4Field);
			processor.Process<DirectoryPropertyStringLength1To2048>(SyncRecipientSchema.ExtensionCustomAttribute5, ref this.mSExchExtensionCustomAttribute5Field);
			processor.Process<DirectoryPropertyStringSingleLength1To64>(SyncOrgPersonSchema.Fax, ref this.facsimileTelephoneNumberField);
			processor.Process<DirectoryPropertyStringSingleLength1To64>(SyncOrgPersonSchema.FirstName, ref this.givenNameField);
			processor.Process<DirectoryPropertyStringSingleLength1To64>(SyncOrgPersonSchema.HomePhone, ref this.homePhoneField);
			processor.Process<DirectoryPropertyStringSingleLength1To1024>(SyncOrgPersonSchema.Notes, ref this.infoField);
			processor.Process<DirectoryPropertyStringSingleLength1To6>(SyncOrgPersonSchema.Initials, ref this.initialsField);
			processor.Process<DirectoryPropertyStringSingleLength1To128>(SyncOrgPersonSchema.City, ref this.lField);
			processor.Process<DirectoryPropertyStringSingleMailNickname>(SyncRecipientSchema.Alias, ref this.mailNicknameField);
			processor.Process<DirectoryPropertyStringSingleLength1To64>(SyncOrgPersonSchema.MobilePhone, ref this.mobileField);
			processor.Process<DirectoryPropertyInt32Single>(SyncRecipientSchema.SeniorityIndex, ref this.mSDSHABSeniorityIndexField);
			processor.Process<DirectoryPropertyStringSingleLength1To256>(SyncRecipientSchema.PhoneticDisplayName, ref this.mSDSPhoneticDisplayNameField);
			processor.Process<DirectoryPropertyGuidSingle>(SyncUserSchema.ArchiveGuid, ref this.mSExchArchiveGuidField);
			processor.Process<DirectoryPropertyStringLength1To512>(SyncUserSchema.ArchiveName, ref this.mSExchArchiveNameField);
			processor.Process<DirectoryPropertyStringSingleLength1To256>(SyncOrgPersonSchema.AssistantName, ref this.mSExchAssistantNameField);
			processor.Process<DirectoryPropertyInt32Single>(SyncUserSchema.AuditAdminFlags, ref this.mSExchAuditAdminField);
			processor.Process<DirectoryPropertyInt32Single>(SyncUserSchema.AuditDelegateFlags, ref this.mSExchAuditDelegateField);
			processor.Process<DirectoryPropertyInt32Single>(SyncUserSchema.AuditDelegateAdminFlags, ref this.mSExchAuditDelegateAdminField);
			processor.Process<DirectoryPropertyInt32Single>(SyncUserSchema.AuditOwnerFlags, ref this.mSExchAuditOwnerField);
			processor.Process<DirectoryPropertyBooleanSingle>(SyncUserSchema.AuditBypassEnabled, ref this.mSExchBypassAuditField);
			processor.Process<DirectoryPropertyDateTimeSingle>(SyncUserSchema.ElcExpirationSuspensionEndDate, ref this.mSExchElcExpirySuspensionEndField);
			processor.Process<DirectoryPropertyDateTimeSingle>(SyncUserSchema.ElcExpirationSuspensionStartDate, ref this.mSExchElcExpirySuspensionStartField);
			processor.Process<DirectoryPropertyInt32Single>(SyncUserSchema.ElcMailboxFlags, ref this.mSExchElcMailboxFlagsField);
			processor.Process<DirectoryPropertyBooleanSingle>(SyncRecipientSchema.ModerationEnabled, ref this.mSExchEnableModerationField);
			processor.Process<DirectoryPropertyBooleanSingle>(SyncRecipientSchema.HiddenFromAddressListsEnabled, ref this.mSExchHideFromAddressListsField);
			processor.Process<DirectoryPropertyStringSingleLength1To256>(SyncUserSchema.ImmutableId, ref this.mSExchImmutableIdField);
			processor.Process<DirectoryPropertyStringLength1To40>(SyncUserSchema.InPlaceHoldsRaw, ref this.mSExchUserHoldPoliciesField);
			processor.Process<DirectoryPropertyDateTimeSingle>(SyncRecipientSchema.LitigationHoldDate, ref this.mSExchLitigationHoldDateField);
			processor.Process<DirectoryPropertyStringSingleLength1To1024>(SyncRecipientSchema.LitigationHoldOwner, ref this.mSExchLitigationHoldOwnerField);
			processor.Process<DirectoryPropertyBooleanSingle>(SyncUserSchema.AuditEnabled, ref this.mSExchMailboxAuditEnableField);
			processor.Process<DirectoryPropertyInt32Single>(SyncUserSchema.AuditLogAgeLimit, ref this.mSExchMailboxAuditLogAgeLimitField);
			processor.Process<DirectoryPropertyGuidSingle>(SyncUserSchema.ExchangeGuid, ref this.mSExchMailboxGuidField);
			processor.Process<DirectoryPropertyInt32Single>(SyncRecipientSchema.RecipientDisplayType, ref this.mSExchRecipientDisplayTypeField);
			processor.Process<DirectoryPropertyInt64Single>(SyncRecipientSchema.RecipientTypeDetailsValue, ref this.mSExchRecipientTypeDetailsField);
			processor.Process<DirectoryPropertyBooleanSingle>(SyncRecipientSchema.RequireAllSendersAreAuthenticated, ref this.mSExchRequireAuthToSendToField);
			processor.Process<DirectoryPropertyStringSingleLength1To1024>(SyncRecipientSchema.RetentionComment, ref this.mSExchRetentionCommentField);
			processor.Process<DirectoryPropertyStringSingleLength1To2048>(SyncRecipientSchema.RetentionUrl, ref this.mSExchRetentionUrlField);
			processor.Process<DirectoryPropertyStringLength2To500>(SyncRecipientSchema.MailTipTranslations, ref this.mSExchSenderHintTranslationsField);
			processor.Process<DirectoryPropertyStringLength1To64>(SyncOrgPersonSchema.OtherFax, ref this.otherFacsimileTelephoneNumberField);
			processor.Process<DirectoryPropertyStringLength1To64>(SyncOrgPersonSchema.OtherHomePhone, ref this.otherHomePhoneField);
			processor.Process<DirectoryPropertyStringLength1To64>(SyncOrgPersonSchema.OtherTelephone, ref this.otherTelephoneField);
			processor.Process<DirectoryPropertyStringSingleLength1To64>(SyncOrgPersonSchema.Pager, ref this.pagerField);
			processor.Process<DirectoryPropertyStringSingleLength1To128>(SyncOrgPersonSchema.Office, ref this.physicalDeliveryOfficeNameField);
			processor.Process<DirectoryPropertyStringSingleLength1To40>(SyncOrgPersonSchema.PostalCode, ref this.postalCodeField);
			processor.Process<DirectoryPropertyProxyAddresses>(SyncRecipientSchema.EmailAddresses, ref this.proxyAddressesField);
			processor.Process<DirectoryPropertyProxyAddresses>(SyncRecipientSchema.SmtpAndX500Addresses, ref this.proxyAddressesField);
			processor.Process<DirectoryPropertyStringSingleLength1To454>(SyncRecipientSchema.SipAddresses, ref this.sipProxyAddressField);
			processor.Process<DirectoryPropertyXmlServiceInfo>(SyncUserSchema.ServiceInfo, ref this.serviceInfoField);
			processor.Process<DirectoryPropertyXmlServiceOriginatedResource>(SyncUserSchema.ServiceOriginatedResource, ref this.serviceOriginatedResourceField);
			processor.Process<DirectoryPropertyStringSingleLength1To64>(SyncRecipientSchema.Cn, ref this.shadowCommonNameField);
			processor.Process<DirectoryPropertyStringSingleLength1To64>(SyncOrgPersonSchema.LastName, ref this.snField);
			processor.Process<DirectoryPropertyStringSingleLength1To256>(SyncRecipientSchema.OnPremisesObjectId, ref this.sourceAnchorField);
			processor.Process<DirectoryPropertyStringSingleLength1To128>(SyncOrgPersonSchema.StateOrProvince, ref this.stField);
			processor.Process<DirectoryPropertyStringSingleLength1To1024>(SyncOrgPersonSchema.StreetAddress, ref this.streetAddressField);
			processor.Process<DirectoryPropertyTargetAddress>(SyncRecipientSchema.ExternalEmailAddress, ref this.targetAddressField);
			processor.Process<DirectoryPropertyStringSingleLength1To64>(SyncOrgPersonSchema.TelephoneAssistant, ref this.telephoneAssistantField);
			processor.Process<DirectoryPropertyStringSingleLength1To64>(SyncOrgPersonSchema.Phone, ref this.telephoneNumberField);
			processor.Process<DirectoryPropertyBinarySingleLength1To102400>(SyncUserSchema.Picture, ref this.thumbnailPhotoField);
			processor.Process<DirectoryPropertyStringSingleLength1To128>(SyncOrgPersonSchema.Title, ref this.titleField);
			processor.Process<DirectoryPropertyStringSingleLength1To3>(SyncUserSchema.UsageLocation, ref this.usageLocationField);
			processor.Process<DirectoryPropertyXmlValidationError>(SyncRecipientSchema.ValidationError, ref this.validationErrorField);
			processor.Process<DirectoryPropertyStringSingleLength1To1024>(SyncUserSchema.WindowsLiveID, ref this.userPrincipalNameField);
			processor.Process<DirectoryPropertyBinarySingleLength8>(SyncUserSchema.NetID, ref this.windowsLiveNetIdField);
			processor.Process<DirectoryPropertyStringSingleLength1To2048>(SyncOrgPersonSchema.WebPage, ref this.wwwHomepageField);
			processor.Process<DirectoryPropertyInt64Single>(SyncUserSchema.RemoteRecipientType, ref this.mSExchRemoteRecipientTypeField);
			processor.Process<DirectoryPropertyInt32Single>(SyncRecipientSchema.ModerationFlags, ref this.mSExchModerationFlagsField);
			processor.Process<DirectoryPropertyBinarySingleLength1To4000>(SyncRecipientSchema.BlockedSendersHash, ref this.mSExchBlockedSendersHashField);
			processor.Process<DirectoryPropertyBinarySingleLength1To12000>(SyncRecipientSchema.SafeRecipientsHash, ref this.mSExchSafeRecipientsHashField);
			processor.Process<DirectoryPropertyBinarySingleLength1To32000>(SyncRecipientSchema.SafeSendersHash, ref this.mSExchSafeSendersHashField);
			processor.Process<DirectoryPropertyInt32Single>(SyncUserSchema.ResourceCapacity, ref this.mSExchResourceCapacityField);
			processor.Process<DirectoryPropertyStringSingleLength1To1024>(SyncUserSchema.ResourcePropertiesDisplay, ref this.mSExchResourceDisplayField);
			processor.Process<DirectoryPropertyStringLength1To1024>(SyncUserSchema.ResourceMetaData, ref this.mSExchResourceMetadataField);
			processor.Process<DirectoryPropertyStringLength1To1024>(SyncUserSchema.ResourceSearchProperties, ref this.mSExchResourceSearchPropertiesField);
			processor.Process<DirectoryPropertyDateTimeSingle>(SyncUserSchema.WhenSoftDeleted, ref this.softDeletionTimestampField);
			processor.Process<DirectoryPropertyDateTimeSingle>(SyncUserSchema.MSExchUserCreatedTimestamp, ref this.mSExchUserCreatedTimestampField);
			processor.Process<DirectoryPropertyReferenceAddressList>(SyncUserSchema.SiteMailboxOwners, ref this.mSExchTeamMailboxOwnersField);
			processor.Process<DirectoryPropertyDateTimeSingle>(SyncUserSchema.SiteMailboxClosedTime, ref this.mSExchTeamMailboxExpirationField);
			processor.Process<DirectoryPropertyStringSingleLength1To2048>(SyncUserSchema.SharePointUrl, ref this.mSExchTeamMailboxSharePointUrlField);
			processor.Process<DirectoryPropertyBinaryLength1To32768>(SyncRecipientSchema.UserCertificate, ref this.userCertificateField);
			processor.Process<DirectoryPropertyBinaryLength1To32768>(SyncRecipientSchema.UserSMimeCertificate, ref this.userSMIMECertificateField);
			processor.Process<DirectoryPropertyBooleanSingle>(SyncUserSchema.AccountEnabled, ref this.accountEnabledField);
			processor.Process<DirectoryPropertyDateTimeSingle>(SyncUserSchema.StsRefreshTokensValidFrom, ref this.stsRefreshTokensValidFromField);
		}

		[XmlElement(Order = 0)]
		public DirectoryPropertyStringSingleLength1To256 AcceptedAs
		{
			get
			{
				return this.acceptedAsField;
			}
			set
			{
				this.acceptedAsField = value;
			}
		}

		[XmlElement(Order = 1)]
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

		[XmlElement(Order = 2)]
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

		[XmlElement(Order = 3)]
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

		[XmlElement(Order = 4)]
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

		[XmlElement(Order = 5)]
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

		[XmlElement(Order = 6)]
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

		[XmlElement(Order = 7)]
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

		[XmlElement(Order = 8)]
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

		[XmlElement(Order = 9)]
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

		[XmlElement(Order = 10)]
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

		[XmlElement(Order = 11)]
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

		[XmlElement(Order = 12)]
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

		[XmlElement(Order = 13)]
		public DirectoryPropertyDateTimeSingle CloudMSExchTeamMailboxExpiration
		{
			get
			{
				return this.cloudMSExchTeamMailboxExpirationField;
			}
			set
			{
				this.cloudMSExchTeamMailboxExpirationField = value;
			}
		}

		[XmlElement(Order = 14)]
		public DirectoryPropertyReferenceAddressList CloudMSExchTeamMailboxOwners
		{
			get
			{
				return this.cloudMSExchTeamMailboxOwnersField;
			}
			set
			{
				this.cloudMSExchTeamMailboxOwnersField = value;
			}
		}

		[XmlElement(Order = 15)]
		public DirectoryPropertyStringSingleLength1To2048 CloudMSExchTeamMailboxSharePointUrl
		{
			get
			{
				return this.cloudMSExchTeamMailboxSharePointUrlField;
			}
			set
			{
				this.cloudMSExchTeamMailboxSharePointUrlField = value;
			}
		}

		[XmlElement(Order = 16)]
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

		[XmlElement(Order = 17)]
		public DirectoryPropertyStringLength1To40 CloudMSExchUserHoldPolicies
		{
			get
			{
				return this.cloudMSExchUserHoldPoliciesField;
			}
			set
			{
				this.cloudMSExchUserHoldPoliciesField = value;
			}
		}

		[XmlElement(Order = 18)]
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

		[XmlElement(Order = 19)]
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

		[XmlElement(Order = 20)]
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

		[XmlElement(Order = 21)]
		public DirectoryPropertyDateTimeSingle CreatedOn
		{
			get
			{
				return this.createdOnField;
			}
			set
			{
				this.createdOnField = value;
			}
		}

		[XmlElement(Order = 22)]
		public DirectoryPropertyInt32SingleMin0 CreationType
		{
			get
			{
				return this.creationTypeField;
			}
			set
			{
				this.creationTypeField = value;
			}
		}

		[XmlElement(Order = 23)]
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

		[XmlElement(Order = 24)]
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

		[XmlElement(Order = 25)]
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

		[XmlElement(Order = 26)]
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

		[XmlElement(Order = 27)]
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

		[XmlElement(Order = 28)]
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

		[XmlElement(Order = 29)]
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

		[XmlElement(Order = 30)]
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

		[XmlElement(Order = 31)]
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

		[XmlElement(Order = 32)]
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

		[XmlElement(Order = 33)]
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

		[XmlElement(Order = 34)]
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

		[XmlElement(Order = 35)]
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

		[XmlElement(Order = 36)]
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

		[XmlElement(Order = 37)]
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

		[XmlElement(Order = 38)]
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

		[XmlElement(Order = 39)]
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

		[XmlElement(Order = 40)]
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

		[XmlElement(Order = 41)]
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

		[XmlElement(Order = 42)]
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

		[XmlElement(Order = 43)]
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

		[XmlElement(Order = 44)]
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

		[XmlElement(Order = 45)]
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

		[XmlElement(Order = 46)]
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

		[XmlElement(Order = 47)]
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

		[XmlElement(Order = 48)]
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

		[XmlElement(Order = 49)]
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

		[XmlElement(Order = 50)]
		public DirectoryPropertyStringLength1To2048 InviteResources
		{
			get
			{
				return this.inviteResourcesField;
			}
			set
			{
				this.inviteResourcesField = value;
			}
		}

		[XmlElement(Order = 51)]
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

		[XmlElement(Order = 52)]
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

		[XmlElement(Order = 53)]
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

		[XmlElement(Order = 54)]
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

		[XmlElement(Order = 55)]
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

		[XmlElement(Order = 56)]
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

		[XmlElement(Order = 57)]
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

		[XmlElement(Order = 58)]
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

		[XmlElement(Order = 59)]
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

		[XmlElement(Order = 60)]
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

		[XmlElement(Order = 61)]
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

		[XmlElement(Order = 62)]
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

		[XmlElement(Order = 63)]
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

		[XmlElement(Order = 64)]
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

		[XmlElement(Order = 65)]
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

		[XmlElement(Order = 66)]
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

		[XmlElement(Order = 67)]
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

		[XmlElement(Order = 68)]
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

		[XmlElement(Order = 69)]
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

		[XmlElement(Order = 70)]
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

		[XmlElement(Order = 71)]
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

		[XmlElement(Order = 72)]
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

		[XmlElement(Order = 73)]
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

		[XmlElement(Order = 74)]
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

		[XmlElement(Order = 75)]
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

		[XmlElement(Order = 76)]
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

		[XmlElement(Order = 77)]
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

		[XmlElement(Order = 78)]
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

		[XmlElement(Order = 79)]
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

		[XmlElement(Order = 80)]
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

		[XmlElement(Order = 81)]
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

		[XmlElement(Order = 82)]
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

		[XmlElement(Order = 83)]
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

		[XmlElement(Order = 84)]
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

		[XmlElement(Order = 85)]
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

		[XmlElement(Order = 86)]
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

		[XmlElement(Order = 87)]
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

		[XmlElement(Order = 88)]
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

		[XmlElement(Order = 89)]
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

		[XmlElement(Order = 90)]
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

		[XmlElement(Order = 91)]
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

		[XmlElement(Order = 92)]
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

		[XmlElement(Order = 93)]
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

		[XmlElement(Order = 94)]
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

		[XmlElement(Order = 95)]
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

		[XmlElement(Order = 96)]
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

		[XmlElement(Order = 97)]
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

		[XmlElement(Order = 98)]
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

		[XmlElement(Order = 99)]
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

		[XmlElement(Order = 100)]
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

		[XmlElement(Order = 101)]
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

		[XmlElement(Order = 102)]
		public DirectoryPropertyDateTimeSingle MSExchUserCreatedTimestamp
		{
			get
			{
				return this.mSExchUserCreatedTimestampField;
			}
			set
			{
				this.mSExchUserCreatedTimestampField = value;
			}
		}

		[XmlElement(Order = 103)]
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

		[XmlElement(Order = 104)]
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

		[XmlElement(Order = 105)]
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

		[XmlElement(Order = 106)]
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

		[XmlElement(Order = 107)]
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

		[XmlElement(Order = 108)]
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

		[XmlElement(Order = 109)]
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

		[XmlElement(Order = 110)]
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

		[XmlElement(Order = 111)]
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

		[XmlElement(Order = 112)]
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

		[XmlElement(Order = 113)]
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

		[XmlElement(Order = 114)]
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

		[XmlElement(Order = 115)]
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

		[XmlElement(Order = 116)]
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

		[XmlElement(Order = 117)]
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

		[XmlElement(Order = 118)]
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

		[XmlElement(Order = 119)]
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

		[XmlElement(Order = 120)]
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

		[XmlElement(Order = 121)]
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

		[XmlElement(Order = 122)]
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

		[XmlElement(Order = 123)]
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

		[XmlElement(Order = 124)]
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

		[XmlElement(Order = 125)]
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

		[XmlElement(Order = 126)]
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

		[XmlElement(Order = 127)]
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

		[XmlElement(Order = 128)]
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

		[XmlElement(Order = 129)]
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

		[XmlElement(Order = 130)]
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

		[XmlElement(Order = 131)]
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

		[XmlElement(Order = 132)]
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

		[XmlElement(Order = 133)]
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

		[XmlElement(Order = 134)]
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

		[XmlElement(Order = 135)]
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

		[XmlElement(Order = 136)]
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

		[XmlElement(Order = 137)]
		public DirectoryPropertyDateTimeSingle StsRefreshTokensValidFrom
		{
			get
			{
				return this.stsRefreshTokensValidFromField;
			}
			set
			{
				this.stsRefreshTokensValidFromField = value;
			}
		}

		[XmlElement(Order = 138)]
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

		[XmlElement(Order = 139)]
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

		[XmlElement(Order = 140)]
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

		[XmlElement(Order = 141)]
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

		[XmlElement(Order = 142)]
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

		[XmlElement(Order = 143)]
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

		[XmlElement(Order = 144)]
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

		[XmlElement(Order = 145)]
		public DirectoryPropertyBinaryLength1To32768 UserCertificate
		{
			get
			{
				return this.userCertificateField;
			}
			set
			{
				this.userCertificateField = value;
			}
		}

		[XmlElement(Order = 146)]
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

		[XmlElement(Order = 147)]
		public DirectoryPropertyBinaryLength1To32768 UserSMIMECertificate
		{
			get
			{
				return this.userSMIMECertificateField;
			}
			set
			{
				this.userSMIMECertificateField = value;
			}
		}

		[XmlElement(Order = 148)]
		public DirectoryPropertyInt32SingleMin0 UserState
		{
			get
			{
				return this.userStateField;
			}
			set
			{
				this.userStateField = value;
			}
		}

		[XmlElement(Order = 149)]
		public DirectoryPropertyInt32SingleMin0Max2 UserType
		{
			get
			{
				return this.userTypeField;
			}
			set
			{
				this.userTypeField = value;
			}
		}

		[XmlElement(Order = 150)]
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

		[XmlElement(Order = 151)]
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

		[XmlElement(Order = 152)]
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

		[XmlArrayItem("AttributeSet", IsNullable = false)]
		[XmlArray(Namespace = "http://schemas.microsoft.com/online/directoryservices/sync/metadata/2010/01", Order = 153)]
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

		private DirectoryPropertyStringSingleLength1To256 acceptedAsField;

		private DirectoryPropertyBooleanSingle accountEnabledField;

		private DirectoryPropertyXmlAlternativeSecurityId alternativeSecurityIdField;

		private DirectoryPropertyXmlAssignedPlan assignedPlanField;

		private DirectoryPropertyReferenceAddressListSingle assistantField;

		private DirectoryPropertyStringSingleLength1To3 cField;

		private DirectoryPropertyStringSingleLength1To2048 cloudLegacyExchangeDNField;

		private DirectoryPropertyInt32Single cloudMSExchArchiveStatusField;

		private DirectoryPropertyBinarySingleLength1To4000 cloudMSExchBlockedSendersHashField;

		private DirectoryPropertyGuidSingle cloudMSExchMailboxGuidField;

		private DirectoryPropertyInt32Single cloudMSExchRecipientDisplayTypeField;

		private DirectoryPropertyBinarySingleLength1To12000 cloudMSExchSafeRecipientsHashField;

		private DirectoryPropertyBinarySingleLength1To32000 cloudMSExchSafeSendersHashField;

		private DirectoryPropertyDateTimeSingle cloudMSExchTeamMailboxExpirationField;

		private DirectoryPropertyReferenceAddressList cloudMSExchTeamMailboxOwnersField;

		private DirectoryPropertyStringSingleLength1To2048 cloudMSExchTeamMailboxSharePointUrlField;

		private DirectoryPropertyStringLength1To1123 cloudMSExchUCVoiceMailSettingsField;

		private DirectoryPropertyStringLength1To40 cloudMSExchUserHoldPoliciesField;

		private DirectoryPropertyStringSingleLength1To128 coField;

		private DirectoryPropertyStringSingleLength1To64 companyField;

		private DirectoryPropertyInt32SingleMin0Max65535 countryCodeField;

		private DirectoryPropertyDateTimeSingle createdOnField;

		private DirectoryPropertyInt32SingleMin0 creationTypeField;

		private DirectoryPropertyBooleanSingle deliverAndRedirectField;

		private DirectoryPropertyStringSingleLength1To64 departmentField;

		private DirectoryPropertyStringSingleLength1To1024 descriptionField;

		private DirectoryPropertyBooleanSingle dirSyncEnabledField;

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

		private DirectoryPropertyStringLength1To2048 inviteResourcesField;

		private DirectoryPropertyStringSingleLength1To64 iPPhoneField;

		private DirectoryPropertyStringSingleLength1To128 lField;

		private DirectoryPropertyStringSingleLength1To256 mailField;

		private DirectoryPropertyStringSingleMailNickname mailNicknameField;

		private DirectoryPropertyStringSingleLength1To64 middleNameField;

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

		private DirectoryPropertyDateTimeSingle mSExchUserCreatedTimestampField;

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

		private DirectoryPropertyStringSingleLength1To64 pagerField;

		private DirectoryPropertyStringSingleLength1To128 physicalDeliveryOfficeNameField;

		private DirectoryPropertyStringSingleLength1To40 postalCodeField;

		private DirectoryPropertyStringLength1To40 postOfficeBoxField;

		private DirectoryPropertyStringSingleLength1To64 preferredLanguageField;

		private DirectoryPropertyProxyAddresses proxyAddressesField;

		private DirectoryPropertyXmlRightsManagementUserKeySingle rightsManagementUserKeyField;

		private DirectoryPropertyXmlServiceInfo serviceInfoField;

		private DirectoryPropertyXmlServiceOriginatedResource serviceOriginatedResourceField;

		private DirectoryPropertyStringSingleLength1To64 shadowCommonNameField;

		private DirectoryPropertyStringLength1To1123 shadowProxyAddressesField;

		private DirectoryPropertyStringSingleLength1To454 sipProxyAddressField;

		private DirectoryPropertyStringSingleLength1To64 snField;

		private DirectoryPropertyDateTimeSingle softDeletionTimestampField;

		private DirectoryPropertyStringSingleLength1To256 sourceAnchorField;

		private DirectoryPropertyStringSingleLength1To128 stField;

		private DirectoryPropertyStringSingleLength1To1024 streetField;

		private DirectoryPropertyStringSingleLength1To1024 streetAddressField;

		private DirectoryPropertyDateTimeSingle stsRefreshTokensValidFromField;

		private DirectoryPropertyTargetAddress targetAddressField;

		private DirectoryPropertyStringSingleLength1To64 telephoneAssistantField;

		private DirectoryPropertyStringSingleLength1To64 telephoneNumberField;

		private DirectoryPropertyBinarySingleLength1To102400 thumbnailPhotoField;

		private DirectoryPropertyStringSingleLength1To128 titleField;

		private DirectoryPropertyStringLength1To1123 urlField;

		private DirectoryPropertyStringSingleLength1To3 usageLocationField;

		private DirectoryPropertyBinaryLength1To32768 userCertificateField;

		private DirectoryPropertyStringSingleLength1To1024 userPrincipalNameField;

		private DirectoryPropertyBinaryLength1To32768 userSMIMECertificateField;

		private DirectoryPropertyInt32SingleMin0 userStateField;

		private DirectoryPropertyInt32SingleMin0Max2 userTypeField;

		private DirectoryPropertyXmlValidationError validationErrorField;

		private DirectoryPropertyBinarySingleLength8 windowsLiveNetIdField;

		private DirectoryPropertyStringSingleLength1To2048 wwwHomepageField;

		private AttributeSet[] singleAuthorityMetadataField;

		private XmlAttribute[] anyAttrField;
	}
}
