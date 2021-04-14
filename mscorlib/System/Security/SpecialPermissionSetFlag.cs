using System;

namespace System.Security
{
	[Serializable]
	internal enum SpecialPermissionSetFlag
	{
		Regular,
		NoSet,
		EmptySet,
		SkipVerification
	}
}
