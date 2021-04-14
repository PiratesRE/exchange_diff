using System;

namespace System.Runtime.InteropServices
{
	[AttributeUsage(AttributeTargets.All, Inherited = false)]
	[ComVisible(true)]
	public sealed class ComConversionLossAttribute : Attribute
	{
	}
}
