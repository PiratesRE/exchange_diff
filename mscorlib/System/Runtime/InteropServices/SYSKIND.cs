using System;

namespace System.Runtime.InteropServices
{
	[Obsolete("Use System.Runtime.InteropServices.ComTypes.SYSKIND instead. http://go.microsoft.com/fwlink/?linkid=14202", false)]
	[Serializable]
	public enum SYSKIND
	{
		SYS_WIN16,
		SYS_WIN32,
		SYS_MAC
	}
}
