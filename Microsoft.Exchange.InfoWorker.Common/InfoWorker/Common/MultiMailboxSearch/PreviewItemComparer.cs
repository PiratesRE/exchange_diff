using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Exchange.InfoWorker.Common.MultiMailboxSearch
{
	internal sealed class PreviewItemComparer : IComparer, IComparer<PreviewItem>
	{
		internal PreviewItemComparer(bool ascendingSort)
		{
			this.ascendingSort = ascendingSort;
		}

		public int Compare(object first, object second)
		{
			PreviewItem lhsItem = first as PreviewItem;
			PreviewItem rhsItem = second as PreviewItem;
			return this.Compare(lhsItem, rhsItem);
		}

		private static void Swap(ref PreviewItem lhs, ref PreviewItem rhs)
		{
			PreviewItem previewItem = rhs;
			rhs = lhs;
			lhs = previewItem;
		}

		public int Compare(PreviewItem lhsItem, PreviewItem rhsItem)
		{
			if (!this.ascendingSort)
			{
				PreviewItemComparer.Swap(ref lhsItem, ref rhsItem);
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
