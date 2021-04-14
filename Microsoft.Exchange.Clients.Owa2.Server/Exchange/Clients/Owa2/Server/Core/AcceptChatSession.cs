using System;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class AcceptChatSession : InstantMessageConversationCommand
	{
		public AcceptChatSession(CallContext callContext, int sessionId) : base(callContext)
		{
			this.sessionId = sessionId;
		}

		protected override InstantMessageOperationError ExecuteInstantMessagingCommand()
		{
			InstantMessageProvider provider = base.Provider;
			if (provider != null)
			{
				return (InstantMessageOperationError)provider.ParticipateInConversation(this.sessionId);
			}
			return InstantMessageOperationError.NotSignedIn;
		}

		private readonly int sessionId;
	}
}
