using System;
using System.Data;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Hygiene.Data.BackgroundJobBackend
{
	internal static class ScheduleItemProperties
	{
		internal static readonly BackgroundJobBackendPropertyDefinition ScheduleIdProperty = new BackgroundJobBackendPropertyDefinition("ScheduleId", typeof(Guid), PropertyDefinitionFlags.Mandatory, Guid.Empty);

		internal static readonly BackgroundJobBackendPropertyDefinition BackgroundJobIdProperty = new BackgroundJobBackendPropertyDefinition("BackgroundJobId", typeof(Guid), PropertyDefinitionFlags.Mandatory, Guid.Empty);

		internal static readonly BackgroundJobBackendPropertyDefinition SchedulingTypeProperty = new BackgroundJobBackendPropertyDefinition("SchedulingType", typeof(byte), PropertyDefinitionFlags.Mandatory, 0);

		internal static readonly BackgroundJobBackendPropertyDefinition StartTimeProperty = new BackgroundJobBackendPropertyDefinition("StartTime", typeof(DateTime?), PropertyDefinitionFlags.Mandatory, null);

		internal static readonly BackgroundJobBackendPropertyDefinition SchedulingIntervalProperty = new BackgroundJobBackendPropertyDefinition("SchedulingInterval", typeof(int?), PropertyDefinitionFlags.Mandatory, null);

		internal static readonly BackgroundJobBackendPropertyDefinition ScheduleDaysSetProperty = new BackgroundJobBackendPropertyDefinition("ScheduleDaysSet", typeof(byte?), PropertyDefinitionFlags.Mandatory, null);

		internal static readonly BackgroundJobBackendPropertyDefinition DCSelectionSetProperty = new BackgroundJobBackendPropertyDefinition("DCSelectionSet", typeof(long), PropertyDefinitionFlags.Mandatory | PropertyDefinitionFlags.PersistDefaultValue, 0L);

		internal static readonly BackgroundJobBackendPropertyDefinition RegionSelectionSetProperty = new BackgroundJobBackendPropertyDefinition("RegionSelectionSet", typeof(int), PropertyDefinitionFlags.Mandatory | PropertyDefinitionFlags.PersistDefaultValue, 0);

		internal static readonly BackgroundJobBackendPropertyDefinition TargetMachineNameProperty = new BackgroundJobBackendPropertyDefinition("TargetMachineName", typeof(string), PropertyDefinitionFlags.Mandatory, null);

		internal static readonly BackgroundJobBackendPropertyDefinition InstancesToRunProperty = new BackgroundJobBackendPropertyDefinition("InstancesToRun", typeof(byte), PropertyDefinitionFlags.Mandatory | PropertyDefinitionFlags.PersistDefaultValue, 0);

		internal static readonly BackgroundJobBackendPropertyDefinition RoleIdProperty = new BackgroundJobBackendPropertyDefinition("RoleId", typeof(Guid), PropertyDefinitionFlags.Mandatory, Guid.Empty);

		internal static readonly BackgroundJobBackendPropertyDefinition SingleInstancePerMachineProperty = new BackgroundJobBackendPropertyDefinition("SingleInstancePerMachine", typeof(bool), PropertyDefinitionFlags.Mandatory, false);

		internal static readonly BackgroundJobBackendPropertyDefinition SchedulingStrategyProperty = new BackgroundJobBackendPropertyDefinition("SchedulingStrategy", typeof(byte), PropertyDefinitionFlags.Mandatory, 0);

		internal static readonly BackgroundJobBackendPropertyDefinition TimeoutProperty = new BackgroundJobBackendPropertyDefinition("Timeout", typeof(int), PropertyDefinitionFlags.Mandatory | PropertyDefinitionFlags.PersistDefaultValue, 0);

		internal static readonly BackgroundJobBackendPropertyDefinition MaxLocalRetryCountProperty = new BackgroundJobBackendPropertyDefinition("MaxLocalRetryCount", typeof(byte), PropertyDefinitionFlags.Mandatory, 0);

		internal static readonly BackgroundJobBackendPropertyDefinition MaxFailoverCountProperty = new BackgroundJobBackendPropertyDefinition("MaxFailoverCount", typeof(short), PropertyDefinitionFlags.Mandatory, 0);

		internal static readonly BackgroundJobBackendPropertyDefinition LastScheduledTimeProperty = new BackgroundJobBackendPropertyDefinition("LastScheduledTime", typeof(DateTime?), PropertyDefinitionFlags.Mandatory, null);

		internal static readonly BackgroundJobBackendPropertyDefinition CreatedDatetimeProperty = new BackgroundJobBackendPropertyDefinition("CreatedDatetime", typeof(DateTime), PropertyDefinitionFlags.Mandatory | PropertyDefinitionFlags.PersistDefaultValue, new DateTime(0L));

		internal static readonly BackgroundJobBackendPropertyDefinition ChangedDatetimeProperty = new BackgroundJobBackendPropertyDefinition("ChangedDatetime", typeof(DateTime), PropertyDefinitionFlags.Mandatory | PropertyDefinitionFlags.PersistDefaultValue, new DateTime(0L));

		internal static readonly BackgroundJobBackendPropertyDefinition NextActiveJobIdProperty = new BackgroundJobBackendPropertyDefinition("NextActiveJobId", typeof(Guid), PropertyDefinitionFlags.Mandatory, Guid.Empty);

		internal static readonly BackgroundJobBackendPropertyDefinition ActiveProperty = new BackgroundJobBackendPropertyDefinition("Active", typeof(bool), PropertyDefinitionFlags.Mandatory, false);

		internal static readonly BackgroundJobBackendPropertyDefinition SchedIdToDCIdMappingProperty = new BackgroundJobBackendPropertyDefinition("tvp_BJMScheduleIdToDataCenterId", typeof(DataTable), PropertyDefinitionFlags.Mandatory, null);
	}
}
