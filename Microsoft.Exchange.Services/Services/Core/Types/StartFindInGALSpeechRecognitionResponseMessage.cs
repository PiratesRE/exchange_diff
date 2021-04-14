using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType("AddSpeechRecognitionResponseMessageType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class StartFindInGALSpeechRecognitionResponseMessage : ResponseMessage
	{
		public StartFindInGALSpeechRecognitionResponseMessage()
		{
		}

		internal StartFindInGALSpeechRecognitionResponseMessage(ServiceResultCode code, ServiceError error, RecognitionId recognitionId) : base(code, error)
		{
			this.RecognitionId = recognitionId;
		}

		[XmlElement(ElementName = "RecognitionId", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		[DataMember(Name = "RecognitionId", IsRequired = false, EmitDefaultValue = false)]
		public RecognitionId RecognitionId { get; set; }

		public override ResponseType GetResponseType()
		{
			return ResponseType.StartFindInGALSpeechRecognitionResponseMessage;
		}
	}
}
