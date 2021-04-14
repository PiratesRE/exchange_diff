using System;

namespace Microsoft.Exchange.RpcClientAccess
{
	internal enum StringFormatType : byte
	{
		NotPresent,
		EmptyString,
		String8,
		ReduceUnicode,
		FullUnicode
	}
}
