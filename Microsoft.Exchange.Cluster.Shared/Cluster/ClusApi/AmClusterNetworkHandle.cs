using System;
using System.Security.Permissions;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Cluster.ClusApi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
	internal class AmClusterNetworkHandle : SafeDisposeTrackerHandleZeroOrMinusOneIsInvalid
	{
		protected override bool ReleaseHandle()
		{
			return ClusapiMethods.CloseClusterNetwork(this.handle);
		}
	}
}
