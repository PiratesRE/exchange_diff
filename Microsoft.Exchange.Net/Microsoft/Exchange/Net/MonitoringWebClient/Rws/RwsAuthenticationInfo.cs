using System;
using System.Security;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.Net.MonitoringWebClient.Rws
{
	internal class RwsAuthenticationInfo
	{
		public CommonAccessToken Token { get; private set; }

		public string UserName { get; private set; }

		public string UserDomain { get; private set; }

		public SecureString Password { get; private set; }

		public RwsAuthenticationType AuthenticationType { get; private set; }

		public RwsAuthenticationInfo(CommonAccessToken token)
		{
			if (token == null)
			{
				throw new ArgumentNullException("token");
			}
			this.AuthenticationType = RwsAuthenticationType.Brick;
			this.Token = token;
		}

		public RwsAuthenticationInfo(string userName, string userDomain, SecureString password)
		{
			if (userName == null)
			{
				throw new ArgumentNullException("userName");
			}
			if (userDomain == null)
			{
				throw new ArgumentNullException("userDomain");
			}
			if (password == null)
			{
				throw new ArgumentNullException("password");
			}
			this.AuthenticationType = RwsAuthenticationType.LiveIdBasic;
			this.UserName = userName;
			this.UserDomain = userDomain;
			this.Password = password;
		}
	}
}
