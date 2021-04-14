using System;
using System.Runtime.InteropServices;

namespace System.Reflection
{
	[Flags]
	[ComVisible(true)]
	[Serializable]
	public enum PortableExecutableKinds
	{
		NotAPortableExecutableImage = 0,
		ILOnly = 1,
		Required32Bit = 2,
		PE32Plus = 4,
		Unmanaged32Bit = 8,
		[ComVisible(false)]
		Preferred32Bit = 16
	}
}
