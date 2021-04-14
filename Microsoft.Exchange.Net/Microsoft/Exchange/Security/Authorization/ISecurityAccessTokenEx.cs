using System;
using System.Security.Principal;

namespace Microsoft.Exchange.Security.Authorization
{
	public interface ISecurityAccessTokenEx
	{
		SecurityIdentifier UserSid { get; }

		SidBinaryAndAttributes[] GroupSids { get; }

		SidBinaryAndAttributes[] RestrictedGroupSids { get; }
	}
}
