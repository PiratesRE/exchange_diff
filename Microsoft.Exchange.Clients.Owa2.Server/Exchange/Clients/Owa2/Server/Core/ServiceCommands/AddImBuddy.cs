using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Diagnostics;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core.ServiceCommands
{
	internal class AddImBuddy : InstantMessageCommandBase<bool>
	{
		static AddImBuddy()
		{
			OwsLogRegistry.Register(OwaApplication.GetRequestDetailsLogger.Get(ExtensibleLoggerMetadata.EventId), typeof(InstantMessagingBuddyMetadata), new Type[0]);
		}

		public AddImBuddy(CallContext callContext, InstantMessageBuddy instantMessageBuddy, InstantMessageGroup instantMessageGroup) : base(callContext)
		{
			WcfServiceCommandBase.ThrowIfNull(instantMessageBuddy, "instantMessageBuddy", "AddImBuddy");
			WcfServiceCommandBase.ThrowIfNull(instantMessageGroup, "instantMessageGroup", "AddImBuddy");
			this.instantMessageBuddy = instantMessageBuddy;
			this.instantMessageGroup = instantMessageGroup;
		}

		protected override bool InternalExecute()
		{
			InstantMessageProvider provider = base.Provider;
			if (provider != null)
			{
				provider.AddBuddy(base.MailboxIdentityMailboxSession, this.instantMessageBuddy, this.instantMessageGroup);
				return true;
			}
			return false;
		}

		private InstantMessageBuddy instantMessageBuddy;

		private InstantMessageGroup instantMessageGroup;
	}
}
