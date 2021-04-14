using System;
using System.Runtime.InteropServices;

namespace System.Security.Permissions
{
	[Flags]
	[ComVisible(true)]
	[Serializable]
	public enum EnvironmentPermissionAccess
	{
		NoAccess = 0,
		Read = 1,
		Write = 2,
		AllAccess = 3
	}
}
