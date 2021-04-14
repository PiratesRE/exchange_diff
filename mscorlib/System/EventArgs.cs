using System;
using System.Runtime.InteropServices;

namespace System
{
	[ComVisible(true)]
	[__DynamicallyInvokable]
	[Serializable]
	public class EventArgs
	{
		[__DynamicallyInvokable]
		public EventArgs()
		{
		}

		[__DynamicallyInvokable]
		public static readonly EventArgs Empty = new EventArgs();
	}
}
