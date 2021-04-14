using System;

namespace System.Runtime.InteropServices.ComTypes
{
	[Flags]
	[__DynamicallyInvokable]
	[Serializable]
	public enum IDLFLAG : short
	{
		[__DynamicallyInvokable]
		IDLFLAG_NONE = 0,
		[__DynamicallyInvokable]
		IDLFLAG_FIN = 1,
		[__DynamicallyInvokable]
		IDLFLAG_FOUT = 2,
		[__DynamicallyInvokable]
		IDLFLAG_FLCID = 4,
		[__DynamicallyInvokable]
		IDLFLAG_FRETVAL = 8
	}
}
