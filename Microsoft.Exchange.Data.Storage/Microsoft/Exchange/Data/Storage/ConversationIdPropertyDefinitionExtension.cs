using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal static class ConversationIdPropertyDefinitionExtension
	{
		internal static QueryFilter NativeFilterToConversationIdBasedSmartFilter(this SmartPropertyDefinition conversationIdSmartPropertyDefinition, QueryFilter filter, PropertyDefinition conversationIdNativePropertyDefinition)
		{
			SinglePropertyFilter singlePropertyFilter = filter as SinglePropertyFilter;
			if (singlePropertyFilter != null && singlePropertyFilter.Property.Equals(conversationIdNativePropertyDefinition))
			{
				ComparisonFilter comparisonFilter = filter as ComparisonFilter;
				if (comparisonFilter != null)
				{
					return new ComparisonFilter(comparisonFilter.ComparisonOperator, conversationIdSmartPropertyDefinition, ConversationId.Create((byte[])comparisonFilter.PropertyValue));
				}
				ExistsFilter existsFilter = filter as ExistsFilter;
				if (existsFilter != null)
				{
					return new ExistsFilter(conversationIdSmartPropertyDefinition);
				}
			}
			return null;
		}

		internal static QueryFilter ConversationIdBasedSmartFilterToNativeFilter(this SmartPropertyDefinition conversationIdSmartPropertyDefinition, SinglePropertyFilter filter, PropertyDefinition conversationIdNativePropertyDefinition)
		{
			ComparisonFilter comparisonFilter = filter as ComparisonFilter;
			if (comparisonFilter != null)
			{
				ConversationId conversationId = (ConversationId)comparisonFilter.PropertyValue;
				return new ComparisonFilter(comparisonFilter.ComparisonOperator, conversationIdNativePropertyDefinition, conversationId.GetBytes());
			}
			ExistsFilter existsFilter = filter as ExistsFilter;
			if (existsFilter != null)
			{
				return new ExistsFilter(conversationIdNativePropertyDefinition);
			}
			throw conversationIdSmartPropertyDefinition.CreateInvalidFilterConversionException(filter);
		}
	}
}
