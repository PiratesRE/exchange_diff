using System;
using System.Runtime.InteropServices;

namespace System.Reflection
{
	[ComVisible(true)]
	[__DynamicallyInvokable]
	public struct InterfaceMapping
	{
		[ComVisible(true)]
		[__DynamicallyInvokable]
		public Type TargetType;

		[ComVisible(true)]
		[__DynamicallyInvokable]
		public Type InterfaceType;

		[ComVisible(true)]
		[__DynamicallyInvokable]
		public MethodInfo[] TargetMethods;

		[ComVisible(true)]
		[__DynamicallyInvokable]
		public MethodInfo[] InterfaceMethods;
	}
}
