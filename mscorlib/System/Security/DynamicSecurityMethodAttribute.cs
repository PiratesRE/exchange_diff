using System;

namespace System.Security
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
	internal sealed class DynamicSecurityMethodAttribute : Attribute
	{
	}
}
