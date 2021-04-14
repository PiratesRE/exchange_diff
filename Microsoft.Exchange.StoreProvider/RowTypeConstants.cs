using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class RowTypeConstants
	{
		internal const int LeafRow = 1;

		internal const int EmptyCategoryRow = 2;

		internal const int ExpandedCategoryRow = 3;

		internal const int CollapsedCategoryRow = 4;
	}
}
