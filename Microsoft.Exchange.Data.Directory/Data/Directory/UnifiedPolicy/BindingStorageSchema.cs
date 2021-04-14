using System;

namespace Microsoft.Exchange.Data.Directory.UnifiedPolicy
{
	internal class BindingStorageSchema : UnifiedPolicyStorageBaseSchema
	{
		public static ADPropertyDefinition PolicyId = new ADPropertyDefinition("PolicyId", ExchangeObjectVersion.Exchange2012, typeof(Guid), "msExchBindingPolicyId", ADPropertyDefinitionFlags.Mandatory | ADPropertyDefinitionFlags.Binary | ADPropertyDefinitionFlags.NonADProperty, System.Guid.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static ADPropertyDefinition Scopes = new ADPropertyDefinition("Scopes", ExchangeObjectVersion.Exchange2012, typeof(string), "msExchBindingScopes", ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.NonADProperty, null, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(1, 1024)
		}, PropertyDefinitionConstraint.None, null, null);

		public static ADPropertyDefinition DeletedScopes = new ADPropertyDefinition("_DELETED_Scopes", ExchangeObjectVersion.Exchange2012, typeof(string), "msExchBindingDeletedScopes", ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.NonADProperty, null, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(1, 1024)
		}, PropertyDefinitionConstraint.None, null, null);

		public static ADPropertyDefinition AppliedScopes = new ADPropertyDefinition("AppliedScopes", ExchangeObjectVersion.Exchange2012, typeof(ScopeStorage), "msExchBindingAppliedScopes", ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.NonADProperty, null, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(1, 1024)
		}, PropertyDefinitionConstraint.None, null, null);

		public static ADPropertyDefinition RemovedScopes = new ADPropertyDefinition("_DELETED_AppliedScopes", ExchangeObjectVersion.Exchange2012, typeof(ScopeStorage), "msExchBindingRemovedScopes", ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.NonADProperty, null, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(1, 1024)
		}, PropertyDefinitionConstraint.None, null, null);
	}
}
