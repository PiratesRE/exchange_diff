using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal struct _MoveCopyAction
	{
		internal int cbStoreEntryID;

		internal unsafe byte* lpbStoreEntryID;

		internal int cbFolderEntryID;

		internal unsafe byte* lpbFolderEntryID;
	}
}
