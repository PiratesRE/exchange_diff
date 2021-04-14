using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(new ConfigScopes[]
	{
		ConfigScopes.TenantLocal,
		ConfigScopes.TenantSubTree
	})]
	[Serializable]
	internal sealed class ADClientAccessRuleCollection : ADConfigurationObject
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return ADClientAccessRuleCollection.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return ADClientAccessRuleCollection.mostDerivedClass;
			}
		}

		public static readonly string ContainerName = "Client Access Rules";

		private static ADClientAccessRuleCollectionSchema schema = ObjectSchema.GetInstance<ADClientAccessRuleCollectionSchema>();

		private static string mostDerivedClass = "msExchContainer";
	}
}
