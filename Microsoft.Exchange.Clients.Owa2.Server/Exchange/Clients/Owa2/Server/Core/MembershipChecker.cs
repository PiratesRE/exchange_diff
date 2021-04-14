using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.MessagingPolicies.Rules;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class MembershipChecker : IStringComparer
	{
		public MembershipChecker(OrganizationId organizationId)
		{
			this.organizationId = organizationId;
		}

		public bool Equals(string senderEmailAddress, string distributionListRoutingAddress)
		{
			return !string.IsNullOrEmpty(senderEmailAddress) && !string.IsNullOrEmpty(distributionListRoutingAddress) && ADUtils.IsMemberOf(senderEmailAddress, (RoutingAddress)distributionListRoutingAddress, this.organizationId);
		}

		private readonly OrganizationId organizationId;
	}
}
