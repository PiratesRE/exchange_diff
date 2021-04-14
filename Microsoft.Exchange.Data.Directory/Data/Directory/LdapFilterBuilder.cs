using System;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;

namespace Microsoft.Exchange.Data.Directory
{
	internal sealed class LdapFilterBuilder
	{
		private LdapFilterBuilder() : this(LdapFilterBuilder.EstimatedFilterStringSize)
		{
		}

		private LdapFilterBuilder(int estimatedStringFilterSize)
		{
			this.ldapFilter = new StringBuilder(estimatedStringFilterSize);
		}

		private LdapFilterBuilder(int estimatedStringFilterSize, bool skipCheckVirtualIndex, SoftLinkMode softLinkMode, bool tenantScoped) : this(estimatedStringFilterSize)
		{
			this.skipCheckVirtualIndex = skipCheckVirtualIndex;
			this.softLinkMode = softLinkMode;
			this.tenantScoped = tenantScoped;
		}

		public static string LdapFilterFromQueryFilter(QueryFilter queryFilter)
		{
			return LdapFilterBuilder.LdapFilterFromQueryFilter(queryFilter, false);
		}

		public static string LdapFilterFromQueryFilter(QueryFilter queryFilter, bool skipCheckVirtualIndex)
		{
			return LdapFilterBuilder.LdapFilterFromQueryFilter(queryFilter, skipCheckVirtualIndex, SoftLinkMode.Disabled, false);
		}

		public static string LdapFilterFromQueryFilter(QueryFilter queryFilter, bool skipCheckVirtualIndex, SoftLinkMode softLinkMode, bool tenantScoped)
		{
			bool flag;
			return LdapFilterBuilder.LdapFilterFromQueryFilter(queryFilter, skipCheckVirtualIndex, softLinkMode, tenantScoped, out flag);
		}

		public static string LdapFilterFromQueryFilter(QueryFilter queryFilter, bool skipCheckVirtualIndex, SoftLinkMode softLinkMode, bool tenantScoped, out bool containsUnsafeIdentity)
		{
			containsUnsafeIdentity = false;
			string text;
			if (queryFilter == null)
			{
				ExTraceGlobals.LdapFilterBuilderTracer.TraceDebug(0L, "LdapFilterBuilder::LdapFilterFromQueryFilter - forming LDAP Filter for <null>.");
				text = "(objectclass=*)";
			}
			else
			{
				ExTraceGlobals.LdapFilterBuilderTracer.TraceDebug<QueryFilter>(0L, "LdapFilterBuilder::LdapFilterFromQueryFilter - forming LDAP Filter for {0}.", queryFilter);
				LdapFilterBuilder ldapFilterBuilder = new LdapFilterBuilder(LdapFilterBuilder.EstimatedFilterStringSize, skipCheckVirtualIndex, softLinkMode, tenantScoped);
				ldapFilterBuilder.BuildLdapFilter(queryFilter, null);
				text = ldapFilterBuilder.ldapFilter.ToString();
				containsUnsafeIdentity = ldapFilterBuilder.containsUnsafeIdentity;
				int value = (LdapFilterBuilder.EstimatedFilterStringSize + text.Length) / 2;
				Interlocked.Exchange(ref LdapFilterBuilder.EstimatedFilterStringSize, value);
			}
			if ((long)text.Length > 128000L)
			{
				throw new ADFilterException(DirectoryStrings.InvalidFilterLength);
			}
			ExTraceGlobals.LdapFilterBuilderTracer.TraceDebug<string>(0L, "LdapFilterBuilder::LdapFilterFromQueryFilter - Ldap filter = {0}.", text);
			return text;
		}

