using System;

namespace Microsoft.Exchange.Protocols.MAPI
{
	[Flags]
	public enum MapiCopyToFlags
	{
		Move = 1,
		NoReplace = 2,
		DeclineOk = 4,
		Dialog = 8
	}
}
