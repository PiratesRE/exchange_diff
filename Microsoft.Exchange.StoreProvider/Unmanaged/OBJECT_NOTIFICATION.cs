using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal struct OBJECT_NOTIFICATION
	{
		internal int cbEntryID;

		internal IntPtr lpEntryID;

		internal int ulObjType;

		internal int cbParentID;

		internal IntPtr lpParentID;

		internal int cbOldID;

		internal IntPtr lpOldID;

		internal int cbOldParentID;

		internal IntPtr lpOldParentID;

		internal unsafe int* lpPropTagArray;
	}
}
