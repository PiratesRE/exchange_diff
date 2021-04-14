using System;

namespace System.Runtime.InteropServices.ComTypes
{
	[Flags]
	[__DynamicallyInvokable]
	[Serializable]
	public enum LIBFLAGS : short
	{
		[__DynamicallyInvokable]
		LIBFLAG_FRESTRICTED = 1,
		[__DynamicallyInvokable]
		LIBFLAG_FCONTROL = 2,
		[__DynamicallyInvokable]
		LIBFLAG_FHIDDEN = 4,
		[__DynamicallyInvokable]
		LIBFLAG_FHASDISKIMAGE = 8
	}
}
