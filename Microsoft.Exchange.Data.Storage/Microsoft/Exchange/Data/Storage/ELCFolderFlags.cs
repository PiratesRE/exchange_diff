using System;

namespace Microsoft.Exchange.Data.Storage
{
	[Flags]
	internal enum ELCFolderFlags
	{
		None = 0,
		Provisioned = 1,
		Protected = 2,
		MustDisplayComment = 4,
		Quota = 8,
		ELCRoot = 16,
		TrackFolderSize = 32,
		DumpsterFolder = 64
	}
}
