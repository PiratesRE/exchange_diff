using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[Flags]
	internal enum LogonExtendedResponseFlags : uint
	{
		None = 0U,
		LocaleInfo = 1U
	}
}
