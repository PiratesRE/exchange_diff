using System;
using System.Runtime.InteropServices;

namespace System.Security.Permissions
{
	[ComVisible(true)]
	[Serializable]
	public enum UIPermissionWindow
	{
		NoWindows,
		SafeSubWindows,
		SafeTopLevelWindows,
		AllWindows
	}
}
