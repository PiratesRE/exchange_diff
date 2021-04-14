using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Management.FfoReporting.Common;
using Microsoft.Exchange.Management.FfoReporting.Data;
using Microsoft.Exchange.Management.FfoReporting.Providers;
using Microsoft.Exchange.Management.ReportingTask;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Management.FfoReporting
{
	[Cmdlet("Get", "MailTrafficSummaryReport")]
	[OutputType(new Type[]
	{
		typeof(MailTrafficSummaryReport)
	})]
	public sealed class GetMailTrafficSummaryReport : FfoReportingDalTask<MailTrafficSummaryReport>
	{
		public override string ComponentName
		{
			get
			{
				return ExchangeComponent.FfoRws.Name;
			}
		}

		public override string MonitorEventName
		{
			get
			{
				return "FFO Reporting Task Status Monitor";
			}
		}

		public override string DalMonitorEventName
		{
			get
			{
				return "FFO DAL Retrieval Status Monitor";
			}
		}

		public GetMailTrafficSummaryReport()
		{
			this.Category = string.Empty;
			this.Domain = new MultiValuedProperty<Fqdn>();
			this.DlpPolicy = new MultiValuedProperty<string>();
			this.TransportRule = new MultiValuedProperty<string>();
			this.mappings.Add(GetMailTrafficSummaryReport.Categories.InboundDLPHits, Tuple.Create<string, GetMailTrafficSummaryReport.AggregateDelegate>("Microsoft.Exchange.Hygiene.Data.MessageTrace.Reports.PolicyTrafficReport, Microsoft.Exchange.Hygiene.Data", new GetMailTrafficSummaryReport.AggregateDelegate(this.AggregateInboundDLPHits)));
			this.mappings.Add(GetMailTrafficSummaryReport.Categories.OutboundDLPHits, Tuple.Create<string, GetMailTrafficSummaryReport.AggregateDelegate>("Microsoft.Exchange.Hygiene.Data.MessageTrace.Reports.PolicyTrafficReport, Microsoft.Exchange.Hygiene.Data", new GetMailTrafficSummaryReport.AggregateDelegate(this.AggregateOutboundDLPHits)));
			this.mappings.Add(GetMailTrafficSummaryReport.Categories.InboundTransportRuleHits, Tuple.Create<string, GetMailTrafficSummaryReport.AggregateDelegate>("Microsoft.Exchange.Hygiene.Data.MessageTrace.Reports.PolicyTrafficReport, Microsoft.Exchange.Hygiene.Data", () => this.AggregateTransportRuleHits(Schema.DirectionValues.Inbound)));
			this.mappings.Add(GetMailTrafficSummaryReport.Categories.OutboundTransportRuleHits, Tuple.Create<string, GetMailTrafficSummaryReport.AggregateDelegate>("Microsoft.Exchange.Hygiene.Data.MessageTrace.Reports.PolicyTrafficReport, Microsoft.Exchange.Hygiene.Data", () => this.AggregateTransportRuleHits(Schema.DirectionValues.Outbound)));
			this.mappings.Add(GetMailTrafficSummaryReport.Categories.InboundDLPPolicyRuleHits, Tuple.Create<string, GetMailTrafficSummaryReport.AggregateDelegate>("Microsoft.Exchange.Hygiene.Data.MessageTrace.Reports.PolicyTrafficReport, Microsoft.Exchange.Hygiene.Data", () => this.AggregateDLPPolicyRuleHits(Schema.DirectionValues.Inbound)));
			this.mappings.Add(GetMailTrafficSummaryReport.Categories.OutboundDLPPolicyRuleHits, Tuple.Create<string, GetMailTrafficSummaryReport.AggregateDelegate>("Microsoft.Exchange.Hygiene.Data.MessageTrace.Reports.PolicyTrafficReport, Microsoft.Exchange.Hygiene.Data", () => this.AggregateDLPPolicyRuleHits(Schema.DirectionValues.Outbound)));
			this.AddTopTraffic(GetMailTrafficSummaryReport.Categories.TopSpamRecipient, Schema.EventTypes.TopSpamUser.ToString(), new string[]
			{
				Schema.DirectionValues.Inbound.ToString()
			});
			this.AddTopTraffic(GetMailTrafficSummaryReport.Categories.TopMailSender, Schema.EventTypes.TopMailUser.ToString(), new string[]
			{
				Schema.DirectionValues.Outbound.ToString()
			});
			this.AddTopTraffic(GetMailTrafficSummaryReport.Categories.TopMailRecipient, Schema.EventTypes.TopMailUser.ToString(), new string[]
			{
				Schema.DirectionValues.Inbound.ToString()
			});
			this.AddTopTraffic(GetMailTrafficSummaryReport.Categories.TopMalwareRecipient, Schema.EventTypes.TopMalwareUser.ToString(), new string[]
			{
				Schema.DirectionValues.Inbound.ToString()
			});
			this.AddTopTraffic(GetMailTrafficSummaryReport.Categories.TopMalware, Schema.EventTypes.TopMalware.ToString(), new string[]
			{
				Schema.DirectionValues.Inbound.ToString(),
				Schema.DirectionValues.Outbound.ToString()
			});
		}

		[CmdletValidator("ValidateEnum", new object[]
		{
			typeof(GetMailTrafficSummaryReport.Categories)
		}, ErrorMessage = Strings.IDs.InvalidCategory)]
		[Parameter(Mandatory = false)]
		public string Category { get; set; }

		[Parameter(Mandatory = false, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
		[CmdletValidator("ValidateDomain", new object[]
		{

		}, ErrorMessage = Strings.IDs.InvalidDomain, ValidatorType = CmdletValidator.ValidatorTypes.PostprocessingWithConfigSession)]
		[QueryParameter("DomainListQueryDefinition", new string[]
		{

		})]
		public MultiValuedProperty<Fqdn> Domain { get; set; }

		[Parameter(Mandatory = false)]
		[QueryParameter("StartDateKeyQueryDefinition", new string[]
		{
			"StartHourKeyQueryDefinition"
		}, MethodName = "AddDateFilter")]
		public DateTime? StartDate { get; set; }

		[QueryParameter("EndDateKeyQueryDefinition", new string[]
		{
			"EndHourKeyQueryDefinition"
		}, MethodName = "AddDateFilter")]
		[Parameter(Mandatory = false)]
		public DateTime? EndDate { get; set; }

		[QueryParameter("PolicyListQueryDefinition", new string[]
		{

		})]
		[Parameter(Mandatory = false)]
		[CmdletValidator("ValidateDlpPolicy", new object[]
		{

		}, ErrorMessage = Strings.IDs.InvalidDlpPolicyParameter, ValidatorType = CmdletValidator.ValidatorTypes.PostprocessingWithConfigSession)]
		public MultiValuedProperty<string> DlpPolicy { get; set; }

		[Parameter(Mandatory = false)]
		[QueryParameter("RuleListQueryDefinition", new string[]
		{

		})]
		[CmdletValidator("ValidateTransportRule", new object[]
		{

		}, ErrorMessage = Strings.IDs.InvalidTransportRule, ValidatorType = CmdletValidator.ValidatorTypes.PostprocessingWithConfigSession)]
		public MultiValuedProperty<string> TransportRule { get; set; }

		private GetMailTrafficSummaryReport.Categories CategoryEnum { get; set; }

		protected override void CustomInternalValidate()
		{
			base.CustomInternalValidate();
			Schema.Utilities.CheckDates(this.StartDate, this.EndDate, new Schema.Utilities.NotifyNeedDefaultDatesDelegate(this.SetDefaultDates), new Schema.Utilities.ValidateDatesDelegate(Schema.Utilities.VerifyDateRange));
			GetMailTrafficSummaryReport.Categories categories;
			if (!Enum.TryParse<GetMailTrafficSummaryReport.Categories>(this.Category, true, out categories))
			{
				throw new InvalidExpressionException(Strings.InvalidCategory);
			}
			Tuple<string, GetMailTrafficSummaryReport.AggregateDelegate> tuple;
			if (!this.mappings.TryGetValue(categories, out tuple))
			{
				throw new InvalidOperationException(Strings.InvalidCategory);
			}
			this.CategoryEnum = categories;
			base.DalObjectTypeName = tuple.Item1;
		}

		protected override IReadOnlyList<MailTrafficSummaryReport> AggregateOutput()
		{
			Tuple<string, GetMailTrafficSummaryReport.AggregateDelegate> tuple;
			if (this.mappings.TryGetValue(this.CategoryEnum, out tuple))
			{
				GetMailTrafficSummaryReport.AggregateDelegate item = tuple.Item2;
				IReadOnlyList<MailTrafficSummaryReport> readOnlyList = item();
				if (base.NeedSuppressingPiiData)
				{
					DataProcessorDriver.Process<MailTrafficSummaryReport>(readOnlyList, RedactionProcessor.Create<MailTrafficSummaryReport>());
				}
				return readOnlyList;
			}
			throw new InvalidOperationException(Strings.InvalidCategory);
		}

		private void AddTopTraffic(GetMailTrafficSummaryReport.Categories category, string eventType, params string[] directions)
		{
			this.mappings.Add(category, Tuple.Create<string, GetMailTrafficSummaryReport.AggregateDelegate>("Microsoft.Exchange.Hygiene.Data.MessageTrace.Reports.TopTrafficReport, Microsoft.Exchange.Hygiene.Data", () => this.AggregateTopTraffic(eventType, directions)));
		}

		private QueryFilter BuildQueryFilter(IEnumerable<ComparisonFilter> optionalFilters)
		{
			CompositeFilter compositeFilter = (CompositeFilter)base.BuildQueryFilter();
			List<ComparisonFilter> list = new List<ComparisonFilter>(compositeFilter.Filters.Cast<ComparisonFilter>());
			list.AddRange(optionalFilters);
			return new AndFilter(list.ToArray<QueryFilter>());
		}

		private IReadOnlyList<TDataObject> GetData<TDataObject>(IEnumerable<ComparisonFilter> filters)
		{
			IEnumerable dalRecords = base.GetDalRecords(new FfoReportingDalTask<MailTrafficSummaryReport>.DalRetrievalDelegate(ServiceLocator.Current.GetService<IDalProvider>().GetAllDataPages), this.BuildQueryFilter(filters));
			return DataProcessorDriver.Process<TDataObject>(dalRecords, ConversionProcessor.Create<TDataObject>(this));
		}

		private ComparisonFilter CreateDataTableFilter(string definitionName, params object[] values)
		{
			values = ((values.Length == 0) ? new object[0] : values);
			return new ComparisonFilter(ComparisonOperator.Equal, Schema.Utilities.GetSchemaPropertyDefinition(definitionName), Schema.Utilities.CreateDataTable(values));
		}

		private ComparisonFilter CreateFilter(string definitionName, object value)
		{
			return new ComparisonFilter(ComparisonOperator.Equal, Schema.Utilities.GetSchemaPropertyDefinition(definitionName), value);
		}

		internal IReadOnlyList<MailTrafficSummaryReport> AggregateInboundDLPHits()
		{
			IReadOnlyList<MailTrafficPolicyReport> data = this.GetData<MailTrafficPolicyReport>(new List<ComparisonFilter>
			{
				this.CreateDataTableFilter("DirectionListQueryDefinition", new object[]
				{
					Schema.DirectionValues.Inbound.ToString()
				}),
				this.CreateFilter("AggregateByQueryDefinition", Schema.AggregateByValues.Summary.ToString()),
				this.CreateDataTableFilter("EventTypeListQueryDefinition", new object[]
				{
					Schema.EventTypes.DLPPolicyHits.ToString()
				}),
				this.CreateDataTableFilter("ActionListQueryDefinition", new object[0]),
				this.CreateDataTableFilter("SummarizeByQueryDefinition", new object[]
				{
					Schema.SummarizeByValues.Action.ToString(),
					Schema.SummarizeByValues.Domain.ToString(),
					Schema.SummarizeByValues.EventType.ToString(),
					Schema.SummarizeByValues.TransportRule.ToString()
				})
			});
			IEnumerable<IGrouping<string, MailTrafficPolicyReport>> enumerable = from trafficReport in data
			group trafficReport by trafficReport.DlpPolicy into policyGroup
			select policyGroup;
			List<Tuple<string, int>> list = new List<Tuple<string, int>>();
			foreach (IGrouping<string, MailTrafficPolicyReport> grouping in enumerable)
			{
				string key = grouping.Key;
				int item = grouping.Sum((MailTrafficPolicyReport report) => report.MessageCount);
				list.Add(Tuple.Create<string, int>(key, item));
			}
			int count = (base.Page - 1) * base.PageSize;
			return (from tuple in (from tuple in list
			orderby tuple.Item2 descending
			select tuple).Skip(count).Take(base.PageSize)
			select new MailTrafficSummaryReport
			{
				C1 = tuple.Item1,
				C2 = tuple.Item2.ToString()
			}).ToList<MailTrafficSummaryReport>();
		}

		internal IReadOnlyList<MailTrafficSummaryReport> AggregateOutboundDLPHits()
		{
			IReadOnlyList<MailTrafficPolicyReport> data = this.GetData<MailTrafficPolicyReport>(new List<ComparisonFilter>
			{
				this.CreateDataTableFilter("DirectionListQueryDefinition", new object[]
				{
					Schema.DirectionValues.Outbound.ToString()
				}),
				this.CreateFilter("AggregateByQueryDefinition", Schema.AggregateByValues.Summary.ToString()),
				this.CreateDataTableFilter("EventTypeListQueryDefinition", new object[]
				{
					Schema.EventTypes.DLPPolicyHits.ToString(),
					Schema.EventTypes.DLPPolicyOverride.ToString(),
					Schema.EventTypes.DLPPolicyFalsePositive.ToString()
				}),
				this.CreateDataTableFilter("ActionListQueryDefinition", new object[0]),
				this.CreateDataTableFilter("SummarizeByQueryDefinition", new object[]
				{
					Schema.SummarizeByValues.Action,
					Schema.SummarizeByValues.Domain,
					Schema.SummarizeByValues.TransportRule
				})
			});
			IEnumerable<IGrouping<string, IGrouping<string, MailTrafficPolicyReport>>> enumerable = from trafficReport in data
			group trafficReport by trafficReport.DlpPolicy into policyGroup
			from eventTypeGroup in 
				from trafficReport in policyGroup
				group trafficReport by trafficReport.EventType
			group eventTypeGroup by policyGroup.Key;
			List<Tuple<string, int, int, int>> list = new List<Tuple<string, int, int, int>>();
			string key = Schema.EventTypes.DLPPolicyHits.ToString().ToLower();
			string key2 = Schema.EventTypes.DLPPolicyOverride.ToString().ToLower();
			string key3 = Schema.EventTypes.DLPPolicyFalsePositive.ToString().ToLower();
			Dictionary<string, int> dictionary = new Dictionary<string, int>
			{
				{
					key,
					0
				},
				{
					key2,
					0
				},
				{
					key3,
					0
				}
			};
			foreach (IGrouping<string, IGrouping<string, MailTrafficPolicyReport>> grouping in enumerable)
			{
				dictionary[key] = 0;
				dictionary[key2] = 0;
				dictionary[key3] = 0;
				foreach (IGrouping<string, MailTrafficPolicyReport> grouping2 in grouping)
				{
					dictionary[grouping2.Key.ToLower()] = grouping2.Sum((MailTrafficPolicyReport report) => report.MessageCount);
				}
				list.Add(Tuple.Create<string, int, int, int>(grouping.Key, dictionary[key], dictionary[key2], dictionary[key3]));
			}
			int count = (base.Page - 1) * base.PageSize;
			return (from tuple in (from tuple in list
			orderby tuple.Item2 descending
			select tuple).Skip(count).Take(base.PageSize)
			select new MailTrafficSummaryReport
			{
				C1 = tuple.Item1,
				C2 = tuple.Item2.ToString(),
				C3 = tuple.Item3.ToString(),
				C4 = tuple.Item4.ToString()
			}).ToList<MailTrafficSummaryReport>();
		}

		internal IReadOnlyList<MailTrafficSummaryReport> AggregateTransportRuleHits(Schema.DirectionValues direction)
		{
			IReadOnlyList<MailTrafficPolicyReport> data = this.GetData<MailTrafficPolicyReport>(new List<ComparisonFilter>
			{
				this.CreateDataTableFilter("DirectionListQueryDefinition", new object[]
				{
					direction.ToString()
				}),
				this.CreateFilter("AggregateByQueryDefinition", Schema.AggregateByValues.Summary.ToString()),
				this.CreateDataTableFilter("EventTypeListQueryDefinition", new object[]
				{
					Schema.EventTypes.TransportRuleHits.ToString()
				}),
				this.CreateDataTableFilter("ActionListQueryDefinition", new object[]
				{
					Schema.Actions.SetAuditSeverityLow.ToString(),
					Schema.Actions.SetAuditSeverityMedium.ToString(),
					Schema.Actions.SetAuditSeverityHigh.ToString()
				}),
				this.CreateDataTableFilter("SummarizeByQueryDefinition", new object[]
				{
					Schema.SummarizeByValues.Domain.ToString(),
					Schema.SummarizeByValues.EventType.ToString()
				})
			});
			IEnumerable<IGrouping<string, IGrouping<string, MailTrafficPolicyReport>>> enumerable = from trafficReport in data
			group trafficReport by trafficReport.TransportRule into ruleGroup
			from actionGroup in 
				from trafficReport in ruleGroup
				group trafficReport by trafficReport.Action
			group actionGroup by ruleGroup.Key;
			List<Tuple<string, string, int>> list = new List<Tuple<string, string, int>>();
			foreach (IGrouping<string, IGrouping<string, MailTrafficPolicyReport>> grouping in enumerable)
			{
				foreach (IGrouping<string, MailTrafficPolicyReport> grouping2 in grouping)
				{
					list.Add(Tuple.Create<string, string, int>(grouping.Key, grouping2.Key, grouping2.Sum((MailTrafficPolicyReport report) => report.MessageCount)));
				}
			}
			int count = (base.Page - 1) * base.PageSize;
			return (from tuple in (from tuple in list
			orderby tuple.Item3 descending
			select tuple).Skip(count).Take(base.PageSize)
			select new MailTrafficSummaryReport
			{
				C1 = tuple.Item1,
				C2 = tuple.Item2,
				C3 = tuple.Item3.ToString()
			}).ToList<MailTrafficSummaryReport>();
		}

		internal IReadOnlyList<MailTrafficSummaryReport> AggregateDLPPolicyRuleHits(Schema.DirectionValues direction)
		{
			IReadOnlyList<MailTrafficPolicyReport> data = this.GetData<MailTrafficPolicyReport>(new List<ComparisonFilter>
			{
				this.CreateDataTableFilter("DirectionListQueryDefinition", new object[]
				{
					direction.ToString()
				}),
				this.CreateFilter("AggregateByQueryDefinition", Schema.AggregateByValues.Summary.ToString()),
				this.CreateDataTableFilter("EventTypeListQueryDefinition", new object[]
				{
					Schema.EventTypes.DLPRuleHits.ToString()
				}),
				this.CreateDataTableFilter("ActionListQueryDefinition", new object[]
				{
					Schema.Actions.SetAuditSeverityLow.ToString(),
					Schema.Actions.SetAuditSeverityMedium.ToString(),
					Schema.Actions.SetAuditSeverityHigh.ToString()
				}),
				this.CreateDataTableFilter("SummarizeByQueryDefinition", new object[]
				{
					Schema.SummarizeByValues.Domain.ToString(),
					Schema.SummarizeByValues.EventType.ToString()
				})
			});
			var enumerable = from trafficReport in data
			group trafficReport by new
			{
				trafficReport.DlpPolicy,
				trafficReport.TransportRule
			} into policyGroup
			from ruleGroup in 
				from trafficReport in policyGroup
				group trafficReport by trafficReport.Action
			group ruleGroup by policyGroup.Key;
			List<Tuple<string, string, string, int>> list = new List<Tuple<string, string, string, int>>();
			foreach (var grouping in enumerable)
			{
				foreach (IGrouping<string, MailTrafficPolicyReport> grouping2 in grouping)
				{
					list.Add(Tuple.Create<string, string, string, int>(grouping.Key.DlpPolicy, grouping.Key.TransportRule, grouping2.Key, grouping2.Sum((MailTrafficPolicyReport report) => report.MessageCount)));
				}
			}
			int count = (base.Page - 1) * base.PageSize;
			return (from tuple in (from tuple in list
			orderby tuple.Item4 descending
			select tuple).Skip(count).Take(base.PageSize)
			select new MailTrafficSummaryReport
			{
				C1 = tuple.Item1,
				C2 = tuple.Item2,
				C3 = tuple.Item3,
				C4 = tuple.Item4.ToString()
			}).ToList<MailTrafficSummaryReport>();
		}

		internal IReadOnlyList<MailTrafficSummaryReport> AggregateTopTraffic(string eventType, params string[] directions)
		{
			IReadOnlyList<MailTrafficTopReport> data = this.GetData<MailTrafficTopReport>(new List<ComparisonFilter>
			{
				this.CreateDataTableFilter("DirectionListQueryDefinition", directions),
				this.CreateFilter("AggregateByQueryDefinition", Schema.AggregateByValues.Summary.ToString()),
				this.CreateDataTableFilter("EventTypeListQueryDefinition", new object[]
				{
					eventType
				}),
				this.CreateDataTableFilter("SummarizeByQueryDefinition", new object[]
				{
					Schema.SummarizeByValues.Action.ToString(),
					Schema.SummarizeByValues.DlpPolicy.ToString(),
					Schema.SummarizeByValues.Domain.ToString(),
					Schema.SummarizeByValues.EventType.ToString(),
					Schema.SummarizeByValues.TransportRule.ToString()
				})
			});
			IEnumerable<IGrouping<string, MailTrafficTopReport>> enumerable = from trafficReport in data
			group trafficReport by trafficReport.Name into userGroup
			select userGroup;
			List<Tuple<string, int>> list = new List<Tuple<string, int>>();
			foreach (IGrouping<string, MailTrafficTopReport> grouping in enumerable)
			{
				list.Add(Tuple.Create<string, int>(grouping.Key, grouping.Sum((MailTrafficTopReport report) => report.MessageCount)));
			}
			int count = (base.Page - 1) * base.PageSize;
			return (from tuple in (from tuple in list
			orderby tuple.Item2 descending
			select tuple).Skip(count).Take(base.PageSize)
			select new MailTrafficSummaryReport
			{
				C1 = tuple.Item1,
				C2 = tuple.Item2.ToString()
			}).ToList<MailTrafficSummaryReport>();
		}

		private void SetDefaultDates()
		{
			DateTime value = (DateTime)ExDateTime.UtcNow;
			this.EndDate = new DateTime?(value);
			this.StartDate = new DateTime?(value.AddDays(-14.0));
		}

		private const int DefaultDateOffset = -14;

		private Dictionary<GetMailTrafficSummaryReport.Categories, Tuple<string, GetMailTrafficSummaryReport.AggregateDelegate>> mappings = new Dictionary<GetMailTrafficSummaryReport.Categories, Tuple<string, GetMailTrafficSummaryReport.AggregateDelegate>>();

		private enum Categories
		{
			InboundDLPHits,
			OutboundDLPHits,
			InboundTransportRuleHits,
			OutboundTransportRuleHits,
			InboundDLPPolicyRuleHits,
			OutboundDLPPolicyRuleHits,
			TopSpamRecipient,
			TopMailSender,
			TopMailRecipient,
			TopMalwareRecipient,
			TopMalware
		}

		private delegate IReadOnlyList<MailTrafficSummaryReport> AggregateDelegate();
	}
}
