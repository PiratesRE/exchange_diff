using System;

namespace System.Runtime.Serialization.Formatters.Binary
{
	[Serializable]
	internal enum InternalNameSpaceE
	{
		None,
		Soap,
		XdrPrimitive,
		XdrString,
		UrtSystem,
		UrtUser,
		UserNameSpace,
		MemberName,
		Interop,
		CallElement
	}
}
