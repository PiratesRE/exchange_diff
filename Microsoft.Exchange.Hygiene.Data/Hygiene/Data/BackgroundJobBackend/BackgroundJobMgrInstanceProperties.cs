using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Hygiene.Data.BackgroundJobBackend
{
	internal static class BackgroundJobMgrInstanceProperties
	{
		internal static readonly BackgroundJobBackendPropertyDefinition MachineIdProperty = new BackgroundJobBackendPropertyDefinition("MachineId", typeof(Guid), PropertyDefinitionFlags.Mandatory, Guid.Empty);

		internal static readonly BackgroundJobBackendPropertyDefinition MachineNameProperty = new BackgroundJobBackendPropertyDefinition("MachineName", typeof(string), PropertyDefinitionFlags.Mandatory, null);

		internal static readonly BackgroundJobBackendPropertyDefinition RoleIdProperty = new BackgroundJobBackendPropertyDefinition("RoleId", typeof(Guid), PropertyDefinitionFlags.Mandatory, Guid.Empty);

		internal static readonly BackgroundJobBackendPropertyDefinition HeartBeatProperty = new BackgroundJobBackendPropertyDefinition("Heartbeat", typeof(DateTime), PropertyDefinitionFlags.Mandatory | PropertyDefinitionFlags.PersistDefaultValue, new DateTime(0L));

		internal static readonly BackgroundJobBackendPropertyDefinition ActiveProperty = new BackgroundJobBackendPropertyDefinition("Active", typeof(bool), PropertyDefinitionFlags.Mandatory, false);

		internal static readonly BackgroundJobBackendPropertyDefinition DCProperty = new BackgroundJobBackendPropertyDefinition("DC", typeof(long), PropertyDefinitionFlags.Mandatory | PropertyDefinitionFlags.PersistDefaultValue, 0L);

		internal static readonly BackgroundJobBackendPropertyDefinition RegionProperty = new BackgroundJobBackendPropertyDefinition("Region", typeof(int), PropertyDefinitionFlags.Mandatory | PropertyDefinitionFlags.PersistDefaultValue, 0);

		internal static readonly BackgroundJobBackendPropertyDefinition SyncContextProperty = new BackgroundJobBackendPropertyDefinition("SyncContext", typeof(Guid), PropertyDefinitionFlags.Mandatory, Guid.Empty);
	}
}
