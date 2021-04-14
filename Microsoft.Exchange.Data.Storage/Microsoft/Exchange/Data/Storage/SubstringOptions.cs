using System;

namespace Microsoft.Exchange.Data.Storage
{
	[Flags]
	internal enum SubstringOptions
	{
		None = 0,
		IgnoreMissingLeftDelimiter = 1,
		IgnoreMissingRightDelimiter = 2,
		Backward = 4
	}
}
