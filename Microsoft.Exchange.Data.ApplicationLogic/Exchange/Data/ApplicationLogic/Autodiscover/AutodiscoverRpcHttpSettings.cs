using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.ApplicationLogic.Autodiscover
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class AutodiscoverRpcHttpSettings
	{
		internal string RpcHttpServer { get; private set; }

		internal bool SslRequired { get; private set; }

		internal string AuthPackageString { get; private set; }

		internal string XropUrl { get; private set; }

		internal AutodiscoverRpcHttpSettings(string rpcHttpServer, bool sslRequired, string authPackageString, string xropUrl)
		{
			this.RpcHttpServer = rpcHttpServer;
			this.SslRequired = sslRequired;
			this.AuthPackageString = authPackageString;
			this.XropUrl = xropUrl;
		}

		internal static AutodiscoverRpcHttpSettings GetRpcHttpAuthSettingsFromService(RpcHttpService service, ClientAccessType clientAccessType, AutodiscoverRpcHttpSettings.AuthMethodGetter authMethodPicker)
		{
			bool flag = clientAccessType == ClientAccessType.Internal;
			bool sslRequired = service.Url.Scheme == Uri.UriSchemeHttps;
			AuthenticationMethod authMethod = authMethodPicker(flag ? service.InternalClientAuthenticationMethod : service.ExternalClientAuthenticationMethod, service.IISAuthenticationMethods, sslRequired);
			string authPackageStringFromAuthMethod = AutodiscoverRpcHttpSettings.GetAuthPackageStringFromAuthMethod(authMethod);
			bool flag2 = service.XropUrl != null;
			return new AutodiscoverRpcHttpSettings(service.Url.DnsSafeHost, sslRequired, authPackageStringFromAuthMethod, flag2 ? service.XropUrl.AbsoluteUri : null);
		}

		internal static AuthenticationMethod UseProvidedAuthenticationMethod(AuthenticationMethod clientAuthenticationMethod, ICollection<AuthenticationMethod> unusedAuthMethods, bool unusedSslRequired)
		{
			return clientAuthenticationMethod;
		}

		internal static string GetAuthPackageStringFromAuthMethod(AuthenticationMethod authMethod)
		{
			string result;
			AutodiscoverRpcHttpSettings.AuthMethodStrings.TryGetValue(authMethod, out result);
			return result;
		}

		internal const string Nego2AuthPackageString = "Nego2";

		internal const string NegotiateAuthPackageString = "Negotiate";

		internal const string NtlmAuthPackageString = "Ntlm";

		internal const string BasicAuthPackageString = "Basic";

		private static readonly Dictionary<AuthenticationMethod, string> AuthMethodStrings = new Dictionary<AuthenticationMethod, string>
		{
			{
				AuthenticationMethod.NegoEx,
				"Nego2"
			},
			{
				AuthenticationMethod.Negotiate,
				"Negotiate"
			},
			{
				AuthenticationMethod.Ntlm,
				"Ntlm"
			},
			{
				AuthenticationMethod.Basic,
				"Basic"
			}
		};

		internal delegate AuthenticationMethod AuthMethodGetter(AuthenticationMethod clientAuthenticationMethod, ICollection<AuthenticationMethod> iisAuthenticationMethods, bool sslRequired);
	}
}
