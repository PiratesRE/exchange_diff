using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Exchange.InfoWorker.Common.MultiMailboxSearch
{
	internal sealed class MailboxStatsComparer : IComparer, IComparer<MailboxStatistics>
	{
		internal MailboxStatsComparer(bool ascendingSort)
		{
			this.ascendingSort = ascendingSort;
		}

		public int Compare(object first, object second)
		{
			MailboxStatistics lhsItem = first as MailboxStatistics;
			MailboxStatistics rhsItem = second as MailboxStatistics;
			return this.Compare(lhsItem, rhsItem);
		}

		private static void Swap(ref MailboxStatistics lhs, ref MailboxStatistics rhs)
		{
			MailboxStatistics mailboxStatistics = rhs;
			rhs = lhs;
			lhs = mailboxStatistics;
		}

		public int Compare(MailboxStatistics lhsItem, MailboxStatistics rhsItem)
		{
			if (!this.ascendingSort)
			{
				MailboxStatsComparer.Swap(ref lhsItem, ref rhsItem);
			}
			if (rhsItem == null && lhsItem == null)
			{
				return 0;
			}
			if (lhsItem != null && rhsItem == null)
			{
				return 1;
			}
			if (lhsItem == null && rhsItem != null)
			{
				return -1;
			}
			return lhsItem.CompareTo(rhsItem);
		}

		private readonly bool ascendingSort;
	}
}
