using System;
using System.Runtime.InteropServices;

namespace System
{
	[AttributeUsage(AttributeTargets.Method)]
	[ComVisible(true)]
	[__DynamicallyInvokable]
	public sealed class MTAThreadAttribute : Attribute
	{
		[__DynamicallyInvokable]
		public MTAThreadAttribute()
		{
		}
	}
}
