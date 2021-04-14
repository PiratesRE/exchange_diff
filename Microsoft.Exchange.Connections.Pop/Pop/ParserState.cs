using System;

namespace Microsoft.Exchange.Connections.Pop
{
	internal enum ParserState
	{
		INIT = -1,
		NONE,
		CR1,
		LF1,
		DOT,
		CR2,
		EOD,
		EOHCR2
	}
}
