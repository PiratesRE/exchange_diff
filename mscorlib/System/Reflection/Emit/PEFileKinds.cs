using System;
using System.Runtime.InteropServices;

namespace System.Reflection.Emit
{
	[ComVisible(true)]
	[Serializable]
	public enum PEFileKinds
	{
		Dll = 1,
		ConsoleApplication,
		WindowApplication
	}
}
