using System;
using System.Runtime.InteropServices;

namespace System.Runtime.CompilerServices
{
	[AttributeUsage(AttributeTargets.Struct, Inherited = true)]
	[ComVisible(true)]
	[Serializable]
	public sealed class NativeCppClassAttribute : Attribute
	{
	}
}
