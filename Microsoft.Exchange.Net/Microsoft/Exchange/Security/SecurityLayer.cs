using System;

namespace Microsoft.Exchange.Security
{
	[Flags]
	internal enum SecurityLayer : byte
	{
		None = 0,
		NoSecurity = 1,
		Integrity = 2,
		Privacy = 4
	}
}