		private void BuildLdapFilter(QueryFilter queryFilter, QueryFilter parentQueryFilter)
		{
			if (queryFilter == null || queryFilter is TrueFilter)
			{
				return;
			}
			if (queryFilter.IsAtomic)
			{
				this.IncrementFilterSize(2);
				this.ldapFilter.Append("(!");
				this.ldapFilter.Append("(!");
			}
			this.BuildLdapFilterInternal(queryFilter, parentQueryFilter);
			if (queryFilter.IsAtomic)
			{
				this.ldapFilter.Append(")");
				this.ldapFilter.Append(")");
			}
		}

		private void BuildLdapFilterInternal(QueryFilter queryFilter, QueryFilter parentQueryFilter)
		{
			SinglePropertyFilter singlePropertyFilter = queryFilter as SinglePropertyFilter;
			if (singlePropertyFilter != null)
			{
				this.ConvertSinglePropertyFilter(singlePropertyFilter, parentQueryFilter);
				return;
			}
			NotFilter notFilter = queryFilter as NotFilter;
			if (notFilter != null)
			{
				this.ConvertNotFilter(notFilter);
				return;
			}
			AmbiguousNameResolutionFilter ambiguousNameResolutionFilter = queryFilter as AmbiguousNameResolutionFilter;
			if (ambiguousNameResolutionFilter != null)
			{
				this.ConvertAnrFilter(ambiguousNameResolutionFilter);
				return;
			}
			CompositeFilter compositeFilter = queryFilter as CompositeFilter;
			if (compositeFilter != null)
			{
				this.ConvertCompositeFilter(compositeFilter, parentQueryFilter);
				return;
			}
			CustomLdapFilter customLdapFilter = queryFilter as CustomLdapFilter;
			if (customLdapFilter != null)
			{
				this.ldapFilter.Append("(");
				this.ldapFilter.Append(customLdapFilter.LdapFilter);
				this.ldapFilter.Append(")");
				return;
			}
			throw new ADFilterException(DirectoryStrings.ExceptionUnsupportedFilter(queryFilter.GetType().ToString()));
		}

		private void ConvertSinglePropertyFilter(SinglePropertyFilter queryFilter, QueryFilter parentFilter)
		{
			ADPropertyDefinition adpropertyDefinition = (ADPropertyDefinition)queryFilter.Property;
			if (adpropertyDefinition.LdapDisplayName == null)
			{
				if (adpropertyDefinition.CustomFilterBuilderDelegate == null)
				{
					throw new ADFilterException(DirectoryStrings.ExceptionPropertyCannotBeSearchedOn(adpropertyDefinition.Name));
				}
				ComparisonFilter comparisonFilter = queryFilter as ComparisonFilter;
				if (comparisonFilter != null && comparisonFilter.ComparisonOperator == ComparisonOperator.NotEqual)
				{
					QueryFilter filter = adpropertyDefinition.CustomFilterBuilderDelegate(new ComparisonFilter(ComparisonOperator.Equal, comparisonFilter.Property, comparisonFilter.PropertyValue));
					this.BuildLdapFilter(QueryFilter.NotFilter(filter), parentFilter);
					return;
				}
				QueryFilter queryFilter2 = adpropertyDefinition.CustomFilterBuilderDelegate(queryFilter);
				this.BuildLdapFilter(queryFilter2, parentFilter);
				return;
			}
			else
			{
				ComparisonFilter comparisonFilter2 = queryFilter as ComparisonFilter;
				if (comparisonFilter2 != null)
				{
					this.ConvertComparisonFilter(comparisonFilter2, parentFilter);
					return;
				}
				ExistsFilter existsFilter = queryFilter as ExistsFilter;
				if (existsFilter != null)
				{
					this.ConvertExistsFilter(existsFilter);
					return;
				}
				TextFilter textFilter = queryFilter as TextFilter;
				if (textFilter != null)
				{
					this.ConvertTextFilter(textFilter);
					return;
				}
				GenericBitMaskFilter genericBitMaskFilter = queryFilter as GenericBitMaskFilter;
				if (genericBitMaskFilter != null)
				{
					this.ConvertBitMaskFilter(genericBitMaskFilter);
					return;
				}
				InChainFilter inChainFilter = queryFilter as InChainFilter;
				if (inChainFilter != null)
				{
					this.ConvertInChainFilter(inChainFilter);
				}
				return;
			}
		}

