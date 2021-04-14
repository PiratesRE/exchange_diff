using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal sealed class RetentionPolicyTagSchema : ADConfigurationObjectSchema
	{
		internal static QueryFilter IsDefaultAutoGroupPolicyTagFilterBuilder(SinglePropertyFilter filter)
		{
			return ADObject.BoolFilterBuilder(filter, new BitMaskAndFilter(RetentionPolicyTagSchema.PolicyTagFlags, 8UL));
		}

		internal static QueryFilter IsDefaultModeratedRecipientsPolicyTagFilterBuilder(SinglePropertyFilter filter)
		{
			return ADObject.BoolFilterBuilder(filter, new BitMaskAndFilter(RetentionPolicyTagSchema.PolicyTagFlags, 16UL));
		}

		public static readonly ADPropertyDefinition LocalizedRetentionPolicyTagName = new ADPropertyDefinition("LocalizedRetentionPolicyTagName", ExchangeObjectVersion.Exchange2007, typeof(string), "msExchELCFolderNameLocalized", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition Comment = SharedPropertyDefinitions.Comment;

		public static readonly ADPropertyDefinition LocalizedComment = SharedPropertyDefinitions.LocalizedComment;

		public static readonly ADPropertyDefinition PolicyTagFlags = SharedPropertyDefinitions.ElcFlags;

		public static readonly ADPropertyDefinition PolicyIds = new ADPropertyDefinition("PolicyIds", ExchangeObjectVersion.Exchange2007, typeof(ADObjectId), "msExchELCFolderBL", ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.BackLink, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition Type = new ADPropertyDefinition("Type", ExchangeObjectVersion.Exchange2007, typeof(ElcFolderType), "msExchELCFolderType", ADPropertyDefinitionFlags.PersistDefaultValue, ElcFolderType.Personal, new PropertyDefinitionConstraint[]
		{
			new EnumValueDefinedConstraint(typeof(ElcFolderType))
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition RetentionId = new ADPropertyDefinition("RetentionId", ExchangeObjectVersion.Exchange2010, typeof(Guid), "msExchAuthoritativePolicyTagGUID", ADPropertyDefinitionFlags.Binary, System.Guid.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition LegacyManagedFolder = new ADPropertyDefinition("LegacyManagedFolder", ExchangeObjectVersion.Exchange2007, typeof(ADObjectId), "msExchPolicyTagLink", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition IsDefaultAutoGroupPolicyTag = new ADPropertyDefinition("IsDefaultAutoGroupPolicyTag", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			RetentionPolicyTagSchema.PolicyTagFlags
		}, new CustomFilterBuilderDelegate(RetentionPolicyTagSchema.IsDefaultAutoGroupPolicyTagFilterBuilder), ADObject.FlagGetterDelegate(RetentionPolicyTagSchema.PolicyTagFlags, 8), ADObject.FlagSetterDelegate(RetentionPolicyTagSchema.PolicyTagFlags, 8), null, null);

		public static readonly ADPropertyDefinition IsDefaultModeratedRecipientsPolicyTag = new ADPropertyDefinition("IsDefaultModeratedRecipientsPolicyTag", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			RetentionPolicyTagSchema.PolicyTagFlags
		}, new CustomFilterBuilderDelegate(RetentionPolicyTagSchema.IsDefaultModeratedRecipientsPolicyTagFilterBuilder), ADObject.FlagGetterDelegate(RetentionPolicyTagSchema.PolicyTagFlags, 16), ADObject.FlagSetterDelegate(RetentionPolicyTagSchema.PolicyTagFlags, 16), null, null);
	}
}
