using System;

namespace System.Runtime.CompilerServices
{
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method)]
	[__DynamicallyInvokable]
	public sealed class ExtensionAttribute : Attribute
	{
		[__DynamicallyInvokable]
		public ExtensionAttribute()
		{
		}
	}
}
