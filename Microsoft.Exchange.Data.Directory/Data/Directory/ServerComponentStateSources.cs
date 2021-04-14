using System;

namespace Microsoft.Exchange.Data.Directory
{
	[Flags]
	internal enum ServerComponentStateSources
	{
		AD = 1,
		Registry = 2,
		All = 3
	}
}
