using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Tasks
{
	internal class DefaultMobileMailboxPolicyUtility<T> : DefaultMailboxPolicyUtility<T> where T : MobileMailboxPolicy, new()
	{
		public static IList<T> GetDefaultPolicies(IConfigurationSession session)
		{
			return DefaultMailboxPolicyUtility<T>.GetDefaultPolicies(session, DefaultMobileMailboxPolicyUtility<T>.filter, null);
		}

		public static IList<T> GetDefaultPolicies(IConfigurationSession session, QueryFilter extraFilter)
		{
			return DefaultMailboxPolicyUtility<T>.GetDefaultPolicies(session, DefaultMobileMailboxPolicyUtility<T>.filter, extraFilter);
		}

		internal static bool ValidateLength(IEnumerable collection, int maxListLength, int maxMemberLength)
		{
			if (collection != null)
			{
				foreach (object obj in collection)
				{
					int length = obj.ToString().Length;
					if (maxMemberLength < length)
					{
						return false;
					}
					maxListLength -= length;
					if (maxListLength < 0)
					{
						return false;
					}
				}
				return true;
			}
			return true;
		}

		private static readonly QueryFilter filter = new BitMaskAndFilter(MobileMailboxPolicySchema.MobileFlags, 4096UL);
	}
}
