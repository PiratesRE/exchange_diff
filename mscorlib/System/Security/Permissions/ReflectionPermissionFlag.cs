using System;
using System.Runtime.InteropServices;

namespace System.Security.Permissions
{
	[ComVisible(true)]
	[Flags]
	[Serializable]
	public enum ReflectionPermissionFlag
	{
		NoFlags = 0,
		[Obsolete("This API has been deprecated. http://go.microsoft.com/fwlink/?linkid=14202")]
		TypeInformation = 1,
		MemberAccess = 2,
		[Obsolete("This permission is no longer used by the CLR.")]
		ReflectionEmit = 4,
		[ComVisible(false)]
		RestrictedMemberAccess = 8,
		[Obsolete("This permission has been deprecated. Use PermissionState.Unrestricted to get full access.")]
		AllFlags = 7
	}
}
