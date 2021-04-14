using System;

namespace Microsoft.Exchange.Management.Deployment
{
	internal enum InstallState
	{
		NotUsed = -7,
		BadConfig,
		Incomplete,
		SourceAbsent,
		MoreData,
		InvalidArg,
		Unknown,
		Broken,
		Advertised,
		Removed = 1,
		Absent,
		Local,
		Source,
		Default
	}
}
