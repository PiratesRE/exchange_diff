using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("SetImGroupResponseMessageType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class SetImGroupResponseMessage : ResponseMessage
	{
		public SetImGroupResponseMessage()
		{
		}

		internal SetImGroupResponseMessage(ServiceResultCode code, ServiceError error) : base(code, error)
		{
		}

		public override ResponseType GetResponseType()
		{
			return ResponseType.SetImGroupResponseMessage;
		}
	}
}
