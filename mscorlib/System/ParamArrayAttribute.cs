using System;
using System.Runtime.InteropServices;

namespace System
{
	[AttributeUsage(AttributeTargets.Parameter, Inherited = true, AllowMultiple = false)]
	[ComVisible(true)]
	[__DynamicallyInvokable]
	public sealed class ParamArrayAttribute : Attribute
	{
		[__DynamicallyInvokable]
		public ParamArrayAttribute()
		{
		}
	}
}
