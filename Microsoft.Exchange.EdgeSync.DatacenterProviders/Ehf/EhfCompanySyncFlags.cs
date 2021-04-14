using System;

namespace Microsoft.Exchange.EdgeSync.Ehf
{
	[Flags]
	internal enum EhfCompanySyncFlags
	{
		None = 0,
		ForceDomainSync = 1
	}
}
