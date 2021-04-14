using System;

namespace System.Runtime.CompilerServices
{
	[AttributeUsage(AttributeTargets.Struct)]
	[__DynamicallyInvokable]
	[Serializable]
	public sealed class UnsafeValueTypeAttribute : Attribute
	{
		[__DynamicallyInvokable]
		public UnsafeValueTypeAttribute()
		{
		}
	}
}
