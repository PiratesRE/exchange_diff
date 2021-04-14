using System;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal abstract class InstantMessageConversationCommand : InstantMessageCommandBase<int>
	{
		public InstantMessageConversationCommand(CallContext callContext) : base(callContext)
		{
		}

		protected override int InternalExecute()
		{
			InstantMessageOperationError instantMessageOperationError = this.ExecuteInstantMessagingCommand();
			if (instantMessageOperationError <= InstantMessageOperationError.Success)
			{
				OwaApplication.GetRequestDetailsLogger.Set(InstantMessagingLogMetadata.OperationErrorCode, instantMessageOperationError);
			}
			else
			{
				OwaApplication.GetRequestDetailsLogger.Set(InstantMessagingLogMetadata.ConversationId, (int)instantMessageOperationError);
			}
			return (int)instantMessageOperationError;
		}

		protected abstract InstantMessageOperationError ExecuteInstantMessagingCommand();
	}
}
