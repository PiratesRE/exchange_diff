using System;
using System.Security;

namespace System.Threading
{
	internal interface IDeferredDisposable
	{
		[SecurityCritical]
		void OnFinalRelease(bool disposed);
	}
}
