using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("AddImContactToGroupResponseMessageType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class AddImContactToGroupResponseMessage : ResponseMessage
	{
		public AddImContactToGroupResponseMessage()
		{
		}

		internal AddImContactToGroupResponseMessage(ServiceResultCode code, ServiceError error) : base(code, error)
		{
		}

		public override ResponseType GetResponseType()
		{
			return ResponseType.AddImContactToGroupResponseMessage;
		}
	}
}
