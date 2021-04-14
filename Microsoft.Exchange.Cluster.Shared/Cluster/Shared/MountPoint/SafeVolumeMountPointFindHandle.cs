using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;
using Microsoft.Win32.SafeHandles;

namespace Microsoft.Exchange.Cluster.Shared.MountPoint
{
	internal sealed class SafeVolumeMountPointFindHandle : SafeHandleZeroOrMinusOneIsInvalid
	{
		public SafeVolumeMountPointFindHandle() : base(true)
		{
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		protected override bool ReleaseHandle()
		{
			return SafeVolumeMountPointFindHandle.FindVolumeMountPointClose(this.handle);
		}

		[SuppressUnmanagedCodeSecurity]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		[DllImport("kernel32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool FindVolumeMountPointClose([In] IntPtr handle);
	}
}
