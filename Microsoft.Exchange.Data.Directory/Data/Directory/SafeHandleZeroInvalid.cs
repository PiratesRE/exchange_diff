using System;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Data.Directory
{
	internal abstract class SafeHandleZeroInvalid : SafeHandle
	{
		public SafeHandleZeroInvalid() : base(IntPtr.Zero, true)
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
