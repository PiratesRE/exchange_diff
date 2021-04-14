using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11")]
	[GeneratedCode("svcutil", "4.0.30319.17627")]
	[Serializable]
	public class Contact : DirectoryObject, IValidationErrorSupport
	{
		internal override void ForEachProperty(IPropertyProcessor processor)
		{
			processor.Process<DirectoryPropertyStringSingleLength1To3>(SyncOrgPersonSchema.C, ref this.cField);
			processor.Process<DirectoryPropertyStringSingleLength1To128>(SyncOrgPersonSchema.Co, ref this.coField);
			processor.Process<DirectoryPropertyStringSingleLength1To64>(SyncOrgPersonSchema.Company, ref this.companyField);
			processor.Process<DirectoryPropertyInt32SingleMin0Max65535>(SyncOrgPersonSchema.CountryCode, ref this.countryCodeField);
			processor.Process<DirectoryPropertyStringSingleLength1To64>(SyncOrgPersonSchema.Department, ref this.departmentField);
			processor.Process<DirectoryPropertyStringSingleLength1To256>(SyncRecipientSchema.DisplayName, ref this.displayNameField);
			processor.Process<DirectoryPropertyStringSingleLength1To2048>(SyncRecipientSchema.CloudLegacyExchangeDN, ref this.cloudLegacyExchangeDNField);
			processor.Process<DirectoryPropertyInt32Single>(SyncRecipientSchema.CloudMsExchRecipientDisplayType, ref this.cloudMSExchRecipientDisplayTypeField);
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
			processor.Process<DirectoryPropertyStringSingleLength1To256>(SyncOrgPersonSchema.AssistantName, ref this.mSExchAssistantNameField);
			processor.Process<DirectoryPropertyBooleanSingle>(SyncRecipientSchema.HiddenFromAddressListsEnabled, ref this.mSExchHideFromAddressListsField);
			processor.Process<DirectoryPropertyDateTimeSingle>(SyncRecipientSchema.LitigationHoldDate, ref this.mSExchLitigationHoldDateField);
			processor.Process<DirectoryPropertyStringSingleLength1To1024>(SyncRecipientSchema.LitigationHoldOwner, ref this.mSExchLitigationHoldOwnerField);
			processor.Process<DirectoryPropertyBooleanSingle>(SyncRecipientSchema.ModerationEnabled, ref this.mSExchEnableModerationField);
			processor.Process<DirectoryPropertyInt64Single>(SyncRecipientSchema.RecipientTypeDetailsValue, ref this.mSExchRecipientTypeDetailsField);
			processor.Process<DirectoryPropertyStringSingleLength1To1024>(SyncRecipientSchema.RetentionComment, ref this.mSExchRetentionCommentField);
			processor.Process<DirectoryPropertyStringSingleLength1To2048>(SyncRecipientSchema.RetentionUrl, ref this.mSExchRetentionUrlField);
			processor.Process<DirectoryPropertyBooleanSingle>(SyncRecipientSchema.RequireAllSendersAreAuthenticated, ref this.mSExchRequireAuthToSendToField);
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
			processor.Process<DirectoryPropertyStringSingleLength1To64>(SyncRecipientSchema.Cn, ref this.shadowCommonNameField);
			processor.Process<DirectoryPropertyStringSingleLength1To64>(SyncOrgPersonSchema.LastName, ref this.snField);
			processor.Process<DirectoryPropertyStringSingleLength1To256>(SyncRecipientSchema.OnPremisesObjectId, ref this.sourceAnchorField);
			processor.Process<DirectoryPropertyStringSingleLength1To128>(SyncOrgPersonSchema.StateOrProvince, ref this.stField);
			processor.Process<DirectoryPropertyStringSingleLength1To1024>(SyncOrgPersonSchema.StreetAddress, ref this.streetAddressField);
			processor.Process<DirectoryPropertyTargetAddress>(SyncRecipientSchema.ExternalEmailAddress, ref this.targetAddressField);
			processor.Process<DirectoryPropertyStringSingleLength1To64>(SyncOrgPersonSchema.TelephoneAssistant, ref this.telephoneAssistantField);
			processor.Process<DirectoryPropertyStringSingleLength1To64>(SyncOrgPersonSchema.Phone, ref this.telephoneNumberField);
			processor.Process<DirectoryPropertyStringSingleLength1To128>(SyncOrgPersonSchema.Title, ref this.titleField);
			processor.Process<DirectoryPropertyXmlValidationError>(SyncRecipientSchema.ValidationError, ref this.validationErrorField);
			processor.Process<DirectoryPropertyStringSingleLength1To2048>(SyncOrgPersonSchema.WebPage, ref this.wwwHomepageField);
			processor.Process<DirectoryPropertyBooleanSingle>(SyncRecipientSchema.IsDirSynced, ref this.dirSyncEnabledField);
			DirectoryPropertyAttributeSet directoryPropertyAttributeSet = (DirectoryPropertyAttributeSet)DirectoryObject.GetDirectoryProperty(this.singleAuthorityMetadataField);
			processor.Process<DirectoryPropertyAttributeSet>(SyncRecipientSchema.DirSyncAuthorityMetadata, ref directoryPropertyAttributeSet);
			processor.Process<DirectoryPropertyInt32Single>(SyncRecipientSchema.ModerationFlags, ref this.mSExchModerationFlagsField);
			processor.Process<DirectoryPropertyBinarySingleLength1To4000>(SyncRecipientSchema.BlockedSendersHash, ref this.mSExchBlockedSendersHashField);
			processor.Process<DirectoryPropertyBinarySingleLength1To12000>(SyncRecipientSchema.SafeRecipientsHash, ref this.mSExchSafeRecipientsHashField);
			processor.Process<DirectoryPropertyBinarySingleLength1To32000>(SyncRecipientSchema.SafeSendersHash, ref this.mSExchSafeSendersHashField);
			processor.Process<DirectoryPropertyInt32Single>(SyncRecipientSchema.RecipientDisplayType, ref this.mSExchRecipientDisplayTypeField);
			processor.Process<DirectoryPropertyBinaryLength1To32768>(SyncRecipientSchema.UserCertificate, ref this.userCertificateField);
			processor.Process<DirectoryPropertyBinaryLength1To32768>(SyncRecipientSchema.UserSMimeCertificate, ref this.userSMIMECertificateField);
		}

		[XmlElement(Order = 0)]
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

		[XmlElement(Order = 1)]
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

		[XmlElement(Order = 2)]
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

		[XmlElement(Order = 3)]
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

		[XmlElement(Order = 4)]
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

		[XmlElement(Order = 5)]
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

		[XmlElement(Order = 6)]
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

		[XmlElement(Order = 7)]
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

		[XmlElement(Order = 8)]
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

		[XmlElement(Order = 9)]
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

		[XmlElement(Order = 10)]
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

		[XmlElement(Order = 11)]
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

		[XmlElement(Order = 12)]
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

		[XmlElement(Order = 13)]
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

		[XmlElement(Order = 14)]
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

		[XmlElement(Order = 15)]
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

		[XmlElement(Order = 16)]
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

		[XmlElement(Order = 17)]
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

		[XmlElement(Order = 18)]
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

		[XmlElement(Order = 19)]
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

		[XmlElement(Order = 20)]
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

		[XmlElement(Order = 21)]
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

		[XmlElement(Order = 22)]
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

		[XmlElement(Order = 23)]
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

		[XmlElement(Order = 24)]
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

		[XmlElement(Order = 25)]
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

		[XmlElement(Order = 26)]
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

		[XmlElement(Order = 27)]
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

		[XmlElement(Order = 28)]
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

		[XmlElement(Order = 29)]
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

		[XmlElement(Order = 30)]
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

		[XmlElement(Order = 31)]
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

		[XmlElement(Order = 32)]
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

		[XmlElement(Order = 33)]
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

		[XmlElement(Order = 34)]
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

		[XmlElement(Order = 35)]
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

		[XmlElement(Order = 36)]
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

		[XmlElement(Order = 37)]
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

		[XmlElement(Order = 38)]
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

		[XmlElement(Order = 39)]
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

		[XmlElement(Order = 40)]
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

		[XmlElement(Order = 41)]
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

		[XmlElement(Order = 42)]
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

		[XmlElement(Order = 43)]
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

		[XmlElement(Order = 44)]
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

		[XmlElement(Order = 45)]
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

		[XmlElement(Order = 46)]
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

		[XmlElement(Order = 47)]
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

		[XmlElement(Order = 48)]
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

		[XmlElement(Order = 49)]
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

		[XmlElement(Order = 50)]
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

		[XmlElement(Order = 51)]
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

		[XmlElement(Order = 52)]
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

		[XmlElement(Order = 53)]
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

		[XmlElement(Order = 54)]
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

		[XmlElement(Order = 55)]
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

		[XmlElement(Order = 56)]
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

		[XmlElement(Order = 57)]
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

		[XmlElement(Order = 58)]
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

		[XmlElement(Order = 59)]
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

		[XmlElement(Order = 60)]
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

		[XmlElement(Order = 61)]
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

		[XmlElement(Order = 62)]
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

		[XmlElement(Order = 63)]
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

		[XmlElement(Order = 64)]
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

		[XmlElement(Order = 65)]
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

		[XmlElement(Order = 66)]
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

		[XmlElement(Order = 67)]
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

		[XmlElement(Order = 68)]
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

		[XmlElement(Order = 69)]
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

		[XmlElement(Order = 70)]
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

		[XmlElement(Order = 71)]
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

		[XmlElement(Order = 72)]
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

		[XmlElement(Order = 73)]
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

		[XmlElement(Order = 74)]
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

		[XmlElement(Order = 75)]
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

		[XmlElement(Order = 76)]
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

		[XmlElement(Order = 77)]
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

		[XmlElement(Order = 78)]
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

		[XmlElement(Order = 79)]
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

		[XmlElement(Order = 80)]
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

		[XmlElement(Order = 81)]
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

		[XmlElement(Order = 82)]
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

		[XmlElement(Order = 83)]
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

		[XmlElement(Order = 84)]
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

		[XmlElement(Order = 85)]
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

		[XmlElement(Order = 86)]
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

		[XmlElement(Order = 87)]
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

		[XmlElement(Order = 88)]
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

		[XmlElement(Order = 89)]
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

		[XmlElement(Order = 90)]
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

		[XmlElement(Order = 91)]
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

		[XmlElement(Order = 92)]
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

		[XmlElement(Order = 93)]
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

		[XmlElement(Order = 94)]
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

		[XmlElement(Order = 95)]
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

		[XmlElement(Order = 96)]
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

		[XmlElement(Order = 97)]
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
		[XmlArray(Namespace = "http://schemas.microsoft.com/online/directoryservices/sync/metadata/2010/01", Order = 98)]
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

		private DirectoryPropertyReferenceAddressListSingle assistantField;

		private DirectoryPropertyStringSingleLength1To3 cField;

		private DirectoryPropertyStringSingleLength1To2048 cloudLegacyExchangeDNField;

		private DirectoryPropertyBinarySingleLength1To4000 cloudMSExchBlockedSendersHashField;

		private DirectoryPropertyInt32Single cloudMSExchRecipientDisplayTypeField;

		private DirectoryPropertyBinarySingleLength1To12000 cloudMSExchSafeRecipientsHashField;

		private DirectoryPropertyBinarySingleLength1To32000 cloudMSExchSafeSendersHashField;

		private DirectoryPropertyStringSingleLength1To128 coField;

		private DirectoryPropertyStringSingleLength1To64 companyField;

		private DirectoryPropertyInt32SingleMin0Max65535 countryCodeField;

		private DirectoryPropertyStringSingleLength1To64 departmentField;

		private DirectoryPropertyStringSingleLength1To1024 descriptionField;

		private DirectoryPropertyBooleanSingle dirSyncEnabledField;

		private DirectoryPropertyStringSingleLength1To256 displayNameField;

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

		private DirectoryPropertyStringSingleLength1To256 mailField;

		private DirectoryPropertyStringSingleMailNickname mailNicknameField;

		private DirectoryPropertyStringSingleLength1To64 middleNameField;

		private DirectoryPropertyStringSingleLength1To64 mobileField;

		private DirectoryPropertyInt32Single mSDSHABSeniorityIndexField;

		private DirectoryPropertyStringSingleLength1To256 mSDSPhoneticDisplayNameField;

		private DirectoryPropertyStringSingleLength1To256 mSExchAssistantNameField;

		private DirectoryPropertyBinarySingleLength1To4000 mSExchBlockedSendersHashField;

		private DirectoryPropertyBooleanSingle mSExchEnableModerationField;

		private DirectoryPropertyStringLength1To2048 mSExchExtensionCustomAttribute1Field;

		private DirectoryPropertyStringLength1To2048 mSExchExtensionCustomAttribute2Field;

		private DirectoryPropertyStringLength1To2048 mSExchExtensionCustomAttribute3Field;

		private DirectoryPropertyStringLength1To2048 mSExchExtensionCustomAttribute4Field;

		private DirectoryPropertyStringLength1To2048 mSExchExtensionCustomAttribute5Field;

		private DirectoryPropertyBooleanSingle mSExchHideFromAddressListsField;

		private DirectoryPropertyDateTimeSingle mSExchLitigationHoldDateField;

		private DirectoryPropertyStringSingleLength1To1024 mSExchLitigationHoldOwnerField;

		private DirectoryPropertyInt32Single mSExchModerationFlagsField;

		private DirectoryPropertyInt32Single mSExchRecipientDisplayTypeField;

		private DirectoryPropertyInt64Single mSExchRecipientTypeDetailsField;

		private DirectoryPropertyBooleanSingle mSExchRequireAuthToSendToField;

		private DirectoryPropertyStringSingleLength1To1024 mSExchRetentionCommentField;

		private DirectoryPropertyStringSingleLength1To2048 mSExchRetentionUrlField;

		private DirectoryPropertyBinarySingleLength1To12000 mSExchSafeRecipientsHashField;

		private DirectoryPropertyBinarySingleLength1To32000 mSExchSafeSendersHashField;

		private DirectoryPropertyStringLength2To500 mSExchSenderHintTranslationsField;

		private DirectoryPropertyStringSingleLength1To256 mSRtcSipDeploymentLocatorField;

		private DirectoryPropertyStringSingleLength1To500 mSRtcSipLineField;

		private DirectoryPropertyInt32Single mSRtcSipOptionFlagsField;

		private DirectoryPropertyStringSingleLength1To454 mSRtcSipPrimaryUserAddressField;

		private DirectoryPropertyBooleanSingle mSRtcSipUserEnabledField;

		private DirectoryPropertyStringLength1To64 otherFacsimileTelephoneNumberField;

		private DirectoryPropertyStringLength1To64 otherHomePhoneField;

		private DirectoryPropertyStringLength1To512 otherIPPhoneField;

		private DirectoryPropertyStringLength1To64 otherMobileField;

		private DirectoryPropertyStringLength1To64 otherPagerField;

		private DirectoryPropertyStringLength1To64 otherTelephoneField;

		private DirectoryPropertyStringSingleLength1To64 pagerField;

		private DirectoryPropertyStringSingleLength1To128 physicalDeliveryOfficeNameField;

		private DirectoryPropertyStringSingleLength1To40 postalCodeField;

		private DirectoryPropertyStringLength1To40 postOfficeBoxField;

		private DirectoryPropertyProxyAddresses proxyAddressesField;

		private DirectoryPropertyXmlServiceInfo serviceInfoField;

		private DirectoryPropertyStringSingleLength1To64 shadowCommonNameField;

		private DirectoryPropertyStringLength1To1123 shadowProxyAddressesField;

		private DirectoryPropertyStringSingleLength1To454 sipProxyAddressField;

		private DirectoryPropertyStringSingleLength1To64 snField;

		private DirectoryPropertyStringSingleLength1To256 sourceAnchorField;

		private DirectoryPropertyStringSingleLength1To128 stField;

		private DirectoryPropertyStringSingleLength1To1024 streetField;

		private DirectoryPropertyStringSingleLength1To1024 streetAddressField;

		private DirectoryPropertyTargetAddress targetAddressField;

		private DirectoryPropertyStringSingleLength1To64 telephoneAssistantField;

		private DirectoryPropertyStringSingleLength1To64 telephoneNumberField;

		private DirectoryPropertyBinarySingleLength1To102400 thumbnailPhotoField;

		private DirectoryPropertyStringSingleLength1To128 titleField;

		private DirectoryPropertyStringLength1To1123 urlField;

		private DirectoryPropertyBinaryLength1To32768 userCertificateField;

		private DirectoryPropertyBinaryLength1To32768 userSMIMECertificateField;

		private DirectoryPropertyXmlValidationError validationErrorField;

		private DirectoryPropertyStringSingleLength1To2048 wwwHomepageField;

		private AttributeSet[] singleAuthorityMetadataField;

		private XmlAttribute[] anyAttrField;
	}
}
