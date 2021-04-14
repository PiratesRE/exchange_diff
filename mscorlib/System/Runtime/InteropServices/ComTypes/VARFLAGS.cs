using System;

namespace System.Runtime.InteropServices.ComTypes
{
	[Flags]
	[__DynamicallyInvokable]
	[Serializable]
	public enum VARFLAGS : short
	{
		[__DynamicallyInvokable]
		VARFLAG_FREADONLY = 1,
		[__DynamicallyInvokable]
		VARFLAG_FSOURCE = 2,
		[__DynamicallyInvokable]
		VARFLAG_FBINDABLE = 4,
		[__DynamicallyInvokable]
		VARFLAG_FREQUESTEDIT = 8,
		[__DynamicallyInvokable]
		VARFLAG_FDISPLAYBIND = 16,
		[__DynamicallyInvokable]
		VARFLAG_FDEFAULTBIND = 32,
		[__DynamicallyInvokable]
		VARFLAG_FHIDDEN = 64,
		[__DynamicallyInvokable]
		VARFLAG_FRESTRICTED = 128,
		[__DynamicallyInvokable]
		VARFLAG_FDEFAULTCOLLELEM = 256,
		[__DynamicallyInvokable]
		VARFLAG_FUIDEFAULT = 512,
		[__DynamicallyInvokable]
		VARFLAG_FNONBROWSABLE = 1024,
		[__DynamicallyInvokable]
		VARFLAG_FREPLACEABLE = 2048,
		[__DynamicallyInvokable]
		VARFLAG_FIMMEDIATEBIND = 4096
	}
}
