using System;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class RemoveSharedFolders : ServiceCommand<bool>
	{
		public RemoveSharedFolders(CallContext callContext, string primarySMTPAddress) : base(callContext)
		{
			this.primarySMTPAddress = primarySMTPAddress;
		}

		protected override bool InternalExecute()
		{
			return OwaOtherMailboxConfiguration.RemoveOtherMailboxConfig(CallContext.Current, this.primarySMTPAddress);
		}

		private readonly string primarySMTPAddress;
	}
}
