using System;
using System.Security;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Security.Authentication;

namespace Microsoft.Exchange.PopImap.Core
{
	internal interface IProxyLogin
	{
		string UserName { get; set; }

		SecureString Password { get; set; }

		string ClientIp { get; set; }

		string ClientPort { get; set; }

		string AuthenticationType { get; set; }

		string AuthenticationError { get; set; }

		string ProxyDestination { get; set; }

		ILiveIdBasicAuthentication LiveIdBasicAuth { get; set; }

		ADUser AdUser { get; set; }
	}
}
