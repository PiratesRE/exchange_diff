using System;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class GetOtherMailboxConfiguration : ServiceCommand<OwaOtherMailboxConfiguration>
	{
		public GetOtherMailboxConfiguration(CallContext callContext) : base(callContext)
		{
		}

		protected override OwaOtherMailboxConfiguration InternalExecute()
		{
			OwaOtherMailboxConfiguration owaOtherMailboxConfiguration = new OwaOtherMailboxConfiguration();
			owaOtherMailboxConfiguration.LoadAll(base.CallContext);
			return owaOtherMailboxConfiguration;
		}
	}
}
