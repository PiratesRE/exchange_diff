using System;
using System.Net;
using System.Security;
using Microsoft.Exchange.Connections.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Pop
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	public class Pop3AuthenticationParameters : AuthenticationParameters
	{
		public Pop3AuthenticationParameters(string userName, SecureString userPassword, Pop3AuthenticationMechanism pop3AuthenticationMechanism, Pop3SecurityMechanism pop3SecurityMechanism) : base(userName, userPassword)
		{
			this.Pop3AuthenticationMechanism = pop3AuthenticationMechanism;
			this.Pop3SecurityMechanism = pop3SecurityMechanism;
		}

		public Pop3AuthenticationParameters(NetworkCredential networkCredential, Pop3AuthenticationMechanism pop3AuthenticationMechanism, Pop3SecurityMechanism pop3SecurityMechanism) : base(networkCredential)
		{
			this.Pop3AuthenticationMechanism = pop3AuthenticationMechanism;
			this.Pop3SecurityMechanism = pop3SecurityMechanism;
		}

		public Pop3AuthenticationMechanism Pop3AuthenticationMechanism { get; private set; }

		public Pop3SecurityMechanism Pop3SecurityMechanism { get; private set; }
	}
}
