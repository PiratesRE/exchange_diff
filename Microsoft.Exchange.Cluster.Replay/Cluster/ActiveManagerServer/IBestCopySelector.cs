using System;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IBestCopySelector
	{
		AmBcsType BestCopySelectionType { get; }

		AmServerName FindNextBestCopy();

		IAmBcsErrorLogger ErrorLogger { get; }

		Exception LastException { get; }
	}
}
