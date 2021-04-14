using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("RemoveImGroupResponseMessageType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class RemoveImGroupResponseMessage : ResponseMessage
	{
		public RemoveImGroupResponseMessage()
		{
		}

		internal RemoveImGroupResponseMessage(ServiceResultCode code, ServiceError error) : base(code, error)
		{
		}

		public override ResponseType GetResponseType()
		{
			return ResponseType.RemoveImGroupResponseMessage;
		}
	}
}
