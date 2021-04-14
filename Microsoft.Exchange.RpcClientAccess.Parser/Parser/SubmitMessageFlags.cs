using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[Flags]
	internal enum SubmitMessageFlags : byte
	{
		None = 0,
		Preprocess = 1,
		NeedsSpooler = 2,
		IgnoreSendAsRight = 4
	}
}
