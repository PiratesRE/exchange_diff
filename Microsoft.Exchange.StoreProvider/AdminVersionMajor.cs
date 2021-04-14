using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class AdminVersionMajor
	{
		internal const int ADMIN_GET_MAILBOX_TABLE_ENTRY_VER_MAJOR = 6;

		internal const int ADMIN_MAILBOX_SIGNATURE_VER_MAJOR = 6;

		internal const int ADMIN_MAILBOX_TABLE_FLAGS_VER_MAJOR = 6;

		internal const int ADMIN_MAILBOX_SIGNATURE_CONTAINER_VER_MAJOR = 7;

		internal const int ADMIN_STORE_INTEGRITY_CHECK_EX_VER_MAJOR = 7;

		internal const int ADMIN_USER_INFO_VER_MAJOR = 7;
	}
}
