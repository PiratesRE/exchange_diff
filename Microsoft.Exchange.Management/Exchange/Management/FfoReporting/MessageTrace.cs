using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Management.FfoReporting.Common;

namespace Microsoft.Exchange.Management.FfoReporting
{
	[Serializable]
	public class MessageTrace : MtrtObject
	{
		[DalConversion("OrganizationFromTask", "Organization", new string[]
		{

		})]
		public string Organization { get; private set; }

		[ODataInput("MessageId")]
		[DalConversion("DefaultSerializer", "ClientMessageId", new string[]
		{

		})]
		public string MessageId { get; internal set; }

		[DalConversion("DefaultSerializer", "Received", new string[]
		{

		})]
		public DateTime Received { get; internal set; }

		[Redact(RedactAs = typeof(SmtpAddress))]
		[ODataInput("SenderAddress")]
		[DalConversion("DefaultSerializer", "SenderAddress", new string[]
		{

		})]
		public string SenderAddress { get; internal set; }

		[DalConversion("DefaultSerializer", "RecipientAddress", new string[]
		{

		})]
		[ODataInput("RecipientAddress")]
		[Redact(RedactAs = typeof(SmtpAddress))]
		public string RecipientAddress { get; internal set; }

		[DalConversion("DefaultSerializer", "MessageSubject", new string[]
		{

		})]
		[Redact]
		public string Subject { get; internal set; }

		[ODataInput("Status")]
		[DalConversion("DefaultSerializer", "MailDeliveryStatus", new string[]
		{

		})]
		public string Status { get; internal set; }

		[ODataInput("ToIP")]
		[DalConversion("DefaultSerializer", "ToIP", new string[]
		{

		})]
		[Redact]
		public string ToIP { get; internal set; }

		[ODataInput("FromIP")]
		[Redact]
		[DalConversion("DefaultSerializer", "FromIP", new string[]
		{

		})]
		public string FromIP { get; internal set; }

		[DalConversion("DefaultSerializer", "MessageSize", new string[]
		{

		})]
		public int Size { get; internal set; }

		[ODataInput("MessageTraceId")]
		[DalConversion("DefaultSerializer", "InternalMessageId", new string[]
		{

		})]
		public Guid MessageTraceId { get; internal set; }
	}
}
