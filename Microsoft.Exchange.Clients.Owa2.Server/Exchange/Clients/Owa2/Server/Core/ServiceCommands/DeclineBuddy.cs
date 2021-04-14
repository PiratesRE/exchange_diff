using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Diagnostics;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core.ServiceCommands
{
	internal class DeclineBuddy : InstantMessageCommandBase<bool>
	{
		static DeclineBuddy()
		{
			OwsLogRegistry.Register(OwaApplication.GetRequestDetailsLogger.Get(ExtensibleLoggerMetadata.EventId), typeof(InstantMessagingBuddyMetadata), new Type[0]);
		}

		public DeclineBuddy(CallContext callContext, InstantMessageBuddy instantMessageBuddy) : base(callContext)
		{
			WcfServiceCommandBase.ThrowIfNull(instantMessageBuddy, "instantMessageBuddy", "DeclineBuddy");
			this.instantMessageBuddy = instantMessageBuddy;
		}

		protected override bool InternalExecute()
		{
			InstantMessageProvider provider = base.Provider;
			if (provider != null)
			{
				provider.DeclineBuddy(this.instantMessageBuddy);
				return true;
			}
			return false;
		}

		private InstantMessageBuddy instantMessageBuddy;
	}
}
