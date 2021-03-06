using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("RemoveImContactFromGroupResponseMessageType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class RemoveImContactFromGroupResponseMessage : ResponseMessage
	{
		public RemoveImContactFromGroupResponseMessage()
		{
		}

		internal RemoveImContactFromGroupResponseMessage(ServiceResultCode code, ServiceError error) : base(code, error)
		{
		}

		public override ResponseType GetResponseType()
		{
			return ResponseType.RemoveImContactFromGroupResponseMessage;
		}
	}
}
