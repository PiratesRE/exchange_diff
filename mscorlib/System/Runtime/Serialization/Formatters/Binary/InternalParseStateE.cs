using System;

namespace System.Runtime.Serialization.Formatters.Binary
{
	[Serializable]
	internal enum InternalParseStateE
	{
		Initial,
		Object,
		Member,
		MemberChild
	}
}
