using System;

namespace Microsoft.Exchange.Transport.Sync.Common.Interop
{
	[Flags]
	internal enum MLDETECTCP
	{
		MLDETECTCP_NONE = 0,
		MLDETECTCP_7BIT = 1,
		MLDETECTCP_8BIT = 2,
		MLDETECTCP_DBCS = 4,
		MLDETECTCP_HTML = 8,
		MLDETECTCP_NUMBER = 16
	}
}
