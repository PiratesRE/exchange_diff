using System;
using System.Security.Permissions;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Cluster.ClusApi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
	internal class AmClusNetworkEnumHandle : SafeDisposeTrackerHandleZeroOrMinusOneIsInvalid
	{
		protected override bool ReleaseHandle()
		{
			int num = ClusapiMethods.ClusterNetworkCloseEnum(this.handle);
			return num == 0;
		}
	}
}
