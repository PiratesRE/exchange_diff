using System;

namespace Microsoft.Exchange.Migration
{
	internal enum GroupMembershipProvisioningState
	{
		MemberNotRetrieved,
		MemberRetrievedButNotProvisioned,
		MemberRetrievedAndProvisioned
	}
}
