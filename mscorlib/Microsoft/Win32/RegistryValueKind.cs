using System;
using System.Runtime.InteropServices;

namespace Microsoft.Win32
{
	[ComVisible(true)]
	public enum RegistryValueKind
	{
		String = 1,
		ExpandString,
		Binary,
		DWord,
		MultiString = 7,
		QWord = 11,
		Unknown = 0,
		[ComVisible(false)]
		None = -1
	}
}
