using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.FfoReporting.Common
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	internal sealed class QueryParameter : Attribute
	{
		internal QueryParameter(string definition, params string[] optionalDefinitions)
		{
			this.definition = definition;
			this.optionalDefinitions = optionalDefinitions;
		}

		public string MethodName { get; set; }

		internal void AddFilter(List<ComparisonFilter> filters, object value)
		{
			if (string.IsNullOrWhiteSpace(this.MethodName))
			{
				if (value is MultiValuedPropertyBase)
				{
					value = Schema.Utilities.CreateDataTable((MultiValuedPropertyBase)value);
				}
				filters.Add(new ComparisonFilter(ComparisonOperator.Equal, Schema.Utilities.GetSchemaPropertyDefinition(this.definition), value));
				return;
			}
			Type type = base.GetType();
			MethodInfo method = type.GetMethod(this.MethodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (method == null)
			{
				throw new NullReferenceException("Unknown method name");
			}
			Schema.Utilities.Invoke(method, this, new object[]
			{
				filters,
				value
			});
		}

		private void AddDateFilter(List<ComparisonFilter> filters, object value)
		{
			DateTime date = (DateTime)value;
			filters.Add(new ComparisonFilter(ComparisonOperator.Equal, Schema.Utilities.GetSchemaPropertyDefinition(this.definition), Schema.Utilities.ToQueryDate(date)));
			if (this.optionalDefinitions.Length > 0)
			{
				filters.Add(new ComparisonFilter(ComparisonOperator.Equal, Schema.Utilities.GetSchemaPropertyDefinition(this.optionalDefinitions[0]), Schema.Utilities.ToQueryHour(date)));
			}
		}

		private readonly string definition;

		private string[] optionalDefinitions;

		internal static class Methods
		{
			internal const string AddDate = "AddDateFilter";

			internal const string AddGuid = "AddGuidFilter";
		}

		internal sealed class Ids
		{
			internal const string Action = "ActionListQueryDefinition";

			internal const string Actor = "SenderAddressListQueryDefinition";

			internal const string AggregateBy = "AggregateByQueryDefinition";

			internal const string ClientMessageId = "ClientMessageIdQueryDefinition";

			internal const string Direction = "DirectionListQueryDefinition";

			internal const string Domain = "DomainListQueryDefinition";

			internal const string EndDate = "EndDateQueryDefinition";

			internal const string EndDateKey = "EndDateKeyQueryDefinition";

			internal const string EndHourKey = "EndHourKeyQueryDefinition";

			internal const string Event = "EventListQueryDefinition";

			internal const string EventType = "EventTypeListQueryDefinition";

			internal const string Source = "DataSourceListQueryDefinition";

			internal const string FromIP = "FromIPAddressQueryDefinition";

			internal const string InternalMessageId = "InternalMessageIdQueryDefinition";

			internal const string MalwareName = "MalwareListQueryDefinition";

			internal const string MessageId = "MessageIdListQueryDefinition";

			internal const string Organization = "OrganizationQueryDefinition";

			internal const string Page = "PageQueryDefinition";

			internal const string PageSize = "PageSizeQueryDefinition";

			internal const string PolicyName = "PolicyListQueryDefinition";

			internal const string RecipientAddress = "RecipientAddressQueryDefinition";

			internal const string RecipientAddressList = "RecipientAddressListQueryDefinition";

			internal const string RuleName = "RuleListQueryDefinition";

			internal const string SenderAddress = "SenderAddressQueryDefinition";

			internal const string SenderAddressList = "SenderAddressListQueryDefinition";

			internal const string SummarizeBy = "SummarizeByQueryDefinition";

			internal const string StartDate = "StartDateQueryDefinition";

			internal const string StartDateKey = "StartDateKeyQueryDefinition";

			internal const string StartHourKey = "StartHourKeyQueryDefinition";

			internal const string Status = "MailDeliveryStatusListDefinition";

			internal const string ToIP = "ToIPAddressQueryDefinition";

			internal const string TransportRuleName = "TransportRuleListQueryDefinition";
		}
	}
}
