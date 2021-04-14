using System;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class SendChatMessage : InstantMessageConversationCommand
	{
		public SendChatMessage(CallContext callContext, ChatMessage chatMessage) : base(callContext)
		{
			if (chatMessage == null)
			{
				throw new ArgumentNullException("chatMessage");
			}
			this.chatMessage = chatMessage;
		}

		protected override InstantMessageOperationError ExecuteInstantMessagingCommand()
		{
			InstantMessageProvider provider = base.Provider;
			if (provider == null)
			{
				return InstantMessageOperationError.NotSignedIn;
			}
			if (this.chatMessage.ChatSessionId > 0)
			{
				return (InstantMessageOperationError)provider.SendChatMessage(this.chatMessage);
			}
			return (InstantMessageOperationError)provider.SendNewChatMessage(this.chatMessage);
		}

		private ChatMessage chatMessage;
	}
}
