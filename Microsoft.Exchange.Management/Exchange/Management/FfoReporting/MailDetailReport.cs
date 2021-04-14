using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Management.FfoReporting.Common;

namespace Microsoft.Exchange.Management.FfoReporting
{
	[Serializable]
	public class MailDetailReport : TrafficObject
	{
		[DalConversion("OrganizationFromTask", "Organization", new string[]
		{

		})]
		public string Organization { get; private set; }

		[Redact]
		[DalConversion("DefaultSerializer", "Domain", new string[]
		{

		})]
		[ODataInput("Domain")]
		public string Domain { get; private set; }

		[DalConversion("DefaultSerializer", "Date", new string[]
		{

		})]
		public DateTime Date { get; private set; }

		[DalConversion("DefaultSerializer", "MessageId", new string[]
		{

		})]
		public string MessageId { get; private set; }

		[DalConversion("DefaultSerializer", "Direction", new string[]
		{

		})]
		[ODataInput("Direction")]
		public string Direction { get; private set; }

		[DalConversion("DefaultSerializer", "RecipientAddress", new string[]
		{

		})]
		[Redact(RedactAs = typeof(SmtpAddress))]
		public string RecipientAddress { get; private set; }

		[Redact(RedactAs = typeof(SmtpAddress))]
		[DalConversion("DefaultSerializer", "SenderAddress", new string[]
		{

		})]
		public string SenderAddress { get; private set; }

		[DalConversion("DefaultSerializer", "SenderIP", new string[]
		{

		})]
		[Redact]
		public string SenderIP { get; private set; }

		[DalConversion("DefaultSerializer", "Subject", new string[]
		{

		})]
		[Redact]
		public string Subject { get; private set; }

		[DalConversion("DefaultSerializer", "MessageSize", new string[]
		{

		})]
		public int MessageSize { get; private set; }

		[DalConversion("DefaultSerializer", "InternalMessageId", new string[]
		{

		})]
		public Guid MessageTraceId { get; private set; }

		[ODataInput("EventType")]
		public string EventType { get; private set; }
	}
}
