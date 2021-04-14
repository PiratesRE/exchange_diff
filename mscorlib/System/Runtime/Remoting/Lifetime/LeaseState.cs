using System;
using System.Runtime.InteropServices;

namespace System.Runtime.Remoting.Lifetime
{
	[ComVisible(true)]
	[Serializable]
	public enum LeaseState
	{
		Null,
		Initial,
		Active,
		Renewing,
		Expired
	}
}
