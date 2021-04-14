using System;
using System.Runtime.InteropServices;

namespace System.Runtime.Remoting.Channels
{
	[ComVisible(true)]
	[Serializable]
	public enum ServerProcessing
	{
		Complete,
		OneWay,
		Async
	}
}
