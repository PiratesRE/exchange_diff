using System;

namespace Microsoft.Exchange.Data.Mime
{
	internal enum LineTerminationState : byte
	{
		CRLF,
		CR,
		Other,
		Unknown,
		NotInteresting
	}
}
