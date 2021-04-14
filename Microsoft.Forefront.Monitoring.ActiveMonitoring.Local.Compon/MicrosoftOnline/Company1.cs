using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.MicrosoftOnline
{
	[DesignerCategory("code")]
	[GeneratedCode("wsdl", "2.0.50727.1432")]
	[DebuggerStepThrough]
	[XmlType(TypeName = "Company", Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11")]
	[Serializable]
	public class Company1 : DirectoryObject
	{
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

		public DirectoryPropertyXmlAuthorizedParty AuthorizedParty
		{
			get
			{
				return this.authorizedPartyField;
			}
			set
			{
				this.authorizedPartyField = value;
			}
		}

		public DirectoryPropertyStringLength1To256 AuthorizedServiceInstance
		{
			get
			{
				return this.authorizedServiceInstanceField;
			}
			set
			{
				this.authorizedServiceInstanceField = value;
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

		public DirectoryPropertyDateTimeSingle CompanyLastDirSyncTime
		{
			get
			{
				return this.companyLastDirSyncTimeField;
			}
			set
			{
				this.companyLastDirSyncTimeField = value;
			}
		}

		public DirectoryPropertyXmlCompanyPartnershipSingle CompanyPartnership
		{
			get
			{
				return this.companyPartnershipField;
			}
			set
			{
				this.companyPartnershipField = value;
			}
		}

		public DirectoryPropertyStringLength1To256 CompanyTags
		{
			get
			{
				return this.companyTagsField;
			}
			set
			{
				this.companyTagsField = value;
			}
		}

		public DirectoryPropertyInt32Single ComplianceRequirements
		{
			get
			{
				return this.complianceRequirementsField;
			}
			set
			{
				this.complianceRequirementsField = value;
			}
		}

		public DirectoryPropertyXmlContextMoveStatusSingle ContextMoveFrom
		{
			get
			{
				return this.contextMoveFromField;
			}
			set
			{
				this.contextMoveFromField = value;
			}
		}

		public DirectoryPropertyBinarySingleLength1To102400 ContextMoveSyncCookie
		{
			get
			{
				return this.contextMoveSyncCookieField;
			}
			set
			{
				this.contextMoveSyncCookieField = value;
			}
		}

		public DirectoryPropertyXmlContextMoveStatusSingle ContextMoveTo
		{
			get
			{
				return this.contextMoveToField;
			}
			set
			{
				this.contextMoveToField = value;
			}
		}

		public DirectoryPropertyXmlContextMoveWatermarksSingle ContextMoveWatermarks
		{
			get
			{
				return this.contextMoveWatermarksField;
			}
			set
			{
				this.contextMoveWatermarksField = value;
			}
		}

		public DirectoryPropertyDateTimeSingle CreationTime
		{
			get
			{
				return this.creationTimeField;
			}
			set
			{
				this.creationTimeField = value;
			}
		}

		public DirectoryPropertyXmlSearchForUsers CustomView
		{
			get
			{
				return this.customViewField;
			}
			set
			{
				this.customViewField = value;
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

		public DirectoryPropertyXmlDirSyncStatus DirSyncStatusAck
		{
			get
			{
				return this.dirSyncStatusAckField;
			}
			set
			{
				this.dirSyncStatusAckField = value;
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

		public DirectoryPropertyGuid FeatureDescriptorIds
		{
			get
			{
				return this.featureDescriptorIdsField;
			}
			set
			{
				this.featureDescriptorIdsField = value;
			}
		}

		public DirectoryPropertyInt32SingleMin0 FirstLoginObjectCount
		{
			get
			{
				return this.firstLoginObjectCountField;
			}
			set
			{
				this.firstLoginObjectCountField = value;
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

		public DirectoryPropertyInt32SingleMin0 LiveAuthorizationScope
		{
			get
			{
				return this.liveAuthorizationScopeField;
			}
			set
			{
				this.liveAuthorizationScopeField = value;
			}
		}

		public DirectoryPropertyStringLength1To256 MarketingNotificationEmails
		{
			get
			{
				return this.marketingNotificationEmailsField;
			}
			set
			{
				this.marketingNotificationEmailsField = value;
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

		public DirectoryPropertyGuidSingle OcpMessageId
		{
			get
			{
				return this.ocpMessageIdField;
			}
			set
			{
				this.ocpMessageIdField = value;
			}
		}

		public DirectoryPropertyGuid OcpOrganizationId
		{
			get
			{
				return this.ocpOrganizationIdField;
			}
			set
			{
				this.ocpOrganizationIdField = value;
			}
		}

		public DirectoryPropertyXmlPropagationTask OrgIdPropagationTask
		{
			get
			{
				return this.orgIdPropagationTaskField;
			}
			set
			{
				this.orgIdPropagationTaskField = value;
			}
		}

		public DirectoryPropertyBinarySingleLength1To28 OwnerIdentifier
		{
			get
			{
				return this.ownerIdentifierField;
			}
			set
			{
				this.ownerIdentifierField = value;
			}
		}

		public DirectoryPropertyStringSingleLength1To256 PartnerBillingSupportEmail
		{
			get
			{
				return this.partnerBillingSupportEmailField;
			}
			set
			{
				this.partnerBillingSupportEmailField = value;
			}
		}

		public DirectoryPropertyStringSingleLength1To1123 PartnerCommerceUrl
		{
			get
			{
				return this.partnerCommerceUrlField;
			}
			set
			{
				this.partnerCommerceUrlField = value;
			}
		}

		public DirectoryPropertyStringSingleLength1To128 PartnerCompanyName
		{
			get
			{
				return this.partnerCompanyNameField;
			}
			set
			{
				this.partnerCompanyNameField = value;
			}
		}

		public DirectoryPropertyStringSingleLength1To1123 PartnerHelpUrl
		{
			get
			{
				return this.partnerHelpUrlField;
			}
			set
			{
				this.partnerHelpUrlField = value;
			}
		}

		public DirectoryPropertyStringLength1To256 PartnerServiceType
		{
			get
			{
				return this.partnerServiceTypeField;
			}
			set
			{
				this.partnerServiceTypeField = value;
			}
		}

		public DirectoryPropertyStringLength1To1123 PartnerSupportEmail
		{
			get
			{
				return this.partnerSupportEmailField;
			}
			set
			{
				this.partnerSupportEmailField = value;
			}
		}

		public DirectoryPropertyStringLength1To64 PartnerSupportTelephone
		{
			get
			{
				return this.partnerSupportTelephoneField;
			}
			set
			{
				this.partnerSupportTelephoneField = value;
			}
		}

		public DirectoryPropertyStringSingleLength1To1123 PartnerSupportUrl
		{
			get
			{
				return this.partnerSupportUrlField;
			}
			set
			{
				this.partnerSupportUrlField = value;
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

		public DirectoryPropertyXmlPropagationTask PropagationTask
		{
			get
			{
				return this.propagationTaskField;
			}
			set
			{
				this.propagationTaskField = value;
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

		public DirectoryPropertyStringLength1To256 ProvisionedServiceInstance
		{
			get
			{
				return this.provisionedServiceInstanceField;
			}
			set
			{
				this.provisionedServiceInstanceField = value;
			}
		}

		public DirectoryPropertyInt32SingleMin0 QuotaAmount
		{
			get
			{
				return this.quotaAmountField;
			}
			set
			{
				this.quotaAmountField = value;
			}
		}

		public DirectoryPropertyStringSingleLength1To16 ReplicationScope
		{
			get
			{
				return this.replicationScopeField;
			}
			set
			{
				this.replicationScopeField = value;
			}
		}

		public DirectoryPropertyXmlRightsManagementTenantConfigurationSingle RightsManagementTenantConfiguration
		{
			get
			{
				return this.rightsManagementTenantConfigurationField;
			}
			set
			{
				this.rightsManagementTenantConfigurationField = value;
			}
		}

		public DirectoryPropertyXmlRightsManagementTenantKey RightsManagementTenantKey
		{
			get
			{
				return this.rightsManagementTenantKeyField;
			}
			set
			{
				this.rightsManagementTenantKeyField = value;
			}
		}

		public DirectoryPropertyBooleanSingle SelfServePasswordResetEnabled
		{
			get
			{
				return this.selfServePasswordResetEnabledField;
			}
			set
			{
				this.selfServePasswordResetEnabledField = value;
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

		public DirectoryPropertyInt32Single SliceId
		{
			get
			{
				return this.sliceIdField;
			}
			set
			{
				this.sliceIdField = value;
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

		public DirectoryPropertyXmlStrongAuthenticationPolicy StrongAuthenticationPolicy
		{
			get
			{
				return this.strongAuthenticationPolicyField;
			}
			set
			{
				this.strongAuthenticationPolicyField = value;
			}
		}

		public DirectoryPropertyStringLength1To256 TechnicalNotificationMail
		{
			get
			{
				return this.technicalNotificationMailField;
			}
			set
			{
				this.technicalNotificationMailField = value;
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

		public DirectoryPropertyInt32SingleMin0Max3 TenantType
		{
			get
			{
				return this.tenantTypeField;
			}
			set
			{
				this.tenantTypeField = value;
			}
		}

		public DirectoryPropertyGuidSingle ThrottlePolicyId
		{
			get
			{
				return this.throttlePolicyIdField;
			}
			set
			{
				this.throttlePolicyIdField = value;
			}
		}

		public DirectoryPropertyXmlCompanyUnverifiedDomain UnverifiedDomain
		{
			get
			{
				return this.unverifiedDomainField;
			}
			set
			{
				this.unverifiedDomainField = value;
			}
		}

		public DirectoryPropertyXmlCompanyVerifiedDomain VerifiedDomain
		{
			get
			{
				return this.verifiedDomainField;
			}
			set
			{
				this.verifiedDomainField = value;
			}
		}

		public DirectoryPropertyXmlDirSyncStatus DirSyncStatus
		{
			get
			{
				return this.dirSyncStatusField;
			}
			set
			{
				this.dirSyncStatusField = value;
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

		private DirectoryPropertyXmlAssignedPlan assignedPlanField;

		private DirectoryPropertyXmlAuthorizedParty authorizedPartyField;

		private DirectoryPropertyStringLength1To256 authorizedServiceInstanceField;

		private DirectoryPropertyStringSingleLength1To3 cField;

		private DirectoryPropertyStringSingleLength1To128 coField;

		private DirectoryPropertyDateTimeSingle companyLastDirSyncTimeField;

		private DirectoryPropertyXmlCompanyPartnershipSingle companyPartnershipField;

		private DirectoryPropertyStringLength1To256 companyTagsField;

		private DirectoryPropertyInt32Single complianceRequirementsField;

		private DirectoryPropertyXmlContextMoveStatusSingle contextMoveFromField;

		private DirectoryPropertyBinarySingleLength1To102400 contextMoveSyncCookieField;

		private DirectoryPropertyXmlContextMoveStatusSingle contextMoveToField;

		private DirectoryPropertyXmlContextMoveWatermarksSingle contextMoveWatermarksField;

		private DirectoryPropertyDateTimeSingle creationTimeField;

		private DirectoryPropertyXmlSearchForUsers customViewField;

		private DirectoryPropertyStringSingleLength1To128 defaultGeographyField;

		private DirectoryPropertyStringSingleLength1To1024 descriptionField;

		private DirectoryPropertyBooleanSingle dirSyncEnabledField;

		private DirectoryPropertyXmlDirSyncStatus dirSyncStatusAckField;

		private DirectoryPropertyStringSingleLength1To256 displayNameField;

		private DirectoryPropertyGuid featureDescriptorIdsField;

		private DirectoryPropertyInt32SingleMin0 firstLoginObjectCountField;

		private DirectoryPropertyStringSingleLength1To128 lField;

		private DirectoryPropertyInt32SingleMin0 liveAuthorizationScopeField;

		private DirectoryPropertyStringLength1To256 marketingNotificationEmailsField;

		private DirectoryPropertyXmlMigrationDetail migrationDetailField;

		private DirectoryPropertyInt32SingleMin0 migrationStateField;

		private DirectoryPropertyGuidSingle ocpMessageIdField;

		private DirectoryPropertyGuid ocpOrganizationIdField;

		private DirectoryPropertyXmlPropagationTask orgIdPropagationTaskField;

		private DirectoryPropertyBinarySingleLength1To28 ownerIdentifierField;

		private DirectoryPropertyStringSingleLength1To256 partnerBillingSupportEmailField;

		private DirectoryPropertyStringSingleLength1To1123 partnerCommerceUrlField;

		private DirectoryPropertyStringSingleLength1To128 partnerCompanyNameField;

		private DirectoryPropertyStringSingleLength1To1123 partnerHelpUrlField;

		private DirectoryPropertyStringLength1To256 partnerServiceTypeField;

		private DirectoryPropertyStringLength1To1123 partnerSupportEmailField;

		private DirectoryPropertyStringLength1To64 partnerSupportTelephoneField;

		private DirectoryPropertyStringSingleLength1To1123 partnerSupportUrlField;

		private DirectoryPropertyXmlAnySingle portalSettingField;

		private DirectoryPropertyStringSingleLength1To40 postalCodeField;

		private DirectoryPropertyStringSingleLength1To64 preferredLanguageField;

		private DirectoryPropertyXmlPropagationTask propagationTaskField;

		private DirectoryPropertyXmlProvisionedPlan provisionedPlanField;

		private DirectoryPropertyStringLength1To256 provisionedServiceInstanceField;

		private DirectoryPropertyInt32SingleMin0 quotaAmountField;

		private DirectoryPropertyStringSingleLength1To16 replicationScopeField;

		private DirectoryPropertyXmlRightsManagementTenantConfigurationSingle rightsManagementTenantConfigurationField;

		private DirectoryPropertyXmlRightsManagementTenantKey rightsManagementTenantKeyField;

		private DirectoryPropertyBooleanSingle selfServePasswordResetEnabledField;

		private DirectoryPropertyXmlServiceInfo serviceInfoField;

		private DirectoryPropertyInt32Single sliceIdField;

		private DirectoryPropertyStringSingleLength1To128 stField;

		private DirectoryPropertyStringSingleLength1To1024 streetField;

		private DirectoryPropertyXmlStrongAuthenticationPolicy strongAuthenticationPolicyField;

		private DirectoryPropertyStringLength1To256 technicalNotificationMailField;

		private DirectoryPropertyStringSingleLength1To64 telephoneNumberField;

		private DirectoryPropertyInt32SingleMin0Max3 tenantTypeField;

		private DirectoryPropertyGuidSingle throttlePolicyIdField;

		private DirectoryPropertyXmlCompanyUnverifiedDomain unverifiedDomainField;

		private DirectoryPropertyXmlCompanyVerifiedDomain verifiedDomainField;

		private DirectoryPropertyXmlDirSyncStatus dirSyncStatusField;

		private XmlAttribute[] anyAttrField;
	}
}
