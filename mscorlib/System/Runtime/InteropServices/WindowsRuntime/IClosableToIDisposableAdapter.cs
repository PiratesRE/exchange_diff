using System;
using System.Runtime.CompilerServices;
using System.Security;

namespace System.Runtime.InteropServices.WindowsRuntime
{
	[SecurityCritical]
	internal sealed class IClosableToIDisposableAdapter
	{
		private IClosableToIDisposableAdapter()
		{
		}

		[SecurityCritical]
		private void Dispose()
		{
			IClosable closable = JitHelpers.UnsafeCast<IClosable>(this);
			closable.Close();
		}
	}
}