		private void ConvertComparisonFilter(ComparisonFilter comparisonFilter, QueryFilter parentFilter)
		{
			ADPropertyDefinition adpropertyDefinition = (ADPropertyDefinition)comparisonFilter.Property;
			if (adpropertyDefinition != ADObjectSchema.ObjectCategory)
			{
				this.containsUnsafeIdentity |= adpropertyDefinition.IsForestSpecific;
			}
			if (!adpropertyDefinition.IsSoftLinkAttribute)
			{
				this.ConvertComparisonFilterInternal(comparisonFilter, parentFilter, false, 0);
				return;
			}
			bool config = ConfigBase<AdDriverConfigSchema>.GetConfig<bool>("SoftLinkFilterVersion2Enabled");
			switch (this.softLinkMode)
			{
			case SoftLinkMode.Enabled:
				if (config)
				{
					this.IncrementFilterSize(2);
					this.ldapFilter.Append("(|");
					this.ConvertComparisonFilterInternal(comparisonFilter, parentFilter, true, 1);
					this.ConvertComparisonFilterInternal(comparisonFilter, parentFilter, true, 2);
					this.ldapFilter.Append(")");
					return;
				}
				this.IncrementFilterSize();
				this.ConvertComparisonFilterInternal(comparisonFilter, parentFilter, true, 1);
				return;
			case SoftLinkMode.Disabled:
				this.ConvertComparisonFilterInternal(comparisonFilter, parentFilter, false, 0);
				return;
			}
			if (config)
			{
				this.IncrementFilterSize(3);
				this.ldapFilter.Append("(|");
				this.ConvertComparisonFilterInternal(comparisonFilter, parentFilter, false, 0);
				this.ConvertComparisonFilterInternal(comparisonFilter, parentFilter, true, 1);
				this.ConvertComparisonFilterInternal(comparisonFilter, parentFilter, true, 2);
				this.ldapFilter.Append(")");
				return;
			}
			this.IncrementFilterSize(2);
			this.ldapFilter.Append("(|");
			this.ConvertComparisonFilterInternal(comparisonFilter, parentFilter, false, 0);
			this.ConvertComparisonFilterInternal(comparisonFilter, parentFilter, true, 1);
			this.ldapFilter.Append(")");
		}

