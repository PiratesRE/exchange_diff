using System;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class InstantMessageSignOut : InstantMessageCommandBase<int>
	{
		public InstantMessageSignOut(CallContext callContext) : base(callContext)
		{
		}

		protected override int InternalExecute()
		{
			InstantMessageOperationError instantMessageOperationError = this.SignOut();
			OwaApplication.GetRequestDetailsLogger.Set(InstantMessagingLogMetadata.OperationErrorCode, instantMessageOperationError);
			return (int)instantMessageOperationError;
		}

		protected InstantMessageOperationError SignOut()
		{
			UserContext userContext = UserContextManager.GetUserContext(CallContext.Current.HttpContext, CallContext.Current.EffectiveCaller, true);
			if (!userContext.IsInstantMessageEnabled)
			{
				return InstantMessageOperationError.NotEnabled;
			}
			if (userContext.InstantMessageManager == null)
			{
				return InstantMessageOperationError.NotConfigured;
			}
			userContext.InstantMessageManager.SignOut();
			InstantMessageUtilities.SetSignedOutFlag(base.MailboxIdentityMailboxSession, true);
			return InstantMessageOperationError.Success;
		}
	}
}
