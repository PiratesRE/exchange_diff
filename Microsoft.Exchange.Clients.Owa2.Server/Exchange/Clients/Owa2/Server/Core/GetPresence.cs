using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Diagnostics;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class GetPresence : InstantMessageCommandBase<int>
	{
		static GetPresence()
		{
			OwsLogRegistry.Register(OwaApplication.GetRequestDetailsLogger.Get(ExtensibleLoggerMetadata.EventId), typeof(InstantMessagingQueryPresenceData), new Type[0]);
		}

		public GetPresence(CallContext callContext, string[] sipUris) : base(callContext)
		{
			WcfServiceCommandBase.ThrowIfNull(sipUris, "sipUris", "GetPresence");
			this.sipUris = sipUris;
		}

		protected override int InternalExecute()
		{
			InstantMessageProvider provider = base.Provider;
			if (provider != null)
			{
				provider.QueryPresence(this.sipUris);
				return 0;
			}
			return -11;
		}

		private readonly string[] sipUris;
	}
}
