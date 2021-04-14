using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("GetUMPromptResponseMessageType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class GetUMPromptResponseMessage : ResponseMessage
	{
		public GetUMPromptResponseMessage()
		{
		}

		internal GetUMPromptResponseMessage(ServiceResultCode code, ServiceError error, GetUMPromptResponseMessage response) : base(code, error)
		{
			if (response != null)
			{
				this.AudioData = response.AudioData;
			}
		}

		[XmlElement(ElementName = "AudioData", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		[DataMember(Name = "AudioData", IsRequired = false, EmitDefaultValue = false)]
		public string AudioData { get; set; }

		public override ResponseType GetResponseType()
		{
			return ResponseType.GetUMPromptResponseMessage;
		}
	}
}
