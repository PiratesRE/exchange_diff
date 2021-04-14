using System;

namespace System.Runtime.Remoting
{
	internal struct IdOps
	{
		internal static bool bStrongIdentity(int flags)
		{
			return (flags & 2) != 0;
		}

		internal static bool bIsInitializing(int flags)
		{
			return (flags & 4) != 0;
		}

		internal const int None = 0;

		internal const int GenerateURI = 1;

		internal const int StrongIdentity = 2;

		internal const int IsInitializing = 4;
	}
}
