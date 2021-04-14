using System;

namespace System.Runtime.InteropServices.WindowsRuntime
{
	[Guid("00000035-0000-0000-C000-000000000046")]
	[__DynamicallyInvokable]
	[ComImport]
	public interface IActivationFactory
	{
		[__DynamicallyInvokable]
		object ActivateInstance();
	}
}
