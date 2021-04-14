using System;

namespace System.Runtime.InteropServices.ComTypes
{
	[Flags]
	[__DynamicallyInvokable]
	[Serializable]
	public enum PARAMFLAG : short
	{
		[__DynamicallyInvokable]
		PARAMFLAG_NONE = 0,
		[__DynamicallyInvokable]
		PARAMFLAG_FIN = 1,
		[__DynamicallyInvokable]
		PARAMFLAG_FOUT = 2,
		[__DynamicallyInvokable]
		PARAMFLAG_FLCID = 4,
		[__DynamicallyInvokable]
		PARAMFLAG_FRETVAL = 8,
		[__DynamicallyInvokable]
		PARAMFLAG_FOPT = 16,
		[__DynamicallyInvokable]
		PARAMFLAG_FHASDEFAULT = 32,
		[__DynamicallyInvokable]
		PARAMFLAG_FHASCUSTDATA = 64
	}
}
