using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.MicrosoftOnline
{
	[GeneratedCode("wsdl", "2.0.50727.1432")]
	[DesignerCategory("code")]
	[DebuggerStepThrough]
	[XmlType(Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11")]
	[Serializable]
	public class Group : DirectoryObject
	{
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

		public DirectoryPropertyBooleanSingle HideDLMembership
		{
			get
			{
				return this.hideDLMembershipField;
			}
			set
			{
				this.hideDLMembershipField = value;
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

		public DirectoryPropertyBooleanSingle MailEnabled
		{
			get
			{
				return this.mailEnabledField;
			}
			set
			{
				this.mailEnabledField = value;
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

		public DirectoryPropertyInt32Single MSExchGroupDepartRestriction
		{
			get
			{
				return this.mSExchGroupDepartRestrictionField;
			}
			set
			{
				this.mSExchGroupDepartRestrictionField = value;
			}
		}

		public DirectoryPropertyInt32Single MSExchGroupJoinRestriction
		{
			get
			{
				return this.mSExchGroupJoinRestrictionField;
			}
			set
			{
				this.mSExchGroupJoinRestrictionField = value;
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

		public DirectoryPropertyBooleanSingle MSOrgIsOrganizational
		{
			get
			{
				return this.mSOrgIsOrganizationalField;
			}
			set
			{
				this.mSOrgIsOrganizationalField = value;
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

		public DirectoryPropertyBooleanSingle OofReplyToOriginator
		{
			get
			{
				return this.oofReplyToOriginatorField;
			}
			set
			{
				this.oofReplyToOriginatorField = value;
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

		public DirectoryPropertyBooleanSingle ReportToOriginator
		{
			get
			{
				return this.reportToOriginatorField;
			}
			set
			{
				this.reportToOriginatorField = value;
			}
		}

		public DirectoryPropertyBooleanSingle ReportToOwner
		{
			get
			{
				return this.reportToOwnerField;
			}
			set
			{
				this.reportToOwnerField = value;
			}
		}

		public DirectoryPropertyBooleanSingle SecurityEnabled
		{
			get
			{
				return this.securityEnabledField;
			}
			set
			{
				this.securityEnabledField = value;
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

		public DirectoryPropertyStringSingleLength1To40 WellKnownObject
		{
			get
			{
				return this.wellKnownObjectField;
			}
			set
			{
				this.wellKnownObjectField = value;
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

		private DirectoryPropertyStringSingleLength1To2048 cloudLegacyExchangeDNField;

		private DirectoryPropertyInt32Single cloudMSExchRecipientDisplayTypeField;

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

		private DirectoryPropertyBooleanSingle hideDLMembershipField;

		private DirectoryPropertyStringSingleLength1To1024 infoField;

		private DirectoryPropertyDateTimeSingle lastDirSyncTimeField;

		private DirectoryPropertyStringSingleLength1To256 mailField;

		private DirectoryPropertyBooleanSingle mailEnabledField;

		private DirectoryPropertyStringSingleMailNickname mailNicknameField;

		private DirectoryPropertyXmlMigrationDetail migrationDetailField;

		private DirectoryPropertyStringSingleLength1To256 migrationSourceAnchorField;

		private DirectoryPropertyInt32SingleMin0 migrationStateField;

		private DirectoryPropertyInt32Single mSDSHABSeniorityIndexField;

		private DirectoryPropertyStringSingleLength1To256 mSDSPhoneticDisplayNameField;

		private DirectoryPropertyBooleanSingle mSExchEnableModerationField;

		private DirectoryPropertyStringLength1To2048 mSExchExtensionCustomAttribute1Field;

		private DirectoryPropertyStringLength1To2048 mSExchExtensionCustomAttribute2Field;

		private DirectoryPropertyStringLength1To2048 mSExchExtensionCustomAttribute3Field;

		private DirectoryPropertyStringLength1To2048 mSExchExtensionCustomAttribute4Field;

		private DirectoryPropertyStringLength1To2048 mSExchExtensionCustomAttribute5Field;

		private DirectoryPropertyInt32Single mSExchGroupDepartRestrictionField;

		private DirectoryPropertyInt32Single mSExchGroupJoinRestrictionField;

		private DirectoryPropertyBooleanSingle mSExchHideFromAddressListsField;

		private DirectoryPropertyDateTimeSingle mSExchLitigationHoldDateField;

		private DirectoryPropertyStringSingleLength1To1024 mSExchLitigationHoldOwnerField;

		private DirectoryPropertyInt32Single mSExchModerationFlagsField;

		private DirectoryPropertyInt32Single mSExchRecipientDisplayTypeField;

		private DirectoryPropertyInt64Single mSExchRecipientTypeDetailsField;

		private DirectoryPropertyBooleanSingle mSExchRequireAuthToSendToField;

		private DirectoryPropertyStringSingleLength1To1024 mSExchRetentionCommentField;

		private DirectoryPropertyStringSingleLength1To2048 mSExchRetentionUrlField;

		private DirectoryPropertyStringLength2To500 mSExchSenderHintTranslationsField;

		private DirectoryPropertyBooleanSingle mSOrgIsOrganizationalField;

		private DirectoryPropertyBinarySingleLength1To128 onPremiseSecurityIdentifierField;

		private DirectoryPropertyBooleanSingle oofReplyToOriginatorField;

		private DirectoryPropertyProxyAddresses proxyAddressesField;

		private DirectoryPropertyBooleanSingle reportToOriginatorField;

		private DirectoryPropertyBooleanSingle reportToOwnerField;

		private DirectoryPropertyBooleanSingle securityEnabledField;

		private DirectoryPropertyXmlServiceInfo serviceInfoField;

		private DirectoryPropertyStringSingleLength1To64 shadowAliasField;

		private DirectoryPropertyStringSingleLength1To64 shadowCommonNameField;

		private DirectoryPropertyStringSingleLength1To256 shadowDisplayNameField;

		private DirectoryPropertyStringSingleLength1To2048 shadowLegacyExchangeDNField;

		private DirectoryPropertyStringSingleLength1To256 shadowMailField;

		private DirectoryPropertyStringLength1To1123 shadowProxyAddressesField;

		private DirectoryPropertyStringSingleLength1To256 sourceAnchorField;

		private DirectoryPropertyXmlValidationError validationErrorField;

		private DirectoryPropertyStringSingleLength1To40 wellKnownObjectField;

		private AttributeSet[] singleAuthorityMetadataField;

		private XmlAttribute[] anyAttrField;
	}
}
