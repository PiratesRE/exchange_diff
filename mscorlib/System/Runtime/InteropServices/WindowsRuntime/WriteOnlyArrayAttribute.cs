using System;

namespace System.Runtime.InteropServices.WindowsRuntime
{
	[AttributeUsage(AttributeTargets.Parameter, Inherited = false, AllowMultiple = false)]
	[__DynamicallyInvokable]
	public sealed class WriteOnlyArrayAttribute : Attribute
	{
		[__DynamicallyInvokable]
		public WriteOnlyArrayAttribute()
		{
		}
	}
}
