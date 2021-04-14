using System;
using System.Security.Permissions;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Win32.SafeHandles;

namespace Microsoft.Exchange.Cluster.ClusApi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
	internal class AmClusterRegkeyHandle : SafeHandleZeroOrMinusOneIsInvalid
	{
		internal string Name { get; set; }

		public AmClusterRegkeyHandle() : base(true)
		{
			base.SetHandle(IntPtr.Zero);
		}

		protected override bool ReleaseHandle()
		{
			return ClusapiMethods.ClusterRegCloseKey(this.handle, this.Name) == 0;
		}
	}
}
