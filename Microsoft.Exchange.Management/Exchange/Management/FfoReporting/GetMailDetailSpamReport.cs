using System;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Management.FfoReporting.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.FfoReporting
{
	[Cmdlet("Get", "MailDetailSpamReport")]
	[OutputType(new Type[]
	{
		typeof(MailDetailSpamReport)
	})]
	public sealed class GetMailDetailSpamReport : DetailTask<MailDetailSpamReport>
	{
		public GetMailDetailSpamReport() : base("Microsoft.Exchange.Hygiene.Data.MessageTrace.Reports.SpamMessageDetail, Microsoft.Exchange.Hygiene.Data")
		{
			this.EventType = new MultiValuedProperty<string>();
			this.EventType.Add(Schema.EventTypes.SpamContentFiltered.ToString());
		}

		[QueryParameter("EventTypeListQueryDefinition", new string[]
		{

		})]
		[Parameter(Mandatory = false)]
		[CmdletValidator("ValidateEnum", new object[]
		{
			typeof(Schema.EventTypes),
			Schema.EventTypes.SpamContentFiltered | Schema.EventTypes.SpamEnvelopeBlock | Schema.EventTypes.SpamIPBlock | Schema.EventTypes.SpamDBEBFilter
		}, ErrorMessage = Strings.IDs.InvalidEventType)]
		public MultiValuedProperty<string> EventType { get; set; }

		private const Schema.EventTypes SubsetEventTypes = Schema.EventTypes.SpamContentFiltered | Schema.EventTypes.SpamEnvelopeBlock | Schema.EventTypes.SpamIPBlock | Schema.EventTypes.SpamDBEBFilter;
	}
}
