using System;
using System.Security.Principal;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal interface IAuthenticationInfo
	{
		WindowsPrincipal WindowsPrincipal { get; }

		SecurityIdentifier Sid { get; }

		string PrincipalName { get; }

		bool IsCertificateAuthentication { get; }
	}
}
