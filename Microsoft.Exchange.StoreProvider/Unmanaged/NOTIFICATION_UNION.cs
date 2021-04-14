using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[StructLayout(LayoutKind.Explicit)]
	internal struct NOTIFICATION_UNION
	{
		[FieldOffset(0)]
		internal ERROR_NOTIFICATION err;

		[FieldOffset(0)]
		internal NEWMAIL_NOTIFICATION newmail;

		[FieldOffset(0)]
		internal OBJECT_NOTIFICATION obj;

		[FieldOffset(0)]
		internal TABLE_NOTIFICATION tab;

		[FieldOffset(0)]
		internal EXTENDED_NOTIFICATION ext;

		[FieldOffset(0)]
		internal STATUS_OBJECT_NOTIFICATION statobj;

		[FieldOffset(0)]
		internal CONNECTION_DROPPED_NOTIFICATION drop;
	}
}