		private void ConvertComparisonFilterInternal(ComparisonFilter comparisonFilter, QueryFilter parentFilter, bool isSoftLink, byte softLinkPrefix = 0)
		{
			ADPropertyDefinition adpropertyDefinition = (ADPropertyDefinition)comparisonFilter.Property;
			object obj = comparisonFilter.PropertyValue;
			string ldapDisplayName = adpropertyDefinition.LdapDisplayName;
			if (isSoftLink && adpropertyDefinition.IsSoftLinkAttribute)
			{
				ldapDisplayName = adpropertyDefinition.SoftLinkShadowProperty.LdapDisplayName;
				if (obj != null)
				{
					obj = ADObjectIdResolutionHelper.ResolveSoftLink((ADObjectId)obj);
				}
			}
			bool flag = adpropertyDefinition.Type.IsGenericType && adpropertyDefinition.Type.GetGenericTypeDefinition() == typeof(Unlimited<>);
			if (flag)
			{
				bool flag2 = adpropertyDefinition.DefaultValue.Equals(obj);
				switch (comparisonFilter.ComparisonOperator)
				{
				case ComparisonOperator.Equal:
					if (flag2)
					{
						this.IncrementFilterSize(2);
						this.ldapFilter.Append("(!(");
						this.ldapFilter.Append(ldapDisplayName);
						this.ldapFilter.Append("=*))");
						return;
					}
					break;
				case ComparisonOperator.NotEqual:
				case ComparisonOperator.LessThan:
					if (flag2)
					{
						this.IncrementFilterSize();
						this.ldapFilter.Append('(');
						this.ldapFilter.Append(ldapDisplayName);
						this.ldapFilter.Append("=*)");
						return;
					}
					break;
				case ComparisonOperator.LessThanOrEqual:
					if (flag2)
					{
						this.IncrementFilterSize();
						this.ldapFilter.Append("(objectclass=*)");
						return;
					}
					break;
				case ComparisonOperator.GreaterThan:
					if (flag2)
					{
						this.IncrementFilterSize();
						this.ldapFilter.Append("(!(objectclass=*))");
						return;
					}
					break;
				case ComparisonOperator.GreaterThanOrEqual:
					if (flag2)
					{
						this.IncrementFilterSize(2);
						this.ldapFilter.Append("(!(");
						this.ldapFilter.Append(ldapDisplayName);
						this.ldapFilter.Append("=*))");
						return;
					}
					this.IncrementFilterSize(4);
					this.ldapFilter.Append("(|(");
					this.ldapFilter.Append(ldapDisplayName);
					this.ldapFilter.Append(">=");
					ADValueConvertor.ConvertToAndAppendFilterString(adpropertyDefinition, obj, this.ldapFilter, isSoftLink, softLinkPrefix);
					this.ldapFilter.Append(")(!(");
					this.ldapFilter.Append(ldapDisplayName);
					this.ldapFilter.Append("=*)))");
					return;
				}
			}
			switch (comparisonFilter.ComparisonOperator)
			{
			case ComparisonOperator.LessThan:
				this.ConvertCompositeFilter(new AndFilter(new QueryFilter[]
				{
					new ComparisonFilter(ComparisonOperator.LessThanOrEqual, comparisonFilter.Property, obj),
					new ComparisonFilter(ComparisonOperator.NotEqual, comparisonFilter.Property, obj)
				}), parentFilter);
				return;
			case ComparisonOperator.GreaterThan:
				this.ConvertCompositeFilter(new AndFilter(new QueryFilter[]
				{
					new ComparisonFilter(ComparisonOperator.GreaterThanOrEqual, comparisonFilter.Property, obj),
					new ComparisonFilter(ComparisonOperator.NotEqual, comparisonFilter.Property, obj)
				}), parentFilter);
				return;
			}
			if (!adpropertyDefinition.PersistDefaultValue && !flag && adpropertyDefinition.DefaultValue != null && adpropertyDefinition.DefaultValue.Equals(comparisonFilter.PropertyValue))
			{
				switch (comparisonFilter.ComparisonOperator)
				{
				case ComparisonOperator.Equal:
					this.IncrementFilterSize(4);
					this.ldapFilter.Append("(|(");
					this.ldapFilter.Append(ldapDisplayName);
					this.ldapFilter.Append("=");
					ADValueConvertor.ConvertToAndAppendFilterString(adpropertyDefinition, adpropertyDefinition.DefaultValue, this.ldapFilter, isSoftLink, softLinkPrefix);
					this.ldapFilter.Append(")(!(");
					this.ldapFilter.Append(ldapDisplayName);
					this.ldapFilter.Append("=*)))");
					return;
				case ComparisonOperator.NotEqual:
					this.IncrementFilterSize(4);
					this.ldapFilter.Append("(&(!(");
					this.ldapFilter.Append(ldapDisplayName);
					this.ldapFilter.Append("=");
					ADValueConvertor.ConvertToAndAppendFilterString(adpropertyDefinition, adpropertyDefinition.DefaultValue, this.ldapFilter, isSoftLink, softLinkPrefix);
					this.ldapFilter.Append("))(");
					this.ldapFilter.Append(ldapDisplayName);
					this.ldapFilter.Append("=*))");
					return;
				default:
					throw new ADFilterException(DirectoryStrings.ExceptionUnsupportedDefaultValueFilter(adpropertyDefinition.Name, comparisonFilter.ComparisonOperator.ToString(), (comparisonFilter.PropertyValue == null) ? string.Empty : obj.ToString()));
				}
			}
			else
			{
				string value = string.Empty;
				switch (comparisonFilter.ComparisonOperator)
				{
				case ComparisonOperator.Equal:
					value = "=";
					goto IL_49B;
				case ComparisonOperator.NotEqual:
					value = "=";
					goto IL_49B;
				case ComparisonOperator.LessThanOrEqual:
					value = "<=";
					goto IL_49B;
				case ComparisonOperator.GreaterThanOrEqual:
					value = ">=";
					goto IL_49B;
				}
				throw new ADFilterException(DirectoryStrings.ExceptionUnsupportedOperator(comparisonFilter.ComparisonOperator.ToString(), typeof(ComparisonFilter).ToString()));
				IL_49B:
				if (comparisonFilter.ComparisonOperator == ComparisonOperator.NotEqual)
				{
					this.IncrementFilterSize(2);
					this.ldapFilter.Append("(!(");
					this.ldapFilter.Append(ldapDisplayName);
					this.ldapFilter.Append(value);
					ADValueConvertor.ConvertToAndAppendFilterString(adpropertyDefinition, obj, this.ldapFilter, isSoftLink, softLinkPrefix);
					this.ldapFilter.Append("))");
					return;
				}
				if (adpropertyDefinition == ADObjectSchema.Id && Guid.Empty != ((ADObjectId)obj).ObjectGuid && this.tenantScoped && comparisonFilter.ComparisonOperator == ComparisonOperator.Equal)
				{
					this.IncrementFilterSize(3);
					this.ldapFilter.Append("(|(");
					this.ldapFilter.Append(ldapDisplayName);
					this.ldapFilter.Append(value);
					ADValueConvertor.ConvertToAndAppendFilterString(adpropertyDefinition, obj, this.ldapFilter, false, softLinkPrefix);
					this.ldapFilter.Append(")(");
					this.ldapFilter.Append(ADObjectSchema.CorrelationIdRaw.LdapDisplayName);
					this.ldapFilter.Append(value);
					this.ldapFilter.Append(ADValueConvertor.EscapeBinaryValue(((ADObjectId)obj).ObjectGuid.ToByteArray()));
					this.ldapFilter.Append("))");
					return;
				}
				if (adpropertyDefinition == ADObjectSchema.Guid && this.tenantScoped && comparisonFilter.ComparisonOperator == ComparisonOperator.Equal)
				{
					string value2 = ADValueConvertor.EscapeBinaryValue(((Guid)obj).ToByteArray());
					this.IncrementFilterSize(3);
					this.ldapFilter.Append("(|(");
					this.ldapFilter.Append(ldapDisplayName);
					this.ldapFilter.Append(value);
					this.ldapFilter.Append(value2);
					this.ldapFilter.Append(")(");
					this.ldapFilter.Append(ADObjectSchema.CorrelationIdRaw.LdapDisplayName);
					this.ldapFilter.Append(value);
					this.ldapFilter.Append(value2);
					this.ldapFilter.Append("))");
					return;
				}
				this.IncrementFilterSize();
				this.ldapFilter.Append("(");
				this.ldapFilter.Append(ldapDisplayName);
				this.ldapFilter.Append(value);
				ADValueConvertor.ConvertToAndAppendFilterString(adpropertyDefinition, obj, this.ldapFilter, isSoftLink, softLinkPrefix);
				this.ldapFilter.Append(")");
				return;
			}
		}

