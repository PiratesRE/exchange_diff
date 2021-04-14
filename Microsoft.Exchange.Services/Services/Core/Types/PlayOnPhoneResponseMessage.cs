using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("PlayOnPhoneResponseMessageType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class PlayOnPhoneResponseMessage : ResponseMessage
	{
		public PlayOnPhoneResponseMessage()
		{
		}

		internal PlayOnPhoneResponseMessage(ServiceResultCode code, ServiceError error, PhoneCallId phoneCallId) : base(code, error)
		{
			this.PhoneCallId = phoneCallId;
		}

		[XmlElement(ElementName = "PhoneCallId", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		[DataMember(Name = "PhoneCallId", IsRequired = false, EmitDefaultValue = false)]
		public PhoneCallId PhoneCallId { get; set; }

		public override ResponseType GetResponseType()
		{
			return ResponseType.PlayOnPhoneResponseMessage;
		}
	}
}
