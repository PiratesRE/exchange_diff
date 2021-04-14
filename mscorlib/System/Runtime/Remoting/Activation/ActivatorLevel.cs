using System;
using System.Runtime.InteropServices;

namespace System.Runtime.Remoting.Activation
{
	[ComVisible(true)]
	[Serializable]
	public enum ActivatorLevel
	{
		Construction = 4,
		Context = 8,
		AppDomain = 12,
		Process = 16,
		Machine = 20
	}
}
