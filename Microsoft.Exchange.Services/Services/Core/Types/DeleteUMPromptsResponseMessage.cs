using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("DeleteUMPromptsResponseMessageType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class DeleteUMPromptsResponseMessage : ResponseMessage
	{
		public DeleteUMPromptsResponseMessage()
		{
		}

		internal DeleteUMPromptsResponseMessage(ServiceResultCode code, ServiceError error) : base(code, error)
		{
		}

		public override ResponseType GetResponseType()
		{
			return ResponseType.DeleteUMPromptsResponseMessage;
		}
	}
}
