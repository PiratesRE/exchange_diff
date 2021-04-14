using System;

namespace Microsoft.Exchange.Data
{
	[Flags]
	internal enum SeekReference
	{
		OriginBeginning = 0,
		OriginCurrent = 1,
		OriginEnd = 2,
		SeekBackward = 4,
		ValidFlags = 7,
		ForwardFromBeginning = 0,
		ForwardFromCurrent = 1,
		BackwardFromCurrent = 5,
		BackwardFromEnd = 6
	}
}
