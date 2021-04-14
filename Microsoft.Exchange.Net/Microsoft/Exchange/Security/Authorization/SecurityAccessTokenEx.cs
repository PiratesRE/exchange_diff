using System;
using System.Security.Principal;

namespace Microsoft.Exchange.Security.Authorization
{
	public class SecurityAccessTokenEx : ISecurityAccessTokenEx
	{
		public SecurityIdentifier UserSid { get; set; }

		public SidBinaryAndAttributes[] GroupSids { get; set; }

		public SidBinaryAndAttributes[] RestrictedGroupSids { get; set; }
	}
}
