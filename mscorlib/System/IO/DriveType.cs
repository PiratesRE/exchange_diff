using System;
using System.Runtime.InteropServices;

namespace System.IO
{
	[ComVisible(true)]
	[Serializable]
	public enum DriveType
	{
		Unknown,
		NoRootDirectory,
		Removable,
		Fixed,
		Network,
		CDRom,
		Ram
	}
}
