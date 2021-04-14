using System;

namespace System.Security
{
	[Flags]
	internal enum PermissionTokenType
	{
		Normal = 1,
		IUnrestricted = 2,
		DontKnow = 4,
		BuiltIn = 8
	}
}
