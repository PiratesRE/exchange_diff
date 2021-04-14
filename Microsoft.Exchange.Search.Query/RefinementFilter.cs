using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Search.AqsParser;
using Microsoft.Exchange.Data.Search.KqlParser;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Search.Core.Common;
using Microsoft.Exchange.Search.OperatorSchema;

namespace Microsoft.Exchange.Search.Query
{
	internal class RefinementFilter
	{
		public RefinementFilter(IReadOnlyCollection<string> refinementQueries)
		{
			InstantSearch.ThrowOnNullOrEmptyArgument(refinementQueries, "refinementQueries");
			this.Filters = refinementQueries;
		}

		internal IReadOnlyCollection<string> Filters { get; private set; }

		internal QueryFilter GetQueryFilter(CultureInfo cultureInfo, MailboxSession mailboxSession, IRecipientResolver recipientResolver, IPolicyTagProvider policyTagProvider)
		{
			QueryFilter[] array = new QueryFilter[this.Filters.Count];
			int num = 0;
			foreach (string text in this.Filters)
			{
				string[] array2 = text.Split(RefinementFilter.QuerySeparator, 2, StringSplitOptions.None);
				RefinementFilter.BuilderDelegate builderDelegate;
				QueryFilter queryFilter;
				if (array2.Length == 2 && RefinementFilter.FilterBuilders.TryGetValue(array2[0], out builderDelegate))
				{
					queryFilter = builderDelegate(RefinementFilter.RemoveQuotes(array2[1]), mailboxSession);
				}
				else
				{
					queryFilter = KqlParser.ParseAndBuildQuery(text, KqlParser.ParseOption.SuppressError | KqlParser.ParseOption.UseCiKeywordOnly, cultureInfo, RescopedAll.Default, recipientResolver, policyTagProvider);
				}
				array[num++] = queryFilter;
			}
			if (array.Length != 1)
			{
				return new AndFilter(array);
			}
			return array[0];
		}

		private static QueryFilter GetHasAttachmentFilter(string arg, MailboxSession mailboxSession)
		{
			if (!(arg == "0"))
			{
				return RefinementFilter.HasAttachmentTrueFilter;
			}
			return RefinementFilter.HasAttachmentFalseFilter;
		}

		private static QueryFilter GetFolderIdFilter(string arg, MailboxSession mailboxSession)
		{
			StoreObjectId storeObjectIdFromHexString = FolderIdHelper.GetStoreObjectIdFromHexString(arg, mailboxSession);
			return new ComparisonFilter(ComparisonOperator.Equal, StoreObjectSchema.ParentItemId, storeObjectIdFromHexString);
		}

		private static QueryFilter GetFromFilter(string arg, MailboxSession mailboxSession)
		{
			return RefinementFilter.GetParticipantFilter(arg, ItemSchema.SearchSender);
		}

		private static QueryFilter GetSearchRecipientsFilter(string arg, MailboxSession mailboxSession)
		{
			return RefinementFilter.GetParticipantFilter(arg, ItemSchema.SearchRecipients);
		}

		private static QueryFilter GetParticipantFilter(string arg, PropertyDefinition property)
		{
			return new TextFilter(property, arg, MatchOptions.ExactPhrase, MatchFlags.Loose);
		}

		private static string RemoveQuotes(string quotedString)
		{
			if (string.IsNullOrEmpty(quotedString) || quotedString.Length < 2 || quotedString[0] != '"' || quotedString[quotedString.Length - 1] != '"')
			{
				return quotedString;
			}
			return quotedString.Substring(1, quotedString.Length - 2);
		}

		private const string HasAttachmentFalse = "0";

		private static readonly string[] QuerySeparator = new string[]
		{
			":"
		};

		private static readonly QueryFilter HasAttachmentTrueFilter = new ComparisonFilter(ComparisonOperator.Equal, ItemSchema.HasAttachment, true);

		private static readonly QueryFilter HasAttachmentFalseFilter = new ComparisonFilter(ComparisonOperator.Equal, ItemSchema.HasAttachment, false);

		private static readonly Dictionary<string, RefinementFilter.BuilderDelegate> FilterBuilders = new Dictionary<string, RefinementFilter.BuilderDelegate>(StringComparer.OrdinalIgnoreCase)
		{
			{
				FastIndexSystemSchema.FolderId.Name,
				new RefinementFilter.BuilderDelegate(RefinementFilter.GetFolderIdFilter)
			},
			{
				FastIndexSystemSchema.HasAttachment.Name,
				new RefinementFilter.BuilderDelegate(RefinementFilter.GetHasAttachmentFilter)
			},
			{
				FastIndexSystemSchema.Recipients.Name,
				new RefinementFilter.BuilderDelegate(RefinementFilter.GetSearchRecipientsFilter)
			},
			{
				FastIndexSystemSchema.From.Name,
				new RefinementFilter.BuilderDelegate(RefinementFilter.GetFromFilter)
			}
		};

		private delegate QueryFilter BuilderDelegate(string arg, MailboxSession mailboxSession);
	}
}
