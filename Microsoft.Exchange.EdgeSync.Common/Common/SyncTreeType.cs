using System;

namespace Microsoft.Exchange.EdgeSync.Common
{
	[Flags]
	public enum SyncTreeType
	{
		Configuration = 1,
		Recipients = 2,
		General = 4
	}
}
