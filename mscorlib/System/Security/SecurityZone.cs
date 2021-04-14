using System;
using System.Runtime.InteropServices;

namespace System.Security
{
	[ComVisible(true)]
	[Serializable]
	public enum SecurityZone
	{
		MyComputer,
		Intranet,
		Trusted,
		Internet,
		Untrusted,
		NoZone = -1
	}
}
