using System;
using System.Diagnostics;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal static class HierarchyChangesFetcherUtils
	{
		[Conditional("DEBUG")]
		internal static void ValidateEnumeration(EnumerateHierarchyChangesFlags flags, IHierarchyChangesFetcher hierarchyChangesFetcher, bool hasMoreHierarchyChangesPrevPage, bool isPagedEnumeration)
		{
			flags.HasFlag(EnumerateHierarchyChangesFlags.FirstPage);
			flags.HasFlag(EnumerateHierarchyChangesFlags.Catchup);
			if (!isPagedEnumeration || flags.HasFlag(EnumerateHierarchyChangesFlags.FirstPage) || flags.HasFlag(EnumerateHierarchyChangesFlags.Catchup))
			{
				return;
			}
		}
	}
}
