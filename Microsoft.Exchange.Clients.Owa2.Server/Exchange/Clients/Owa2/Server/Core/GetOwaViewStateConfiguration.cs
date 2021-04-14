using System;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class GetOwaViewStateConfiguration : ServiceCommand<OwaViewStateConfiguration>
	{
		public GetOwaViewStateConfiguration(CallContext callContext) : base(callContext)
		{
		}

		protected override OwaViewStateConfiguration InternalExecute()
		{
			OwaViewStateConfiguration owaViewStateConfiguration = new OwaViewStateConfiguration();
			owaViewStateConfiguration.LoadAll(base.CallContext.SessionCache.GetMailboxIdentityMailboxSession());
			return owaViewStateConfiguration;
		}
	}
}
