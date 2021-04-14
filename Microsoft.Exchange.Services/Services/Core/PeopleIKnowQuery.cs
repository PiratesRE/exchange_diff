using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Services.Core
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal static class PeopleIKnowQuery
	{
		public static QueryFilter GetConversationQueryFilter(string senderDisplayName)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("senderDisplayName", senderDisplayName);
			return new MultivaluedInstanceComparisonFilter(ComparisonOperator.Equal, PeopleIKnowQuery.ConversationQuerySenderProperty, senderDisplayName);
		}

		public static QueryFilter GetItemQueryFilter(string senderSmtpAddress)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("senderSmtpAddress", senderSmtpAddress);
			return new ComparisonFilter(ComparisonOperator.Equal, PeopleIKnowQuery.ItemQuerySenderProperty, senderSmtpAddress);
		}

		public static SortBy[] GetConversationQuerySortBy(SortBy[] originalSortBy)
		{
			ArgumentValidator.ThrowIfNull("originalSortBy", originalSortBy);
			return PeopleIKnowQuery.MergeSortBys(new SortBy(PeopleIKnowQuery.ConversationQuerySenderProperty, SortOrder.Ascending), originalSortBy);
		}

		public static SortBy[] GetItemQuerySortBy(SortBy[] originalSortBy)
		{
			ArgumentValidator.ThrowIfNull("originalSortBy", originalSortBy);
			return PeopleIKnowQuery.MergeSortBys(new SortBy(PeopleIKnowQuery.ItemQuerySenderProperty, SortOrder.Ascending), originalSortBy);
		}

		private static SortBy[] MergeSortBys(SortBy sortByToMerge, SortBy[] originalSortBy)
		{
			List<SortBy> list = new List<SortBy>
			{
				sortByToMerge
			};
			foreach (SortBy sortBy in originalSortBy)
			{
				if (!sortBy.ColumnDefinition.Equals(sortByToMerge.ColumnDefinition))
				{
					list.Add(sortBy);
				}
			}
			return list.ToArray();
		}

		private static readonly StorePropertyDefinition ItemQuerySenderProperty = MessageItemSchema.SenderSmtpAddress;

		private static readonly StorePropertyDefinition ConversationQuerySenderProperty = ConversationItemSchema.ConversationMVFrom;
	}
}