		private void ConvertExistsFilter(ExistsFilter existsFilter)
		{
			ADPropertyDefinition adpropertyDefinition = (ADPropertyDefinition)existsFilter.Property;
			if (adpropertyDefinition.IsSoftLinkAttribute)
			{
				switch (this.softLinkMode)
				{
				case SoftLinkMode.Enabled:
					this.ConvertExistsFilterInternal(existsFilter, true);
					return;
				case SoftLinkMode.Disabled:
					this.ConvertExistsFilterInternal(existsFilter, false);
					return;
				}
				this.IncrementFilterSize();
				this.ldapFilter.Append("(|");
				this.ConvertExistsFilterInternal(existsFilter, false);
				this.ConvertExistsFilterInternal(existsFilter, true);
				this.ldapFilter.Append(")");
				return;
			}
			this.ConvertExistsFilterInternal(existsFilter, false);
		}

		private void ConvertExistsFilterInternal(ExistsFilter existsFilter, bool isSoftLink)
		{
			ADPropertyDefinition adpropertyDefinition = (ADPropertyDefinition)existsFilter.Property;
			string ldapDisplayName = adpropertyDefinition.LdapDisplayName;
			if (isSoftLink && adpropertyDefinition.IsSoftLinkAttribute)
			{
				ldapDisplayName = adpropertyDefinition.SoftLinkShadowProperty.LdapDisplayName;
			}
			this.IncrementFilterSize();
			this.ldapFilter.Append("(");
			this.ldapFilter.Append(ldapDisplayName);
			this.ldapFilter.Append("=*)");
		}

