using System;

namespace System.Security
{
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false, Inherited = false)]
	[__DynamicallyInvokable]
	public sealed class SecurityTransparentAttribute : Attribute
	{
		[__DynamicallyInvokable]
		public SecurityTransparentAttribute()
		{
		}
	}
}
