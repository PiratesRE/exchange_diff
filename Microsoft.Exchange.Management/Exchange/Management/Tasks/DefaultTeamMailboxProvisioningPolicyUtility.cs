using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Tasks
{
	internal class DefaultTeamMailboxProvisioningPolicyUtility : DefaultMailboxPolicyUtility<TeamMailboxProvisioningPolicy>
	{
		public static IList<TeamMailboxProvisioningPolicy> GetDefaultPolicies(IConfigurationSession session)
		{
			return DefaultMailboxPolicyUtility<TeamMailboxProvisioningPolicy>.GetDefaultPolicies(session, DefaultTeamMailboxProvisioningPolicyUtility.filter, null);
		}

		public static IList<TeamMailboxProvisioningPolicy> GetDefaultPolicies(IConfigurationSession session, QueryFilter additionalFilter)
		{
			return DefaultMailboxPolicyUtility<TeamMailboxProvisioningPolicy>.GetDefaultPolicies(session, DefaultTeamMailboxProvisioningPolicyUtility.filter, additionalFilter);
		}

		private static readonly QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, TeamMailboxProvisioningPolicySchema.IsDefault, true);
	}
}
