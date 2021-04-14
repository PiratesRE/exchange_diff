using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Tasks
{
	internal static class RoleAssignmentPolicyHelper
	{
		public static IList<RoleAssignmentPolicy> GetDefaultPolicies(IConfigurationSession session, QueryFilter extraFilter)
		{
			QueryFilter queryFilter = RoleAssignmentPolicyHelper.filter;
			if (extraFilter != null)
			{
				queryFilter = new AndFilter(new QueryFilter[]
				{
					extraFilter,
					RoleAssignmentPolicyHelper.filter
				});
			}
			return RoleAssignmentPolicyHelper.GetPolicies(session, queryFilter);
		}

		public static IList<RoleAssignmentPolicy> GetPolicies(IConfigurationSession session, QueryFilter filter)
		{
			if (session == null)
			{
				throw new ArgumentNullException("session");
			}
			ADPagedReader<RoleAssignmentPolicy> adpagedReader = session.FindPaged<RoleAssignmentPolicy>(null, QueryScope.SubTree, filter, null, 0);
			List<RoleAssignmentPolicy> list = new List<RoleAssignmentPolicy>();
			foreach (RoleAssignmentPolicy item in adpagedReader)
			{
				list.Add(item);
			}
			return list;
		}

		public static void ClearIsDefaultOnPolicies(IConfigurationSession session, IList<RoleAssignmentPolicy> defaultPolicies)
		{
			if (session == null)
			{
				throw new ArgumentNullException("session");
			}
			if (defaultPolicies != null && defaultPolicies.Count > 0)
			{
				foreach (RoleAssignmentPolicy roleAssignmentPolicy in defaultPolicies)
				{
					roleAssignmentPolicy.IsDefault = false;
					session.Save(roleAssignmentPolicy);
				}
			}
		}

		public static bool RoleAssignmentsForPolicyExist(IConfigurationSession session, RoleAssignmentPolicy policy)
		{
			if (session == null)
			{
				throw new ArgumentNullException("session");
			}
			QueryFilter queryFilter = new ComparisonFilter(ComparisonOperator.Equal, ExchangeRoleAssignmentSchema.User, policy.Id);
			ExchangeRoleAssignment[] array = session.Find<ExchangeRoleAssignment>(null, QueryScope.SubTree, queryFilter, null, 1);
			return array.Length > 0;
		}

		private static readonly QueryFilter filter = new BitMaskAndFilter(RoleAssignmentPolicySchema.Flags, 1UL);
	}
}
