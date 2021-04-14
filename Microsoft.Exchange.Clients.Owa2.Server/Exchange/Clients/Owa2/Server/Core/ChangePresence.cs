using System;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class ChangePresence : InstantMessageCommandBase<int>
	{
		public ChangePresence(CallContext callContext, InstantMessagePresenceType? presenceSetting) : base(callContext)
		{
			this.presenceSetting = presenceSetting;
		}

		protected override int InternalExecute()
		{
			InstantMessageProvider provider = base.Provider;
			if (provider == null)
			{
				return -11;
			}
			if (this.presenceSetting != null)
			{
				return provider.PublishSelfPresence(this.presenceSetting.Value);
			}
			provider.PublishResetStatus();
			return 0;
		}

		private readonly InstantMessagePresenceType? presenceSetting;
	}
}
