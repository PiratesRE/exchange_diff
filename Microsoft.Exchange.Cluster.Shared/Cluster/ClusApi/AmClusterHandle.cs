using System;
using System.Security.Permissions;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Win32.SafeHandles;

namespace Microsoft.Exchange.Cluster.ClusApi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
	internal class AmClusterHandle : SafeHandleZeroOrMinusOneIsInvalid
	{
		public AmClusterHandle() : base(true)
		{
			base.SetHandle(IntPtr.Zero);
		}

		protected override bool ReleaseHandle()
		{
			return ClusapiMethods.CloseCluster(this.handle);
		}
	}
}
