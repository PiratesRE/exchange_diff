using System;
using System.Runtime.InteropServices;

namespace System.Threading
{
	[ComVisible(true)]
	[Serializable]
	public enum ApartmentState
	{
		STA,
		MTA,
		Unknown
	}
}
