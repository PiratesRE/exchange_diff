using System;

namespace System.Runtime.InteropServices
{
	[Obsolete("Use System.Runtime.InteropServices.ComTypes.FUNCKIND instead. http://go.microsoft.com/fwlink/?linkid=14202", false)]
	[Serializable]
	public enum FUNCKIND
	{
		FUNC_VIRTUAL,
		FUNC_PUREVIRTUAL,
		FUNC_NONVIRTUAL,
		FUNC_STATIC,
		FUNC_DISPATCH
	}
}
