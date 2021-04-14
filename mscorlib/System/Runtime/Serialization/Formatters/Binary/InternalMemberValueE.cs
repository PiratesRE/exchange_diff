using System;

namespace System.Runtime.Serialization.Formatters.Binary
{
	[Serializable]
	internal enum InternalMemberValueE
	{
		Empty,
		InlineValue,
		Nested,
		Reference,
		Null
	}
}
