using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal struct ERROR_NOTIFICATION
	{
		internal int cbEntryID;

		internal IntPtr lpEntryID;

		internal int scode;

		internal int ulFlags;

		internal unsafe MAPIERROR* lpMAPIError;
	}
}
