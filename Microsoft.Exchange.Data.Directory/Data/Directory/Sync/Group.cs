using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11")]
	[GeneratedCode("svcutil", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[Serializable]
	public class Group : DirectoryObject, IValidationErrorSupport
	{
		internal override void ForEachProperty(IPropertyProcessor processor)
		{
			processor.Process<DirectoryPropertyStringSingleLength1To1024>(SyncGroupSchema.Description, ref this.descriptionField);
			processor.Process<DirectoryPropertyStringSingleLength1To2048>(SyncRecipientSchema.CloudLegacyExchangeDN, ref this.cloudLegacyExchangeDNField);
			processor.Process<DirectoryPropertyInt32Single>(SyncRecipientSchema.CloudMsExchRecipientDisplayType, ref this.cloudMSExchRecipientDisplayTypeField);
			processor.Process<DirectoryPropertyStringSingleLength1To256>(SyncRecipientSchema.DisplayName, ref this.displayNameField);
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
			processor.Process<DirectoryPropertyBooleanSingle>(SyncGroupSchema.MailEnabled, ref this.mailEnabledField);
			processor.Process<DirectoryPropertyStringSingleMailNickname>(SyncRecipientSchema.Alias, ref this.mailNicknameField);
			processor.Process<DirectoryPropertyInt32Single>(SyncRecipientSchema.SeniorityIndex, ref this.mSDSHABSeniorityIndexField);
			processor.Process<DirectoryPropertyStringSingleLength1To256>(SyncRecipientSchema.PhoneticDisplayName, ref this.mSDSPhoneticDisplayNameField);
			processor.Process<DirectoryPropertyBooleanSingle>(SyncRecipientSchema.ModerationEnabled, ref this.mSExchEnableModerationField);
			processor.Process<DirectoryPropertyBooleanSingle>(SyncRecipientSchema.HiddenFromAddressListsEnabled, ref this.mSExchHideFromAddressListsField);
			processor.Process<DirectoryPropertyDateTimeSingle>(SyncRecipientSchema.LitigationHoldDate, ref this.mSExchLitigationHoldDateField);
			processor.Process<DirectoryPropertyStringSingleLength1To1024>(SyncRecipientSchema.LitigationHoldOwner, ref this.mSExchLitigationHoldOwnerField);
			processor.Process<DirectoryPropertyInt32Single>(SyncRecipientSchema.ModerationFlags, ref this.mSExchModerationFlagsField);
			processor.Process<DirectoryPropertyInt64Single>(SyncGroupSchema.RecipientTypeDetailsValue, ref this.mSExchRecipientTypeDetailsField);
			processor.Process<DirectoryPropertyStringSingleLength1To1024>(SyncRecipientSchema.RetentionComment, ref this.mSExchRetentionCommentField);
			processor.Process<DirectoryPropertyStringSingleLength1To2048>(SyncRecipientSchema.RetentionUrl, ref this.mSExchRetentionUrlField);
			processor.Process<DirectoryPropertyBooleanSingle>(SyncRecipientSchema.RequireAllSendersAreAuthenticated, ref this.mSExchRequireAuthToSendToField);
			processor.Process<DirectoryPropertyStringLength2To500>(SyncRecipientSchema.MailTipTranslations, ref this.mSExchSenderHintTranslationsField);
			processor.Process<DirectoryPropertyBooleanSingle>(SyncGroupSchema.IsHierarchicalGroup, ref this.mSOrgIsOrganizationalField);
			processor.Process<DirectoryPropertyBooleanSingle>(SyncGroupSchema.SendOofMessageToOriginatorEnabled, ref this.oofReplyToOriginatorField);
			processor.Process<DirectoryPropertyProxyAddresses>(SyncRecipientSchema.EmailAddresses, ref this.proxyAddressesField);
			processor.Process<DirectoryPropertyProxyAddresses>(SyncRecipientSchema.SmtpAndX500Addresses, ref this.proxyAddressesField);
			processor.Process<DirectoryPropertyBooleanSingle>(SyncGroupSchema.ReportToOriginatorEnabled, ref this.reportToOriginatorField);
			processor.Process<DirectoryPropertyBooleanSingle>(SyncGroupSchema.ReportToManagerEnabled, ref this.reportToOwnerField);
			processor.Process<DirectoryPropertyBooleanSingle>(SyncGroupSchema.SecurityEnabled, ref this.securityEnabledField);
			processor.Process<DirectoryPropertyStringSingleLength1To64>(SyncRecipientSchema.Cn, ref this.shadowCommonNameField);
			processor.Process<DirectoryPropertyStringSingleLength1To256>(SyncRecipientSchema.OnPremisesObjectId, ref this.sourceAnchorField);
			processor.Process<DirectoryPropertyXmlValidationError>(SyncRecipientSchema.ValidationError, ref this.validationErrorField);
			processor.Process<DirectoryPropertyStringSingleLength1To40>(SyncGroupSchema.WellKnownObject, ref this.wellKnownObjectField);
			processor.Process<DirectoryPropertyBooleanSingle>(SyncRecipientSchema.IsDirSynced, ref this.dirSyncEnabledField);
			DirectoryPropertyAttributeSet directoryPropertyAttributeSet = (DirectoryPropertyAttributeSet)DirectoryObject.GetDirectoryProperty(this.singleAuthorityMetadataField);
			processor.Process<DirectoryPropertyAttributeSet>(SyncRecipientSchema.DirSyncAuthorityMetadata, ref directoryPropertyAttributeSet);
			processor.Process<DirectoryPropertyStringLength1To1024>(SyncGroupSchema.SharePointResources, ref this.sharepointResourcesField);
			processor.Process<DirectoryPropertyReferenceUserAndServicePrincipalSingle>(SyncGroupSchema.Creator, ref this.createdOnBehalfOfField);
			processor.Process<DirectoryPropertyBooleanSingle>(SyncGroupSchema.IsPublic, ref this.isPublicField);
		}

		[XmlElement(Order = 0)]
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

		[XmlElement(Order = 1)]
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

		[XmlElement(Order = 2)]
		public DirectoryPropertyReferenceUserAndServicePrincipalSingle CreatedOnBehalfOf
		{
			get
			{
				return this.createdOnBehalfOfField;
			}
			set
			{
				this.createdOnBehalfOfField = value;
			}
		}

		[XmlElement(Order = 3)]
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

		[XmlElement(Order = 4)]
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

		[XmlElement(Order = 5)]
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

		[XmlElement(Order = 6)]
		public DirectoryPropertyStringLength1To1024 ExchangeResources
		{
			get
			{
				return this.exchangeResourcesField;
			}
			set
			{
				this.exchangeResourcesField = value;
			}
		}

		[XmlElement(Order = 7)]
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

		[XmlElement(Order = 8)]
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

		[XmlElement(Order = 9)]
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

		[XmlElement(Order = 10)]
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

		[XmlElement(Order = 11)]
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

		[XmlElement(Order = 12)]
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

		[XmlElement(Order = 13)]
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

		[XmlElement(Order = 14)]
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

		[XmlElement(Order = 15)]
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

		[XmlElement(Order = 16)]
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

		[XmlElement(Order = 17)]
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

		[XmlElement(Order = 18)]
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

		[XmlElement(Order = 19)]
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

		[XmlElement(Order = 20)]
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

		[XmlElement(Order = 21)]
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

		[XmlElement(Order = 22)]
		public DirectoryPropertyInt32SingleMin0 GroupType
		{
			get
			{
				return this.groupTypeField;
			}
			set
			{
				this.groupTypeField = value;
			}
		}

		[XmlElement(Order = 23)]
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

		[XmlElement(Order = 24)]
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

		[XmlElement(Order = 25)]
		public DirectoryPropertyBooleanSingle IsPublic
		{
			get
			{
				return this.isPublicField;
			}
			set
			{
				this.isPublicField = value;
			}
		}

		[XmlElement(Order = 26)]
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

		[XmlElement(Order = 27)]
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

		[XmlElement(Order = 28)]
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

		[XmlElement(Order = 29)]
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

		[XmlElement(Order = 30)]
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

		[XmlElement(Order = 31)]
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

		[XmlElement(Order = 32)]
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

		[XmlElement(Order = 33)]
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

		[XmlElement(Order = 34)]
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

		[XmlElement(Order = 35)]
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

		[XmlElement(Order = 36)]
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

		[XmlElement(Order = 37)]
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

		[XmlElement(Order = 38)]
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

		[XmlElement(Order = 39)]
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

		[XmlElement(Order = 40)]
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

		[XmlElement(Order = 41)]
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

		[XmlElement(Order = 42)]
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

		[XmlElement(Order = 43)]
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

		[XmlElement(Order = 44)]
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

		[XmlElement(Order = 45)]
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

		[XmlElement(Order = 46)]
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

		[XmlElement(Order = 47)]
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

		[XmlElement(Order = 48)]
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

		[XmlElement(Order = 49)]
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

		[XmlElement(Order = 50)]
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

		[XmlElement(Order = 51)]
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

		[XmlElement(Order = 52)]
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

		[XmlElement(Order = 53)]
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

		[XmlElement(Order = 54)]
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

		[XmlElement(Order = 55)]
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

		[XmlElement(Order = 56)]
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

		[XmlElement(Order = 57)]
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

		[XmlElement(Order = 58)]
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

		[XmlElement(Order = 59)]
		public DirectoryPropertyStringLength1To1024 SharepointResources
		{
			get
			{
				return this.sharepointResourcesField;
			}
			set
			{
				this.sharepointResourcesField = value;
			}
		}

		[XmlElement(Order = 60)]
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

		[XmlElement(Order = 61)]
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

		[XmlElement(Order = 62)]
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

		[XmlArray(Namespace = "http://schemas.microsoft.com/online/directoryservices/sync/metadata/2010/01", Order = 63)]
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

		private DirectoryPropertyReferenceUserAndServicePrincipalSingle createdOnBehalfOfField;

		private DirectoryPropertyStringSingleLength1To1024 descriptionField;

		private DirectoryPropertyBooleanSingle dirSyncEnabledField;

		private DirectoryPropertyStringSingleLength1To256 displayNameField;

		private DirectoryPropertyStringLength1To1024 exchangeResourcesField;

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

		private DirectoryPropertyInt32SingleMin0 groupTypeField;

		private DirectoryPropertyBooleanSingle hideDLMembershipField;

		private DirectoryPropertyStringSingleLength1To1024 infoField;

		private DirectoryPropertyBooleanSingle isPublicField;

		private DirectoryPropertyStringSingleLength1To256 mailField;

		private DirectoryPropertyBooleanSingle mailEnabledField;

		private DirectoryPropertyStringSingleMailNickname mailNicknameField;

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

		private DirectoryPropertyStringSingleLength1To64 shadowCommonNameField;

		private DirectoryPropertyStringLength1To1123 shadowProxyAddressesField;

		private DirectoryPropertyStringLength1To1024 sharepointResourcesField;

		private DirectoryPropertyStringSingleLength1To256 sourceAnchorField;

		private DirectoryPropertyXmlValidationError validationErrorField;

		private DirectoryPropertyStringSingleLength1To40 wellKnownObjectField;

		private AttributeSet[] singleAuthorityMetadataField;

		private XmlAttribute[] anyAttrField;
	}
}
