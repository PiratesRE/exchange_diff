using System;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class AutoAttendantCallHandler : ICallHandler
	{
		public void HandleCall(CafeRoutingContext context)
		{
			ValidateArgument.NotNull(context, "RoutingContext");
			if (context.AutoAttendant != null)
			{
				context.Tracer.Trace("AutoAttendantCallHandler : TryHandleCall AA = {0}", new object[]
				{
					context.AutoAttendant.Name
				});
				context.RedirectUri = RedirectionTarget.Instance.GetForNonUserSpecificCall(context.AutoAttendant.OrganizationId, context).Uri;
			}
		}
	}
}
