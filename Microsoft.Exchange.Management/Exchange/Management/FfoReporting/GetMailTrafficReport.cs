using System;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Management.FfoReporting.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.FfoReporting
{
	[OutputType(new Type[]
	{
		typeof(MailTrafficReport)
	})]
	[Cmdlet("Get", "MailTrafficReport")]
	public sealed class GetMailTrafficReport : TrafficTask<MailTrafficReport>
	{
		public GetMailTrafficReport() : base("Microsoft.Exchange.Hygiene.Data.MessageTrace.Reports.TrafficReport, Microsoft.Exchange.Hygiene.Data")
		{
			this.Action = new MultiValuedProperty<string>();
			this.EventType = new MultiValuedProperty<string>();
			Schema.Utilities.AddRange<string>(this.EventType, GetMailTrafficReport.EventTypeStrings);
			this.SummarizeBy = new MultiValuedProperty<string>();
		}

		[CmdletValidator("ScrubDlp", new object[]
		{
			Schema.EventTypes.DLPMessages | Schema.EventTypes.GoodMail | Schema.EventTypes.Malware | Schema.EventTypes.SpamContentFiltered | Schema.EventTypes.SpamEnvelopeBlock | Schema.EventTypes.SpamIPBlock | Schema.EventTypes.TransportRuleMessages | Schema.EventTypes.SpamDBEBFilter
		}, ErrorMessage = Strings.IDs.InvalidDlpRoleAccess)]
		[Parameter(Mandatory = false)]
		[QueryParameter("EventTypeListQueryDefinition", new string[]
		{

		})]
		[CmdletValidator("ValidateEnum", new object[]
		{
			typeof(Schema.EventTypes),
			Schema.EventTypes.DLPMessages | Schema.EventTypes.GoodMail | Schema.EventTypes.Malware | Schema.EventTypes.SpamContentFiltered | Schema.EventTypes.SpamEnvelopeBlock | Schema.EventTypes.SpamIPBlock | Schema.EventTypes.TransportRuleMessages | Schema.EventTypes.SpamDBEBFilter
		}, ErrorMessage = Strings.IDs.InvalidEventType)]
		public MultiValuedProperty<string> EventType { get; set; }

		[QueryParameter("ActionListQueryDefinition", new string[]
		{

		})]
		[CmdletValidator("ValidateEnum", new object[]
		{
			typeof(Schema.Actions)
		}, ErrorMessage = Strings.IDs.InvalidActionParameter)]
		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> Action { get; set; }

		[CmdletValidator("ValidateEnum", new object[]
		{
			typeof(Schema.SummarizeByValues),
			Schema.SummarizeByValues.Action | Schema.SummarizeByValues.Domain | Schema.SummarizeByValues.EventType
		}, ErrorMessage = Strings.IDs.InvalidSummmarizeBy)]
		[Parameter(Mandatory = false)]
		[QueryParameter("SummarizeByQueryDefinition", new string[]
		{

		})]
		public MultiValuedProperty<string> SummarizeBy { get; set; }

		private const Schema.EventTypes SubsetEventTypes = Schema.EventTypes.DLPMessages | Schema.EventTypes.GoodMail | Schema.EventTypes.Malware | Schema.EventTypes.SpamContentFiltered | Schema.EventTypes.SpamEnvelopeBlock | Schema.EventTypes.SpamIPBlock | Schema.EventTypes.TransportRuleMessages | Schema.EventTypes.SpamDBEBFilter;

		private static readonly string[] EventTypeStrings = Schema.Utilities.Split(Schema.EventTypes.DLPMessages | Schema.EventTypes.GoodMail | Schema.EventTypes.Malware | Schema.EventTypes.SpamContentFiltered | Schema.EventTypes.SpamEnvelopeBlock | Schema.EventTypes.SpamIPBlock | Schema.EventTypes.TransportRuleMessages | Schema.EventTypes.SpamDBEBFilter);
	}
}
