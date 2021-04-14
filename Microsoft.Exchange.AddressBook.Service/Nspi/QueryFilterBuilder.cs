using System;
using System.Text;
using Microsoft.Exchange.AddressBook.Service;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Nspi;
using Microsoft.Mapi;

namespace Microsoft.Exchange.AddressBook.Nspi
{
	internal class QueryFilterBuilder
	{
		internal QueryFilterBuilder(Encoding encoding)
		{
			this.encoding = encoding;
		}

		internal static bool IsAnrRestriction(Restriction restriction)
		{
			return restriction.Type == Restriction.ResType.Property && ((Restriction.PropertyRestriction)restriction).PropTag.Id() == PropTag.Anr.Id();
		}

		internal static QueryFilter RestrictToVisibleItems(QueryFilter filter, string legacyDN)
		{
			QueryFilter queryFilter = new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.LegacyExchangeDN, legacyDN);
			QueryFilter queryFilter2 = new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.RecipientTypeDetails, RecipientTypeDetails.DiscoveryMailbox);
			return new AndFilter(new QueryFilter[]
			{
				QueryFilterBuilder.DisplayNameExists,
				new OrFilter(new QueryFilter[]
				{
					QueryFilterBuilder.NotHiddenFromAddressLists,
					queryFilter,
					queryFilter2
				}),
				filter
			});
		}

		internal QueryFilter TranslateRestriction(Restriction restriction)
		{
			return this.TranslateInternal(restriction, 0);
		}

		internal QueryFilter TranslateANR(Restriction restriction, string legacyDN, ADObjectId addressListScope)
		{
			return this.TranslateANR(this.TranslateRestriction(restriction), legacyDN, addressListScope);
		}

		internal QueryFilter TranslateANR(string name, string legacyDN, ADObjectId addressListScope)
		{
			return this.TranslateANR(new AmbiguousNameResolutionFilter(name), legacyDN, addressListScope);
		}

		private static ComparisonOperator ConvertRelOpToComparisonOperator(Restriction.RelOp relOp)
		{
			switch (relOp)
			{
			case Restriction.RelOp.LessThan:
				return ComparisonOperator.LessThan;
			case Restriction.RelOp.LessThanOrEqual:
				return ComparisonOperator.LessThanOrEqual;
			case Restriction.RelOp.GreaterThan:
				return ComparisonOperator.GreaterThan;
			case Restriction.RelOp.GreaterThanOrEqual:
				return ComparisonOperator.GreaterThanOrEqual;
			case Restriction.RelOp.Equal:
				return ComparisonOperator.Equal;
			case Restriction.RelOp.NotEqual:
				return ComparisonOperator.NotEqual;
			case Restriction.RelOp.RegularExpression:
				break;
			default:
				switch (relOp)
				{
				case Restriction.RelOp.Include:
				case Restriction.RelOp.Exclude:
					break;
				default:
					if (relOp != Restriction.RelOp.MemberOfDL)
					{
					}
					break;
				}
				break;
			}
			throw new NspiException(NspiStatus.TooComplex, "Restriction.RelOp is not supported");
		}

		private static PropertyDefinition PropTagToPropertyDefinition(PropTag tag)
		{
			PropertyDefinition propertyDefinition = NspiPropMapper.GetPropertyDefinition(tag);
			if (propertyDefinition == null)
			{
				throw new NspiException(NspiStatus.TooComplex, "Unknown proptag");
			}
			return propertyDefinition;
		}

		private QueryFilter TranslateANR(QueryFilter filter, string legacyDN, ADObjectId addressListScope)
		{
			QueryFilter queryFilter = new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.AddressListMembership, addressListScope);
			QueryFilter queryFilter2 = new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.LegacyExchangeDN, legacyDN);
			QueryFilter queryFilter3 = new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.RecipientTypeDetails, RecipientTypeDetails.DiscoveryMailbox);
			return new AndFilter(new QueryFilter[]
			{
				filter,
				new OrFilter(new QueryFilter[]
				{
					queryFilter,
					queryFilter2,
					queryFilter3
				})
			});
		}

		private QueryFilter TranslateInternal(Restriction restriction, int depth)
		{
			if (restriction == null)
			{
				throw new NspiException(NspiStatus.TooComplex, "Null restriction");
			}
			if (depth > 256)
			{
				throw new NspiException(NspiStatus.TooComplex, "Restriction is too deep");
			}
			switch (restriction.Type)
			{
			case Restriction.ResType.And:
			case Restriction.ResType.Or:
				return this.TranslateAndOrRestriction((Restriction.AndOrNotRestriction)restriction, depth + 1);
			case Restriction.ResType.Not:
			{
				Restriction.NotRestriction notRestriction = (Restriction.NotRestriction)restriction;
				return new NotFilter(this.TranslateInternal(notRestriction.Restriction, depth + 1));
			}
			case Restriction.ResType.Content:
				return this.TranslateContentRestriction((Restriction.ContentRestriction)restriction);
			case Restriction.ResType.Property:
				return this.TranslatePropertyRestriction((Restriction.PropertyRestriction)restriction);
			case Restriction.ResType.Exist:
			{
				Restriction.ExistRestriction existRestriction = (Restriction.ExistRestriction)restriction;
				return new ExistsFilter(QueryFilterBuilder.PropTagToPropertyDefinition(existRestriction.Tag));
			}
			}
			throw new NspiException(NspiStatus.TooComplex, "Restriction.Type is not supported");
		}

		private QueryFilter TranslateAndOrRestriction(Restriction.AndOrNotRestriction restriction, int depth)
		{
			Restriction[] restrictions;
			if (restriction.Type == Restriction.ResType.And)
			{
				restrictions = ((Restriction.AndRestriction)restriction).Restrictions;
			}
			else
			{
				restrictions = ((Restriction.OrRestriction)restriction).Restrictions;
			}
			QueryFilter[] array = new QueryFilter[restrictions.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = this.TranslateInternal(restrictions[i], depth + 1);
			}
			if (restriction.Type == Restriction.ResType.And)
			{
				return new AndFilter(array);
			}
			return new OrFilter(array);
		}

		private QueryFilter TranslateContentRestriction(Restriction.ContentRestriction restriction)
		{
			MatchOptions matchOptions;
			if ((restriction.Flags & ContentFlags.SubString) == ContentFlags.SubString)
			{
				matchOptions = MatchOptions.SubString;
			}
			else
			{
				if ((restriction.Flags & ContentFlags.Prefix) != ContentFlags.Prefix)
				{
					throw new NspiException(NspiStatus.TooComplex, "restriction.Flags not SubString or Prefix");
				}
				matchOptions = MatchOptions.Prefix;
			}
			object obj = this.ConvertValue(restriction.PropValue);
			string text = obj as string;
			if (text != null)
			{
				return new TextFilter(QueryFilterBuilder.PropTagToPropertyDefinition(restriction.PropTag), text, matchOptions, MatchFlags.Default);
			}
			byte[] array = obj as byte[];
			if (array != null)
			{
				return new BinaryFilter(QueryFilterBuilder.PropTagToPropertyDefinition(restriction.PropTag), array, matchOptions, MatchFlags.Default);
			}
			throw new NspiException(NspiStatus.TooComplex, "Unknown PropValue");
		}

		private QueryFilter TranslatePropertyRestriction(Restriction.PropertyRestriction restriction)
		{
			object obj = this.ConvertValue(restriction.PropValue);
			if (restriction.PropTag.Id() == PropTag.Anr.Id())
			{
				return new AmbiguousNameResolutionFilter((string)obj);
			}
			if (restriction.PropTag.Id() == PropTag.AddrType.Id())
			{
				return new TrueFilter();
			}
			return new ComparisonFilter(QueryFilterBuilder.ConvertRelOpToComparisonOperator(restriction.Op), QueryFilterBuilder.PropTagToPropertyDefinition(restriction.PropTag), obj);
		}

		private object ConvertValue(PropValue propValue)
		{
			if ((propValue.PropType & PropType.MultiValueFlag) == PropType.MultiValueFlag)
			{
				object[] array = propValue.Value as object[];
				if (array == null || array.Length != 1)
				{
					throw new NspiException(NspiStatus.TooComplex, "Only one value permitted with multivalue");
				}
				if (propValue.PropType == PropType.AnsiStringArray && propValue.Value is byte[][])
				{
					byte[][] array2 = (byte[][])propValue.Value;
					return this.encoding.GetString(array2[0]);
				}
				return array[0];
			}
			else
			{
				if (propValue.PropType == PropType.AnsiString && propValue.Value is byte[])
				{
					return this.encoding.GetString((byte[])propValue.Value);
				}
				return propValue.Value;
			}
		}

		private static readonly QueryFilter DisplayNameExists = new ExistsFilter(ADRecipientSchema.DisplayName);

		private static readonly QueryFilter NotHiddenFromAddressLists = new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.HiddenFromAddressListsEnabled, false);

		private readonly Encoding encoding;
	}
}
