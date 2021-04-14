using System;

namespace Microsoft.Exchange.Server.Storage.AdminInterface
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
