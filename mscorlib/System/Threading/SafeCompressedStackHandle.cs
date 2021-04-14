using System;
using System.Runtime.InteropServices;
using System.Security;

namespace System.Threading
{
	[SecurityCritical]
	internal class SafeCompressedStackHandle : SafeHandle
	{
		public SafeCompressedStackHandle() : base(IntPtr.Zero, true)
		{
		}

		public override bool IsInvalid
		{
			[SecurityCritical]
			get
			{
				return this.handle == IntPtr.Zero;
			}
		}

		[SecurityCritical]
		protected override bool ReleaseHandle()
		{
			CompressedStack.DestroyDelayedCompressedStack(this.handle);
			this.handle = IntPtr.Zero;
			return true;
		}
	}
}
