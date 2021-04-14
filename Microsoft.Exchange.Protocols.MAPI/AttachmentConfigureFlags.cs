using System;

namespace Microsoft.Exchange.Protocols.MAPI
{
	[Flags]
	public enum AttachmentConfigureFlags
	{
		None = 0,
		RequestReadOnly = 1,
		RequestWrite = 2
	}
}
