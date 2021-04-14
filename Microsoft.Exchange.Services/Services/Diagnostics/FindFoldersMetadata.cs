using System;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;

namespace Microsoft.Exchange.Services.Diagnostics
{
	internal enum FindFoldersMetadata
	{
		[DisplayName("FF", "TFC")]
		TotalFolderCount,
		[DisplayName("FF", "CFC")]
		TopLevelChildFolderCount
	}
}
