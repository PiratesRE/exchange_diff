using System;

namespace Microsoft.Exchange.Hygiene.Data.AsyncQueue
{
	internal enum AsyncQueuePriority : byte
	{
		High,
		Normal = 50,
		Low = 100,
		System_High = 253,
		System_Normal,
		System_Low
	}
}
