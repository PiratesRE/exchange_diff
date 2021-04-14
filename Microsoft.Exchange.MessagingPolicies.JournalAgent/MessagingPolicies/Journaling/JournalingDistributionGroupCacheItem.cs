using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.MessagingPolicies.Journaling
{
	internal sealed class JournalingDistributionGroupCacheItem
	{
		public JournalingDistributionGroupCacheItem(List<string> members)
		{
			if (members == null)
			{
				throw new ArgumentNullException("members");
			}
			this.members = members;
		}

		public bool TryGetNextGroupMember(out string groupMember)
		{
			if (this.members.Count == 0)
			{
				groupMember = null;
				return false;
			}
			groupMember = this.members[this.nextMemberIndex];
			this.nextMemberIndex = (this.nextMemberIndex + 1) % this.members.Count;
			return true;
		}

		private int nextMemberIndex;

		private List<string> members;
	}
}
