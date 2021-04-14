using System;
using System.Runtime.InteropServices;

namespace System
{
	[AttributeUsage(AttributeTargets.Field, Inherited = false)]
	[ComVisible(true)]
	[__DynamicallyInvokable]
	[Serializable]
	public class ThreadStaticAttribute : Attribute
	{
		[__DynamicallyInvokable]
		public ThreadStaticAttribute()
		{
		}
	}
}
