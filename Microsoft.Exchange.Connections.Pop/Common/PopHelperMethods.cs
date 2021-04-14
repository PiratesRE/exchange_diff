using System;
using Microsoft.Exchange.Connections.Pop;

namespace Microsoft.Exchange.Connections.Common
{
	internal static class PopHelperMethods
	{
		public static string ToStringParameterValue(Pop3SecurityMechanism securityMechanism)
		{
			if (securityMechanism == Pop3SecurityMechanism.None)
			{
				return string.Empty;
			}
			return securityMechanism.ToString();
		}

		public static Pop3SecurityMechanism ToPopSecurityMechanism(string securityMechanism)
		{
			if (string.IsNullOrWhiteSpace(securityMechanism))
			{
				return Pop3SecurityMechanism.None;
			}
			return (Pop3SecurityMechanism)Enum.Parse(typeof(Pop3SecurityMechanism), securityMechanism, true);
		}

		public static string ToStringParameterValue(Pop3AuthenticationMechanism authMechanism)
		{
			return authMechanism.ToString();
		}

		public static Pop3AuthenticationMechanism ToPopAuthenticationMechanism(string authMechanism)
		{
			if (string.IsNullOrWhiteSpace(authMechanism))
			{
				return Pop3AuthenticationMechanism.Basic;
			}
			return (Pop3AuthenticationMechanism)Enum.Parse(typeof(Pop3AuthenticationMechanism), authMechanism, true);
		}
	}
}
