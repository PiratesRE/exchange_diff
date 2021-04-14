using System;
using System.Runtime.InteropServices;

namespace System.Security.Permissions
{
	[ComVisible(true)]
	[Serializable]
	public enum PermissionState
	{
		Unrestricted = 1,
		None = 0
	}
}
