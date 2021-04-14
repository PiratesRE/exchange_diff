using System;

namespace System.Runtime.InteropServices.ComTypes
{
	[Flags]
	[__DynamicallyInvokable]
	[Serializable]
	public enum FUNCFLAGS : short
	{
		[__DynamicallyInvokable]
		FUNCFLAG_FRESTRICTED = 1,
		[__DynamicallyInvokable]
		FUNCFLAG_FSOURCE = 2,
		[__DynamicallyInvokable]
		FUNCFLAG_FBINDABLE = 4,
		[__DynamicallyInvokable]
		FUNCFLAG_FREQUESTEDIT = 8,
		[__DynamicallyInvokable]
		FUNCFLAG_FDISPLAYBIND = 16,
		[__DynamicallyInvokable]
		FUNCFLAG_FDEFAULTBIND = 32,
		[__DynamicallyInvokable]
		FUNCFLAG_FHIDDEN = 64,
		[__DynamicallyInvokable]
		FUNCFLAG_FUSESGETLASTERROR = 128,
		[__DynamicallyInvokable]
		FUNCFLAG_FDEFAULTCOLLELEM = 256,
		[__DynamicallyInvokable]
		FUNCFLAG_FUIDEFAULT = 512,
		[__DynamicallyInvokable]
		FUNCFLAG_FNONBROWSABLE = 1024,
		[__DynamicallyInvokable]
		FUNCFLAG_FREPLACEABLE = 2048,
		[__DynamicallyInvokable]
		FUNCFLAG_FIMMEDIATEBIND = 4096
	}
}
