using System;

namespace Microsoft.Exchange.HttpProxy
{
	[Flags]
	internal enum RpcHttpRtsFlags
	{
		None = 0,
		Ping = 1,
		OtherCommand = 2,
		RecycleChannel = 4,
		InChannel = 8,
		OutChannel = 16,
		EndOfFile = 32,
		Echo = 64
	}
}
