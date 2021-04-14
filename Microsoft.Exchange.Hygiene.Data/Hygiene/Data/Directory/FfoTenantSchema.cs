using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Hygiene.Data.Sync;

namespace Microsoft.Exchange.Hygiene.Data.Directory
{
	internal class FfoTenantSchema : ADObjectSchema
	{
		public static readonly HygienePropertyDefinition DescriptionProperty = new HygienePropertyDefinition("Description", typeof(string));

		public static readonly HygienePropertyDefinition CompanyPartnershipProperty = new HygienePropertyDefinition("CompanyPartnership", typeof(string));

		public static readonly HygienePropertyDefinition ResellerTypeProperty = new HygienePropertyDefinition("ResellerType", typeof(ResellerType), ResellerType.None, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition ServiceInstanceProp = CommonSyncProperties.ServiceInstanceProp;

		public static readonly HygienePropertyDefinition BatchIdProp = CommonSyncProperties.BatchIdProp;

		public static readonly HygienePropertyDefinition SyncTypeProp = CommonSyncProperties.SyncTypeProp;

		public static readonly HygienePropertyDefinition VerifiedDomainsProp = new HygienePropertyDefinition("verifiedDomains", typeof(object));

		public static readonly HygienePropertyDefinition AssignedPlansProp = new HygienePropertyDefinition("assignedPlans", typeof(object));

		public static readonly HygienePropertyDefinition CompanyTagsProp = new HygienePropertyDefinition("CompanyTags", typeof(string), null, ADPropertyDefinitionFlags.MultiValued);

		public static readonly HygienePropertyDefinition RmsoUpgradeStatusProp = new HygienePropertyDefinition("RmsoUpgradeStatus", typeof(RmsoUpgradeStatus), RmsoUpgradeStatus.None, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition SharepointTenantAdminUrl = new HygienePropertyDefinition("SharepointTenantAdminUrl", typeof(string));

		public static readonly HygienePropertyDefinition SharepointRootSiteUrl = new HygienePropertyDefinition("SharepointRootSiteUrl", typeof(string));

		public static readonly HygienePropertyDefinition OdmsEndpointUrl = new HygienePropertyDefinition("OdmsEndpointUrl", typeof(string));

		public static readonly HygienePropertyDefinition MigratedToProp = new HygienePropertyDefinition("MigratedTo", typeof(string));

		public static readonly HygienePropertyDefinition UnifiedPolicyPreReqState = new HygienePropertyDefinition("UnifiedPolicyPreReqState", typeof(string), null, ADPropertyDefinitionFlags.MultiValued);

		public static readonly HygienePropertyDefinition OrganizationStatusProp = new HygienePropertyDefinition("OrganizationStatus", typeof(OrganizationStatus), OrganizationStatus.Active, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly ADPropertyDefinition C = ADOrgPersonSchema.C;

		public static readonly ADPropertyDefinition DisplayName = ADRecipientSchema.DisplayName;

		public static readonly ADPropertyDefinition IsDirSyncRunning = OrganizationSchema.IsDirSyncRunning;

		public static readonly ADPropertyDefinition DirSyncStatus = OrganizationSchema.DirSyncStatus;

		public static readonly ADPropertyDefinition DirSyncStatusAck = ExtendedOrganizationalUnitSchema.DirSyncStatusAck;
	}
}
