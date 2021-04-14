using System;

namespace System.Security
{
	internal enum WindowsImpersonationFlowMode
	{
		IMP_FASTFLOW,
		IMP_NOFLOW,
		IMP_ALWAYSFLOW,
		IMP_DEFAULT = 0
	}
}
