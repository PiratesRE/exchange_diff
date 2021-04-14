using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.Exchange.Win32;

namespace Microsoft.Exchange.Security.Authorization
{
	[ComVisible(false)]
	[SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
	[SecurityPermission(SecurityAction.InheritanceDemand, UnmanagedCode = true)]
	internal sealed class ResourceManagerHandle : SafeHandle
	{
		internal ResourceManagerHandle() : base(IntPtr.Zero, true)
		{
		}

		public override bool IsInvalid
		{
			get
			{
				return this.handle == IntPtr.Zero;
			}
		}

		protected override bool ReleaseHandle()
		{
			return NativeMethods.AuthzFreeResourceManager(this.handle);
		}

		public static ResourceManagerHandle Create(string name)
		{
			ResourceManagerHandle result;
			if (!NativeMethods.AuthzInitializeResourceManager(ResourceManagerFlags.NoAudit, name, out result))
			{
				Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
			}
			return result;
		}
	}
}
