using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType("SaveUMPinResponseMessageType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class SaveUMPinResponseMessage : ResponseMessage
	{
		public SaveUMPinResponseMessage()
		{
		}

		internal SaveUMPinResponseMessage(ServiceResultCode code, ServiceError error, SaveUMPinResponseMessage response) : base(code, error)
		{
		}

		public override ResponseType GetResponseType()
		{
			return ResponseType.SaveUMPinResponseMessage;
		}
	}
}
