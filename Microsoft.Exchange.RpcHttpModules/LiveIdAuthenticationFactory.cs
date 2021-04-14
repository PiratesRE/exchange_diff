using System;
using Microsoft.Exchange.Security.Authentication;

namespace Microsoft.Exchange.RpcHttpModules
{
	internal class LiveIdAuthenticationFactory : ILiveIdAuthenticationFactory
	{
		public ILiveIdBasicAuthentication CreateLiveIdAuthentication()
		{
			return new LiveIdBasicAuthentication
			{
				ApplicationName = "Microsoft.Exchange.Rpc.BackEnd"
			};
		}

		internal const string ApplicationName = "Microsoft.Exchange.Rpc.BackEnd";
	}
}
