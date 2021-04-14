using System;
using System.Runtime.InteropServices;

namespace System.IO
{
	[Flags]
	[ComVisible(true)]
	[Serializable]
	public enum FileAttributes
	{
		ReadOnly = 1,
		Hidden = 2,
		System = 4,
		Directory = 16,
		Archive = 32,
		Device = 64,
		Normal = 128,
		Temporary = 256,
		SparseFile = 512,
		ReparsePoint = 1024,
		Compressed = 2048,
		Offline = 4096,
		NotContentIndexed = 8192,
		Encrypted = 16384,
		[ComVisible(false)]
		IntegrityStream = 32768,
		[ComVisible(false)]
		NoScrubData = 131072
	}
}
