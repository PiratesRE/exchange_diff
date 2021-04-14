using System;
using System.Runtime.InteropServices;

namespace System.IO
{
	[Flags]
	[ComVisible(true)]
	[Serializable]
	public enum FileOptions
	{
		None = 0,
		WriteThrough = -2147483648,
		Asynchronous = 1073741824,
		RandomAccess = 268435456,
		DeleteOnClose = 67108864,
		SequentialScan = 134217728,
		Encrypted = 16384
	}
}
