using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[Flags]
	internal enum DiagnosticContextFlags : byte
	{
		Overflow = 1,
		Circular = 2
	}
}
