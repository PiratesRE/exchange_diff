using System;
using System.Runtime.InteropServices;

namespace System.IO
{
	[Flags]
	[ComVisible(true)]
	[Serializable]
	public enum FileAccess
	{
		Read = 1,
		Write = 2,
		ReadWrite = 3
	}
}
