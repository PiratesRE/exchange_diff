using System;

namespace System.Runtime.InteropServices
{
	[AttributeUsage(AttributeTargets.Method, Inherited = false)]
	[ComVisible(true)]
	public sealed class ComUnregisterFunctionAttribute : Attribute
	{
	}
}
