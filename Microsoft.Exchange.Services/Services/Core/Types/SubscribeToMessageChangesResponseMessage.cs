using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	public class SubscribeToMessageChangesResponseMessage : ResponseMessage
	{
		public SubscribeToMessageChangesResponseMessage()
		{
		}

		internal SubscribeToMessageChangesResponseMessage(ServiceResultCode code, ServiceError error) : base(code, error)
		{
		}

		public override ResponseType GetResponseType()
		{
			return ResponseType.SubscribeToMessageChangesResponseMessage;
		}
	}
}
