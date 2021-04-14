using System;

namespace Microsoft.Exchange.Data.Storage
{
	[Flags]
	internal enum AutoResponseSuppress
	{
		DR = 1,
		NDR = 2,
		RN = 4,
		NRN = 8,
		OOF = 16,
		AutoReply = 32,
		None = 0,
		All = -1
	}
}
