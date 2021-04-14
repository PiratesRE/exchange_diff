using System;

namespace System.Runtime.Remoting.Contexts
{
	internal class ArrayWithSize
	{
		internal ArrayWithSize(IDynamicMessageSink[] sinks, int count)
		{
			this.Sinks = sinks;
			this.Count = count;
		}

		internal IDynamicMessageSink[] Sinks;

		internal int Count;
	}
}
