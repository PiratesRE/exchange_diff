using System;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[Flags]
	internal enum DiagnosticsLoadFlags
	{
		Default = 0,
		DumpsterInfo = 1,
		HierarchyInfo = 2
	}
}
