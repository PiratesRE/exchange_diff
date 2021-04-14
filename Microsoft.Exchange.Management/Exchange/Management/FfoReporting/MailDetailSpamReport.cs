using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Management.FfoReporting.Common;

namespace Microsoft.Exchange.Management.FfoReporting
{
	[Serializable]
	public class MailDetailSpamReport : DetailObject
	{
		[DalConversion("OrganizationFromTask", "Organization", new string[]
		{

		})]
		public string Organization { get; private set; }

		[DalConversion("DefaultSerializer", "Received", new string[]
		{

		})]
		public DateTime Date { get; private set; }

		[ODataInput("MessageId")]
		[DalConversion("DefaultSerializer", "ClientMessageId", new string[]
		{

		})]
		public string MessageId { get; private set; }

		[Redact]
		[DalConversion("DefaultSerializer", "Domain", new string[]
		{

		})]
		[ODataInput("Domain")]
		public string Domain { get; private set; }

		[DalConversion("DefaultSerializer", "MessageSubject", new string[]
		{

		})]
		[Redact]
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
		[ODataInput("SenderAddress")]
		[Redact(RedactAs = typeof(SmtpAddress))]
		public string SenderAddress { get; private set; }

		[ODataInput("RecipientAddress")]
		[Redact(RedactAs = typeof(SmtpAddress))]
		[DalConversion("DefaultSerializer", "RecipientAddress", new string[]
		{

		})]
		public string RecipientAddress { get; private set; }

		[DalConversion("DefaultSerializer", "EventType", new string[]
		{

		})]
		[ODataInput("EventType")]
		public string EventType { get; private set; }

		[ODataInput("Action")]
		[DalConversion("DefaultSerializer", "Action", new string[]
		{

		})]
		public string Action { get; private set; }

		[DalConversion("DefaultSerializer", "InternalMessageId", new string[]
		{

		})]
		public Guid MessageTraceId { get; private set; }
	}
}
