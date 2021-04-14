using System;

namespace Microsoft.Exchange.Data.TextConverters
{
	[Flags]
	internal enum DbcsLeadBits : byte
	{
		Lead1361 = 1,
		Lead10001 = 2,
		Lead10002 = 4,
		Lead10003 = 8,
		Lead10008 = 16,
		Lead932 = 32,
		Lead9XX = 64
	}
}
