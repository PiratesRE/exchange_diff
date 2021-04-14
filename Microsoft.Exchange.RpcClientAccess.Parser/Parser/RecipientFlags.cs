using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[Flags]
	internal enum RecipientFlags : ushort
	{
		None = 0,
		TransmitSameAsDisplayName = 64,
		Responsibility = 128,
		SendNoRichInformation = 256,
		ValidMask = 448
	}
}
