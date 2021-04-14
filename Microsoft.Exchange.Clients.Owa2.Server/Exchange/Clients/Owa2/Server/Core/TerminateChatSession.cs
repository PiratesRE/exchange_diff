using System;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class TerminateChatSession : InstantMessageCommandBase<bool>
	{
		public TerminateChatSession(CallContext callContext, int sessionId) : base(callContext)
		{
			this.sessionId = sessionId;
		}

		protected override bool InternalExecute()
		{
			InstantMessageProvider provider = base.Provider;
			if (provider != null)
			{
				provider.EndChatSession(this.sessionId, true);
				return true;
			}
			return false;
		}

		private readonly int sessionId;
	}
}
