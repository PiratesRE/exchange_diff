using System;

namespace System
{
	[__DynamicallyInvokable]
	[Serializable]
	public delegate void EventHandler<TEventArgs>(object sender, TEventArgs e);
}
