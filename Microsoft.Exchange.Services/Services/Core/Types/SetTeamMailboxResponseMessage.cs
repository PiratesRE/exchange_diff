using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("SetTeamMailboxResponseMessageType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class SetTeamMailboxResponseMessage : ResponseMessage
	{
		public SetTeamMailboxResponseMessage()
		{
		}

		public override ResponseType GetResponseType()
		{
			return ResponseType.SetTeamMailboxResponseMessage;
		}

		internal SetTeamMailboxResponseMessage(ServiceResultCode code, ServiceError error) : base(code, error)
		{
		}
	}
}
