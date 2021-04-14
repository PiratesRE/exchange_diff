using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Services;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Diagnostics;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core.ServiceCommands
{
	internal class RemoveFavoriteCommand : InstantMessageCommandBase<bool>
	{
		static RemoveFavoriteCommand()
		{
			OwsLogRegistry.Register(OwaApplication.GetRequestDetailsLogger.Get(ExtensibleLoggerMetadata.EventId), typeof(RemoveFavoriteMetadata), new Type[0]);
		}

		public RemoveFavoriteCommand(CallContext callContext, ItemId personaId) : base(callContext)
		{
			WcfServiceCommandBase.ThrowIfNull(personaId, "personaId", "RemoveFavoriteCommand");
			this.personaId = personaId;
			this.logger = callContext.ProtocolLog;
		}

		protected override bool InternalExecute()
		{
			return new RemoveFavorite(new XSOFactory(), base.MailboxIdentityMailboxSession, this.logger, this.personaId).Execute();
		}

		private readonly ItemId personaId;

		private readonly RequestDetailsLogger logger;
	}
}
