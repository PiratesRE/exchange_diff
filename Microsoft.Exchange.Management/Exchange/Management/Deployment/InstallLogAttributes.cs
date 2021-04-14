using System;

namespace Microsoft.Exchange.Management.Deployment
{
	[Flags]
	internal enum InstallLogAttributes
	{
		Append = 1,
		FlushEachLine = 2
	}
}
