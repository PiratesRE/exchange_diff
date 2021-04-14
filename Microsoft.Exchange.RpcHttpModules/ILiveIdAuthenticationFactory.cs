using System;
using Microsoft.Exchange.Security.Authentication;

namespace Microsoft.Exchange.RpcHttpModules
{
	internal interface ILiveIdAuthenticationFactory
	{
		ILiveIdBasicAuthentication CreateLiveIdAuthentication();
	}
}
