using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("UnpinTeamMailboxResponseMessageType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class UnpinTeamMailboxResponseMessage : ResponseMessage
	{
		public UnpinTeamMailboxResponseMessage()
		{
		}

		public override ResponseType GetResponseType()
		{
			return ResponseType.UnpinTeamMailboxResponseMessage;
		}

		internal UnpinTeamMailboxResponseMessage(ServiceResultCode code, ServiceError error) : base(code, error)
		{
		}
	}
}
