using System;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Management.FfoReporting.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.FfoReporting
{
	[Cmdlet("Get", "MailDetailDlpPolicyReport")]
	[OutputType(new Type[]
	{
		typeof(MailDetailDlpPolicyReport)
	})]
	public sealed class GetMailDetailDlpPolicyReport : DetailTask<MailDetailDlpPolicyReport>
	{
		public GetMailDetailDlpPolicyReport() : base("Microsoft.Exchange.Hygiene.Data.MessageTrace.Reports.DLPMessageDetail, Microsoft.Exchange.Hygiene.Data")
		{
			this.DlpPolicy = new MultiValuedProperty<string>();
			this.TransportRule = new MultiValuedProperty<string>();
			this.EventType = new MultiValuedProperty<string>();
			Schema.Utilities.AddRange<string>(this.EventType, GetMailDetailDlpPolicyReport.EventTypeStrings);
		}

		[CmdletValidator("ValidateDlpPolicy", new object[]
		{

		}, ErrorMessage = Strings.IDs.InvalidDlpPolicyParameter, ValidatorType = CmdletValidator.ValidatorTypes.PostprocessingWithConfigSession)]
		[Parameter(Mandatory = false)]
		[QueryParameter("PolicyListQueryDefinition", new string[]
		{

		})]
		public MultiValuedProperty<string> DlpPolicy { get; set; }

		[CmdletValidator("ValidateTransportRule", new object[]
		{

		}, ErrorMessage = Strings.IDs.InvalidTransportRule, ValidatorType = CmdletValidator.ValidatorTypes.PostprocessingWithConfigSession)]
		[Parameter(Mandatory = false)]
		[QueryParameter("TransportRuleListQueryDefinition", new string[]
		{

		})]
		public MultiValuedProperty<string> TransportRule { get; set; }

		[CmdletValidator("ValidateEnum", new object[]
		{
			typeof(Schema.EventTypes),
			Schema.EventTypes.DLPActionHits | Schema.EventTypes.DLPPolicyFalsePositive | Schema.EventTypes.DLPPolicyHits | Schema.EventTypes.DLPPolicyOverride | Schema.EventTypes.DLPRuleHits
		}, ErrorMessage = Strings.IDs.InvalidEventType)]
		[Parameter(Mandatory = false)]
		[QueryParameter("EventTypeListQueryDefinition", new string[]
		{

		})]
		public MultiValuedProperty<string> EventType { get; set; }

		private const Schema.EventTypes SubsetEventTypes = Schema.EventTypes.DLPActionHits | Schema.EventTypes.DLPPolicyFalsePositive | Schema.EventTypes.DLPPolicyHits | Schema.EventTypes.DLPPolicyOverride | Schema.EventTypes.DLPRuleHits;

		private static readonly string[] EventTypeStrings = Schema.Utilities.Split(Schema.EventTypes.DLPActionHits | Schema.EventTypes.DLPPolicyFalsePositive | Schema.EventTypes.DLPPolicyHits | Schema.EventTypes.DLPPolicyOverride | Schema.EventTypes.DLPRuleHits);
	}
}
