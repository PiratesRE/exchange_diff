using System;
using System.Security.Permissions;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Cluster.ClusApi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
	internal class AmClusterBatchHandle : SafeDisposeTrackerHandleZeroOrMinusOneIsInvalid
	{
		internal int CommitAndClose()
		{
			int num;
			int result = ClusapiMethods.ClusterRegCloseBatch(this.handle, true, out num);
			base.SetHandle(IntPtr.Zero);
			return result;
		}

		protected override bool ReleaseHandle()
		{
			int num;
			return ClusapiMethods.ClusterRegCloseBatch(this.handle, false, out num) == 0;
		}
	}
}
