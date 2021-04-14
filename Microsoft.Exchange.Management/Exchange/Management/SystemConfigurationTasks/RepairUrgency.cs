using System;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	public enum RepairUrgency
	{
		[LocDescription(Strings.IDs.RepairUrgencyNormal)]
		Normal,
		[LocDescription(Strings.IDs.RepairUrgencyHigh)]
		High,
		[LocDescription(Strings.IDs.RepairUrgencyCritical)]
		Critical,
		[LocDescription(Strings.IDs.RepairUrgencyProhibited)]
		Prohibited
	}
}
