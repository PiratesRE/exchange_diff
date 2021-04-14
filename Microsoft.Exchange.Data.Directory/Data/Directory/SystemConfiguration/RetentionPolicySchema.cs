using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal sealed class RetentionPolicySchema : MailboxPolicySchema
	{
		internal static QueryFilter IsDefaultFilterBuilder(SinglePropertyFilter filter)
		{
			return ADObject.BoolFilterBuilder(filter, new BitMaskAndFilter(MailboxPolicySchema.MailboxPolicyFlags, 1UL));
		}

		internal static QueryFilter IsDefaultArbitrationMailboxFilterBuilder(SinglePropertyFilter filter)
		{
			return ADObject.BoolFilterBuilder(filter, new BitMaskAndFilter(MailboxPolicySchema.MailboxPolicyFlags, 2UL));
		}

		public static readonly ADPropertyDefinition AssociatedUsers = new ADPropertyDefinition("AssociatedUsers", ExchangeObjectVersion.Exchange2007, typeof(ADObjectId), "msExchMailboxTemplateBL", ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.FilterOnly | ADPropertyDefinitionFlags.BackLink, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition RetentionPolicyTagLinks = new ADPropertyDefinition("RetentionPolicyLinks", ExchangeObjectVersion.Exchange2007, typeof(ADObjectId), "msExchElcFolderLink", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition RetentionId = new ADPropertyDefinition("RetentionId", ExchangeObjectVersion.Exchange2010, typeof(Guid), "msExchUnmergedAttsPt", ADPropertyDefinitionFlags.Binary, System.Guid.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition IsDefault = new ADPropertyDefinition("IsDefault", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			MailboxPolicySchema.MailboxPolicyFlags
		}, new CustomFilterBuilderDelegate(RetentionPolicySchema.IsDefaultFilterBuilder), ADObject.FlagGetterDelegate(MailboxPolicySchema.MailboxPolicyFlags, 1), ADObject.FlagSetterDelegate(MailboxPolicySchema.MailboxPolicyFlags, 1), null, null);

		public static readonly ADPropertyDefinition IsDefaultArbitrationMailbox = new ADPropertyDefinition("IsDefaultArbitrationMailbox", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			MailboxPolicySchema.MailboxPolicyFlags
		}, new CustomFilterBuilderDelegate(RetentionPolicySchema.IsDefaultArbitrationMailboxFilterBuilder), ADObject.FlagGetterDelegate(MailboxPolicySchema.MailboxPolicyFlags, 2), ADObject.FlagSetterDelegate(MailboxPolicySchema.MailboxPolicyFlags, 2), null, null);
	}
}
