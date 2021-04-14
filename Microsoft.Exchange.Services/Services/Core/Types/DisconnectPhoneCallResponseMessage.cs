using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType("DisconnectPhoneCallResponseMessageType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class DisconnectPhoneCallResponseMessage : ResponseMessage
	{
		public DisconnectPhoneCallResponseMessage()
		{
		}

		internal DisconnectPhoneCallResponseMessage(ServiceResultCode code, ServiceError error) : base(code, error)
		{
		}

		public override ResponseType GetResponseType()
		{
			return ResponseType.DisconnectPhoneCallResponseMessage;
		}
	}
}
