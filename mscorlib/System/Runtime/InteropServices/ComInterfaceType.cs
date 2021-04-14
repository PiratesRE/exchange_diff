using System;

namespace System.Runtime.InteropServices
{
	[ComVisible(true)]
	[__DynamicallyInvokable]
	[Serializable]
	public enum ComInterfaceType
	{
		[__DynamicallyInvokable]
		InterfaceIsDual,
		[__DynamicallyInvokable]
		InterfaceIsIUnknown,
		[__DynamicallyInvokable]
		InterfaceIsIDispatch,
		[ComVisible(false)]
		[__DynamicallyInvokable]
		InterfaceIsIInspectable
	}
}
