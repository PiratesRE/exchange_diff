using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	public class SubscribeToConversationChangesResponseMessage : ResponseMessage
	{
		public SubscribeToConversationChangesResponseMessage()
		{
		}

		internal SubscribeToConversationChangesResponseMessage(ServiceResultCode code, ServiceError error) : base(code, error)
		{
		}

		public override ResponseType GetResponseType()
		{
			return ResponseType.SubscribeToConversationChangesResponseMessage;
		}
	}
}
