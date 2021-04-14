using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[Flags]
	internal enum RecipientSerializationFlags
	{
		RecipientRowId = 1,
		ExtraUnicodeProperties = 2,
		CodePageId = 4
	}
}
