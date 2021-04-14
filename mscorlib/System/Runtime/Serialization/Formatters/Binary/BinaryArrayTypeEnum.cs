using System;

namespace System.Runtime.Serialization.Formatters.Binary
{
	[Serializable]
	internal enum BinaryArrayTypeEnum
	{
		Single,
		Jagged,
		Rectangular,
		SingleOffset,
		JaggedOffset,
		RectangularOffset
	}
}
