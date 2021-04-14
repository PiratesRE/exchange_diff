using System;

namespace Microsoft.Exchange.Data.Storage
{
	[Flags]
	internal enum CharsetDetectionDataFlags
	{
		None = 0,
		Complete = 1,
		NoMessageDecoding = 2
	}
}
