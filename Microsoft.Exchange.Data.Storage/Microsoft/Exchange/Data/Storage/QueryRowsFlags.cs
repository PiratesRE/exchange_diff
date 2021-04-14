using System;

namespace Microsoft.Exchange.Data.Storage
{
	[Flags]
	internal enum QueryRowsFlags
	{
		None = 0,
		NoAdvance = 1
	}
}
