using System;

namespace Microsoft.Mapi
{
	[Flags]
	internal enum CopyMessagesFlags
	{
		None = 0,
		Move = 1,
		DeclineOk = 4,
		SendEntryId = 32,
		DontUpdateSource = 64
	}
}
