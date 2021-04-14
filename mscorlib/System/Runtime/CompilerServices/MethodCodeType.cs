using System;
using System.Runtime.InteropServices;

namespace System.Runtime.CompilerServices
{
	[ComVisible(true)]
	[Serializable]
	public enum MethodCodeType
	{
		IL,
		Native,
		OPTIL,
		Runtime
	}
}
