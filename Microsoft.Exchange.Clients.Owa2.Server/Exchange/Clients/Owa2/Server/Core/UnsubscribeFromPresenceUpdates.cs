using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Diagnostics;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class UnsubscribeFromPresenceUpdates : InstantMessageCommandBase<int>
	{
		static UnsubscribeFromPresenceUpdates()
		{
			OwsLogRegistry.Register(OwaApplication.GetRequestDetailsLogger.Get(ExtensibleLoggerMetadata.EventId), typeof(InstantMessagingSubscriptionMetadata), new Type[0]);
		}

		public UnsubscribeFromPresenceUpdates(CallContext callContext, string sipUri) : base(callContext)
		{
			WcfServiceCommandBase.ThrowIfNull(sipUri, "sipUri", "UnsubscribeFromPresenceUpdates");
			this.sipUri = sipUri;
		}

		protected override int InternalExecute()
		{
			InstantMessageProvider provider = base.Provider;
			if (provider != null)
			{
				provider.RemoveSubscription(this.sipUri);
				return 0;
			}
			return -11;
		}

		private readonly string sipUri;
	}
}
