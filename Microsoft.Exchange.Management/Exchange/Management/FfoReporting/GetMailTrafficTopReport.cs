using System;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Management.FfoReporting.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.FfoReporting
{
	[OutputType(new Type[]
	{
		typeof(MailTrafficTopReport)
	})]
	[Cmdlet("Get", "MailTrafficTopReport")]
	public sealed class GetMailTrafficTopReport : TrafficTask<MailTrafficTopReport>
	{
		public GetMailTrafficTopReport() : base("Microsoft.Exchange.Hygiene.Data.MessageTrace.Reports.TopTrafficReport, Microsoft.Exchange.Hygiene.Data")
		{
			this.EventType = new MultiValuedProperty<string>();
			Schema.Utilities.AddRange<string>(this.EventType, GetMailTrafficTopReport.EventTypeStrings);
			this.SummarizeBy = new MultiValuedProperty<string>();
		}

		[Parameter(Mandatory = false)]
		[QueryParameter("EventTypeListQueryDefinition", new string[]
		{

		})]
		[CmdletValidator("ValidateEnum", new object[]
		{
			typeof(Schema.EventTypes),
			Schema.EventTypes.TopMailUser | Schema.EventTypes.TopMalware | Schema.EventTypes.TopMalwareUser | Schema.EventTypes.TopSpamUser
		}, ErrorMessage = Strings.IDs.InvalidEventType)]
		public MultiValuedProperty<string> EventType { get; set; }

		[Parameter(Mandatory = false)]
		[CmdletValidator("ValidateEnum", new object[]
		{
			typeof(Schema.SummarizeByValues),
			Schema.SummarizeByValues.Domain | Schema.SummarizeByValues.EventType
		}, ErrorMessage = Strings.IDs.InvalidSummmarizeBy)]
		[QueryParameter("SummarizeByQueryDefinition", new string[]
		{

		})]
		public MultiValuedProperty<string> SummarizeBy { get; set; }

		private const Schema.EventTypes SubsetEventTypes = Schema.EventTypes.TopMailUser | Schema.EventTypes.TopMalware | Schema.EventTypes.TopMalwareUser | Schema.EventTypes.TopSpamUser;

		private static readonly string[] EventTypeStrings = Schema.Utilities.Split(Schema.EventTypes.TopMailUser | Schema.EventTypes.TopMalware | Schema.EventTypes.TopMalwareUser | Schema.EventTypes.TopSpamUser);
	}
}
