using System;

namespace System.Reflection
{
	[Flags]
	[Serializable]
	internal enum MethodSemanticsAttributes
	{
		Setter = 1,
		Getter = 2,
		Other = 4,
		AddOn = 8,
		RemoveOn = 16,
		Fire = 32
	}
}
