using System;

namespace Microsoft.Exchange.RpcClientAccess
{
	internal enum RestrictionType : uint
	{
		And,
		Or,
		Not,
		Content,
		Property,
		CompareProps,
		BitMask,
		Size,
		Exists,
		SubRestriction,
		Comment,
		Count,
		Near = 13U,
		True = 131U,
		False,
		Null = 255U
	}
}
