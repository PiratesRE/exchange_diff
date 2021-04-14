using System;
using System.Net;
using System.Security;
using Microsoft.Exchange.Connections.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Imap
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class ImapAuthenticationParameters : AuthenticationParameters
	{
		public ImapAuthenticationParameters(string userName, SecureString userPassword, ImapAuthenticationMechanism imapAuthenticationMechanism, ImapSecurityMechanism imapSecurityMechanism) : base(userName, userPassword)
		{
			this.ImapAuthenticationMechanism = imapAuthenticationMechanism;
			this.ImapSecurityMechanism = imapSecurityMechanism;
		}

		public ImapAuthenticationParameters(NetworkCredential networkCredential, ImapAuthenticationMechanism imapAuthenticationMechanism, ImapSecurityMechanism imapSecurityMechanism) : base(networkCredential)
		{
			this.ImapAuthenticationMechanism = imapAuthenticationMechanism;
			this.ImapSecurityMechanism = imapSecurityMechanism;
		}

		public ImapAuthenticationMechanism ImapAuthenticationMechanism { get; private set; }

		public ImapSecurityMechanism ImapSecurityMechanism { get; private set; }
	}
}
