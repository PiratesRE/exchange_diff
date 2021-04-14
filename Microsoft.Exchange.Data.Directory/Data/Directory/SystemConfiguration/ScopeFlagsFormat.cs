using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal struct ScopeFlagsFormat
	{
		internal static object ScopeRestrictionTypeGetter(IPropertyBag propertyBag)
		{
			ScopeRestrictionType scopeRestrictionType = (ScopeRestrictionType)((int)propertyBag[ManagementScopeSchema.ScopeRestrictionFlags] & 255);
			return (scopeRestrictionType == ScopeRestrictionType.DomainScope_Obsolete) ? ScopeRestrictionType.RecipientScope : scopeRestrictionType;
		}

		internal static void ScopeRestrictionTypeSetter(object value, IPropertyBag propertyBag)
		{
			uint num = (uint)((int)propertyBag[ManagementScopeSchema.ScopeRestrictionFlags]);
			propertyBag[ManagementScopeSchema.ScopeRestrictionFlags] = (int)((num & 4294967040U) | (uint)((ScopeRestrictionType)value));
		}

		internal static object ExclusiveTypeGetter(IPropertyBag propertyBag)
		{
			return (uint)((int)propertyBag[ManagementScopeSchema.ScopeRestrictionFlags]) >> 31 != 0U;
		}

		internal static void ExclusiveTypeSetter(object value, IPropertyBag propertyBag)
		{
			uint num = (uint)((int)propertyBag[ManagementScopeSchema.ScopeRestrictionFlags]);
			uint num2 = ((bool)value) ? 1U : 0U;
			propertyBag[ManagementScopeSchema.ScopeRestrictionFlags] = (int)(num2 << 31 | (num & 255U));
		}

		private static QueryFilter ScopeRestrictionFlagsFilterBuilder<T>(SinglePropertyFilter filter, ScopeFlagsFormat.ConvertToMaskDelegate convertor)
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
			QueryFilter queryFilter = new BitMaskAndFilter(ManagementScopeSchema.ScopeRestrictionFlags, convertor(comparisonFilter.PropertyValue));
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
					return queryFilter;
				}
				return new NotFilter(queryFilter);
			}
		}

		internal static QueryFilter ScopeRestrictionTypeFilterBuilder(SinglePropertyFilter filter)
		{
			return ScopeFlagsFormat.ScopeRestrictionFlagsFilterBuilder<ScopeRestrictionType>(filter, (object propertyValue) => (ulong)((ScopeRestrictionType)propertyValue & (ScopeRestrictionType)255));
		}

		internal static QueryFilter ExclusiveTypeFilterBuilder(SinglePropertyFilter filter)
		{
			return ScopeFlagsFormat.ScopeRestrictionFlagsFilterBuilder<bool>(filter, (object propertyValue) => (ulong)int.MinValue);
		}

		internal static GetterDelegate FilterGetterDelegate(ScopeRestrictionType filterType)
		{
			return delegate(IPropertyBag bag)
			{
				ScopeRestrictionType scopeRestrictionType = (ScopeRestrictionType)bag[ManagementScopeSchema.ScopeRestrictionType];
				if (scopeRestrictionType == ScopeRestrictionType.DomainScope_Obsolete)
				{
					scopeRestrictionType = ScopeRestrictionType.RecipientScope;
				}
				if (scopeRestrictionType != filterType)
				{
					return string.Empty;
				}
				return (string)bag[ManagementScopeSchema.Filter];
			};
		}

		internal static SetterDelegate FilterSetterDelegate(ScopeRestrictionType filterType)
		{
			return delegate(object value, IPropertyBag bag)
			{
				ScopeRestrictionType scopeRestrictionType = (ScopeRestrictionType)bag[ManagementScopeSchema.ScopeRestrictionType];
				if (scopeRestrictionType == ScopeRestrictionType.DomainScope_Obsolete)
				{
					scopeRestrictionType = ScopeRestrictionType.RecipientScope;
				}
				if (scopeRestrictionType != filterType)
				{
					throw new ArgumentException();
				}
				bag[ManagementScopeSchema.Filter] = (string)value;
			};
		}

		internal const uint ScopeRestrictionTypeMask = 255U;

		internal const uint ExclusiveBitMask = 2147483648U;

		internal const int ExclusiveBitShift = 31;

		private delegate ulong ConvertToMaskDelegate(object propertyValue);
	}
}
