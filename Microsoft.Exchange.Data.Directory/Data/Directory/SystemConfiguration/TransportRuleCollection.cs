using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(new ConfigScopes[]
	{
		ConfigScopes.TenantLocal,
		ConfigScopes.TenantSubTree
	})]
	[Serializable]
	public sealed class TransportRuleCollection : ADConfigurationObject
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return TransportRuleCollection.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return TransportRuleCollection.mostDerivedClass;
			}
		}

		internal TransportRule[] GetRules()
		{
			return base.Session.Find<TransportRule>(base.Id, QueryScope.OneLevel, null, new SortBy(TransportRuleSchema.Priority, SortOrder.Ascending), 0);
		}

		private static TransportRuleCollectionSchema schema = ObjectSchema.GetInstance<TransportRuleCollectionSchema>();

		private static string mostDerivedClass = "msExchTransportRuleCollection";
	}
}
