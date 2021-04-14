using System;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf;
using Microsoft.Online.BOX.UI.Shell;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class GetBposShellInfoNavBarData : ServiceCommand<NavBarData>
	{
		public GetBposShellInfoNavBarData(CallContext callContext) : base(callContext)
		{
		}

		protected override NavBarData InternalExecute()
		{
			AuthZClientInfo effectiveCaller = CallContext.Current.EffectiveCaller;
			UserContext userContext = UserContextManager.GetUserContext(CallContext.Current.HttpContext, effectiveCaller, true);
			if (!userContext.IsBposUser)
			{
				return null;
			}
			BposShellInfoAssetReader bposShellInfoAssetReader = userContext.BposShellInfoAssetReader;
			BposAssetReader<BposShellInfo>.RegisterEvents(base.GetType().Name);
			return bposShellInfoAssetReader.GetData(effectiveCaller).NavBarData;
		}
	}
}
