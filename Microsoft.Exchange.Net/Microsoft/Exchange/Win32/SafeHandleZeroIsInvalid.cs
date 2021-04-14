using System;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Win32
{
	internal abstract class SafeHandleZeroIsInvalid : SafeHandle
	{
		public SafeHandleZeroIsInvalid() : base(IntPtr.Zero, true)
		{
		}

		protected SafeHandleZeroIsInvalid(IntPtr handle, bool ownsHandle) : base(handle, ownsHandle)
		{
		}

		public override bool IsInvalid
		{
			get
			{
				return this.handle == IntPtr.Zero;
			}
		}
	}
}
