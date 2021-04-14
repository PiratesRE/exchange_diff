using System;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	public enum TransitionState : short
	{
		[LocDescription(Strings.IDs.TransitionStateUnknown)]
		Unknown,
		[LocDescription(Strings.IDs.TransitionStateActive)]
		Active,
		[LocDescription(Strings.IDs.TransitionStateInactive)]
		Inactive
	}
}
