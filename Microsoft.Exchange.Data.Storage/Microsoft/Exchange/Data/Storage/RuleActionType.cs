using System;

namespace Microsoft.Exchange.Data.Storage
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
