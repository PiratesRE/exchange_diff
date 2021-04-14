using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Management.FfoReporting.Common;

namespace Microsoft.Exchange.Management.FfoReporting
{
	[Serializable]
	public class MailDetailTransportRuleReport : DetailObject
	{
		[DalConversion("OrganizationFromTask", "Organization", new string[]
		{

		})]
		public string Organization { get; private set; }

		[DalConversion("DefaultSerializer", "Received", new string[]
		{

		})]
		public DateTime Date { get; private set; }

		[DalConversion("DefaultSerializer", "ClientMessageId", new string[]
		{

		})]
		[ODataInput("MessageId")]
		public string MessageId { get; private set; }

		[DalConversion("DefaultSerializer", "Domain", new string[]
		{

		})]
		[ODataInput("Domain")]
		[Redact]
		public string Domain { get; private set; }

		[Redact]
		[DalConversion("DefaultSerializer", "MessageSubject", new string[]
		{

		})]
		public string Subject { get; private set; }

		[DalConversion("DefaultSerializer", "MessageSize", new string[]
		{

		})]
		public int MessageSize { get; private set; }

		[DalConversion("DefaultSerializer", "Direction", new string[]
		{

		})]
		[ODataInput("Direction")]
		public string Direction { get; private set; }

		[DalConversion("DefaultSerializer", "SenderAddress", new string[]
		{

		})]
		[Redact(RedactAs = typeof(SmtpAddress))]
		[ODataInput("SenderAddress")]
		public string SenderAddress { get; private set; }

		[DalConversion("DefaultSerializer", "RecipientAddress", new string[]
		{

		})]
		[ODataInput("RecipientAddress")]
		[Redact(RedactAs = typeof(SmtpAddress))]
		public string RecipientAddress { get; private set; }

		[DalConversion("DefaultSerializer", "TransportRuleName", new string[]
		{

		})]
		[ODataInput("TransportRule")]
		public string TransportRule { get; private set; }

		[ODataInput("EventType")]
		[DalConversion("DefaultSerializer", "EventType", new string[]
		{

		})]
		public string EventType { get; private set; }

		[DalConversion("DefaultSerializer", "Action", new string[]
		{

		})]
		[ODataInput("Action")]
		public string Action { get; private set; }

		[DalConversion("DefaultSerializer", "InternalMessageId", new string[]
		{

		})]
		public Guid MessageTraceId { get; private set; }
	}
}
