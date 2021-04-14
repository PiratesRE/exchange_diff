using System;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class ResetPresence : InstantMessageCommandBase<int>
	{
		public ResetPresence(CallContext callContext) : base(callContext)
		{
		}

		protected override int InternalExecute()
		{
			UserContext userContext = UserContextManager.GetUserContext(CallContext.Current.HttpContext, CallContext.Current.EffectiveCaller, true);
			if (userContext != null)
			{
				userContext.UpdateLastUserRequestTime();
				return 0;
			}
			return -10;
		}
	}
}
