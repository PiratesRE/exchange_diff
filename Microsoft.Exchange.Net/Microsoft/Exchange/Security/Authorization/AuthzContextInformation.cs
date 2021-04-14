using System;

namespace Microsoft.Exchange.Security.Authorization
{
	internal enum AuthzContextInformation
	{
		UserSid = 1,
		GroupSids,
		RestrictedSids,
		Privileges,
		ExpirationTime,
		ServerContext,
		Identifier
	}
}
