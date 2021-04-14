using System;

namespace Microsoft.Exchange.Data.Storage
{
	[Flags]
	internal enum PropertyAccess
	{
		None = 0,
		Read = 1,
		Write = 2,
		ReadWrite = 3
	}
}
