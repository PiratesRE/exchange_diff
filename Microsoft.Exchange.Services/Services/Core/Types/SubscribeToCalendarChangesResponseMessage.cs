using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	public class SubscribeToCalendarChangesResponseMessage : ResponseMessage
	{
		public SubscribeToCalendarChangesResponseMessage()
		{
		}

		internal SubscribeToCalendarChangesResponseMessage(ServiceResultCode code, ServiceError error) : base(code, error)
		{
		}
	}
}
