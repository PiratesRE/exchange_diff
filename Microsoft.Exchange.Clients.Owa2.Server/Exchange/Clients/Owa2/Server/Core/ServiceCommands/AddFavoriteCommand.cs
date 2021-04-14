using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Diagnostics;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core.ServiceCommands
{
	internal class AddFavoriteCommand : InstantMessageCommandBase<bool>
	{
		static AddFavoriteCommand()
		{
			OwsLogRegistry.Register(OwaApplication.GetRequestDetailsLogger.Get(ExtensibleLoggerMetadata.EventId), typeof(InstantMessagingBuddyMetadata), new Type[0]);
		}

		public AddFavoriteCommand(CallContext callContext, InstantMessageBuddy instantMessageBuddy) : base(callContext)
		{
			WcfServiceCommandBase.ThrowIfNull(instantMessageBuddy, "instantMessageBuddy", "AddFavoriteCommand");
			this.instantMessageBuddy = instantMessageBuddy;
		}

		protected override bool InternalExecute()
		{
			return new AddFavorite(new XSOFactory(), base.MailboxIdentityMailboxSession, this.instantMessageBuddy).Execute();
		}

		private readonly InstantMessageBuddy instantMessageBuddy;
	}
}
