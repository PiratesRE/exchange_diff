using System;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	internal class SyncCompanySchema : SyncObjectSchema
	{
		public override DirectoryObjectClass DirectoryObjectClass
		{
			get
			{
				return DirectoryObjectClass.Company;
			}
		}

		public static SyncPropertyDefinition DisplayName = new SyncPropertyDefinition(ADRecipientSchema.DisplayName, "DisplayName", typeof(DirectoryPropertyStringSingleLength1To256), SyncPropertyDefinitionFlags.ForwardSync, SyncPropertyDefinition.InitialSyncPropertySetVersion);

		public static SyncPropertyDefinition VerifiedDomain = new SyncPropertyDefinition("VerifiedDomain", "VerifiedDomain", typeof(CompanyVerifiedDomainValue), typeof(CompanyVerifiedDomainValue), SyncPropertyDefinitionFlags.ForwardSync | SyncPropertyDefinitionFlags.MultiValued, SyncPropertyDefinition.InitialSyncPropertySetVersion, null);

		public static SyncPropertyDefinition AssignedPlan = new SyncPropertyDefinition("AssignedPlan", "AssignedPlan", typeof(AssignedPlanValue), typeof(AssignedPlanValue), SyncPropertyDefinitionFlags.ForwardSync | SyncPropertyDefinitionFlags.MultiValued, SyncPropertyDefinition.InitialSyncPropertySetVersion, null);

		public static SyncPropertyDefinition C = new SyncPropertyDefinition(ADOrgPersonSchema.C, "C", typeof(DirectoryPropertyStringSingleLength1To128), SyncPropertyDefinitionFlags.ForwardSync, SyncPropertyDefinition.InitialSyncPropertySetVersion);

		public static SyncPropertyDefinition IsDirSyncRunning = new SyncPropertyDefinition(OrganizationSchema.IsDirSyncRunning, "DirSyncEnabled", typeof(DirectoryPropertyBooleanSingle), SyncPropertyDefinitionFlags.ForwardSync, SyncPropertyDefinition.InitialSyncPropertySetVersion);

		public static SyncPropertyDefinition DirSyncStatus = new SyncPropertyDefinition(OrganizationSchema.DirSyncStatus, "DirSyncStatus", typeof(DirectoryPropertyXmlDirSyncStatus), SyncPropertyDefinitionFlags.ForwardSync | SyncPropertyDefinitionFlags.MultiValued, SyncPropertyDefinition.InitialSyncPropertySetVersion);

		public static SyncPropertyDefinition DirSyncStatusAck = new SyncPropertyDefinition(ExtendedOrganizationalUnitSchema.DirSyncStatusAck, "DirSyncStatusAck", typeof(DirectoryPropertyXmlDirSyncStatus), SyncPropertyDefinitionFlags.BackSync | SyncPropertyDefinitionFlags.MultiValued, SyncPropertyDefinition.InitialSyncPropertySetVersion);

		public static SyncPropertyDefinition RelocationInProgress = new SyncPropertyDefinition(ADOrganizationalUnitSchema.RelocationInProgress, null, typeof(bool), SyncPropertyDefinitionFlags.BackSync, SyncPropertyDefinition.InitialSyncPropertySetVersion);

		public static SyncPropertyDefinition Description = new SyncPropertyDefinition("Description", "Description", typeof(string), typeof(DirectoryPropertyStringSingleLength1To1024), SyncPropertyDefinitionFlags.ForwardSync | SyncPropertyDefinitionFlags.FilteringOnly, SyncPropertyDefinition.SyncPropertySetVersion4, string.Empty);

		public static SyncPropertyDefinition TenantType = new SyncPropertyDefinition("TenantType", "TenantType", typeof(int?), typeof(DirectoryPropertyInt32Single), SyncPropertyDefinitionFlags.ForwardSync | SyncPropertyDefinitionFlags.FilteringOnly, SyncPropertyDefinition.SyncPropertySetVersion4, null);

		public static SyncPropertyDefinition RightsManagementTenantConfiguration = new SyncPropertyDefinition("RightsManagementTenantConfiguration", "RightsManagementTenantConfiguration", typeof(RightsManagementTenantConfigurationValue), typeof(XmlValueRightsManagementTenantConfiguration), SyncPropertyDefinitionFlags.ForwardSync, SyncPropertyDefinition.SyncPropertySetVersion11, null);

		public static SyncPropertyDefinition RightsManagementTenantKey = new SyncPropertyDefinition("RightsManagementTenantKey", "RightsManagementTenantKey", typeof(RightsManagementTenantKeyValue), typeof(RightsManagementTenantKeyValue), SyncPropertyDefinitionFlags.ForwardSync | SyncPropertyDefinitionFlags.MultiValued, SyncPropertyDefinition.SyncPropertySetVersion11, null);

		public static SyncPropertyDefinition ServiceInfo = new SyncPropertyDefinition("ServiceInfo", "ServiceInfo", typeof(ServiceInfoValue), typeof(ServiceInfoValue), SyncPropertyDefinitionFlags.ForwardSync | SyncPropertyDefinitionFlags.MultiValued, SyncPropertyDefinition.SyncPropertySetVersion13, null);

		public static SyncPropertyDefinition CompanyPartnership = new SyncPropertyDefinition("CompanyPartnership", "CompanyPartnership", typeof(string), typeof(DirectoryPropertyXmlCompanyPartnershipSingle), SyncPropertyDefinitionFlags.ForwardSync | SyncPropertyDefinitionFlags.FilteringOnly, SyncPropertyDefinition.SyncPropertySetVersion4, string.Empty);

		public static SyncPropertyDefinition QuotaAmount = new SyncPropertyDefinition("QuotaAmount", "QuotaAmount", typeof(int), typeof(DirectoryPropertyInt32SingleMin0), SyncPropertyDefinitionFlags.ForwardSync, SyncPropertyDefinition.SyncPropertySetVersion6, -1);

		public static SyncPropertyDefinition CompanyTags = new SyncPropertyDefinition("CompanyTags", "CompanyTags", typeof(string), typeof(DirectoryPropertyStringLength1To256), SyncPropertyDefinitionFlags.ForwardSync | SyncPropertyDefinitionFlags.MultiValued, SyncPropertyDefinition.SyncPropertySetVersion10, null);

		public static SyncPropertyDefinition PersistedCapabilities = new SyncPropertyDefinition("PersistedCapabilities", null, typeof(Capability), typeof(Capability), SyncPropertyDefinitionFlags.ForwardSync | SyncPropertyDefinitionFlags.MultiValued | SyncPropertyDefinitionFlags.Calculated | SyncPropertyDefinitionFlags.ReadOnly, SyncPropertyDefinition.InitialSyncPropertySetVersion, null, new ProviderPropertyDefinition[]
		{
			SyncCompanySchema.AssignedPlan
		}, new GetterDelegate(SyncCompany.PersistedCapabilityGetter), null);
	}
}
