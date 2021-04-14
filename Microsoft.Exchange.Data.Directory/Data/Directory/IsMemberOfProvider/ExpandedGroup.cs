using System;
using System.Collections.Generic;
using Microsoft.Exchange.Common.Cache;

namespace Microsoft.Exchange.Data.Directory.IsMemberOfProvider
{
	internal class ExpandedGroup : CachableItem
	{
		public ExpandedGroup() : this(new List<Guid>(), new List<Guid>())
		{
		}

		public ExpandedGroup(List<Guid> memberGroups, List<Guid> memberRecipients)
		{
			this.memberGroups = memberGroups;
			this.memberRecipients = memberRecipients;
		}

		public IEnumerable<Guid> MemberGroups
		{
			get
			{
				return this.memberGroups;
			}
		}

		public override long ItemSize
		{
			get
			{
				return (long)((this.memberGroups.Count + this.memberRecipients.Count) * 16);
			}
		}

		public bool ContainsRecipient(Guid recipientGuid)
		{
			return this.memberRecipients.Contains(recipientGuid);
		}

		private const int GuidLength = 16;

		private List<Guid> memberGroups;

		private List<Guid> memberRecipients;
	}
}
