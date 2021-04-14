using System;
using System.Runtime.InteropServices;

namespace System.Security.Permissions
{
	[Flags]
	[ComVisible(true)]
	[Serializable]
	public enum FileDialogPermissionAccess
	{
		None = 0,
		Open = 1,
		Save = 2,
		OpenSave = 3
	}
}
