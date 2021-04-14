using System;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class AddSharedFolders : ServiceCommand<bool>
	{
		public AddSharedFolders(CallContext callContext, string displayName, string primarySMTPAddress) : base(callContext)
		{
			this.displayName = displayName;
			this.primarySMTPAddress = primarySMTPAddress;
		}

		protected override bool InternalExecute()
		{
			return OwaOtherMailboxConfiguration.AddOtherMailboxConfig(CallContext.Current, this.displayName, this.primarySMTPAddress);
		}

		private readonly string displayName;

		private readonly string primarySMTPAddress;
	}
}
