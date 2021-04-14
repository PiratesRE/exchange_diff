using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class GetInstantMessageProxySettings : InstantMessageCommandBase<ProxySettings[]>
	{
		public GetInstantMessageProxySettings(CallContext callContext, string[] userPrincipalNames) : base(callContext)
		{
			ExAssert.RetailAssert(userPrincipalNames != null, "userPrincipalNames is null");
			this.userPrincipalNames = userPrincipalNames;
		}

		protected override ProxySettings[] InternalExecute()
		{
			UserContext userContext = UserContextManager.GetUserContext(CallContext.Current.HttpContext, CallContext.Current.EffectiveCaller, true);
			if (this.userPrincipalNames.Length > 0)
			{
				return InstantMessageUtilities.GetProxySettings(this.userPrincipalNames, userContext);
			}
			return new ProxySettings[0];
		}

		private string[] userPrincipalNames;
	}
}
