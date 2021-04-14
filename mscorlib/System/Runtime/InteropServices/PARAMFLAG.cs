using System;

namespace System.Runtime.InteropServices
{
	[Obsolete("Use System.Runtime.InteropServices.ComTypes.PARAMFLAG instead. http://go.microsoft.com/fwlink/?linkid=14202", false)]
	[Flags]
	[Serializable]
	public enum PARAMFLAG : short
	{
		PARAMFLAG_NONE = 0,
		PARAMFLAG_FIN = 1,
		PARAMFLAG_FOUT = 2,
		PARAMFLAG_FLCID = 4,
		PARAMFLAG_FRETVAL = 8,
		PARAMFLAG_FOPT = 16,
		PARAMFLAG_FHASDEFAULT = 32,
		PARAMFLAG_FHASCUSTDATA = 64
	}
}
