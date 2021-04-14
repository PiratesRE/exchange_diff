using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Tasks
{
	internal class DefaultMailboxPolicyUtility<T> where T : MailboxPolicy, new()
	{
		public static IList<T> GetDefaultPolicies(IConfigurationSession session, QueryFilter defaultFilter, QueryFilter extraFilter)
		{
			if (session == null)
			{
				throw new ArgumentNullException("session is null probably due to not of IConfigurationSession type");
			}
			QueryFilter filter;
			if (extraFilter != null)
			{
				filter = new AndFilter(new QueryFilter[]
				{
					extraFilter,
					defaultFilter
				});
			}
			else
			{
				filter = defaultFilter;
			}
			return session.Find<T>(null, QueryScope.SubTree, filter, DefaultMailboxPolicyUtility<T>.sortBy, int.MaxValue);
		}

		public static void ClearDefaultPolicies(IConfigurationSession session, IList<T> defaultPolicies)
		{
			if (session == null)
			{
				throw new ArgumentNullException("session is null probably due to not of IConfigurationSession type");
			}
			if (defaultPolicies != null && defaultPolicies.Count > 0)
			{
				foreach (T t in defaultPolicies)
				{
					t.IsDefault = false;
					session.Save(t);
				}
			}
		}

		private static readonly SortBy sortBy = new SortBy(ADObjectSchema.WhenChanged, SortOrder.Descending);
	}
}
