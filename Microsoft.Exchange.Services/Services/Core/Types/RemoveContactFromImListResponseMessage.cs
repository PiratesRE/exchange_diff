using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("RemoveContactFromImListResponseMessageType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class RemoveContactFromImListResponseMessage : ResponseMessage
	{
		public RemoveContactFromImListResponseMessage()
		{
		}

		internal RemoveContactFromImListResponseMessage(ServiceResultCode code, ServiceError error) : base(code, error)
		{
		}

		public override ResponseType GetResponseType()
		{
			return ResponseType.RemoveContactFromImListResponseMessage;
		}
	}
}
