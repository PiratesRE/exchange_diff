using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[ComVisible(false)]
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class SafeExProwsHandle : SafeExMemoryHandle
	{
		internal SafeExProwsHandle()
		{
		}

		internal SafeExProwsHandle(IntPtr handle) : base(handle)
		{
		}

		public override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<SafeExProwsHandle>(this);
		}

		protected override bool ReleaseHandle()
		{
			if (!this.IsInvalid)
			{
				SafeExProwsHandle.FreeProwsFnEx(this.handle);
			}
			return true;
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern void FreeProwsFnEx(IntPtr buffer);
	}
}
