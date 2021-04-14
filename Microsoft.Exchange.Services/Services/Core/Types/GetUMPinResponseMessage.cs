using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.UM.Rpc;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType("GetUMPinResponseMessageType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class GetUMPinResponseMessage : ResponseMessage
	{
		[DataMember(Name = "PinInfo")]
		[XmlElement("PinInfo")]
		public PINInfo PinInfo { get; set; }

		public GetUMPinResponseMessage()
		{
		}

		internal GetUMPinResponseMessage(ServiceResultCode code, ServiceError error, GetUMPinResponseMessage response) : base(code, error)
		{
			if (response != null)
			{
				this.PinInfo = response.PinInfo;
			}
		}

		public override ResponseType GetResponseType()
		{
			return ResponseType.GetUMPinResponseMessage;
		}
	}
}
