using System;
using System.Runtime.InteropServices;

namespace System.Runtime.Serialization
{
	[AttributeUsage(AttributeTargets.Method, Inherited = false)]
	[ComVisible(true)]
	[__DynamicallyInvokable]
	public sealed class OnDeserializedAttribute : Attribute
	{
		[__DynamicallyInvokable]
		public OnDeserializedAttribute()
		{
		}
	}
}
