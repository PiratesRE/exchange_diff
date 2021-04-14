using System;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Management.FfoReporting.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.FfoReporting
{
	[OutputType(new Type[]
	{
		typeof(MailTrafficPolicyReport)
	})]
	[Cmdlet("Get", "MailTrafficPolicyReport")]
	public sealed class GetMailTrafficPolicyReport : TrafficTask<MailTrafficPolicyReport>
	{
		public GetMailTrafficPolicyReport() : base("Microsoft.Exchange.Hygiene.Data.MessageTrace.Reports.PolicyTrafficReport, Microsoft.Exchange.Hygiene.Data")
		{
			this.DlpPolicy = new MultiValuedProperty<string>();
			this.TransportRule = new MultiValuedProperty<string>();
			this.Action = new MultiValuedProperty<string>();
			this.EventType = new MultiValuedProperty<string>();
			Schema.Utilities.AddRange<string>(this.EventType, GetMailTrafficPolicyReport.EventTypeStrings);
			this.SummarizeBy = new MultiValuedProperty<string>();
		}

		[QueryParameter("EventTypeListQueryDefinition", new string[]
		{

		})]
		[CmdletValidator("ValidateEnum", new object[]
		{
			typeof(Schema.EventTypes),
			Schema.EventTypes.DLPActionHits | Schema.EventTypes.DLPPolicyFalsePositive | Schema.EventTypes.DLPPolicyHits | Schema.EventTypes.DLPPolicyOverride | Schema.EventTypes.DLPRuleHits | Schema.EventTypes.TransportRuleActionHits | Schema.EventTypes.TransportRuleHits
		}, ErrorMessage = Strings.IDs.InvalidEventType)]
		[CmdletValidator("ScrubDlp", new object[]
		{
			Schema.EventTypes.DLPActionHits | Schema.EventTypes.DLPPolicyFalsePositive | Schema.EventTypes.DLPPolicyHits | Schema.EventTypes.DLPPolicyOverride | Schema.EventTypes.DLPRuleHits | Schema.EventTypes.TransportRuleActionHits | Schema.EventTypes.TransportRuleHits
		}, ErrorMessage = Strings.IDs.InvalidDlpRoleAccess)]
		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> EventType { get; set; }

		[Parameter(Mandatory = false)]
		[CmdletValidator("ValidateDlpPolicy", new object[]
		{

		}, ErrorMessage = Strings.IDs.InvalidDlpPolicyParameter, ValidatorType = CmdletValidator.ValidatorTypes.PostprocessingWithConfigSession)]
		[QueryParameter("PolicyListQueryDefinition", new string[]
		{

		})]
		public MultiValuedProperty<string> DlpPolicy { get; set; }

		[QueryParameter("RuleListQueryDefinition", new string[]
		{

		})]
		[CmdletValidator("ValidateTransportRule", new object[]
		{

		}, ErrorMessage = Strings.IDs.InvalidTransportRule, ValidatorType = CmdletValidator.ValidatorTypes.PostprocessingWithConfigSession)]
		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> TransportRule { get; set; }

		[CmdletValidator("ValidateEnum", new object[]
		{
			typeof(Schema.Actions)
		}, ErrorMessage = Strings.IDs.InvalidActionParameter)]
		[Parameter(Mandatory = false)]
		[QueryParameter("ActionListQueryDefinition", new string[]
		{

		})]
		public MultiValuedProperty<string> Action { get; set; }

		[CmdletValidator("ValidateEnum", new object[]
		{
			typeof(Schema.SummarizeByValues)
		}, ErrorMessage = Strings.IDs.InvalidSummmarizeBy)]
		[Parameter(Mandatory = false)]
		[QueryParameter("SummarizeByQueryDefinition", new string[]
		{

		})]
		public MultiValuedProperty<string> SummarizeBy { get; set; }

		private const Schema.EventTypes SubsetEventTypes = Schema.EventTypes.DLPActionHits | Schema.EventTypes.DLPPolicyFalsePositive | Schema.EventTypes.DLPPolicyHits | Schema.EventTypes.DLPPolicyOverride | Schema.EventTypes.DLPRuleHits | Schema.EventTypes.TransportRuleActionHits | Schema.EventTypes.TransportRuleHits;

		private static readonly string[] EventTypeStrings = Schema.Utilities.Split(Schema.EventTypes.DLPActionHits | Schema.EventTypes.DLPPolicyFalsePositive | Schema.EventTypes.DLPPolicyHits | Schema.EventTypes.DLPPolicyOverride | Schema.EventTypes.DLPRuleHits | Schema.EventTypes.TransportRuleActionHits | Schema.EventTypes.TransportRuleHits);
	}
}
