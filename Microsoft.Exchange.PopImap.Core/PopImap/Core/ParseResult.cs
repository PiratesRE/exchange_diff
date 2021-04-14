using System;

namespace Microsoft.Exchange.PopImap.Core
{
	internal enum ParseResult
	{
		notYetParsed,
		success,
		invalidArgument,
		invalidNumberOfArguments,
		invalidMessageSet,
		invalidCharset
	}
}
