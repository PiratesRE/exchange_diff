using System;
using System.Runtime.InteropServices;

namespace System.Security.Permissions
{
	[Flags]
	[ComVisible(true)]
	[Serializable]
	public enum RegistryPermissionAccess
	{
		NoAccess = 0,
		Read = 1,
		Write = 2,
		Create = 4,
		AllAccess = 7
	}
}
