using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class AdminVersionMinor
	{
		internal const int ADMIN_GET_MAILBOX_TABLE_ENTRY_VER_MINOR = 17;

		internal const int ADMIN_MAILBOX_SIGNATURE_VER_MINOR = 24;

		internal const int ADMIN_MAILBOX_TABLE_FLAGS_VER_MINOR = 28;

		internal const int ADMIN_MAILBOX_SIGNATURE_CONTAINER_VER_MINOR = 9;

		internal const int ADMIN_STORE_INTEGRITY_CHECK_EX_VER_MINOR = 15;

		internal const int ADMIN_USER_INFO_VER_MINOR = 16;
	}
}
