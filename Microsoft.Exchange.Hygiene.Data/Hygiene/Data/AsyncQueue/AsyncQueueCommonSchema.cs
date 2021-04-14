using System;
using System.Data.SqlTypes;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Hygiene.Data.AsyncQueue
{
	internal class AsyncQueueCommonSchema
	{
		internal static readonly HygienePropertyDefinition PriorityProperty = new HygienePropertyDefinition("Priority", typeof(byte));

		internal static readonly HygienePropertyDefinition RequestIdProperty = new HygienePropertyDefinition("RequestId", typeof(Guid));

		internal static readonly HygienePropertyDefinition RequestStepIdProperty = new HygienePropertyDefinition("RequestStepId", typeof(Guid));

		internal static readonly HygienePropertyDefinition FriendlyNameProperty = new HygienePropertyDefinition("FriendlyName", typeof(string));

		internal static readonly HygienePropertyDefinition RequestFlagsProperty = new HygienePropertyDefinition("RequestFlags", typeof(int), 0, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition FlagsProperty = new HygienePropertyDefinition("Flags", typeof(int), 0, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition OwnerIdProperty = new HygienePropertyDefinition("OwnerId", typeof(string));

		internal static readonly HygienePropertyDefinition RequestStatusProperty = new HygienePropertyDefinition("RequestStatus", typeof(short), 0, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition RequestCookieProperty = new HygienePropertyDefinition("RequestCookie", typeof(string));

		internal static readonly HygienePropertyDefinition CreatedDatetimeProperty = new HygienePropertyDefinition("CreatedDatetime", typeof(DateTime), SqlDateTime.MinValue.Value, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition LastModifiedDatetimeProperty = new HygienePropertyDefinition("LastModifiedDatetime", typeof(DateTime), SqlDateTime.MinValue.Value, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition OrganizationalUnitRootProperty = new HygienePropertyDefinition("OrganizationalUnitRoot", typeof(Guid));

		internal static readonly HygienePropertyDefinition DependantOrganizationalUnitRootProperty = new HygienePropertyDefinition("DepOrganizationalUnitRoot", typeof(Guid?));

		internal static readonly HygienePropertyDefinition DependantRequestIdProperty = new HygienePropertyDefinition("DependantRequestId", typeof(Guid?));

		internal static readonly HygienePropertyDefinition StepNumberProperty = new HygienePropertyDefinition("StepNumber", typeof(short), short.MinValue, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition StepNameProperty = new HygienePropertyDefinition("StepName", typeof(string));

		internal static readonly HygienePropertyDefinition StepOrdinalProperty = new HygienePropertyDefinition("StepOrdinal", typeof(short), 0, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition FetchCountProperty = new HygienePropertyDefinition("FetchCount", typeof(int), 0, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition ErrorCountProperty = new HygienePropertyDefinition("ErrorCount", typeof(int), 0, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition ProcessInstanceNameProperty = new HygienePropertyDefinition("ProcessInstanceName", typeof(string));

		internal static readonly HygienePropertyDefinition MaxExecutionTimeProperty = new HygienePropertyDefinition("MaxExecutionTime", typeof(int), 0, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition CookieProperty = new HygienePropertyDefinition("Cookie", typeof(string));

		internal static readonly HygienePropertyDefinition BatchSizeQueryProperty = new HygienePropertyDefinition("BatchSize", typeof(int), int.MinValue, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition FailoverWaitInSecondsQueryProperty = new HygienePropertyDefinition("FailoverWaitInSeconds", typeof(int), 0, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition PageSizeQueryProperty = new HygienePropertyDefinition("PageSize", typeof(int), int.MinValue, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition MinCreationDateTimeQueryProperty = new HygienePropertyDefinition("MinCreateDateTime", typeof(DateTime?));

		internal static readonly HygienePropertyDefinition MaxCreationDateTimeQueryProperty = new HygienePropertyDefinition("MaxCreateDateTime", typeof(DateTime?));

		internal static readonly HygienePropertyDefinition DependantOnRequestIdQueryProperty = new HygienePropertyDefinition("DependantOnRequestId", typeof(Guid));

		internal static readonly HygienePropertyDefinition OwnerListQueryProperty = new HygienePropertyDefinition("OwnerProperties", typeof(object), null, ADPropertyDefinitionFlags.MultiValued);

		internal static readonly HygienePropertyDefinition StepFlagsProperty = new HygienePropertyDefinition("StepFlags", typeof(int), 0, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition StepStatusProperty = new HygienePropertyDefinition("StepStatus", typeof(short), 0, ADPropertyDefinitionFlags.PersistDefaultValue);
	}
}
