using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	public class SubscribeToHierarchyChangesResponseMessage : ResponseMessage
	{
		public SubscribeToHierarchyChangesResponseMessage()
		{
		}

		internal SubscribeToHierarchyChangesResponseMessage(ServiceResultCode code, ServiceError error) : base(code, error)
		{
		}

		public override ResponseType GetResponseType()
		{
			return ResponseType.SubscribeToHierarchyChangesResponseMessage;
		}
	}
}
