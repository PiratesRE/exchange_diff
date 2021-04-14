using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.PublicFolder
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IPublicFolderSplitPlan
	{
		List<SplitPlanFolder> FoldersToSplit { get; set; }

		ulong TotalSizeOccupied { get; set; }

		ulong TotalSizeToSplit { get; set; }
	}
}
