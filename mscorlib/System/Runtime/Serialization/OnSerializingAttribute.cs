using System;
using System.Runtime.InteropServices;

namespace System.Runtime.Serialization
{
	[AttributeUsage(AttributeTargets.Method, Inherited = false)]
	[ComVisible(true)]
	[__DynamicallyInvokable]
	public sealed class OnSerializingAttribute : Attribute
	{
		[__DynamicallyInvokable]
		public OnSerializingAttribute()
		{
		}
	}
}
