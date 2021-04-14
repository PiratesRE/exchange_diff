using System;

namespace System.Runtime.CompilerServices
{
	[AttributeUsage(AttributeTargets.Struct)]
	[Serializable]
	public sealed class HasCopySemanticsAttribute : Attribute
	{
	}
}