		private void ConvertTextFilter(TextFilter textFilter)
		{
			if (textFilter.Property.Type.IsEnum)
			{
				throw new ADFilterException(DirectoryStrings.ExceptionUnsupportedOperatorForProperty(textFilter.Property.Name, "LIKE"));
			}
			ADPropertyDefinition adpropertyDefinition = (ADPropertyDefinition)textFilter.Property;
			string ldapDisplayName = adpropertyDefinition.LdapDisplayName;
			this.IncrementFilterSize();
			this.ldapFilter.Append("(");
			this.ldapFilter.Append(ldapDisplayName);
			this.ldapFilter.Append("=");
			switch (textFilter.MatchOptions)
			{
			case MatchOptions.FullString:
				ADValueConvertor.ConvertToAndAppendFilterString(adpropertyDefinition, textFilter.Text, this.ldapFilter, false, 0);
				this.ldapFilter.Append(")");
				return;
			case MatchOptions.SubString:
				this.ldapFilter.Append("*");
				ADValueConvertor.ConvertToAndAppendFilterString(adpropertyDefinition, textFilter.Text, this.ldapFilter, false, 0);
				this.ldapFilter.Append("*)");
				return;
			case MatchOptions.Prefix:
				ADValueConvertor.ConvertToAndAppendFilterString(adpropertyDefinition, textFilter.Text, this.ldapFilter, false, 0);
				this.ldapFilter.Append("*)");
				return;
			case MatchOptions.Suffix:
				this.ldapFilter.Append("*");
				ADValueConvertor.ConvertToAndAppendFilterString(adpropertyDefinition, textFilter.Text, this.ldapFilter, false, 0);
				this.ldapFilter.Append(")");
				return;
			case MatchOptions.WildcardString:
				ADValueConvertor.EscapeAndAppendString(textFilter.Text, this.ldapFilter, true);
				this.ldapFilter.Append(")");
				return;
			}
			throw new ADFilterException(DirectoryStrings.ExceptionUnsupportedTextFilterOption(textFilter.MatchOptions.ToString()));
		}

		private void ConvertInChainFilter(InChainFilter inChainFilter)
		{
			ADPropertyDefinition adpropertyDefinition = (ADPropertyDefinition)inChainFilter.Property;
			string ldapDisplayName = adpropertyDefinition.LdapDisplayName;
			this.IncrementFilterSize();
			this.ldapFilter.Append("(");
			this.ldapFilter.Append(ldapDisplayName);
			this.ldapFilter.Append(":1.2.840.113556.1.4.1941:=");
			ADValueConvertor.ConvertToAndAppendFilterString(adpropertyDefinition, inChainFilter.Value, this.ldapFilter, false, 0);
			this.ldapFilter.Append(")");
		}

