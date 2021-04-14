using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Tasks
{
	internal class RetentionPolicyUtility : DefaultMailboxPolicyUtility<RetentionPolicy>
	{
		public static IList<RetentionPolicy> GetDefaultPolicies(IConfigurationSession session, bool isArbitrationMailbox)
		{
			return SharedConfiguration.GetDefaultRetentionPolicy(session, isArbitrationMailbox, RetentionPolicyUtility.sortBy, int.MaxValue);
		}

		public static void ClearDefaultPolicies(IConfigurationSession session, IList<RetentionPolicy> defaultPolicies, bool isArbitrationMailbox)
		{
			if (isArbitrationMailbox)
			{
				RetentionPolicyUtility.ClearDefaultArbitrationMailboxPolicies(session, defaultPolicies);
				return;
			}
			DefaultMailboxPolicyUtility<RetentionPolicy>.ClearDefaultPolicies(session, defaultPolicies);
		}

		private static void ClearDefaultArbitrationMailboxPolicies(IConfigurationSession session, IList<RetentionPolicy> defaultPolicies)
		{
			if (session == null)
			{
				throw new ArgumentNullException("session is null probably due to not of IConfigurationSession type");
			}
			if (defaultPolicies != null && defaultPolicies.Count > 0)
			{
				foreach (RetentionPolicy retentionPolicy in defaultPolicies)
				{
					retentionPolicy.IsDefaultArbitrationMailbox = false;
					session.Save(retentionPolicy);
				}
			}
		}

		private static readonly SortBy sortBy = new SortBy(ADObjectSchema.WhenChanged, SortOrder.Descending);
	}
}
