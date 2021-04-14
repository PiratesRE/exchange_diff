using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Flags]
	internal enum PerimeterDomainFlags
	{
		None = 0,
		PerimeterDuplicateDetected = 1
	}
}
