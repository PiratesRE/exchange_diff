using System;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class CallAnsweringCallHandler : ICallHandler
	{
		public void HandleCall(CafeRoutingContext context)
		{
			ValidateArgument.NotNull(context, "RoutingContext");
			if (context.DivertedUser != null)
			{
				context.Tracer.Trace("CallAnsweringCallHandler : TryHandleCall : Diverted User = {0}", new object[]
				{
					context.DivertedUser.DisplayName
				});
				context.RedirectUri = RedirectionTarget.Instance.GetForCallAnsweringCall(context.DivertedUser, context).Uri;
			}
		}
	}
}
