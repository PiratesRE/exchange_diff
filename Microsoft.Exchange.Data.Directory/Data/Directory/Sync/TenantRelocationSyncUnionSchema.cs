using System;
using System.Collections.Generic;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Provisioning;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Directory.UnifiedPolicy;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	internal class TenantRelocationSyncUnionSchema : ADPropertyUnionSchema
	{
		public static TenantRelocationSyncUnionSchema Instance
		{
			get
			{
				if (TenantRelocationSyncUnionSchema.instance == null)
				{
					TenantRelocationSyncUnionSchema.instance = ObjectSchema.GetInstance<TenantRelocationSyncUnionSchema>();
				}
				return TenantRelocationSyncUnionSchema.instance;
			}
		}

		public override ReadOnlyCollection<ADObjectSchema> ObjectSchemas
		{
			get
			{
				return TenantRelocationSyncUnionSchema.AllSyncSchemas;
			}
		}

		public TenantRelocationSyncUnionSchema()
		{
			this.ldapDisplayNameToPropertyDefinitinoMappings = new Dictionary<string, ADPropertyDefinition>(StringComparer.InvariantCultureIgnoreCase);
			foreach (PropertyDefinition propertyDefinition in base.AllProperties)
			{
				ADPropertyDefinition adpropertyDefinition = (ADPropertyDefinition)propertyDefinition;
				if (!string.IsNullOrEmpty(adpropertyDefinition.LdapDisplayName))
				{
					string ldapDisplayName = adpropertyDefinition.LdapDisplayName;
					if (!this.ldapDisplayNameToPropertyDefinitinoMappings.ContainsKey(ldapDisplayName))
					{
						this.ldapDisplayNameToPropertyDefinitinoMappings[ldapDisplayName] = adpropertyDefinition;
						ExTraceGlobals.TenantRelocationTracer.TraceDebug<string>((long)this.GetHashCode(), "TenantRelocationSyncUnionSchema constructor: added LdapDisplayName {0} to dictionary", adpropertyDefinition.LdapDisplayName);
					}
					else
					{
						ExTraceGlobals.TenantRelocationTracer.TraceDebug<string>((long)this.GetHashCode(), "TenantRelocationSyncUnionSchema constructor: skip adding {0} to dictionary, because the key exists", adpropertyDefinition.LdapDisplayName);
					}
				}
				if (adpropertyDefinition.SoftLinkShadowProperty != null && !string.IsNullOrEmpty(adpropertyDefinition.SoftLinkShadowProperty.LdapDisplayName))
				{
					this.ldapDisplayNameToPropertyDefinitinoMappings[adpropertyDefinition.SoftLinkShadowProperty.LdapDisplayName] = adpropertyDefinition;
					ExTraceGlobals.TenantRelocationTracer.TraceDebug<string>((long)this.GetHashCode(), "TenantRelocationSyncUnionSchema constructor: added LdapDisplayName {0} to dictionary, soft link", adpropertyDefinition.LdapDisplayName);
				}
				if (adpropertyDefinition.ShadowProperty != null && !string.IsNullOrEmpty(adpropertyDefinition.ShadowProperty.LdapDisplayName))
				{
					this.ldapDisplayNameToPropertyDefinitinoMappings[adpropertyDefinition.ShadowProperty.LdapDisplayName] = adpropertyDefinition;
					ExTraceGlobals.TenantRelocationTracer.TraceDebug<string>((long)this.GetHashCode(), "TenantRelocationSyncUnionSchema constructor: added LdapDisplayName {0} to dictionary, shadow attribute", adpropertyDefinition.LdapDisplayName);
				}
			}
		}

		public bool TryGetPropertyDefinitionByLdapDisplayName(string propertyName, out ADPropertyDefinition propertyDefinition, out bool isSoftLink, out bool isShadow)
		{
			propertyDefinition = null;
			isSoftLink = false;
			isShadow = false;
			if (this.ldapDisplayNameToPropertyDefinitinoMappings.ContainsKey(propertyName))
			{
				propertyDefinition = this.ldapDisplayNameToPropertyDefinitinoMappings[propertyName];
				if (propertyDefinition.SoftLinkShadowProperty != null && propertyName.Equals(propertyDefinition.SoftLinkShadowProperty.LdapDisplayName, StringComparison.OrdinalIgnoreCase))
				{
					isSoftLink = true;
				}
				else if (propertyDefinition.ShadowProperty != null && propertyName.Equals(propertyDefinition.ShadowProperty.LdapDisplayName, StringComparison.OrdinalIgnoreCase))
				{
					isShadow = true;
				}
				return true;
			}
			return false;
		}

		internal static readonly ReadOnlyCollection<ADObjectSchema> AllSyncSchemas = new ReadOnlyCollection<ADObjectSchema>(new ADObjectSchema[]
		{
			ObjectSchema.GetInstance<AcceptedDomainSchema>(),
			ObjectSchema.GetInstance<ActiveSyncDeviceAccessRuleSchema>(),
			ObjectSchema.GetInstance<ActiveSyncDeviceClassesSchema>(),
			ObjectSchema.GetInstance<ActiveSyncDeviceClassSchema>(),
			ObjectSchema.GetInstance<ActiveSyncDevicesSchema>(),
			ObjectSchema.GetInstance<ActiveSyncOrganizationSettingsSchema>(),
			ObjectSchema.GetInstance<ADClientAccessRuleCollectionSchema>(),
			ObjectSchema.GetInstance<ADClientAccessRuleSchema>(),
			ObjectSchema.GetInstance<ADComplianceProgramCollectionSchema>(),
			ObjectSchema.GetInstance<ADComplianceProgramSchema>(),
			ObjectSchema.GetInstance<ADComputerRecipientSchema>(),
			ObjectSchema.GetInstance<ADContactSchema>(),
			ObjectSchema.GetInstance<ADContainerSchema>(),
			ObjectSchema.GetInstance<AddressBookBaseSchema>(),
			ObjectSchema.GetInstance<AddressBookMailboxPolicySchema>(),
			ObjectSchema.GetInstance<ADDynamicGroupSchema>(),
			ObjectSchema.GetInstance<ADGroupSchema>(),
			ObjectSchema.GetInstance<ADMicrosoftExchangeRecipientSchema>(),
			ObjectSchema.GetInstance<AdminAuditLogConfigSchema>(),
			ObjectSchema.GetInstance<ADOrganizationalUnitSchema>(),
			ObjectSchema.GetInstance<ADOrganizationConfigSchema>(),
			ObjectSchema.GetInstance<ADProvisioningPolicySchema>(),
			ObjectSchema.GetInstance<ADPublicDatabaseSchema>(),
			ObjectSchema.GetInstance<ADPublicFolderSchema>(),
			ObjectSchema.GetInstance<ADSystemAttendantMailboxSchema>(),
			ObjectSchema.GetInstance<ADSystemMailboxSchema>(),
			ObjectSchema.GetInstance<ADUserSchema>(),
			ObjectSchema.GetInstance<ApprovalApplicationContainerSchema>(),
			ObjectSchema.GetInstance<ApprovalApplicationSchema>(),
			ObjectSchema.GetInstance<AssociationStorageSchema>(),
			ObjectSchema.GetInstance<AttachmentFilteringConfigSchema>(),
			ObjectSchema.GetInstance<AuthConfigSchema>(),
			ObjectSchema.GetInstance<AuthServerSchema>(),
			ObjectSchema.GetInstance<AvailabilityAddressSpaceSchema>(),
			ObjectSchema.GetInstance<AvailabilityConfigSchema>(),
			ObjectSchema.GetInstance<BindingStorageSchema>(),
			ObjectSchema.GetInstance<ClassificationSchema>(),
			ObjectSchema.GetInstance<ContainerSchema>(),
			ObjectSchema.GetInstance<ContentConfigContainerSchema>(),
			ObjectSchema.GetInstance<ContentFilterConfigSchema>(),
			ObjectSchema.GetInstance<DataClassificationConfigSchema>(),
			ObjectSchema.GetInstance<DeletedObjectSchema>(),
			ObjectSchema.GetInstance<DomainContentConfigSchema>(),
			ObjectSchema.GetInstance<ElcContentSettingsSchema>(),
			ObjectSchema.GetInstance<ELCFolderSchema>(),
			ObjectSchema.GetInstance<EmailAddressPolicyContainerSchema>(),
			ObjectSchema.GetInstance<EmailAddressPolicySchema>(),
			ObjectSchema.GetInstance<ExchangeAssistanceSchema>(),
			ObjectSchema.GetInstance<ExchangeConfigurationContainerSchemaWithAddressLists>(),
			ObjectSchema.GetInstance<ExchangeConfigurationUnitSchema>(),
			ObjectSchema.GetInstance<ExchangeRoleAssignmentSchema>(),
			ObjectSchema.GetInstance<ExchangeRoleSchema>(),
			ObjectSchema.GetInstance<ExtendedOrganizationalUnitSchema>(),
			ObjectSchema.GetInstance<FederatedOrganizationIdSchema>(),
			ObjectSchema.GetInstance<GALSyncOrganizationSchema>(),
			ObjectSchema.GetInstance<HostedConnectionFilterPolicySchema>(),
			ObjectSchema.GetInstance<HostedContentFilterPolicySchema>(),
			ObjectSchema.GetInstance<HostedOutboundSpamFilterPolicySchema>(),
			ObjectSchema.GetInstance<HostedSpamFilterConfigSchema>(),
			ObjectSchema.GetInstance<IntraOrganizationConnectorSchema>(),
			ObjectSchema.GetInstance<IPBlockListConfigSchema>(),
			ObjectSchema.GetInstance<IPBlockListProviderConfigSchema>(),
			ObjectSchema.GetInstance<IRMConfigurationSchema>(),
			ObjectSchema.GetInstance<LegacyThrottlingPolicySchema>(),
			ObjectSchema.GetInstance<MailboxDatabaseSchema>(),
			ObjectSchema.GetInstance<MalwareFilterPolicySchema>(),
			ObjectSchema.GetInstance<ManagedFolderMailboxPolicySchema>(),
			ObjectSchema.GetInstance<ManagementScopeSchema>(),
			ObjectSchema.GetInstance<MessageDeliveryGlobalSettingsSchema>(),
			ObjectSchema.GetInstance<MessageHygieneAgentConfigSchema>(),
			ObjectSchema.GetInstance<MicrosoftExchangeRecipientSchema>(),
			ObjectSchema.GetInstance<MobileDeviceSchema>(),
			ObjectSchema.GetInstance<MobileMailboxPolicySchema>(),
			ObjectSchema.GetInstance<MRSRequestSchema>(),
			ObjectSchema.GetInstance<MsoTenantCookieContainerSchema>(),
			ObjectSchema.GetInstance<OfflineAddressBookSchema>(),
			ObjectSchema.GetInstance<OnPremisesOrganizationSchema>(),
			ObjectSchema.GetInstance<OrganizationRelationshipSchema>(),
			ObjectSchema.GetInstance<OrganizationSchema>(),
			ObjectSchema.GetInstance<OwaMailboxPolicySchema>(),
			ObjectSchema.GetInstance<PartnerApplicationSchema>(),
			ObjectSchema.GetInstance<PerimeterConfigSchema>(),
			ObjectSchema.GetInstance<PolicyStorageSchema>(),
			ObjectSchema.GetInstance<PolicyTipMessageConfigSchema>(),
			ObjectSchema.GetInstance<PublicFolderTreeContainerSchema>(),
			ObjectSchema.GetInstance<PublicFolderTreeSchema>(),
			ObjectSchema.GetInstance<RecipientEnforcementProvisioningPolicySchema>(),
			ObjectSchema.GetInstance<RecipientFilterConfigSchema>(),
			ObjectSchema.GetInstance<RecipientPoliciesContainerSchema>(),
			ObjectSchema.GetInstance<RecipientTemplateProvisioningPolicySchema>(),
			ObjectSchema.GetInstance<ReducedRecipientSchema>(),
			ObjectSchema.GetInstance<RemoteAccountPolicySchema>(),
			ObjectSchema.GetInstance<RemovedMailboxSchema>(),
			ObjectSchema.GetInstance<ResourceBookingConfigSchema>(),
			ObjectSchema.GetInstance<RetentionPolicySchema>(),
			ObjectSchema.GetInstance<RetentionPolicyTagSchema>(),
			ObjectSchema.GetInstance<RMSTrustedPublishingDomainSchema>(),
			ObjectSchema.GetInstance<RoleAssignmentPolicySchema>(),
			ObjectSchema.GetInstance<RuleStorageSchema>(),
			ObjectSchema.GetInstance<SenderFilterConfigSchema>(),
			ObjectSchema.GetInstance<SenderIdConfigSchema>(),
			ObjectSchema.GetInstance<SenderReputationConfigSchema>(),
			ObjectSchema.GetInstance<ScopeStorageSchema>(),
			ObjectSchema.GetInstance<SharingPolicySchema>(),
			ObjectSchema.GetInstance<SmimeConfigurationContainerSchema>(),
			ObjectSchema.GetInstance<SyncedAcceptedDomainSchema>(),
			ObjectSchema.GetInstance<SyncedPerimeterConfigSchema>(),
			ObjectSchema.GetInstance<SyncOrganizationSchema>(),
			ObjectSchema.GetInstance<TeamMailboxProvisioningPolicySchema>(),
			ObjectSchema.GetInstance<TenantInboundConnectorSchema>(),
			ObjectSchema.GetInstance<TenantOutboundConnectorSchema>(),
			ObjectSchema.GetInstance<TenantRelocationRequestSchema>(),
			ObjectSchema.GetInstance<ThrottlingPolicySchema>(),
			ObjectSchema.GetInstance<TransportConfigContainerSchema>(),
			ObjectSchema.GetInstance<TransportRuleCollectionSchema>(),
			ObjectSchema.GetInstance<TransportRuleSchema>(),
			ObjectSchema.GetInstance<UMMailboxPolicySchema>(),
			ObjectSchema.GetInstance<UMAutoAttendantSchema>(),
			ObjectSchema.GetInstance<UMDialPlanSchema>(),
			ObjectSchema.GetInstance<UMHuntGroupSchema>(),
			ObjectSchema.GetInstance<UMIPGatewaySchema>(),
			ObjectSchema.GetInstance<UnifiedPolicySettingStatusSchema>()
		});

		private static TenantRelocationSyncUnionSchema instance;

		private Dictionary<string, ADPropertyDefinition> ldapDisplayNameToPropertyDefinitinoMappings;
	}
}
