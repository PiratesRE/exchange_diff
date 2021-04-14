using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal struct MapiEventNative
	{
		public static readonly int SizeOf = Marshal.SizeOf(typeof(MapiEventNative));

		internal ulong llEventCounter;

		internal uint ulMask;

		internal Guid mailboxGuid;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 60)]
		internal string rgchObjectClass;

		internal long ftCreate;

		internal uint ulItemType;

		internal CountAndPtr binEidItem;

		internal CountAndPtr binEidParent;

		internal CountAndPtr binEidOldItem;

		internal CountAndPtr binEidOldParent;

		internal int lItemCount;

		internal int lUnreadCount;

		internal uint ulFlags;

		internal ulong ullExtendedFlags;

		internal uint ulClientType;

		internal CountAndPtr binSid;

		internal uint ulDocId;

		internal uint ulMailboxNumber;

		internal CountAndPtr binTenantHintBob;

		internal Guid unifiedMailboxGuid;
	}
}
