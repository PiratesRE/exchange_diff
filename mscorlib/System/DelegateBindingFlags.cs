using System;

namespace System
{
	internal enum DelegateBindingFlags
	{
		StaticMethodOnly = 1,
		InstanceMethodOnly,
		OpenDelegateOnly = 4,
		ClosedDelegateOnly = 8,
		NeverCloseOverNull = 16,
		CaselessMatching = 32,
		SkipSecurityChecks = 64,
		RelaxedSignature = 128
	}
}
