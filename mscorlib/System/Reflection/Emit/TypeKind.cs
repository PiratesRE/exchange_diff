using System;

namespace System.Reflection.Emit
{
	[Serializable]
	internal enum TypeKind
	{
		IsArray = 1,
		IsPointer,
		IsByRef
	}
}
