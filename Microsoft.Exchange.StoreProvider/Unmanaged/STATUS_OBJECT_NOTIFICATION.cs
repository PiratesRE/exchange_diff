using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal struct STATUS_OBJECT_NOTIFICATION
	{
		internal int cbEntryID;

		internal IntPtr lpEntryID;

		internal int cValues;

		internal unsafe SPropValue* lpPropVals;
	}
}
