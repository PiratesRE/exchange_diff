using System;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Management.FfoReporting.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.FfoReporting
{
	[OutputType(new Type[]
	{
		typeof(MailDetailReport)
	})]
	[Cmdlet("Get", "MailDetailReport")]
	public sealed class GetMailDetailReport : TrafficTask<MailDetailReport>
	{
		public GetMailDetailReport() : base("Microsoft.Exchange.Hygiene.Data.MessageTrace.Reports.MessageDetailReport, Microsoft.Exchange.Hygiene.Data")
		{
			this.MessageId = new MultiValuedProperty<string>();
			this.MessageTraceId = new MultiValuedProperty<Guid>();
			this.EventType = new MultiValuedProperty<string>();
			Schema.Utilities.AddRange<string>(this.EventType, GetMailDetailReport.EventTypeStrings);
		}

		[CmdletValidator("ValidateEnum", new object[]
		{
			typeof(Schema.EventTypes),
			Schema.EventTypes.GoodMail
		}, ErrorMessage = Strings.IDs.InvalidEventType)]
		[QueryParameter("EventTypeListQueryDefinition", new string[]
		{

		})]
		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> EventType { get; set; }

		[Parameter(Mandatory = false)]
		[QueryParameter("MessageIdListQueryDefinition", new string[]
		{

		})]
		public MultiValuedProperty<string> MessageId { get; set; }

		[Parameter(Mandatory = false)]
		[QueryParameter("InternalMessageIdQueryDefinition", new string[]
		{

		})]
		public MultiValuedProperty<Guid> MessageTraceId { get; set; }

		private const Schema.EventTypes SubsetEventTypes = Schema.EventTypes.GoodMail;

		private static readonly string[] EventTypeStrings = Schema.Utilities.Split(Schema.EventTypes.GoodMail);
	}
}
