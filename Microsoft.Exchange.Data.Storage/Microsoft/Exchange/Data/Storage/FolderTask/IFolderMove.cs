using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.FolderTask
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IFolderMove
	{
		List<FolderInfo> CandidateFolders { get; }

		ulong TotalSizeOccupiedByTarget { get; }

		ulong TotalSizeToMove { get; }
	}
}
