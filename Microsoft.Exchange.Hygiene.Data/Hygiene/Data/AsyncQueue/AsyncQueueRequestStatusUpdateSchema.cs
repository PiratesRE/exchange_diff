using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Hygiene.Data.AsyncQueue
{
	internal class AsyncQueueRequestStatusUpdateSchema
	{
		internal static readonly HygienePropertyDefinition RequestIdProperty = AsyncQueueCommonSchema.RequestIdProperty;

		internal static readonly HygienePropertyDefinition OrganizationalUnitRootProperty = AsyncQueueCommonSchema.OrganizationalUnitRootProperty;

		internal static readonly HygienePropertyDefinition RequestStepIdProperty = AsyncQueueCommonSchema.RequestStepIdProperty;

		internal static readonly HygienePropertyDefinition CurrentStatusProperty = new HygienePropertyDefinition("FromStatus", typeof(short), 0, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition StatusProperty = new HygienePropertyDefinition("Status", typeof(short), 0, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition ProcessInstanceNameProperty = AsyncQueueCommonSchema.ProcessInstanceNameProperty;

		internal static readonly HygienePropertyDefinition RetryIntervalProperty = new HygienePropertyDefinition("RetryInterval", typeof(int), 0, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition RequestCompleteProperty = new HygienePropertyDefinition("RequestComplete", typeof(bool), false, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition CookieProperty = AsyncQueueCommonSchema.CookieProperty;

		internal static readonly HygienePropertyDefinition RequestStatusProperty = AsyncQueueCommonSchema.RequestStatusProperty;

		internal static readonly HygienePropertyDefinition RequestStartDatetimeProperty = new HygienePropertyDefinition("RequestStartDatetime", typeof(DateTime?));

		internal static readonly HygienePropertyDefinition RequestEndDatetimeProperty = new HygienePropertyDefinition("RequestEndDatetime", typeof(DateTime?));
	}
}
