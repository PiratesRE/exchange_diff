using System;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Management.FfoReporting.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.FfoReporting
{
	[Cmdlet("Get", "MailDetailTransportRuleReport")]
	[OutputType(new Type[]
	{
		typeof(MailDetailTransportRuleReport)
	})]
	public sealed class GetMailDetailTransportRuleReport : DetailTask<MailDetailTransportRuleReport>
	{
		public GetMailDetailTransportRuleReport() : base("Microsoft.Exchange.Hygiene.Data.MessageTrace.Reports.PolicyMessageDetail, Microsoft.Exchange.Hygiene.Data")
		{
			this.TransportRule = new MultiValuedProperty<string>();
			this.EventType = new MultiValuedProperty<string>();
			Schema.Utilities.AddRange<string>(this.EventType, GetMailDetailTransportRuleReport.EventTypeStrings);
		}

		[CmdletValidator("ValidateTransportRule", new object[]
		{

		}, ErrorMessage = Strings.IDs.InvalidTransportRule, ValidatorType = CmdletValidator.ValidatorTypes.PostprocessingWithConfigSession)]
		[Parameter(Mandatory = false)]
		[QueryParameter("TransportRuleListQueryDefinition", new string[]
		{

		})]
		public MultiValuedProperty<string> TransportRule { get; set; }

		[QueryParameter("EventTypeListQueryDefinition", new string[]
		{

		})]
		[CmdletValidator("ValidateEnum", new object[]
		{
			typeof(Schema.EventTypes),
			Schema.EventTypes.TransportRuleActionHits | Schema.EventTypes.TransportRuleHits
		}, ErrorMessage = Strings.IDs.InvalidEventType)]
		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> EventType { get; set; }

		private const Schema.EventTypes SubsetEventTypes = Schema.EventTypes.TransportRuleActionHits | Schema.EventTypes.TransportRuleHits;

		private static readonly string[] EventTypeStrings = Schema.Utilities.Split(Schema.EventTypes.TransportRuleActionHits | Schema.EventTypes.TransportRuleHits);
	}
}
