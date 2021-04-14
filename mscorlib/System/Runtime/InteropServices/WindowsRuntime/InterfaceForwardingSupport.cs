using System;

namespace System.Runtime.InteropServices.WindowsRuntime
{
	[Flags]
	internal enum InterfaceForwardingSupport
	{
		None = 0,
		IBindableVector = 1,
		IVector = 2,
		IBindableVectorView = 4,
		IVectorView = 8,
		IBindableIterableOrIIterable = 16
	}
}