		private void ConvertBitMaskFilter(GenericBitMaskFilter bitmaskFilter)
		{
			if (bitmaskFilter.GetType() == typeof(BitMaskFilter))
			{
				throw new ADFilterException(DirectoryStrings.ExceptionUnsupportedFilter(bitmaskFilter.GetType().ToString()));
			}
			ADPropertyDefinition adpropertyDefinition = (ADPropertyDefinition)bitmaskFilter.Property;
			string ldapDisplayName = adpropertyDefinition.LdapDisplayName;
			this.IncrementFilterSize();
			this.ldapFilter.Append("(");
			this.ldapFilter.Append(ldapDisplayName);
			if (bitmaskFilter is BitMaskAndFilter)
			{
				this.ldapFilter.Append(":1.2.840.113556.1.4.803:=");
			}
			else
			{
				this.ldapFilter.Append(":1.2.840.113556.1.4.804:=");
			}
			this.ldapFilter.Append(bitmaskFilter.Mask.ToString());
			this.ldapFilter.Append(")");
		}

		private void ConvertNotFilter(NotFilter notFilter)
		{
			this.IncrementFilterSize();
			this.ldapFilter.Append("(!");
			this.BuildLdapFilter(notFilter.Filter, null);
			this.ldapFilter.Append(")");
		}

		private void ConvertAnrFilter(AmbiguousNameResolutionFilter anrFilter)
		{
			this.IncrementFilterSize();
			this.ldapFilter.Append("(anr=");
			ADValueConvertor.EscapeAndAppendString(anrFilter.ValueToMatch, this.ldapFilter);
			this.ldapFilter.Append(")");
		}

		private void ConvertCompositeFilter(CompositeFilter compositeFilter, QueryFilter parentFilter)
		{
			if (compositeFilter.FilterCount == 0)
			{
				return;
			}
			bool flag = compositeFilter is AndFilter;
			if (!flag && compositeFilter.FilterCount > 1 && Filters.IsEqualityFilterOnPropertyDefinition(compositeFilter.Filters[0], new PropertyDefinition[]
			{
				ADRecipientSchema.RecipientTypeDetails
			}))
			{
				RecipientTypeDetails recipientTypeDetails = RecipientTypeDetails.None;
				foreach (QueryFilter queryFilter in compositeFilter.Filters)
				{
					if (!Filters.IsEqualityFilterOnPropertyDefinition(queryFilter, new PropertyDefinition[]
					{
						ADRecipientSchema.RecipientTypeDetails
					}))
					{
						recipientTypeDetails = RecipientTypeDetails.None;
						break;
					}
					recipientTypeDetails |= (RecipientTypeDetails)ADObject.PropertyValueFromEqualityFilter((ComparisonFilter)queryFilter);
				}
				if (recipientTypeDetails != RecipientTypeDetails.None)
				{
					this.ConvertSinglePropertyFilter(new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.RecipientTypeDetails, recipientTypeDetails), parentFilter);
					return;
				}
			}
			if (!flag && Filters.IsOrFilterOnPropertyDefinitionComparisons(compositeFilter, new PropertyDefinition[]
			{
				ADRecipientSchema.RecipientType
			}))
			{
				QueryFilter queryFilter2 = Filters.OptimizeRecipientTypeFilter((OrFilter)compositeFilter);
				if (queryFilter2 != compositeFilter)
				{
					ExTraceGlobals.LdapFilterBuilderTracer.TraceDebug<QueryFilter, CompositeFilter>(0L, "LdapFilterBuilder::ConvertCompositeFilter:  Filters.OptimizeRecipientTypeFilter found an optimized filter. Will use {0} instead of {1}", queryFilter2, compositeFilter);
					this.BuildLdapFilter(queryFilter2, parentFilter);
					return;
				}
			}
			bool flag2 = false;
			if (parentFilter != null)
			{
				if (parentFilter is AndFilter)
				{
					flag2 = flag;
				}
				else
				{
					flag2 = !flag;
				}
			}
			if (!flag2)
			{
				this.IncrementFilterSize();
				this.ldapFilter.Append("(");
				if (flag)
				{
					this.ldapFilter.Append("&");
				}
				else
				{
					this.ldapFilter.Append("|");
				}
			}
			if (flag)
			{
				this.CheckVirtualIndexOnIdOrAbMembership(compositeFilter);
			}
			foreach (QueryFilter queryFilter3 in compositeFilter.Filters)
			{
				this.BuildLdapFilter(queryFilter3, compositeFilter);
			}
			if (!flag2)
			{
				this.ldapFilter.Append(")");
			}
		}

