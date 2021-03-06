using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("ResetUMMailboxResponseMessageType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class ResetUMMailboxResponseMessage : ResponseMessage
	{
		public ResetUMMailboxResponseMessage()
		{
		}

		internal ResetUMMailboxResponseMessage(ServiceResultCode code, ServiceError error, ResetUMMailboxResponseMessage response) : base(code, error)
		{
		}

		public override ResponseType GetResponseType()
		{
			return ResponseType.ResetUMMailboxResponseMessage;
		}
	}
}
