using System;

namespace Microsoft.Mapi
{
	internal enum SnapshotOperationCode : uint
	{
		None,
		Prepare,
		Freeze,
		Thaw,
		Truncate,
		Stop,
		Last
	}
}
