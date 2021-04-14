using System;
using Microsoft.Exchange.Data.Directory.ProvisioningCache;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public class ExchangeOrganizationalUnit : ADConfigurationObject, IProvisioningCacheInvalidation
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return ExchangeOrganizationalUnit.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return ExchangeOrganizationalUnit.mostDerivedClass;
			}
		}

		internal override QueryFilter ImplicitFilter
		{
			get
			{
				return new OrFilter(new QueryFilter[]
				{
					new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectCategory, ADOrganizationalUnit.MostDerivedClass),
					new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectCategory, ADContainer.MostDerivedClass),
					new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectCategory, ADBuiltinDomain.MostDerivedClass),
					new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectCategory, MesoContainer.MostDerivedClass),
					new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectCategory, ADDomain.MostDerivedClass)
				});
			}
		}

		internal ADObjectId HierarchicalAddressBookRoot
		{
			get
			{
				return (ADObjectId)this[ExchangeOrganizationalUnitSchema.HABRootDepartmentLink];
			}
			set
			{
				this[ExchangeOrganizationalUnitSchema.HABRootDepartmentLink] = value;
			}
		}

		internal bool ShouldInvalidProvisioningCache(out OrganizationId orgId, out Guid[] keys)
		{
			orgId = null;
			keys = null;
			if (base.ObjectState == ObjectState.Deleted)
			{
				keys = new Guid[1];
				keys[0] = CannedProvisioningCacheKeys.OrganizationalUnitDictionary;
				return true;
			}
			return false;
		}

		bool IProvisioningCacheInvalidation.ShouldInvalidProvisioningCache(out OrganizationId orgId, out Guid[] keys)
		{
			return this.ShouldInvalidProvisioningCache(out orgId, out keys);
		}

		private static ExchangeOrganizationalUnitSchema schema = ObjectSchema.GetInstance<ExchangeOrganizationalUnitSchema>();

		private static string mostDerivedClass = "organizationalUnit";
	}
}
