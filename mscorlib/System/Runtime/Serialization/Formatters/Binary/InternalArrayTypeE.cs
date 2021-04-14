using System;

namespace System.Runtime.Serialization.Formatters.Binary
{
	[Serializable]
	internal enum InternalArrayTypeE
	{
		Empty,
		Single,
		Jagged,
		Rectangular,
		Base64
	}
}
