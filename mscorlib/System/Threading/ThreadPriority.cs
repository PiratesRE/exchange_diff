using System;
using System.Runtime.InteropServices;

namespace System.Threading
{
	[ComVisible(true)]
	[Serializable]
	public enum ThreadPriority
	{
		Lowest,
		BelowNormal,
		Normal,
		AboveNormal,
		Highest
	}
}
