using System;

namespace Microsoft.Mapi
{
	[Flags]
	internal enum LogReplayRpcFlags : uint
	{
		None = 0U,
		SetDbScan = 1U,
		EnableDbScan = 2U
	}
}
