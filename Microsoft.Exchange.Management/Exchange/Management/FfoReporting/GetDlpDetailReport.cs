using System;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Management.FfoReporting.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.FfoReporting
{
	[Cmdlet("Get", "DlpDetailReport")]
	[OutputType(new Type[]
	{
		typeof(DlpDetailReport)
	})]
	public sealed class GetDlpDetailReport : DetailTask<DlpDetailReport>
	{
		public GetDlpDetailReport() : base("Microsoft.Exchange.Hygiene.Data.MessageTrace.Reports.DLPUnifiedDetail, Microsoft.Exchange.Hygiene.Data")
		{
			this.DlpPolicy = new MultiValuedProperty<string>();
			this.TransportRule = new MultiValuedProperty<string>();
			this.EventType = new MultiValuedProperty<string>();
			this.Source = new MultiValuedProperty<string>();
			Schema.Utilities.AddRange<string>(this.EventType, GetDlpDetailReport.EventTypeStrings);
		}

		[QueryParameter("PolicyListQueryDefinition", new string[]
		{

		})]
		[Parameter(Mandatory = false)]
		[CmdletValidator("ValidateDlpPolicy", new object[]
		{

		}, ErrorMessage = Strings.IDs.InvalidDlpPolicyParameter, ValidatorType = CmdletValidator.ValidatorTypes.PostprocessingWithConfigSession)]
		public MultiValuedProperty<string> DlpPolicy { get; set; }

		[QueryParameter("TransportRuleListQueryDefinition", new string[]
		{

		})]
		[CmdletValidator("ValidateTransportRule", new object[]
		{

		}, ErrorMessage = Strings.IDs.InvalidTransportRule, ValidatorType = CmdletValidator.ValidatorTypes.PostprocessingWithConfigSession)]
		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> TransportRule { get; set; }

		[Parameter(Mandatory = false)]
		[QueryParameter("EventTypeListQueryDefinition", new string[]
		{

		})]
		[CmdletValidator("ValidateEnum", new object[]
		{
			typeof(Schema.EventTypes),
			Schema.EventTypes.DLPActionHits | Schema.EventTypes.DLPPolicyFalsePositive | Schema.EventTypes.DLPPolicyHits | Schema.EventTypes.DLPPolicyOverride | Schema.EventTypes.DLPRuleHits
		}, ErrorMessage = Strings.IDs.InvalidEventType)]
		public MultiValuedProperty<string> EventType { get; set; }

		[Parameter(Mandatory = false)]
		[QueryParameter("SenderAddressListQueryDefinition", new string[]
		{

		})]
		public MultiValuedProperty<string> Actor { get; set; }

		[CmdletValidator("ValidateEnum", new object[]
		{
			typeof(Schema.Source),
			Schema.Source.SPO | Schema.Source.ODB
		}, ErrorMessage = Strings.IDs.InvalidSource)]
		[Parameter(Mandatory = false)]
		[QueryParameter("DataSourceListQueryDefinition", new string[]
		{

		})]
		public MultiValuedProperty<string> Source { get; set; }

		private const Schema.EventTypes SubsetEventTypes = Schema.EventTypes.DLPActionHits | Schema.EventTypes.DLPPolicyFalsePositive | Schema.EventTypes.DLPPolicyHits | Schema.EventTypes.DLPPolicyOverride | Schema.EventTypes.DLPRuleHits;

		private const Schema.Source SubsetSources = Schema.Source.SPO | Schema.Source.ODB;

		private static readonly string[] EventTypeStrings = Schema.Utilities.Split(Schema.EventTypes.DLPActionHits | Schema.EventTypes.DLPPolicyFalsePositive | Schema.EventTypes.DLPPolicyHits | Schema.EventTypes.DLPPolicyOverride | Schema.EventTypes.DLPRuleHits);
	}
}
