using System;

namespace Microsoft.Exchange.Data.Storage
{
	[Flags]
	internal enum UserConfigurationSearchFlags
	{
		FullString = 0,
		SubString = 1,
		Prefix = 2
	}
}
