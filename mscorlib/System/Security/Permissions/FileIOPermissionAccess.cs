using System;
using System.Runtime.InteropServices;

namespace System.Security.Permissions
{
	[Flags]
	[ComVisible(true)]
	[Serializable]
	public enum FileIOPermissionAccess
	{
		NoAccess = 0,
		Read = 1,
		Write = 2,
		Append = 4,
		PathDiscovery = 8,
		AllAccess = 15
	}
}
