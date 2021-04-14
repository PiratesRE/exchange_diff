using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal static class RoleAssignmentFlagsFormat
	{
		static RoleAssignmentFlagsFormat()
		{
			int num = 64;
			for (int i = 0; i <= 11; i++)
			{
				RoleAssignmentFlagsFormat.masks[i] = RoleAssignmentFlagsFormat.Mask(RoleAssignmentFlagsFormat.sizes[i]);
				num -= RoleAssignmentFlagsFormat.sizes[i];
				RoleAssignmentFlagsFormat.shifts[i] = num;
			}
		}

		private static ulong Mask(int numberOfBits)
		{
			return ulong.MaxValue >> 64 - numberOfBits;
		}

		private static ulong GetBits(IPropertyBag propertyBag, RoleAssignmentFlagsFormat.Bitfields bitfield)
		{
			long num = (long)propertyBag[ExchangeRoleAssignmentSchema.ExchangeRoleAssignmentFlags];
			return (ulong)num >> RoleAssignmentFlagsFormat.shifts[(int)bitfield] & RoleAssignmentFlagsFormat.masks[(int)bitfield];
		}

		private static void SetBits(ulong valueToSet, IPropertyBag propertyBag, RoleAssignmentFlagsFormat.Bitfields bitfield)
		{
			long num = (long)propertyBag[ExchangeRoleAssignmentSchema.ExchangeRoleAssignmentFlags];
			propertyBag[ExchangeRoleAssignmentSchema.ExchangeRoleAssignmentFlags] = ((num & (long)(~(long)((long)RoleAssignmentFlagsFormat.masks[(int)bitfield] << RoleAssignmentFlagsFormat.shifts[(int)bitfield]))) | (long)((long)(valueToSet & RoleAssignmentFlagsFormat.masks[(int)bitfield]) << RoleAssignmentFlagsFormat.shifts[(int)bitfield]));
		}

		internal static ulong GetRawUInt64Bits(ulong valueToSet, RoleAssignmentFlagsFormat.Bitfields bitfield)
		{
			return (valueToSet & RoleAssignmentFlagsFormat.masks[(int)bitfield]) << RoleAssignmentFlagsFormat.shifts[(int)bitfield];
		}

		private static QueryFilter RoleAssignmentFlagsFilterBuilder<T>(SinglePropertyFilter filter, RoleAssignmentFlagsFormat.Bitfields bitfield, RoleAssignmentFlagsFormat.ConvertToUlongDelegate convertor)
		{
			if (!(filter is ComparisonFilter))
			{
				throw new ADFilterException(DirectoryStrings.ExceptionUnsupportedFilterForProperty(filter.Property.Name, filter.GetType(), typeof(ComparisonFilter)));
			}
			ComparisonFilter comparisonFilter = (ComparisonFilter)filter;
			if (comparisonFilter.ComparisonOperator != ComparisonOperator.Equal && ComparisonOperator.NotEqual != comparisonFilter.ComparisonOperator)
			{
				throw new ADFilterException(DirectoryStrings.ExceptionUnsupportedOperatorForProperty(comparisonFilter.Property.Name, comparisonFilter.ComparisonOperator.ToString()));
			}
			if (!(comparisonFilter.PropertyValue is T))
			{
				throw new ArgumentException("filter.PropertyValue");
			}
			ulong rawUInt64Bits = RoleAssignmentFlagsFormat.GetRawUInt64Bits(convertor(comparisonFilter.PropertyValue), bitfield);
			ulong mask = RoleAssignmentFlagsFormat.masks[(int)bitfield] << RoleAssignmentFlagsFormat.shifts[(int)bitfield] & ~rawUInt64Bits;
			QueryFilter queryFilter = new BitMaskAndFilter(ExchangeRoleAssignmentSchema.ExchangeRoleAssignmentFlags, rawUInt64Bits);
			QueryFilter queryFilter2 = new BitMaskOrFilter(ExchangeRoleAssignmentSchema.ExchangeRoleAssignmentFlags, mask);
			if (comparisonFilter.PropertyValue is bool && !(bool)comparisonFilter.PropertyValue)
			{
				if (ComparisonOperator.NotEqual != comparisonFilter.ComparisonOperator)
				{
					return new NotFilter(queryFilter);
				}
				return queryFilter;
			}
			else
			{
				if (ComparisonOperator.NotEqual != comparisonFilter.ComparisonOperator)
				{
					return new AndFilter(new QueryFilter[]
					{
						queryFilter,
						new NotFilter(queryFilter2)
					});
				}
				return new OrFilter(new QueryFilter[]
				{
					new NotFilter(queryFilter),
					queryFilter2
				});
			}
		}

		internal static GetterDelegate ScopeTypeGetterDelegate(RoleAssignmentFlagsFormat.Bitfields bitfield)
		{
			return (IPropertyBag propertyBag) => (ScopeType)RoleAssignmentFlagsFormat.GetBits(propertyBag, bitfield);
		}

		internal static SetterDelegate ScopeTypeSetterDelegate(RoleAssignmentFlagsFormat.Bitfields bitfield)
		{
			return delegate(object value, IPropertyBag propertyBag)
			{
				RoleAssignmentFlagsFormat.SetBits((ulong)((long)((ScopeType)value)), propertyBag, bitfield);
			};
		}

		internal static object RecipientWriteScopeGetter(IPropertyBag propertyBag)
		{
			return (RecipientWriteScopeType)RoleAssignmentFlagsFormat.GetBits(propertyBag, RoleAssignmentFlagsFormat.Bitfields.RecipientWriteScope);
		}

		internal static void RecipientWriteScopeSetter(object value, IPropertyBag propertyBag)
		{
			RoleAssignmentFlagsFormat.SetBits((ulong)((long)((RecipientWriteScopeType)value)), propertyBag, RoleAssignmentFlagsFormat.Bitfields.RecipientWriteScope);
		}

		internal static QueryFilter RecipientWriteScopeFilterBuilder(SinglePropertyFilter filter)
		{
			return RoleAssignmentFlagsFormat.RoleAssignmentFlagsFilterBuilder<RecipientWriteScopeType>(filter, RoleAssignmentFlagsFormat.Bitfields.RecipientWriteScope, (object propertyValue) => (ulong)((long)((RecipientWriteScopeType)propertyValue)));
		}

		internal static void ConfigWriteScopeSetter(object value, IPropertyBag propertyBag)
		{
			RoleAssignmentFlagsFormat.SetBits((ulong)((long)((ConfigWriteScopeType)value)), propertyBag, RoleAssignmentFlagsFormat.Bitfields.ConfigWriteScope);
		}

		internal static object ConfigWriteScopeGetter(IPropertyBag propertyBag)
		{
			return (ConfigWriteScopeType)RoleAssignmentFlagsFormat.GetBits(propertyBag, RoleAssignmentFlagsFormat.Bitfields.ConfigWriteScope);
		}

		internal static QueryFilter ConfigWriteScopeFilterBuilder(SinglePropertyFilter filter)
		{
			return RoleAssignmentFlagsFormat.RoleAssignmentFlagsFilterBuilder<ConfigWriteScopeType>(filter, RoleAssignmentFlagsFormat.Bitfields.ConfigWriteScope, (object propertyValue) => (ulong)((long)((ConfigWriteScopeType)propertyValue)));
		}

		internal static object RoleAssignmentDelegationTypeGetter(IPropertyBag propertyBag)
		{
			return (RoleAssignmentDelegationType)RoleAssignmentFlagsFormat.GetBits(propertyBag, RoleAssignmentFlagsFormat.Bitfields.RoleAssignmentDelegationType);
		}

		internal static void RoleAssignmentDelegationTypeSetter(object value, IPropertyBag propertyBag)
		{
			RoleAssignmentFlagsFormat.SetBits((ulong)((long)((RoleAssignmentDelegationType)value)), propertyBag, RoleAssignmentFlagsFormat.Bitfields.RoleAssignmentDelegationType);
		}

		internal static QueryFilter RoleAssignmentDelegationFilterBuilder(SinglePropertyFilter filter)
		{
			return RoleAssignmentFlagsFormat.RoleAssignmentFlagsFilterBuilder<RoleAssignmentDelegationType>(filter, RoleAssignmentFlagsFormat.Bitfields.RoleAssignmentDelegationType, (object propertyValue) => (ulong)((long)((RoleAssignmentDelegationType)propertyValue)));
		}

		internal static object EnabledGetter(IPropertyBag propertyBag)
		{
			return RoleAssignmentFlagsFormat.GetBits(propertyBag, RoleAssignmentFlagsFormat.Bitfields.IsEnabled) != 0UL;
		}

		internal static void EnabledSetter(object value, IPropertyBag propertyBag)
		{
			RoleAssignmentFlagsFormat.SetBits((ulong)(((bool)value) ? 1L : 0L), propertyBag, RoleAssignmentFlagsFormat.Bitfields.IsEnabled);
		}

		internal static QueryFilter RoleAssignmentEnabledFilterBuilder(SinglePropertyFilter filter)
		{
			return RoleAssignmentFlagsFormat.RoleAssignmentFlagsFilterBuilder<bool>(filter, RoleAssignmentFlagsFormat.Bitfields.IsEnabled, (object propertyValue) => ((bool)propertyValue) ? 1UL : 0UL);
		}

		internal static object RoleAssigneeTypeGetter(IPropertyBag propertyBag)
		{
			return (RoleAssigneeType)RoleAssignmentFlagsFormat.GetBits(propertyBag, RoleAssignmentFlagsFormat.Bitfields.RoleAssigneeType);
		}

		internal static void RoleAssigneeTypeSetter(object value, IPropertyBag propertyBag)
		{
			RoleAssignmentFlagsFormat.SetBits((ulong)((long)((RoleAssigneeType)value)), propertyBag, RoleAssignmentFlagsFormat.Bitfields.RoleAssigneeType);
		}

		internal static QueryFilter RoleAssigneeTypeFilterBuilder(SinglePropertyFilter filter)
		{
			return RoleAssignmentFlagsFormat.RoleAssignmentFlagsFilterBuilder<RoleAssigneeType>(filter, RoleAssignmentFlagsFormat.Bitfields.RoleAssigneeType, (object propertyValue) => (ulong)((long)((RoleAssigneeType)propertyValue)));
		}

		internal static QueryFilter GetPartnerFilter(bool partnerMode)
		{
			if (RoleAssignmentFlagsFormat.partnerFilter == null || RoleAssignmentFlagsFormat.notPartnerFilter == null)
			{
				QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, ExchangeRoleAssignmentSchema.ConfigWriteScope, ConfigWriteScopeType.PartnerDelegatedTenantScope);
				RoleAssignmentFlagsFormat.partnerFilter = filter;
				RoleAssignmentFlagsFormat.notPartnerFilter = new NotFilter(filter);
			}
			if (!partnerMode)
			{
				return RoleAssignmentFlagsFormat.notPartnerFilter;
			}
			return RoleAssignmentFlagsFormat.partnerFilter;
		}

		private static int[] sizes = new int[]
		{
			1,
			4,
			7,
			4,
			3,
			5,
			3,
			5,
			3,
			5,
			19,
			5
		};

		private static ulong[] masks = new ulong[RoleAssignmentFlagsFormat.sizes.Length];

		private static int[] shifts = new int[RoleAssignmentFlagsFormat.sizes.Length];

		private static QueryFilter partnerFilter;

		private static QueryFilter notPartnerFilter;

		internal enum Bitfields
		{
			IsEnabled,
			RoleAssigneeType,
			Reserved,
			RoleAssignmentDelegationType,
			Reserved2,
			ConfigReadScope,
			Reserved3,
			ConfigWriteScope,
			Reserved4,
			RecipientReadScope,
			Reserved5,
			RecipientWriteScope
		}

		private delegate ulong ConvertToUlongDelegate(object propertyValue);
	}
}
