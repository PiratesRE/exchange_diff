using System;
using Microsoft.Exchange.Connections.Imap;

namespace Microsoft.Exchange.Connections.Common
{
	internal static class ImapHelperMethods
	{
		public static string ToStringParameterValue(ImapAuthenticationMechanism authMechanism)
		{
			return authMechanism.ToString();
		}

		public static ImapAuthenticationMechanism ToImapAuthenticationMechanism(string authMechanism)
		{
			if (string.IsNullOrWhiteSpace(authMechanism))
			{
				return ImapAuthenticationMechanism.Basic;
			}
			return (ImapAuthenticationMechanism)Enum.Parse(typeof(ImapAuthenticationMechanism), authMechanism, true);
		}

		public static string ToStringParameterValue(ImapSecurityMechanism securityMechanism)
		{
			return securityMechanism.ToString();
		}

		public static ImapSecurityMechanism ToImapSecurityMechanism(string securityMechanism)
		{
			if (string.IsNullOrWhiteSpace(securityMechanism))
			{
				return ImapSecurityMechanism.None;
			}
			return (ImapSecurityMechanism)Enum.Parse(typeof(ImapSecurityMechanism), securityMechanism, true);
		}
	}
}
