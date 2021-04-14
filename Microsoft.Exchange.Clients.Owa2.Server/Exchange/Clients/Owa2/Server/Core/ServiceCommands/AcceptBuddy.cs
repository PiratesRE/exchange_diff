using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Diagnostics;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core.ServiceCommands
{
	internal class AcceptBuddy : InstantMessageCommandBase<bool>
	{
		static AcceptBuddy()
		{
			OwsLogRegistry.Register(OwaApplication.GetRequestDetailsLogger.Get(ExtensibleLoggerMetadata.EventId), typeof(InstantMessagingBuddyMetadata), new Type[0]);
		}

		public AcceptBuddy(CallContext callContext, InstantMessageBuddy instantMessageBuddy, InstantMessageGroup instantMessageGroup) : base(callContext)
		{
			WcfServiceCommandBase.ThrowIfNull(instantMessageBuddy, "instantMessageBuddy", "AcceptBuddy");
			WcfServiceCommandBase.ThrowIfNull(instantMessageBuddy, "instantMessageGroup", "AcceptBuddy");
			this.instantMessageBuddy = instantMessageBuddy;
			this.instantMessageGroup = instantMessageGroup;
		}

		protected override bool InternalExecute()
		{
			InstantMessageProvider provider = base.Provider;
			if (provider != null)
			{
				provider.AcceptBuddy(base.MailboxIdentityMailboxSession, this.instantMessageBuddy, this.instantMessageGroup);
				return true;
			}
			return false;
		}

		private InstantMessageBuddy instantMessageBuddy;

		private InstantMessageGroup instantMessageGroup;
	}
}
