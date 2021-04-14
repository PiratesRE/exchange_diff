using System;
using Microsoft.Exchange.ManagementGUI.Resources;

namespace Microsoft.Exchange.Management.SystemManager
{
	public enum DatabaseStatus
	{
		[LocDescription(Strings.IDs.DatabaseStatusUnknown)]
		Unknown,
		[LocDescription(Strings.IDs.DatabaseStatusMounted)]
		Mounted,
		[LocDescription(Strings.IDs.DatabaseStatusDismounted)]
		Dismounted
	}
}
