using System;

namespace Microsoft.Exchange.Management.Metabase
{
	[Flags]
	internal enum MBKeyAccess
	{
		Read = 1,
		Write = 2
	}
}
