using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	[XmlType(Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11")]
	[DesignerCategory("code")]
	[GeneratedCode("svcutil", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[Serializable]
	public class Company : DirectoryObject
	{
		internal override void ForEachProperty(IPropertyProcessor processor)
		{
			processor.Process<DirectoryPropertyXmlAssignedPlan>(SyncCompanySchema.AssignedPlan, ref this.assignedPlanField);
			processor.Process<DirectoryPropertyStringSingleLength1To3>(SyncCompanySchema.C, ref this.cField);
			processor.Process<DirectoryPropertyXmlCompanyPartnershipSingle>(SyncCompanySchema.CompanyPartnership, ref this.companyPartnershipField);
			processor.Process<DirectoryPropertyStringLength1To256>(SyncCompanySchema.CompanyTags, ref this.companyTagsField);
			processor.Process<DirectoryPropertyStringSingleLength1To1024>(SyncCompanySchema.Description, ref this.descriptionField);
			processor.Process<DirectoryPropertyStringSingleLength1To256>(SyncCompanySchema.DisplayName, ref this.displayNameField);
			processor.Process<DirectoryPropertyBooleanSingle>(SyncCompanySchema.IsDirSyncRunning, ref this.dirSyncEnabledField);
			processor.Process<DirectoryPropertyXmlDirSyncStatus>(SyncCompanySchema.DirSyncStatus, ref this.dirSyncStatusField);
			processor.Process<DirectoryPropertyXmlDirSyncStatus>(SyncCompanySchema.DirSyncStatusAck, ref this.dirSyncStatusAckField);
			processor.Process<DirectoryPropertyXmlServiceInfo>(SyncCompanySchema.ServiceInfo, ref this.serviceInfoField);
			processor.Process<DirectoryPropertyInt32SingleMin0Max4>(SyncCompanySchema.TenantType, ref this.tenantTypeField);
			processor.Process<DirectoryPropertyInt32SingleMin0>(SyncCompanySchema.QuotaAmount, ref this.quotaAmountField);
			processor.Process<DirectoryPropertyXmlCompanyVerifiedDomain>(SyncCompanySchema.VerifiedDomain, ref this.verifiedDomainField);
			processor.Process<DirectoryPropertyXmlRightsManagementTenantKey>(SyncCompanySchema.RightsManagementTenantKey, ref this.rightsManagementTenantKeyField);
			processor.Process<DirectoryPropertyXmlRightsManagementTenantConfigurationSingle>(SyncCompanySchema.RightsManagementTenantConfiguration, ref this.rightsManagementTenantConfigurationField);
		}

		[XmlElement(Order = 0)]
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

		[XmlElement(Order = 1)]
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

		[XmlElement(Order = 2)]
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

		[XmlElement(Order = 3)]
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

		[XmlElement(Order = 4)]
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

		[XmlElement(Order = 5)]
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

		[XmlElement(Order = 6)]
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

		[XmlElement(Order = 7)]
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

		[XmlElement(Order = 8)]
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

		[XmlElement(Order = 9)]
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

		[XmlElement(Order = 10)]
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

		[XmlElement(Order = 11)]
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

		[XmlElement(Order = 12)]
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

		[XmlElement(Order = 13)]
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

		[XmlElement(Order = 14)]
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

		[XmlElement(Order = 15)]
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

		[XmlElement(Order = 16)]
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

		[XmlElement(Order = 17)]
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

		[XmlElement(Order = 18)]
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

		[XmlElement(Order = 19)]
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

		[XmlElement(Order = 20)]
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

		[XmlElement(Order = 21)]
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

		[XmlElement(Order = 22)]
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

		[XmlElement(Order = 23)]
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

		[XmlElement(Order = 24)]
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

		[XmlElement(Order = 25)]
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

		[XmlElement(Order = 26)]
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

		[XmlElement(Order = 27)]
		public DirectoryPropertyXmlTakeoverAction TakeoverAction
		{
			get
			{
				return this.takeoverActionField;
			}
			set
			{
				this.takeoverActionField = value;
			}
		}

		[XmlElement(Order = 28)]
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

		[XmlElement(Order = 29)]
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

		[XmlElement(Order = 30)]
		public DirectoryPropertyInt32SingleMin0Max4 TenantType
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

		[XmlElement(Order = 31)]
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

		[XmlElement(Order = 32)]
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

		private DirectoryPropertyStringSingleLength1To3 cField;

		private DirectoryPropertyStringSingleLength1To128 coField;

		private DirectoryPropertyXmlCompanyPartnershipSingle companyPartnershipField;

		private DirectoryPropertyStringLength1To256 companyTagsField;

		private DirectoryPropertyInt32Single complianceRequirementsField;

		private DirectoryPropertyStringSingleLength1To1024 descriptionField;

		private DirectoryPropertyBooleanSingle dirSyncEnabledField;

		private DirectoryPropertyXmlDirSyncStatus dirSyncStatusAckField;

		private DirectoryPropertyStringSingleLength1To256 displayNameField;

		private DirectoryPropertyStringSingleLength1To128 lField;

		private DirectoryPropertyStringSingleLength1To1123 partnerCommerceUrlField;

		private DirectoryPropertyStringSingleLength1To128 partnerCompanyNameField;

		private DirectoryPropertyStringSingleLength1To1123 partnerHelpUrlField;

		private DirectoryPropertyStringLength1To256 partnerServiceTypeField;

		private DirectoryPropertyStringLength1To1123 partnerSupportEmailField;

		private DirectoryPropertyStringLength1To64 partnerSupportTelephoneField;

		private DirectoryPropertyStringSingleLength1To1123 partnerSupportUrlField;

		private DirectoryPropertyStringSingleLength1To40 postalCodeField;

		private DirectoryPropertyStringSingleLength1To64 preferredLanguageField;

		private DirectoryPropertyInt32SingleMin0 quotaAmountField;

		private DirectoryPropertyXmlRightsManagementTenantConfigurationSingle rightsManagementTenantConfigurationField;

		private DirectoryPropertyXmlRightsManagementTenantKey rightsManagementTenantKeyField;

		private DirectoryPropertyXmlServiceInfo serviceInfoField;

		private DirectoryPropertyStringSingleLength1To128 stField;

		private DirectoryPropertyStringSingleLength1To1024 streetField;

		private DirectoryPropertyXmlTakeoverAction takeoverActionField;

		private DirectoryPropertyStringLength1To256 technicalNotificationMailField;

		private DirectoryPropertyStringSingleLength1To64 telephoneNumberField;

		private DirectoryPropertyInt32SingleMin0Max4 tenantTypeField;

		private DirectoryPropertyXmlCompanyVerifiedDomain verifiedDomainField;

		private DirectoryPropertyXmlDirSyncStatus dirSyncStatusField;

		private XmlAttribute[] anyAttrField;
	}
}
