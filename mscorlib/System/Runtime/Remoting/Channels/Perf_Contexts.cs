using System;

namespace System.Runtime.Remoting.Channels
{
	internal struct Perf_Contexts
	{
		internal volatile int cRemoteCalls;

		internal volatile int cChannels;
	}
}
