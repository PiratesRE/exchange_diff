using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Management.FfoReporting.Common;

namespace Microsoft.Exchange.Management.FfoReporting
{
	[Serializable]
	public class MailDetailDlpPolicyReport : DetailObject
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

		[Redact]
		[ODataInput("Domain")]
		[DalConversion("DefaultSerializer", "Domain", new string[]
		{

		})]
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

		[ODataInput("SenderAddress")]
		[DalConversion("DefaultSerializer", "SenderAddress", new string[]
		{

		})]
		[Redact(RedactAs = typeof(SmtpAddress))]
		public string SenderAddress { get; private set; }

		[ODataInput("RecipientAddress")]
		[Redact(RedactAs = typeof(SmtpAddress))]
		[DalConversion("DefaultSerializer", "RecipientAddress", new string[]
		{

		})]
		public string RecipientAddress { get; private set; }

		[ODataInput("DlpPolicy")]
		[DalConversion("DefaultSerializer", "PolicyName", new string[]
		{

		})]
		public string DlpPolicy { get; private set; }

		[DalConversion("DefaultSerializer", "TransportRuleName", new string[]
		{

		})]
		[ODataInput("TransportRule")]
		public string TransportRule { get; private set; }

		[Redact]
		[DalConversion("DefaultSerializer", "ClassificationSndoverride", new string[]
		{

		})]
		public string UserAction { get; private set; }

		[DalConversion("DefaultSerializer", "ClassificationJustification", new string[]
		{

		})]
		[Redact]
		public string Justification { get; private set; }

		[Redact]
		[DalConversion("DefaultSerializer", "DataClassification", new string[]
		{

		})]
		public string SensitiveInformationType { get; private set; }

		[DalConversion("DefaultSerializer", "ClassificationCount", new string[]
		{

		})]
		public int SensitiveInformationCount { get; private set; }

		[DalConversion("DefaultSerializer", "ClassificationConfidence", new string[]
		{

		})]
		public int SensitiveInformationConfidence { get; private set; }

		[DalConversion("DefaultSerializer", "EventType", new string[]
		{

		})]
		[ODataInput("EventType")]
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
