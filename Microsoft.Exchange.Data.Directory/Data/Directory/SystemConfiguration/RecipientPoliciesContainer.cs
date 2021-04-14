using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(new ConfigScopes[]
	{
		ConfigScopes.TenantLocal,
		ConfigScopes.TenantSubTree
	})]
	[Serializable]
	public class RecipientPoliciesContainer : ADConfigurationObject
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return RecipientPoliciesContainer.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return RecipientPoliciesContainer.mostDerivedClass;
			}
		}

		private static RecipientPoliciesContainerSchema schema = ObjectSchema.GetInstance<RecipientPoliciesContainerSchema>();

		private static string mostDerivedClass = "msExchRecipientPolicyContainer";
	}
}
