using System;
using System.Runtime.InteropServices;

namespace System
{
	[ComVisible(true)]
	[Serializable]
	public enum PlatformID
	{
		Win32S,
		Win32Windows,
		Win32NT,
		WinCE,
		Unix,
		Xbox,
		MacOSX
	}
}
