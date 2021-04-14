using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public class NtdsDsa : ADNonExchangeObject
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return NtdsDsa.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return NtdsDsa.mostDerivedClass;
			}
		}

		internal override QueryFilter ImplicitFilter
		{
			get
			{
				return new OrFilter(new QueryFilter[]
				{
					new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectCategory, this.MostDerivedObjectClass),
					new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectCategory, NtdsDsa.mostDerivedClassForRodc)
				});
			}
		}

		internal static object DsIsRodcGetter(IPropertyBag propertyBag)
		{
			ADObjectId adobjectId = (ADObjectId)propertyBag[ADObjectSchema.ObjectCategory];
			return adobjectId.Rdn.Equals(NtdsDsa.roName);
		}

		internal static QueryFilter DsIsRodcFilterBuilder(SinglePropertyFilter filter)
		{
			Database.InternalAssertComparisonFilter(filter, NtdsDsaSchema.DsIsRodc);
			ComparisonFilter comparisonFilter = filter as ComparisonFilter;
			return new ComparisonFilter(comparisonFilter.ComparisonOperator, ADObjectSchema.ObjectCategory, ((bool)comparisonFilter.PropertyValue) ? NtdsDsa.mostDerivedClassForRodc : NtdsDsa.mostDerivedClass);
		}

		public bool DsIsRodc
		{
			get
			{
				return (bool)this[NtdsDsaSchema.DsIsRodc];
			}
		}

		public Guid InvocationId
		{
			get
			{
				return (Guid)this[NtdsDsaSchema.InvocationId];
			}
		}

		internal NtdsdsaOptions Options
		{
			get
			{
				return (NtdsdsaOptions)this[NtdsDsaSchema.Options];
			}
		}

		internal MultiValuedProperty<ADObjectId> MasterNCs
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[NtdsDsaSchema.MasterNCs];
			}
		}

		private static NtdsDsaSchema schema = ObjectSchema.GetInstance<NtdsDsaSchema>();

		private static string mostDerivedClass = "ntdsDsa";

		private static string mostDerivedClassForRodc = "ntdsDsaRo";

		private static AdName roName = new AdName("CN", "NTDS-DSA-RO");
	}
}
