using System;
using Microsoft.Exchange.Management.FfoReporting.Common;

namespace Microsoft.Exchange.Management.FfoReporting
{
	[Serializable]
	public class MessageTraceDetail : MtrtObject
	{
		public string Organization { get; internal set; }

		[ODataInput("MessageId")]
		public string MessageId { get; internal set; }

		[ODataInput("MessageTraceId")]
		public Guid MessageTraceId { get; internal set; }

		public DateTime Date { get; internal set; }

		public string Event { get; internal set; }

		[Redact]
		public string Action { get; internal set; }

		[Redact]
		public string Detail { get; internal set; }

		[Redact]
		public string Data { get; internal set; }

		[ODataInput("SenderAddress")]
		public string SenderAddress { get; internal set; }

		[ODataInput("RecipientAddress")]
		public string RecipientAddress { get; internal set; }
	}
}
