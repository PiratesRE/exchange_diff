using System;

namespace Microsoft.Exchange.Security.Authorization
{
	public interface ISecurityAccessToken
	{
		string UserSid { get; set; }

		SidStringAndAttributes[] GroupSids { get; set; }

		SidStringAndAttributes[] RestrictedGroupSids { get; set; }
	}
}
