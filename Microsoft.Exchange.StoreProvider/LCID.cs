using System;

namespace Microsoft.Mapi
{
	internal enum LCID : uint
	{
		LOCALE_NEUTRAL,
		LOCALE_INVARIANT = 127U,
		LOCALE_SYSTEM_DEFAULT = 2048U,
		LOCALE_USER_DEFAULT = 1024U
	}
}
