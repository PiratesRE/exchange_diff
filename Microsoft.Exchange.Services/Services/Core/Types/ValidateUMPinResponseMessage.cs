using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.UM.Rpc;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType("ValidateUMPinResponseMessageType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class ValidateUMPinResponseMessage : ResponseMessage
	{
		[XmlElement("PinInfo")]
		[DataMember(Name = "PinInfo")]
		public PINInfo PinInfo { get; set; }

		public ValidateUMPinResponseMessage()
		{
		}

		internal ValidateUMPinResponseMessage(ServiceResultCode code, ServiceError error, ValidateUMPinResponseMessage response) : base(code, error)
		{
			if (response != null)
			{
				this.PinInfo = response.PinInfo;
			}
		}

		public override ResponseType GetResponseType()
		{
			return ResponseType.ValidateUMPinResponseMessage;
		}
	}
}