		private void IncrementFilterSize()
		{
			this.IncrementFilterSize(1);
		}

		private void IncrementFilterSize(int count)
		{
			this.filterSize += count;
			if (this.filterSize > LdapFilterBuilder.MaxFilterTreeSize)
			{
				throw new ADFilterException(DirectoryStrings.InvalidFilterSize(LdapFilterBuilder.MaxFilterTreeSize));
			}
		}

		private void CheckVirtualIndexOnIdOrAbMembership(CompositeFilter filter)
		{
			if (this.skipCheckVirtualIndex)
			{
				return;
			}
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			Guid a = Guid.Empty;
			string text = string.Empty;
			foreach (QueryFilter queryFilter in filter.Filters)
			{
				if (queryFilter is OrFilter)
				{
					flag = true;
				}
				else
				{
					ComparisonFilter comparisonFilter = queryFilter as ComparisonFilter;
					if (comparisonFilter != null)
					{
						if (comparisonFilter.ComparisonOperator == ComparisonOperator.Equal)
						{
							if (comparisonFilter.Property == ADObjectSchema.Id)
							{
								ADObjectId adobjectId = (ADObjectId)comparisonFilter.PropertyValue;
								a = adobjectId.ObjectGuid;
								text = adobjectId.ToDNString();
							}
							else if (comparisonFilter.Property == ADObjectSchema.Guid)
							{
								a = (Guid)comparisonFilter.PropertyValue;
							}
							else if (comparisonFilter.Property == ADObjectSchema.DistinguishedName)
							{
								text = (string)comparisonFilter.PropertyValue;
							}
							else if (comparisonFilter.Property == ADRecipientSchema.AddressListMembership)
							{
								flag2 = true;
							}
							else if (comparisonFilter.Property == ADRecipientSchema.DisplayName)
							{
								flag3 = true;
							}
						}
					}
					else
					{
						ExistsFilter existsFilter = queryFilter as ExistsFilter;
						if (existsFilter != null)
						{
							if (existsFilter.Property == ADRecipientSchema.DisplayName)
							{
								flag3 = true;
							}
						}
						else if (queryFilter is AmbiguousNameResolutionFilter)
						{
							flag4 = true;
						}
					}
				}
			}
			if (flag2 && !flag3 && !flag4 && (text != string.Empty || a != Guid.Empty || flag))
			{
				this.ldapFilter.Append("(displayName=*)");
			}
			if (flag)
			{
				if (text != string.Empty && !text.Contains(",CN=DeletedObjects,"))
				{
					this.ldapFilter.Append("(objectCategory=*)");
					return;
				}
				if (a != Guid.Empty)
				{
					this.ldapFilter.Append("(objectCategory=*)");
				}
			}
		}

		private const long MaxFilterStringLength = 128000L;

		public static readonly int MaxFilterTreeSize = 250;

		public static readonly int MaxCustomFilterTreeSize = 100;

		public static int EstimatedFilterStringSize = 1024;

		private readonly SoftLinkMode softLinkMode = SoftLinkMode.Disabled;

		private readonly bool tenantScoped;

		private bool skipCheckVirtualIndex;

		private int filterSize;

		private StringBuilder ldapFilter;

		private bool containsUnsafeIdentity;
	}
}
