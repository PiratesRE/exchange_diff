using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.UM.UMCommon.MessageContent;

namespace Microsoft.Exchange.UM.UMCore
{
	internal abstract class Filters
	{
		internal static QueryFilter CreateNewVoiceMessageFilter()
		{
			return new ComparisonFilter(ComparisonOperator.NotEqual, MessageItemSchema.IsRead, true);
		}

		internal static QueryFilter CreateSavedVoiceMessageFilter()
		{
			return new ComparisonFilter(ComparisonOperator.Equal, MessageItemSchema.IsRead, true);
		}

		internal static QueryFilter CreatePureVoiceFilter()
		{
			QueryFilter queryFilter = new ComparisonFilter(ComparisonOperator.Equal, StoreObjectSchema.ItemClass, "IPM.Note.Microsoft.Voicemail.UM");
			QueryFilter queryFilter2 = new ComparisonFilter(ComparisonOperator.Equal, StoreObjectSchema.ItemClass, "IPM.Note.Microsoft.Voicemail.UM.CA");
			QueryFilter queryFilter3 = new ComparisonFilter(ComparisonOperator.Equal, StoreObjectSchema.ItemClass, "IPM.Note.rpmsg.Microsoft.Voicemail.UM.CA");
			QueryFilter queryFilter4 = new ComparisonFilter(ComparisonOperator.Equal, StoreObjectSchema.ItemClass, "IPM.Note.rpmsg.Microsoft.Voicemail.UM");
			QueryFilter queryFilter5 = new OrFilter(new QueryFilter[]
			{
				queryFilter,
				queryFilter2,
				queryFilter3,
				queryFilter4
			});
			QueryFilter queryFilter6 = new ComparisonFilter(ComparisonOperator.Equal, ItemSchema.HasAttachment, true);
			return new AndFilter(new QueryFilter[]
			{
				queryFilter5,
				queryFilter6
			});
		}

		internal static QueryFilter CreateVoicemailFindByNameFilter(ContactSearchItem searchResult, CultureInfo callerCulture)
		{
			List<QueryFilter> list = new List<QueryFilter>(6);
			if (!string.IsNullOrEmpty(searchResult.FullName))
			{
				list.Add(new TextFilter(ItemSchema.SentRepresentingDisplayName, searchResult.FullName, MatchOptions.FullString, MatchFlags.IgnoreCase));
				if (callerCulture != null)
				{
					list.Add(new TextFilter(ItemSchema.SentRepresentingDisplayName, Strings.NoEmailAddressDisplayName(searchResult.FullName).ToString(callerCulture), MatchOptions.FullString, MatchFlags.IgnoreCase));
				}
			}
			Filters.AddCommonFindByNameFilters(list, searchResult);
			return Filters.GenerateOrFilter(list);
		}

		internal static QueryFilter CreateFindByNameFilter(ContactSearchItem searchResult)
		{
			List<QueryFilter> list = new List<QueryFilter>(6);
			if (!string.IsNullOrEmpty(searchResult.FullName))
			{
				list.Add(new TextFilter(ItemSchema.SentRepresentingDisplayName, searchResult.FullName, MatchOptions.FullString, MatchFlags.IgnoreCase));
			}
			Filters.AddCommonFindByNameFilters(list, searchResult);
			return Filters.GenerateOrFilter(list);
		}

		private static void AddCommonFindByNameFilters(List<QueryFilter> filterList, ContactSearchItem searchResult)
		{
			if (!string.IsNullOrEmpty(searchResult.PrimarySmtpAddress))
			{
				filterList.Add(new TextFilter(ItemSchema.SentRepresentingEmailAddress, searchResult.PrimarySmtpAddress, MatchOptions.FullString, MatchFlags.IgnoreCase));
			}
			foreach (string text in searchResult.ContactEmailAddresses)
			{
				if (!string.IsNullOrEmpty(text))
				{
					filterList.Add(new TextFilter(ItemSchema.SentRepresentingEmailAddress, text, MatchOptions.FullString, MatchFlags.IgnoreCase));
				}
			}
			if (searchResult.Recipient != null && !string.IsNullOrEmpty(searchResult.Recipient.LegacyExchangeDN))
			{
				filterList.Add(new TextFilter(ItemSchema.SentRepresentingEmailAddress, searchResult.Recipient.LegacyExchangeDN, MatchOptions.FullString, MatchFlags.IgnoreCase));
			}
		}

		private static QueryFilter GenerateOrFilter(List<QueryFilter> filterList)
		{
			if (filterList.Count == 0)
			{
				return null;
			}
			return new OrFilter(filterList.ToArray());
		}
	}
}
