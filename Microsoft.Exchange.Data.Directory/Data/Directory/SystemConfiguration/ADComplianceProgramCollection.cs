using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(new ConfigScopes[]
	{
		ConfigScopes.TenantLocal,
		ConfigScopes.TenantSubTree
	})]
	[Serializable]
	public sealed class ADComplianceProgramCollection : ADConfigurationObject
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return ADComplianceProgramCollection.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return ADComplianceProgramCollection.mostDerivedClass;
			}
		}

		internal ADComplianceProgram[] GetCompliancePrograms()
		{
			return base.Session.Find<ADComplianceProgram>(base.Id, QueryScope.OneLevel, null, new SortBy(ADObjectSchema.Name, SortOrder.Ascending), 0);
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2012;
			}
		}

		private static ADComplianceProgramCollectionSchema schema = ObjectSchema.GetInstance<ADComplianceProgramCollectionSchema>();

		private static string mostDerivedClass = "msExchMailflowPolicyCollection";
	}
}
