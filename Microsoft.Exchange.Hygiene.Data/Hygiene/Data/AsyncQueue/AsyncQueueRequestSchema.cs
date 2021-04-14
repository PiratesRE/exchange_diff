using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Hygiene.Data.AsyncQueue
{
	internal class AsyncQueueRequestSchema
	{
		internal static readonly HygienePropertyDefinition PriorityProperty = AsyncQueueCommonSchema.PriorityProperty;

		internal static readonly HygienePropertyDefinition RequestIdProperty = AsyncQueueCommonSchema.RequestIdProperty;

		internal static readonly HygienePropertyDefinition FriendlyNameProperty = AsyncQueueCommonSchema.FriendlyNameProperty;

		internal static readonly HygienePropertyDefinition RequestFlagsProperty = AsyncQueueCommonSchema.RequestFlagsProperty;

		internal static readonly HygienePropertyDefinition OwnerIdProperty = AsyncQueueCommonSchema.OwnerIdProperty;

		internal static readonly HygienePropertyDefinition OrganizationalUnitRootProperty = AsyncQueueCommonSchema.OrganizationalUnitRootProperty;

		internal static readonly HygienePropertyDefinition DependantOrganizationalUnitRootProperty = AsyncQueueCommonSchema.DependantOrganizationalUnitRootProperty;

		internal static readonly HygienePropertyDefinition DependantRequestIdProperty = AsyncQueueCommonSchema.DependantRequestIdProperty;

		internal static readonly HygienePropertyDefinition RequestStatusProperty = AsyncQueueCommonSchema.RequestStatusProperty;

		internal static readonly HygienePropertyDefinition CreatedDatetimeProperty = AsyncQueueCommonSchema.CreatedDatetimeProperty;

		internal static readonly HygienePropertyDefinition LastModifiedDatetimeProperty = AsyncQueueCommonSchema.LastModifiedDatetimeProperty;

		internal static readonly HygienePropertyDefinition RejectIfExistsProperty = new HygienePropertyDefinition("RejectIfExists", typeof(bool), false, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition FailIfDependencyMissingProperty = new HygienePropertyDefinition("FailIfDependencyMissing", typeof(bool), true, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition RequestCookieProperty = AsyncQueueCommonSchema.RequestCookieProperty;

		internal static readonly HygienePropertyDefinition ScheduleTimeProperty = new HygienePropertyDefinition("ScheduleDatetime", typeof(DateTime?));

		internal static readonly HygienePropertyDefinition AsyncQueueStepsProperty = new HygienePropertyDefinition("StepProperties", typeof(AsyncQueueStep), null, ADPropertyDefinitionFlags.MultiValued);
	}
}
