using System;
using System.Runtime.InteropServices;

namespace System.Runtime.Remoting
{
	[ComVisible(true)]
	[Serializable]
	public enum WellKnownObjectMode
	{
		Singleton = 1,
		SingleCall
	}
}
