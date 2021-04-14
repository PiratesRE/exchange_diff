using System;

namespace Microsoft.Exchange.Rpc.ActiveManager
{
	[Flags]
	public enum AmMountFlags
	{
		None = 0,
		MountWithForce = 1,
		MoveWithSkipHealth = 2
	}
}
