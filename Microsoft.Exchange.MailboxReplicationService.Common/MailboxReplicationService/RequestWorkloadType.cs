using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	public enum RequestWorkloadType
	{
		[LocDescription(MrsStrings.IDs.WorkloadTypeNone)]
		None,
		[LocDescription(MrsStrings.IDs.WorkloadTypeLocal)]
		Local,
		[LocDescription(MrsStrings.IDs.WorkloadTypeOnboarding)]
		Onboarding,
		[LocDescription(MrsStrings.IDs.WorkloadTypeOffboarding)]
		Offboarding,
		[LocDescription(MrsStrings.IDs.WorkloadTypeTenantUpgrade)]
		TenantUpgrade,
		[LocDescription(MrsStrings.IDs.WorkloadTypeLoadBalancing)]
		LoadBalancing,
		[LocDescription(MrsStrings.IDs.WorkloadTypeEmergency)]
		Emergency,
		[LocDescription(MrsStrings.IDs.WorkloadTypeRemotePstIngestion)]
		RemotePstIngestion,
		[LocDescription(MrsStrings.IDs.WorkloadTypeSyncAggregation)]
		SyncAggregation,
		[LocDescription(MrsStrings.IDs.WorkloadTypeRemotePstExport)]
		RemotePstExport
	}
}
