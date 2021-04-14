using System;

namespace System.Runtime.InteropServices
{
	[Obsolete("Use System.Runtime.InteropServices.ComTypes.CALLCONV instead. http://go.microsoft.com/fwlink/?linkid=14202", false)]
	[Serializable]
	public enum CALLCONV
	{
		CC_CDECL = 1,
		CC_MSCPASCAL,
		CC_PASCAL = 2,
		CC_MACPASCAL,
		CC_STDCALL,
		CC_RESERVED,
		CC_SYSCALL,
		CC_MPWCDECL,
		CC_MPWPASCAL,
		CC_MAX
	}
}
