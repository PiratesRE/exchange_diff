using System;

namespace Microsoft.Exchange.Server.Storage.Common
{
	[Flags]
	public enum TaskFlags : byte
	{
		None = 0,
		AutoStart = 1,
		UseThreadPoolThread = 2,
		RunOnceOnly = 4
	}
}
