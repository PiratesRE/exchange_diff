using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.Search
{
	internal class SubFilterConverter : BaseLeafFilterConverter
	{
		internal override QueryFilter ConvertToQueryFilter(SearchExpressionType searchExpression)
		{
			return null;
		}

		internal override SearchExpressionType ConvertToSearchExpression(QueryFilter queryFilter)
		{
			SubFilter subFilter = (SubFilter)queryFilter;
			SubFilterProperties subFilterProperty = subFilter.SubFilterProperty;
			if (subFilterProperty == SubFilterProperties.Recipients)
			{
				return SubFilterConverter.ConvertRecipients(subFilter);
			}
			if (subFilterProperty == SubFilterProperties.Attachments)
			{
				return SubFilterConverter.ConvertAttachments(subFilter);
			}
			throw new UnsupportedQueryFilterException(CoreResources.IDs.ErrorUnsupportedSubFilter);
		}

		private static SearchExpressionType ConvertRecipients(SubFilter subFilter)
		{
			AndFilter andFilter = subFilter.Filter as AndFilter;
			if (andFilter == null)
			{
				throw new UnsupportedQueryFilterException(CoreResources.IDs.ErrorInvalidRecipientSubfilter);
			}
			if (andFilter.FilterCount != 2)
			{
				throw new UnsupportedQueryFilterException(CoreResources.IDs.ErrorInvalidRecipientSubfilter);
			}
			TextFilter textFilter = andFilter.Filters[0] as TextFilter;
			ComparisonFilter comparisonFilter = andFilter.Filters[1] as ComparisonFilter;
			if (textFilter == null || comparisonFilter == null)
			{
				throw new UnsupportedQueryFilterException((CoreResources.IDs)3776856310U);
			}
			SubFilterType subFilterType = SubFilterType.None;
			if (comparisonFilter.ComparisonOperator != ComparisonOperator.Equal)
			{
				throw new UnsupportedQueryFilterException(CoreResources.IDs.ErrorInvalidRecipientSubfilterComparison);
			}
			if (comparisonFilter.Property == RecipientSchema.RecipientType)
			{
				RecipientItemType recipientItemType = (RecipientItemType)comparisonFilter.PropertyValue;
				if (recipientItemType == RecipientItemType.To)
				{
					subFilterType = SubFilterType.RecipientTo;
				}
				else if (recipientItemType == RecipientItemType.Cc)
				{
					subFilterType = SubFilterType.RecipientCc;
				}
				else if (recipientItemType == RecipientItemType.Bcc)
				{
					subFilterType = SubFilterType.RecipientBcc;
				}
			}
			else
			{
				if (comparisonFilter.Property != AttendeeSchema.AttendeeType)
				{
					throw new UnsupportedQueryFilterException(CoreResources.IDs.ErrorInvalidRecipientSubfilterComparison);
				}
				AttendeeType attendeeType = (AttendeeType)comparisonFilter.PropertyValue;
				if (attendeeType == AttendeeType.Required)
				{
					subFilterType = SubFilterType.AttendeeRequired;
				}
				else if (attendeeType == AttendeeType.Optional)
				{
					subFilterType = SubFilterType.AttendeeOptional;
				}
				else if (attendeeType == AttendeeType.Resource)
				{
					subFilterType = SubFilterType.AttendeeResource;
				}
			}
			if (textFilter.Property != RecipientSchema.SmtpAddress)
			{
				throw new UnsupportedQueryFilterException((CoreResources.IDs)4094604515U);
			}
			return new ContainsExpressionType
			{
				ContainmentModeString = SubFilterBuilder.ExtractMatchOptionsText(textFilter),
				ContainmentComparisonString = SubFilterBuilder.ExtractMatchFlagsText(textFilter),
				Item = SearchSchemaMap.GetPathToElementForSubFilter(subFilterType),
				Constant = new ConstantValueType
				{
					Value = textFilter.Text
				}
			};
		}

		private static SearchExpressionType ConvertAttachments(SubFilter subFilter)
		{
			TextFilter textFilter = subFilter.Filter as TextFilter;
			if (textFilter == null)
			{
				throw new UnsupportedQueryFilterException((CoreResources.IDs)2798800298U);
			}
			if (textFilter.Property != AttachmentSchema.DisplayName)
			{
				throw new UnsupportedQueryFilterException(CoreResources.IDs.ErrorInvalidAttachmentSubfilterTextFilter);
			}
			return new ContainsExpressionType
			{
				ContainmentModeString = SubFilterBuilder.ExtractMatchOptionsText(textFilter),
				ContainmentComparisonString = SubFilterBuilder.ExtractMatchFlagsText(textFilter),
				Item = SearchSchemaMap.GetPropertyPath(textFilter.Property),
				Constant = new ConstantValueType
				{
					Value = textFilter.Text
				}
			};
		}
	}
}
