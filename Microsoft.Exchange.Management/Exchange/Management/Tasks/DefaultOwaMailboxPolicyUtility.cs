using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Tasks
{
	internal class DefaultOwaMailboxPolicyUtility : DefaultMailboxPolicyUtility<OwaMailboxPolicy>
	{
		public static IList<OwaMailboxPolicy> GetDefaultPolicies(IConfigurationSession session)
		{
			return DefaultMailboxPolicyUtility<OwaMailboxPolicy>.GetDefaultPolicies(session, DefaultOwaMailboxPolicyUtility.filter, null);
		}

		public static IList<OwaMailboxPolicy> GetDefaultPolicies(IConfigurationSession session, QueryFilter extraFilter)
		{
			return DefaultMailboxPolicyUtility<OwaMailboxPolicy>.GetDefaultPolicies(session, DefaultOwaMailboxPolicyUtility.filter, extraFilter);
		}

		private static readonly QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, OwaMailboxPolicySchema.IsDefault, true);
	}
}
