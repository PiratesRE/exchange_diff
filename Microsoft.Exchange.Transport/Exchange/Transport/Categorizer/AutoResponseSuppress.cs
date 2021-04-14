using System;

namespace Microsoft.Exchange.Transport.Categorizer
{
	[Flags]
	internal enum AutoResponseSuppress
	{
		DR = 1,
		NDR = 2,
		RN = 4,
		NRN = 8,
		OOF = 16,
		AutoReply = 32
	}
}
