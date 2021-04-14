using System;

namespace System.Runtime.Serialization.Formatters.Binary
{
	[Serializable]
	internal enum ValueFixupEnum
	{
		Empty,
		Array,
		Header,
		Member
	}
}
