using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Flags]
	internal enum SmartHostType
	{
		None = 0,
		UseMx = 1
	}
}
