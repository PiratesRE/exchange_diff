using System;

namespace System.Runtime.Serialization.Formatters.Binary
{
	[Serializable]
	internal enum BinaryTypeEnum
	{
		Primitive,
		String,
		Object,
		ObjectUrt,
		ObjectUser,
		ObjectArray,
		StringArray,
		PrimitiveArray
	}
}
