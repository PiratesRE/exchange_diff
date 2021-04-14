using System;
using System.Net;
using System.Security;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Common
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	public class AuthenticationParameters
	{
		public AuthenticationParameters(string userName, SecureString password)
		{
			this.NetworkCredential = new NetworkCredential(userName, password);
		}

		public AuthenticationParameters(NetworkCredential networkCredential)
		{
			this.NetworkCredential = networkCredential;
		}

		internal NetworkCredential NetworkCredential { get; private set; }
	}
}
