using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal struct _ReplyAction
	{
		internal int cbMessageEntryID;

		internal unsafe byte* lpbMessageEntryID;

		internal Guid guidReplyTemplate;
	}
}
