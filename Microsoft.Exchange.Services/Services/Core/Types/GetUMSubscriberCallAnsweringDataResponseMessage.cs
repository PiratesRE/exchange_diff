using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType("GetUMSubscriberCallAnsweringDataResponseMessageType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class GetUMSubscriberCallAnsweringDataResponseMessage : ResponseMessage
	{
		public GetUMSubscriberCallAnsweringDataResponseMessage()
		{
		}

		internal GetUMSubscriberCallAnsweringDataResponseMessage(ServiceResultCode code, ServiceError error, GetUMSubscriberCallAnsweringDataResponseMessage response) : base(code, error)
		{
			if (response != null)
			{
				this.IsOOF = response.IsOOF;
				this.IsTranscriptionEnabledInMailboxConfig = response.IsTranscriptionEnabledInMailboxConfig;
				this.IsMailboxQuotaExceeded = response.IsMailboxQuotaExceeded;
				this.Greeting = response.Greeting;
				this.GreetingName = response.GreetingName;
				this.TaskTimedOut = response.TaskTimedOut;
			}
		}

		[DataMember(Name = "IsOOF")]
		[XmlElement("IsOOF")]
		public bool IsOOF { get; set; }

		[XmlElement("IsTranscriptionEnabledInMailboxConfig")]
		[DataMember(Name = "IsTranscriptionEnabledInMailboxConfig")]
		public TranscriptionEnabledSetting IsTranscriptionEnabledInMailboxConfig { get; set; }

		[XmlElement("IsMailboxQuotaExceeded")]
		[DataMember(Name = "IsMailboxQuotaExceeded")]
		public bool IsMailboxQuotaExceeded { get; set; }

		[DataMember(Name = "TaskTimedOut")]
		[XmlElement("TaskTimedOut")]
		public bool TaskTimedOut { get; set; }

		[XmlElement("Greeting")]
		[DataMember(Name = "Greeting")]
		public byte[] Greeting { get; set; }

		[XmlElement("GreetingName")]
		[DataMember(Name = "GreetingName")]
		public string GreetingName { get; set; }

		public override ResponseType GetResponseType()
		{
			return ResponseType.GetUMSubscriberCallAnsweringDataResponseMessage;
		}
	}
}
