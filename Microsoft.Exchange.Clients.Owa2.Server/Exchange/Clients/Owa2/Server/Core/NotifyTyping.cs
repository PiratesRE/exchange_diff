using System;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class NotifyTyping : InstantMessageCommandBase<bool>
	{
		public NotifyTyping(CallContext callContext, int sessionId, bool typingCancelled) : base(callContext)
		{
			this.sessionId = sessionId;
			this.typingCancelled = typingCancelled;
		}

		protected override bool InternalExecute()
		{
			InstantMessageProvider provider = base.Provider;
			if (provider != null)
			{
				provider.NotifyTyping(this.sessionId, this.typingCancelled);
				return true;
			}
			return false;
		}

		private readonly int sessionId;

		private readonly bool typingCancelled;
	}
}
