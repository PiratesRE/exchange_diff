using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType("CompleteFindInGALSpeechRecognitionResponseMessageType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class CompleteFindInGALSpeechRecognitionResponseMessage : ResponseMessage
	{
		public CompleteFindInGALSpeechRecognitionResponseMessage()
		{
		}

		internal CompleteFindInGALSpeechRecognitionResponseMessage(ServiceResultCode code, ServiceError error, RecognitionResult recognitionResult) : base(code, error)
		{
			this.RecognitionResult = recognitionResult;
		}

		[XmlElement(ElementName = "RecognitionResult", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		[DataMember(Name = "RecognitionResult", IsRequired = false, EmitDefaultValue = false)]
		public RecognitionResult RecognitionResult { get; set; }

		public override ResponseType GetResponseType()
		{
			return ResponseType.CompleteFindInGALSpeechRecognitionResponseMessage;
		}
	}
}
