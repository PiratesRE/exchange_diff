using System;

namespace Microsoft.Exchange.Data.Directory.Provisioning
{
	internal class RecipientEnforcementProvisioningPolicySchema : EnforcementProvisioningPolicySchema
	{
		public static readonly ADPropertyDefinition ObjectCountQuota = new ADPropertyDefinition("ObjectCountQuota", ExchangeObjectVersion.Exchange2010, typeof(string), "msExchObjectCountQuota", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, SimpleProviderPropertyDefinition.None, null, null, null, null, null);

		public static readonly ADPropertyDefinition DistributionListCountQuota = new ADPropertyDefinition("DistributionListCountQuota", ExchangeObjectVersion.Exchange2010, typeof(Unlimited<int>), null, ADPropertyDefinitionFlags.Calculated, Unlimited<int>.UnlimitedValue, new PropertyDefinitionConstraint[]
		{
			new RangedUnlimitedConstraint<int>(0, int.MaxValue)
		}, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			RecipientEnforcementProvisioningPolicySchema.ObjectCountQuota
		}, null, new GetterDelegate(RecipientEnforcementProvisioningPolicy.DistributionListCountQuotaGetter), new SetterDelegate(RecipientEnforcementProvisioningPolicy.DistributionListCountQuotaSetter), null, null);

		public static readonly ADPropertyDefinition DistributionListCount = new ADPropertyDefinition("DistributionListCount", ExchangeObjectVersion.Exchange2010, typeof(int?), null, ADPropertyDefinitionFlags.TaskPopulated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition MailboxCountQuota = new ADPropertyDefinition("MailboxCountQuota", ExchangeObjectVersion.Exchange2010, typeof(Unlimited<int>), null, ADPropertyDefinitionFlags.Calculated, Unlimited<int>.UnlimitedValue, new PropertyDefinitionConstraint[]
		{
			new RangedUnlimitedConstraint<int>(0, int.MaxValue)
		}, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			RecipientEnforcementProvisioningPolicySchema.ObjectCountQuota
		}, null, new GetterDelegate(RecipientEnforcementProvisioningPolicy.MailboxCountQuotaGetter), new SetterDelegate(RecipientEnforcementProvisioningPolicy.MailboxCountQuotaSetter), null, null);

		public static readonly ADPropertyDefinition MailboxCount = new ADPropertyDefinition("MailboxCount", ExchangeObjectVersion.Exchange2010, typeof(int?), null, ADPropertyDefinitionFlags.TaskPopulated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition MailUserCountQuota = new ADPropertyDefinition("MailUserCountQuota", ExchangeObjectVersion.Exchange2010, typeof(Unlimited<int>), null, ADPropertyDefinitionFlags.Calculated, Unlimited<int>.UnlimitedValue, new PropertyDefinitionConstraint[]
		{
			new RangedUnlimitedConstraint<int>(0, int.MaxValue)
		}, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			RecipientEnforcementProvisioningPolicySchema.ObjectCountQuota
		}, null, new GetterDelegate(RecipientEnforcementProvisioningPolicy.MailUserCountQuotaGetter), new SetterDelegate(RecipientEnforcementProvisioningPolicy.MailUserCountQuotaSetter), null, null);

		public static readonly ADPropertyDefinition MailUserCount = new ADPropertyDefinition("MailUserCount", ExchangeObjectVersion.Exchange2010, typeof(int?), null, ADPropertyDefinitionFlags.TaskPopulated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ContactCountQuota = new ADPropertyDefinition("ContactCountQuota", ExchangeObjectVersion.Exchange2010, typeof(Unlimited<int>), null, ADPropertyDefinitionFlags.Calculated, Unlimited<int>.UnlimitedValue, new PropertyDefinitionConstraint[]
		{
			new RangedUnlimitedConstraint<int>(0, int.MaxValue)
		}, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			RecipientEnforcementProvisioningPolicySchema.ObjectCountQuota
		}, null, new GetterDelegate(RecipientEnforcementProvisioningPolicy.ContactCountQuotaGetter), new SetterDelegate(RecipientEnforcementProvisioningPolicy.ContactCountQuotaSetter), null, null);

		public static readonly ADPropertyDefinition ContactCount = new ADPropertyDefinition("ContactCount", ExchangeObjectVersion.Exchange2010, typeof(int?), null, ADPropertyDefinitionFlags.TaskPopulated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition TeamMailboxCountQuota = new ADPropertyDefinition("TeamMailboxCountQuota", ExchangeObjectVersion.Exchange2010, typeof(Unlimited<int>), null, ADPropertyDefinitionFlags.Calculated, Unlimited<int>.UnlimitedValue, new PropertyDefinitionConstraint[]
		{
			new RangedUnlimitedConstraint<int>(0, int.MaxValue)
		}, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			RecipientEnforcementProvisioningPolicySchema.ObjectCountQuota
		}, null, new GetterDelegate(RecipientEnforcementProvisioningPolicy.TeamMailboxCountQuotaGetter), new SetterDelegate(RecipientEnforcementProvisioningPolicy.TeamMailboxCountQuotaSetter), null, null);

		public static readonly ADPropertyDefinition TeamMailboxCount = new ADPropertyDefinition("TeamMailboxCount", ExchangeObjectVersion.Exchange2010, typeof(int?), null, ADPropertyDefinitionFlags.TaskPopulated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition PublicFolderMailboxCountQuota = new ADPropertyDefinition("PublicFolderMailboxCountQuota", ExchangeObjectVersion.Exchange2010, typeof(Unlimited<int>), null, ADPropertyDefinitionFlags.Calculated, Unlimited<int>.UnlimitedValue, new PropertyDefinitionConstraint[]
		{
			new RangedUnlimitedConstraint<int>(0, int.MaxValue)
		}, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			RecipientEnforcementProvisioningPolicySchema.ObjectCountQuota
		}, null, new GetterDelegate(RecipientEnforcementProvisioningPolicy.PublicFolderMailboxCountQuotaGetter), new SetterDelegate(RecipientEnforcementProvisioningPolicy.PublicFolderMailboxCountQuotaSetter), null, null);

		public static readonly ADPropertyDefinition PublicFolderMailboxCount = new ADPropertyDefinition("PublicFolderMailboxCount", ExchangeObjectVersion.Exchange2010, typeof(int?), null, ADPropertyDefinitionFlags.TaskPopulated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition MailPublicFolderCountQuota = new ADPropertyDefinition("MailPublicFolderCountQuota", ExchangeObjectVersion.Exchange2010, typeof(Unlimited<int>), null, ADPropertyDefinitionFlags.Calculated, Unlimited<int>.UnlimitedValue, new PropertyDefinitionConstraint[]
		{
			new RangedUnlimitedConstraint<int>(0, int.MaxValue)
		}, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			RecipientEnforcementProvisioningPolicySchema.ObjectCountQuota
		}, null, new GetterDelegate(RecipientEnforcementProvisioningPolicy.MailPublicFolderCountQuotaGetter), new SetterDelegate(RecipientEnforcementProvisioningPolicy.MailPublicFolderCountQuotaSetter), null, null);

		public static readonly ADPropertyDefinition MailPublicFolderCount = new ADPropertyDefinition("MailPublicFolderCount", ExchangeObjectVersion.Exchange2010, typeof(int?), null, ADPropertyDefinitionFlags.TaskPopulated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);
	}
}
