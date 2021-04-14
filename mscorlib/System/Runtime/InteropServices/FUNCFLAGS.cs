using System;

namespace System.Runtime.InteropServices
{
	[Obsolete("Use System.Runtime.InteropServices.ComTypes.FUNCFLAGS instead. http://go.microsoft.com/fwlink/?linkid=14202", false)]
	[Flags]
	[Serializable]
	public enum FUNCFLAGS : short
	{
		FUNCFLAG_FRESTRICTED = 1,
		FUNCFLAG_FSOURCE = 2,
		FUNCFLAG_FBINDABLE = 4,
		FUNCFLAG_FREQUESTEDIT = 8,
		FUNCFLAG_FDISPLAYBIND = 16,
		FUNCFLAG_FDEFAULTBIND = 32,
		FUNCFLAG_FHIDDEN = 64,
		FUNCFLAG_FUSESGETLASTERROR = 128,
		FUNCFLAG_FDEFAULTCOLLELEM = 256,
		FUNCFLAG_FUIDEFAULT = 512,
		FUNCFLAG_FNONBROWSABLE = 1024,
		FUNCFLAG_FREPLACEABLE = 2048,
		FUNCFLAG_FIMMEDIATEBIND = 4096
	}
}
