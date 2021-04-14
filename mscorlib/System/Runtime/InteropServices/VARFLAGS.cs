using System;

namespace System.Runtime.InteropServices
{
	[Obsolete("Use System.Runtime.InteropServices.ComTypes.VARFLAGS instead. http://go.microsoft.com/fwlink/?linkid=14202", false)]
	[Flags]
	[Serializable]
	public enum VARFLAGS : short
	{
		VARFLAG_FREADONLY = 1,
		VARFLAG_FSOURCE = 2,
		VARFLAG_FBINDABLE = 4,
		VARFLAG_FREQUESTEDIT = 8,
		VARFLAG_FDISPLAYBIND = 16,
		VARFLAG_FDEFAULTBIND = 32,
		VARFLAG_FHIDDEN = 64,
		VARFLAG_FRESTRICTED = 128,
		VARFLAG_FDEFAULTCOLLELEM = 256,
		VARFLAG_FUIDEFAULT = 512,
		VARFLAG_FNONBROWSABLE = 1024,
		VARFLAG_FREPLACEABLE = 2048,
		VARFLAG_FIMMEDIATEBIND = 4096
	}
}
