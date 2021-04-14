using System;

namespace Microsoft.Exchange.Data.Directory.ABProviderFramework
{
	[Flags]
	internal enum ABProviderFlags
	{
		None = 0,
		HasGal = 1,
		CanBrowse = 2
	}
}
