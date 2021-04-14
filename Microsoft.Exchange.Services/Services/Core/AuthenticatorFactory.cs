using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.PswsClient;
using Microsoft.Exchange.Security.Authentication;

namespace Microsoft.Exchange.Services.Core
{
	internal static class AuthenticatorFactory
	{
		internal static void SetCreateHook(Func<IAuthenticator> func)
		{
			AuthenticatorFactory.CreateHook = Hookable<Func<IAuthenticator>>.Create(true, func);
		}

		internal static IAuthenticator Create()
		{
			if (AuthenticatorFactory.CreateHook != null)
			{
				return AuthenticatorFactory.CreateHook.Value();
			}
			return Authenticator.NetworkService;
		}

		private static Hookable<Func<IAuthenticator>> CreateHook;
	}
}
