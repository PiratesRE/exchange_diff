using System;
using Microsoft.Win32.SafeHandles;

namespace Microsoft.Exchange.Security
{
	internal abstract class DebugSafeHandle : SafeHandleZeroOrMinusOneIsInvalid
	{
		internal DebugSafeHandle() : base(true)
		{
		}
	}
}
