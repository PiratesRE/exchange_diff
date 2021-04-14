using System;
using Microsoft.Win32.SafeHandles;

namespace Microsoft.Exchange.Cluster.Shared
{
	internal class ModuleHandle : SafeHandleZeroOrMinusOneIsInvalid
	{
		public ModuleHandle() : base(true)
		{
			base.SetHandle(IntPtr.Zero);
		}

		protected override bool ReleaseHandle()
		{
			return NativeMethods.FreeLibrary(this.handle);
		}
	}
}
