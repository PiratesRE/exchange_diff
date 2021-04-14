using System;

namespace Microsoft.Exchange.RpcClientAccess
{
	internal enum RuleActionType : byte
	{
		Move = 1,
		Copy,
		Reply,
		OutOfOfficeReply,
		DeferAction,
		Bounce,
		Forward,
		Delegate,
		Tag,
		Delete,
		MarkAsRead
	}
}
