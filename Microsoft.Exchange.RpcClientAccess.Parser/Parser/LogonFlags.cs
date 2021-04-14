using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[Flags]
	internal enum LogonFlags : byte
	{
		None = 0,
		Private = 1,
		Undercover = 2,
		Ghosted = 4,
		SplProcess = 8,
		Mapi0 = 16,
		MbxGuids = 32,
		Extended = 64,
		NoRules = 128
	}
}
