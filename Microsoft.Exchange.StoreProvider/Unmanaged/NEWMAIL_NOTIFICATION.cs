using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal struct NEWMAIL_NOTIFICATION
	{
		internal int cbEntryID;

		internal IntPtr lpEntryID;

		internal int cbParentID;

		internal IntPtr lpParentID;

		internal int ulFlags;

		internal IntPtr lpszMessageClass;

		internal int ulMessageFlags;
	}
}
