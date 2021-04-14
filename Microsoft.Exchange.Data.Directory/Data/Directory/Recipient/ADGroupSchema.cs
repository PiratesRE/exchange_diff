using System;
using System.Security.Principal;

namespace Microsoft.Exchange.Data.Directory.Recipient
{
	internal class ADGroupSchema : ADMailboxRecipientSchema
	{
		public static readonly ADPropertyDefinition GroupType = IADSecurityPrincipalSchema.GroupType;

		public static readonly ADPropertyDefinition IsExecutingUserGroupOwner = new ADPropertyDefinition("IsExecutingUserGroupOwner", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.TaskPopulated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition RoleGroupTypeId = new ADPropertyDefinition("RoleGroupTypeId", ExchangeObjectVersion.Exchange2010, typeof(int), "msExchRoleGroupType", ADPropertyDefinitionFlags.PersistDefaultValue, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition LocalizationFlags = new ADPropertyDefinition("LocalizationFlags", ExchangeObjectVersion.Exchange2010, typeof(int), "msExchLocalizationFlags", ADPropertyDefinitionFlags.PersistDefaultValue, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition GroupMemberCount = new ADPropertyDefinition("GroupMemberCount", ExchangeObjectVersion.Exchange2003, typeof(int), "msExchGroupMemberCount", ADPropertyDefinitionFlags.PersistDefaultValue, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition GroupExternalMemberCount = new ADPropertyDefinition("GroupExternalMemberCount", ExchangeObjectVersion.Exchange2003, typeof(int), "msExchGroupExternalMemberCount", ADPropertyDefinitionFlags.PersistDefaultValue, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition HiddenGroupMembershipEnabled = new ADPropertyDefinition("HiddenGroupMembershipEnabled", ExchangeObjectVersion.Exchange2003, typeof(bool), "hideDLMembership", ADPropertyDefinitionFlags.ReadOnly, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, SimpleProviderPropertyDefinition.None, null, null, null, null, null);

		public static readonly ADPropertyDefinition ExpansionServer = IADDistributionListSchema.ExpansionServer;

		public static readonly ADPropertyDefinition RawManagedBy = IADDistributionListSchema.RawManagedBy;

		public static readonly ADPropertyDefinition CoManagedBy = new ADPropertyDefinition("CoManagedBy", ExchangeObjectVersion.Exchange2003, typeof(ADObjectId), "msExchCoManagedByLink", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition MemberDepartRestriction = new ADPropertyDefinition("MemberDepartRestriction", ExchangeObjectVersion.Exchange2010, typeof(MemberUpdateType), "msExchGroupDepartRestriction", ADPropertyDefinitionFlags.PersistDefaultValue, MemberUpdateType.Closed, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition MemberJoinRestriction = new ADPropertyDefinition("MemberJoinRestriction", ExchangeObjectVersion.Exchange2010, typeof(MemberUpdateType), "msExchGroupJoinRestriction", ADPropertyDefinitionFlags.PersistDefaultValue, MemberUpdateType.Closed, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition Members = IADDistributionListSchema.Members;

		public static readonly ADPropertyDefinition ReportToManagerEnabled = IADDistributionListSchema.ReportToManagerEnabled;

		public static readonly ADPropertyDefinition ReportToOriginatorEnabled = IADDistributionListSchema.ReportToOriginatorEnabled;

		public static readonly ADPropertyDefinition SendDeliveryReportsTo = IADDistributionListSchema.SendDeliveryReportsTo;

		public static readonly ADPropertyDefinition SendOofMessageToOriginatorEnabled = IADDistributionListSchema.SendOofMessageToOriginatorEnabled;

		public static readonly ADPropertyDefinition ForeignGroupSid = new ADPropertyDefinition("ForeignGroupSid", ExchangeObjectVersion.Exchange2010, typeof(SecurityIdentifier), "msExchForeignGroupSid", ADPropertyDefinitionFlags.Binary | ADPropertyDefinitionFlags.DoNotProvisionalClone, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition LinkedGroup = new ADPropertyDefinition("LinkedGroup", ExchangeObjectVersion.Exchange2010, typeof(string), null, ADPropertyDefinitionFlags.TaskPopulated, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition RoleAssignments = new ADPropertyDefinition("RoleAssignments", ExchangeObjectVersion.Exchange2003, typeof(ADObjectId), null, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.TaskPopulated | ADPropertyDefinitionFlags.ValidateInSharedConfig, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition Roles = new ADPropertyDefinition("Roles", ExchangeObjectVersion.Exchange2003, typeof(ADObjectId), null, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.TaskPopulated | ADPropertyDefinitionFlags.BackLink, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition LinkedPartnerGroupAndOrganizationId = new ADPropertyDefinition("LinkedPartnerGroupAndOrganizationId", ExchangeObjectVersion.Exchange2010, typeof(LinkedPartnerGroupInformation), "msExchPartnerGroupID", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(0, 576)
		}, null, null);

		public static readonly ADPropertyDefinition LinkedPartnerGroupId = new ADPropertyDefinition("LinkedPartnerGroupId", ExchangeObjectVersion.Exchange2010, typeof(string), null, ADPropertyDefinitionFlags.Calculated, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADGroupSchema.LinkedPartnerGroupAndOrganizationId
		}, null, delegate(IPropertyBag propertyBag)
		{
			LinkedPartnerGroupInformation linkedPartnerGroupInformation = (LinkedPartnerGroupInformation)propertyBag[ADGroupSchema.LinkedPartnerGroupAndOrganizationId];
			if (linkedPartnerGroupInformation == null)
			{
				return string.Empty;
			}
			return linkedPartnerGroupInformation.LinkedPartnerGroupId;
		}, delegate(object partnerGroupIdValue, IPropertyBag propertyBag)
		{
			LinkedPartnerGroupInformation linkedPartnerGroupInformation = (LinkedPartnerGroupInformation)propertyBag[ADGroupSchema.LinkedPartnerGroupAndOrganizationId];
			linkedPartnerGroupInformation = ((linkedPartnerGroupInformation == null) ? new LinkedPartnerGroupInformation() : linkedPartnerGroupInformation);
			linkedPartnerGroupInformation.LinkedPartnerGroupId = (string)partnerGroupIdValue;
			propertyBag[ADGroupSchema.LinkedPartnerGroupAndOrganizationId] = linkedPartnerGroupInformation;
		}, null, null);

		public static readonly ADPropertyDefinition LinkedPartnerOrganizationId = new ADPropertyDefinition("LinkedPartnerOrganizationId", ExchangeObjectVersion.Exchange2010, typeof(string), null, ADPropertyDefinitionFlags.Calculated, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADGroupSchema.LinkedPartnerGroupAndOrganizationId
		}, null, delegate(IPropertyBag propertyBag)
		{
			LinkedPartnerGroupInformation linkedPartnerGroupInformation = (LinkedPartnerGroupInformation)propertyBag[ADGroupSchema.LinkedPartnerGroupAndOrganizationId];
			if (linkedPartnerGroupInformation == null)
			{
				return string.Empty;
			}
			return linkedPartnerGroupInformation.LinkedPartnerOrganizationId;
		}, delegate(object partnerOrganizationIdValue, IPropertyBag propertyBag)
		{
			LinkedPartnerGroupInformation linkedPartnerGroupInformation = (LinkedPartnerGroupInformation)propertyBag[ADGroupSchema.LinkedPartnerGroupAndOrganizationId];
			linkedPartnerGroupInformation = ((linkedPartnerGroupInformation == null) ? new LinkedPartnerGroupInformation() : linkedPartnerGroupInformation);
			linkedPartnerGroupInformation.LinkedPartnerOrganizationId = (string)partnerOrganizationIdValue;
			propertyBag[ADGroupSchema.LinkedPartnerGroupAndOrganizationId] = linkedPartnerGroupInformation;
		}, null, null);

		public static readonly ADPropertyDefinition IsOrganizationalGroup = new ADPropertyDefinition("IsOrganizationalGroup", ExchangeObjectVersion.Exchange2010, typeof(bool), "msOrg-IsOrganizational", ADPropertyDefinitionFlags.None, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition GroupSubtypeName = new ADPropertyDefinition("GroupSubtypeName", ExchangeObjectVersion.Exchange2010, typeof(string), "msOrg-GroupSubtypeName", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition OtherDisplayNames = new ADPropertyDefinition("OtherDisplayNames", ExchangeObjectVersion.Exchange2010, typeof(string), "msOrg-OtherDisplayNames", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition OrgLeaders = new ADPropertyDefinition("OrgLeaders", ExchangeObjectVersion.Exchange2010, typeof(ADObjectId), "msOrg-Leaders", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition RoleGroupDescription = new ADPropertyDefinition("RoleGroupDescription", ExchangeObjectVersion.Exchange2010, typeof(string), null, ADPropertyDefinitionFlags.Calculated, string.Empty, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(0, 1024)
		}, new ProviderPropertyDefinition[]
		{
			ADRecipientSchema.Description,
			ADGroupSchema.RoleGroupTypeId,
			ADGroupSchema.LocalizationFlags
		}, null, new GetterDelegate(SchemaHelpers.RoleGroupDescriptionGetter), new SetterDelegate(SchemaHelpers.RoleGroupDescriptionSetter), null, null);

		public static readonly ADPropertyDefinition HomeMtaServerId = new ADPropertyDefinition("HomeMtaServerId", ExchangeObjectVersion.Exchange2003, typeof(ADObjectId), null, ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.Calculated | ADPropertyDefinitionFlags.DoNotValidate, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ADPropertyDefinition[]
		{
			ADRecipientSchema.HomeMTA
		}, null, new GetterDelegate(ADGroup.HomeMtaServerIdGetter), null, null, null);

		public static readonly ADPropertyDefinition ManagedBy = new ADPropertyDefinition("ManagedBy", ExchangeObjectVersion.Exchange2003, typeof(ADObjectId), null, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADGroupSchema.RawManagedBy,
			ADGroupSchema.CoManagedBy,
			ADObjectSchema.ExchangeVersion
		}, new CustomFilterBuilderDelegate(ADGroup.ManagedByFilterBuilder), new GetterDelegate(ADGroup.ManagedByGetter), new SetterDelegate(ADGroup.ManagedBySetter), null, null);

		public static readonly ADPropertyDefinition RoleGroupType = new ADPropertyDefinition("RoleGroupType", ExchangeObjectVersion.Exchange2010, typeof(RoleGroupType), null, ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.Calculated, Microsoft.Exchange.Data.Directory.Recipient.RoleGroupType.Standard, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADGroupSchema.ForeignGroupSid
		}, new CustomFilterBuilderDelegate(ADGroup.RoleGroupTypeFilterBuilder), new GetterDelegate(ADGroup.RoleGroupTypeGetter), null, null, null);
	}
}
