using System;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.RpcClientAccess.Handler
{
	internal class FilterRestrictionTranslator
	{
		public FilterRestrictionTranslator(StoreSession session)
		{
			this.session = session;
		}

		public Restriction Translate(QueryFilter filter, bool useUnicodeType)
		{
			if (filter is CompositeFilter)
			{
				CompositeFilter compositeFilter = (CompositeFilter)filter;
				Restriction[] array = new Restriction[compositeFilter.Filters.Count];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = this.Translate(compositeFilter.Filters[i], useUnicodeType);
				}
				if (filter is AndFilter)
				{
					return new AndRestriction(array);
				}
				if (filter is OrFilter)
				{
					return new OrRestriction(array);
				}
			}
			else
			{
				if (filter is NotFilter)
				{
					NotFilter notFilter = (NotFilter)filter;
					return new NotRestriction(this.Translate(notFilter.Filter, useUnicodeType));
				}
				if (filter is CommentFilter)
				{
					CommentFilter commentFilter = (CommentFilter)filter;
					Restriction childRestriction = this.Translate(commentFilter.Filter, useUnicodeType);
					PropertyValue[] array2 = new PropertyValue[commentFilter.Properties.Length];
					for (int j = 0; j < commentFilter.Properties.Length; j++)
					{
						array2[j] = new PropertyValue(this.PropertyTagFromPropertyDefinition(commentFilter.Properties[j], useUnicodeType), commentFilter.Values[j]);
					}
					return new CommentRestriction(array2, childRestriction);
				}
				if (filter is NearFilter)
				{
					throw new RopExecutionException(string.Format("Unsupported near filter found: {0}", filter), (ErrorCode)2147746050U);
				}
				if (filter is SizeFilter)
				{
					SizeFilter sizeFilter = (SizeFilter)filter;
					PropertyTag propertyTag = this.PropertyTagFromPropertyDefinition(sizeFilter.Property, useUnicodeType);
					uint num = sizeFilter.PropertySize;
					if (propertyTag.ElementPropertyType == PropertyType.String8)
					{
						num /= 2U;
					}
					return new SizeRestriction(FilterRestrictionTranslator.FromComparisonOperator(sizeFilter.ComparisonOperator), propertyTag, num);
				}
				if (filter is CountFilter)
				{
					CountFilter countFilter = (CountFilter)filter;
					Restriction childRestriction2 = this.Translate(countFilter.Filter, useUnicodeType);
					return new CountRestriction(countFilter.Count, childRestriction2);
				}
				if (filter is SubFilter)
				{
					SubFilter subFilter = (SubFilter)filter;
					Restriction childRestriction3 = this.Translate(subFilter.Filter, useUnicodeType);
					return new SubRestriction(FilterRestrictionTranslator.FromSubFilterProperties(subFilter.SubFilterProperty), childRestriction3);
				}
				if (filter is PropertyComparisonFilter)
				{
					PropertyComparisonFilter propertyComparisonFilter = (PropertyComparisonFilter)filter;
					return new ComparePropsRestriction(FilterRestrictionTranslator.FromComparisonOperator(propertyComparisonFilter.ComparisonOperator), this.PropertyTagFromPropertyDefinition(propertyComparisonFilter.Property1, useUnicodeType), this.PropertyTagFromPropertyDefinition(propertyComparisonFilter.Property2, useUnicodeType));
				}
				if (filter is TrueFilter)
				{
					return new TrueRestriction();
				}
				if (filter is FalseFilter)
				{
					return new FalseRestriction();
				}
				if (filter is SinglePropertyFilter)
				{
					return this.TranslateSinglePropertyFilter((SinglePropertyFilter)filter, useUnicodeType);
				}
				if (filter is NullFilter)
				{
					return NullRestriction.Instance;
				}
			}
			throw new RopExecutionException(string.Format("Unknown filter type found: {0}", filter), (ErrorCode)2147746050U);
		}

		public QueryFilter Translate(Restriction restriction)
		{
			return this.TranslateInternal(restriction, 0);
		}

		private static SubFilterProperties FromSubRestrictionType(SubRestrictionType subRestrictionType)
		{
			if (subRestrictionType == SubRestrictionType.Recipients)
			{
				return SubFilterProperties.Recipients;
			}
			if (subRestrictionType == SubRestrictionType.Attachments)
			{
				return SubFilterProperties.Attachments;
			}
			throw new NotSupportedException(string.Format("The flag is not supported which should have been caught by the Validate method. subRestrictionType = {0}.", subRestrictionType));
		}

		private static SubRestrictionType FromSubFilterProperties(SubFilterProperties subFilterProperties)
		{
			switch (subFilterProperties)
			{
			case SubFilterProperties.Attachments:
				return SubRestrictionType.Attachments;
			case SubFilterProperties.Recipients:
				return SubRestrictionType.Recipients;
			default:
				throw new NotSupportedException(string.Format("The flag is not supported which should have been caught by the Validate method. subFilterProperties = {0}.", subFilterProperties));
			}
		}

		private static ComparisonOperator FromRelationOperator(RelationOperator relop)
		{
			switch (relop)
			{
			case RelationOperator.LessThan:
				return ComparisonOperator.LessThan;
			case RelationOperator.LessThanEquals:
				return ComparisonOperator.LessThanOrEqual;
			case RelationOperator.GreaterThan:
				return ComparisonOperator.GreaterThan;
			case RelationOperator.GreaterThanEquals:
				return ComparisonOperator.GreaterThanOrEqual;
			case RelationOperator.Equals:
				return ComparisonOperator.Equal;
			case RelationOperator.NotEquals:
				return ComparisonOperator.NotEqual;
			case RelationOperator.RegularExpression:
				return ComparisonOperator.Like;
			default:
				if (relop != RelationOperator.MemberOfDistributionList)
				{
					throw new NotSupportedException(string.Format("The relop is not supported. relop = {0}.", relop));
				}
				return ComparisonOperator.IsMemberOf;
			}
		}

		private static RelationOperator FromComparisonOperator(ComparisonOperator comparisonOperator)
		{
			switch (comparisonOperator)
			{
			case ComparisonOperator.Equal:
				return RelationOperator.Equals;
			case ComparisonOperator.NotEqual:
				return RelationOperator.NotEquals;
			case ComparisonOperator.LessThan:
				return RelationOperator.LessThan;
			case ComparisonOperator.LessThanOrEqual:
				return RelationOperator.LessThanEquals;
			case ComparisonOperator.GreaterThan:
				return RelationOperator.GreaterThan;
			case ComparisonOperator.GreaterThanOrEqual:
				return RelationOperator.GreaterThanEquals;
			case ComparisonOperator.Like:
				return RelationOperator.RegularExpression;
			case ComparisonOperator.IsMemberOf:
				return RelationOperator.MemberOfDistributionList;
			default:
				throw new NotSupportedException(string.Format("The comparisonOperator is not supported. comparisonOperator = {0}.", comparisonOperator));
			}
		}

		private static void FromFuzzyLevel(ContentRestriction contentRestriction, out MatchOptions matchOptions, out MatchFlags matchFlags)
		{
			matchOptions = MatchOptions.FullString;
			matchFlags = MatchFlags.Default;
			if ((contentRestriction.FuzzyLevel & FuzzyLevel.SubString) == FuzzyLevel.SubString)
			{
				if ((contentRestriction.FuzzyLevel & FuzzyLevel.PrefixOnWords) == FuzzyLevel.PrefixOnWords)
				{
					matchOptions = MatchOptions.PrefixOnWords;
				}
				else if ((contentRestriction.FuzzyLevel & FuzzyLevel.ExactPhrase) == FuzzyLevel.ExactPhrase)
				{
					matchOptions = MatchOptions.ExactPhrase;
				}
				else
				{
					matchOptions = MatchOptions.SubString;
				}
			}
			else if ((contentRestriction.FuzzyLevel & FuzzyLevel.Prefix) == FuzzyLevel.Prefix)
			{
				matchOptions = MatchOptions.Prefix;
			}
			if ((contentRestriction.FuzzyLevel & FuzzyLevel.IgnoreCase) == FuzzyLevel.IgnoreCase)
			{
				matchFlags |= MatchFlags.IgnoreCase;
			}
			if ((contentRestriction.FuzzyLevel & FuzzyLevel.IgnoreNonSpace) == FuzzyLevel.IgnoreNonSpace)
			{
				matchFlags |= MatchFlags.IgnoreNonSpace;
			}
			if ((contentRestriction.FuzzyLevel & FuzzyLevel.Loose) == FuzzyLevel.Loose)
			{
				matchFlags |= MatchFlags.Loose;
			}
		}

		private static FuzzyLevel FromMatchOptionFlags(MatchOptions matchOptions, MatchFlags matchFlags)
		{
			FuzzyLevel fuzzyLevel;
			switch (matchOptions)
			{
			case MatchOptions.FullString:
				fuzzyLevel = FuzzyLevel.FullString;
				goto IL_4E;
			case MatchOptions.SubString:
				fuzzyLevel = FuzzyLevel.SubString;
				goto IL_4E;
			case MatchOptions.Prefix:
				fuzzyLevel = FuzzyLevel.Prefix;
				goto IL_4E;
			case MatchOptions.PrefixOnWords:
				fuzzyLevel = (FuzzyLevel.SubString | FuzzyLevel.PrefixOnWords);
				goto IL_4E;
			case MatchOptions.ExactPhrase:
				fuzzyLevel = (FuzzyLevel.SubString | FuzzyLevel.ExactPhrase);
				goto IL_4E;
			}
			throw new NotSupportedException(string.Format("MatchOptions is not supported. matchOptions = {0}.", matchOptions));
			IL_4E:
			if ((matchFlags & MatchFlags.IgnoreCase) == MatchFlags.IgnoreCase)
			{
				fuzzyLevel |= FuzzyLevel.IgnoreCase;
			}
			if ((matchFlags & MatchFlags.IgnoreNonSpace) == MatchFlags.IgnoreNonSpace)
			{
				fuzzyLevel |= FuzzyLevel.IgnoreNonSpace;
			}
			if ((matchFlags & MatchFlags.Loose) == MatchFlags.Loose)
			{
				fuzzyLevel |= FuzzyLevel.Loose;
			}
			return fuzzyLevel;
		}

		private Restriction TranslateSinglePropertyFilter(SinglePropertyFilter filter, bool useUnicodeType)
		{
			if (!(filter.Property is NativeStorePropertyDefinition))
			{
				throw new NotSupportedException("This is programming error. I should have disabled smart filter conversion.");
			}
			if (filter is BitMaskFilter)
			{
				BitMaskFilter bitMaskFilter = filter as BitMaskFilter;
				return new BitMaskRestriction(bitMaskFilter.IsNonZero ? BitMaskOperator.NotEquals : BitMaskOperator.Equals, this.PropertyTagFromPropertyDefinition(bitMaskFilter.Property, useUnicodeType), (uint)bitMaskFilter.Mask);
			}
			if (filter is ExistsFilter)
			{
				ExistsFilter existsFilter = filter as ExistsFilter;
				return new ExistsRestriction(this.PropertyTagFromPropertyDefinition(existsFilter.Property, useUnicodeType));
			}
			if (filter is ComparisonFilter)
			{
				ComparisonFilter comparisonFilter = filter as ComparisonFilter;
				PropertyTag propertyTag = this.PropertyTagFromPropertyDefinition(comparisonFilter.Property, useUnicodeType);
				PropertyTag propertyTag2 = propertyTag;
				if (propertyTag.IsMultiValuedProperty)
				{
					propertyTag2 = new PropertyTag(propertyTag.PropertyId, propertyTag.ElementPropertyType);
				}
				return new PropertyRestriction(FilterRestrictionTranslator.FromComparisonOperator(comparisonFilter.ComparisonOperator), propertyTag, new PropertyValue?(new PropertyValue(propertyTag2, comparisonFilter.PropertyValue)));
			}
			if (filter is ContentFilter)
			{
				ContentFilter contentFilter = filter as ContentFilter;
				PropertyTag propertyTag3 = this.PropertyTagFromPropertyDefinition(contentFilter.Property, useUnicodeType);
				PropertyTag propertyTag4;
				if (propertyTag3.IsMultiValuedProperty)
				{
					propertyTag4 = new PropertyTag(propertyTag3.PropertyId, propertyTag3.ElementPropertyType);
				}
				else
				{
					propertyTag4 = propertyTag3;
				}
				object value;
				if (filter is TextFilter)
				{
					value = ((TextFilter)filter).Text;
				}
				else
				{
					value = ((BinaryFilter)filter).BinaryData;
				}
				return new ContentRestriction(FilterRestrictionTranslator.FromMatchOptionFlags(contentFilter.MatchOptions, contentFilter.MatchFlags), propertyTag3, new PropertyValue?(new PropertyValue(propertyTag4, value)));
			}
			throw new RopExecutionException(string.Format("Filter type not supported: {0}.", filter), (ErrorCode)2147746050U);
		}

		private PropertyTag PropertyTagFromPropertyDefinition(PropertyDefinition property, bool useUnicodeType)
		{
			return MEDSPropertyTranslator.PropertyTagsFromPropertyDefinitions<PropertyDefinition>(this.session, new PropertyDefinition[]
			{
				property
			}, useUnicodeType).First<PropertyTag>();
		}

		private NativeStorePropertyDefinition PropertyDefinitionFromPropertyTag(PropertyTag propertyTag)
		{
			return MEDSPropertyTranslator.PropertyDefinitionFromPropertyTag(this.session.Mailbox.CoreObject, propertyTag);
		}

		private QueryFilter TranslateInternal(Restriction restriction, int depth)
		{
			if (depth > 256)
			{
				throw new RopExecutionException("Restriction is too deep.", (ErrorCode)2147746071U);
			}
			ErrorCode errorCode = restriction.Validate();
			if (errorCode != ErrorCode.None)
			{
				throw new RopExecutionException(string.Format("Invalid restriction found: {0}.", restriction), errorCode);
			}
			if (restriction.RestrictionType == RestrictionType.Property)
			{
				PropertyRestriction propertyRestriction = (PropertyRestriction)restriction;
				return new ComparisonFilter(FilterRestrictionTranslator.FromRelationOperator(propertyRestriction.RelationOperator), this.PropertyDefinitionFromPropertyTag(propertyRestriction.PropertyTag), MEDSPropertyTranslator.TranslatePropertyValue(this.session, propertyRestriction.PropertyValue.Value));
			}
			if (restriction.RestrictionType == RestrictionType.Not || restriction.RestrictionType == RestrictionType.Near || restriction.RestrictionType == RestrictionType.Comment || restriction.RestrictionType == RestrictionType.Count || restriction.RestrictionType == RestrictionType.SubRestriction)
			{
				SingleRestriction singleRestriction = (SingleRestriction)restriction;
				QueryFilter queryFilter = this.TranslateInternal(singleRestriction.ChildRestriction, depth + 1);
				if (restriction.RestrictionType == RestrictionType.Not)
				{
					return new NotFilter(queryFilter);
				}
				if (restriction.RestrictionType == RestrictionType.Comment)
				{
					CommentRestriction commentRestriction = (CommentRestriction)restriction;
					NativeStorePropertyDefinition[] array = new NativeStorePropertyDefinition[commentRestriction.PropertyValues.Length];
					object[] array2 = new object[commentRestriction.PropertyValues.Length];
					for (int i = 0; i < array.Length; i++)
					{
						array[i] = this.PropertyDefinitionFromPropertyTag(commentRestriction.PropertyValues[i].PropertyTag);
						array2[i] = MEDSPropertyTranslator.TranslatePropertyValue(this.session, commentRestriction.PropertyValues[i]);
					}
					return new CommentFilter(array, array2, queryFilter);
				}
				if (restriction.RestrictionType == RestrictionType.Count)
				{
					CountRestriction countRestriction = (CountRestriction)restriction;
					return new CountFilter(countRestriction.Count, queryFilter);
				}
				if (restriction.RestrictionType == RestrictionType.Near)
				{
					NearRestriction nearRestriction = (NearRestriction)restriction;
					return new NearFilter(nearRestriction.Distance, nearRestriction.Ordered, queryFilter as AndFilter);
				}
				if (restriction.RestrictionType == RestrictionType.SubRestriction)
				{
					SubRestriction subRestriction = (SubRestriction)restriction;
					return new SubFilter(FilterRestrictionTranslator.FromSubRestrictionType(subRestriction.SubRestrictionType), queryFilter);
				}
			}
			else
			{
				if (restriction.RestrictionType == RestrictionType.Exists)
				{
					ExistsRestriction existsRestriction = (ExistsRestriction)restriction;
					return new ExistsFilter(this.PropertyDefinitionFromPropertyTag(existsRestriction.PropertyTag));
				}
				if (restriction.RestrictionType == RestrictionType.Content)
				{
					ContentRestriction contentRestriction = (ContentRestriction)restriction;
					MatchOptions matchOptions;
					MatchFlags matchFlags;
					FilterRestrictionTranslator.FromFuzzyLevel(contentRestriction, out matchOptions, out matchFlags);
					PropertyDefinition property = this.PropertyDefinitionFromPropertyTag(contentRestriction.PropertyTag);
					if (contentRestriction.PropertyValue.Value.PropertyTag.PropertyType == PropertyType.Binary)
					{
						return new BinaryFilter(property, contentRestriction.PropertyValue.Value.GetValueAssert<byte[]>(), matchOptions, matchFlags);
					}
					if (contentRestriction.PropertyValue.Value.PropertyTag.IsStringProperty)
					{
						return new TextFilter(property, contentRestriction.PropertyValue.Value.GetValueAssert<string>(), matchOptions, matchFlags);
					}
				}
				else if (restriction.RestrictionType == RestrictionType.BitMask)
				{
					BitMaskRestriction bitMaskRestriction = (BitMaskRestriction)restriction;
					bool flag = bitMaskRestriction.BitMaskOperator == BitMaskOperator.Equals || bitMaskRestriction.BitMaskOperator == BitMaskOperator.NotEquals;
					bool flag2 = bitMaskRestriction.PropertyTag.PropertyType == PropertyType.Int32;
					if (flag && flag2)
					{
						return new BitMaskFilter(this.PropertyDefinitionFromPropertyTag(bitMaskRestriction.PropertyTag), (ulong)bitMaskRestriction.BitMask, bitMaskRestriction.BitMaskOperator == BitMaskOperator.NotEquals);
					}
				}
				else if (restriction.RestrictionType == RestrictionType.And || restriction.RestrictionType == RestrictionType.Or)
				{
					CompositeRestriction compositeRestriction = (CompositeRestriction)restriction;
					QueryFilter[] array3 = new QueryFilter[compositeRestriction.ChildRestrictions.Length];
					for (int j = 0; j < array3.Length; j++)
					{
						array3[j] = this.TranslateInternal(compositeRestriction.ChildRestrictions[j], depth + 1);
					}
					if (restriction.RestrictionType == RestrictionType.And)
					{
						return new AndFilter(array3);
					}
					if (restriction.RestrictionType == RestrictionType.Or)
					{
						return new OrFilter(array3);
					}
				}
				else
				{
					if (restriction.RestrictionType == RestrictionType.CompareProps)
					{
						ComparePropsRestriction comparePropsRestriction = (ComparePropsRestriction)restriction;
						return new PropertyComparisonFilter(FilterRestrictionTranslator.FromRelationOperator(comparePropsRestriction.RelationOperator), this.PropertyDefinitionFromPropertyTag(comparePropsRestriction.Property1), this.PropertyDefinitionFromPropertyTag(comparePropsRestriction.Property2));
					}
					if (restriction.RestrictionType == RestrictionType.Size)
					{
						SizeRestriction sizeRestriction = (SizeRestriction)restriction;
						uint num = sizeRestriction.Size;
						if (sizeRestriction.PropertyTag.ElementPropertyType == PropertyType.String8)
						{
							num *= 2U;
						}
						return new SizeFilter(FilterRestrictionTranslator.FromRelationOperator(sizeRestriction.RelationOperator), this.PropertyDefinitionFromPropertyTag(sizeRestriction.PropertyTag), num);
					}
					if (restriction.RestrictionType == RestrictionType.True)
					{
						return new TrueFilter();
					}
					if (restriction.RestrictionType == RestrictionType.False)
					{
						return new FalseFilter();
					}
					if (restriction.RestrictionType == RestrictionType.Null)
					{
						return new NullFilter();
					}
				}
			}
			throw new RopExecutionException(string.Format("Invalid restriction type found: {0}", restriction.RestrictionType), (ErrorCode)2147746050U);
		}

		private readonly StoreSession session;
	}
}
