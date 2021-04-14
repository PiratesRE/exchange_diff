using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Microsoft.Exchange.Data.Search.AqsParser;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.ApplicationLogic
{
	internal static class AqsQueryBuilder
	{
		internal static void AppendDateClause(StringBuilder query, PropertyKeyword keyword, DateRangeQueryOperation operation, DateTime date)
		{
			EnumValidator.ThrowIfInvalid<PropertyKeyword>(keyword, AqsQueryBuilder.ValidDateKeywords);
			EnumValidator.ThrowIfInvalid<DateRangeQueryOperation>(operation);
			AqsQueryBuilder.AppendLeadingSpaceIfNecessary(query);
			query.Append(keyword.ToString().ToLower()).Append(":(");
			switch (operation)
			{
			case DateRangeQueryOperation.Equal:
				query.Append("=");
				break;
			case DateRangeQueryOperation.GreaterThan:
				query.Append(">");
				break;
			case DateRangeQueryOperation.GreaterThanOrEqual:
				query.Append(">=");
				break;
			case DateRangeQueryOperation.LessThan:
				query.Append("<");
				break;
			case DateRangeQueryOperation.LessThanOrEqual:
				query.Append("<=");
				break;
			}
			query.Append(date.ToLocalTime().ToString(AqsQueryBuilder.AqsDateTimeFormat, CultureInfo.InvariantCulture));
			query.Append(")");
		}

		internal static void AppendKeywordOrClause(StringBuilder query, PropertyKeyword keyword, ICollection<string> valuesToQuery)
		{
			EnumValidator.ThrowIfInvalid<PropertyKeyword>(keyword);
			if (valuesToQuery != null && valuesToQuery.Count > 0)
			{
				bool flag = false;
				foreach (string value in valuesToQuery)
				{
					if (!string.IsNullOrEmpty(value))
					{
						if (!flag)
						{
							flag = true;
							AqsQueryBuilder.AppendLeadingSpaceIfNecessary(query);
							query.Append(keyword.ToString().ToLower()).Append(":(");
						}
						else
						{
							query.Append(AqsQueryBuilder.AqsQueryORSeparator);
						}
						query.Append("\"").Append(value).Append("\"");
					}
				}
				if (flag)
				{
					query.Append(")");
				}
			}
		}

		private static void AppendLeadingSpaceIfNecessary(StringBuilder query)
		{
			if (query.Length > 0)
			{
				query.Append(" ");
			}
		}

		private static readonly string AqsDateTimeFormat = "yyyy-MM-dd HH:mm:ss";

		private static readonly string AqsQueryORSeparator = " OR ";

		private static readonly PropertyKeyword[] ValidDateKeywords = new PropertyKeyword[]
		{
			PropertyKeyword.Sent,
			PropertyKeyword.Received
		};
	}
}
