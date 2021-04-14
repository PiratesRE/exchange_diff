using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("CreateUMPromptResponseMessageType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class CreateUMPromptResponseMessage : ResponseMessage
	{
		public CreateUMPromptResponseMessage()
		{
		}

		internal CreateUMPromptResponseMessage(ServiceResultCode code, ServiceError error) : base(code, error)
		{
		}

		public override ResponseType GetResponseType()
		{
			return ResponseType.CreateUMPromptResponseMessage;
		}
	}
}
