using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.Search
{
	internal class ExistsFilterConverter : BaseLeafFilterConverter
	{
		internal override QueryFilter ConvertToQueryFilter(SearchExpressionType element)
		{
			ExistsType existsType = element as ExistsType;
			ExistsFilterConverter.ConfirmNotMessageFlag(existsType.Item as PropertyUri);
			PropertyDefinition andValidatePropertyDefinitionForQuery = BaseLeafFilterConverter.GetAndValidatePropertyDefinitionForQuery(existsType.Item);
			return new ExistsFilter(andValidatePropertyDefinitionForQuery);
		}

		internal override SearchExpressionType ConvertToSearchExpression(QueryFilter queryFilter)
		{
			ExistsFilter existsFilter = (ExistsFilter)queryFilter;
			return new ExistsType
			{
				Item = SearchSchemaMap.GetPropertyPath(existsFilter.Property)
			};
		}

		private static void ConfirmNotMessageFlag(PropertyUri uri)
		{
			if (uri == null)
			{
				return;
			}
			PropertyUriEnum uri2 = uri.Uri;
			switch (uri2)
			{
			case PropertyUriEnum.IsDraft:
			case PropertyUriEnum.IsFromMe:
			case PropertyUriEnum.IsResend:
			case PropertyUriEnum.IsSubmitted:
			case PropertyUriEnum.IsUnmodified:
				break;
			default:
				if (uri2 != PropertyUriEnum.IsRead)
				{
					return;
				}
				break;
			}
			throw new InvalidPropertyForExistsException(uri);
		}
	}
}
