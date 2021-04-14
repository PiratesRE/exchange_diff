using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface ICalendarFolder : IFolder, IStoreObject, IStorePropertyBag, IPropertyBag, IReadOnlyPropertyBag, IDisposable
	{
		Guid ConsumerCalendarGuid { get; set; }

		long ConsumerCalendarOwnerId { get; set; }

		Guid ConsumerCalendarPrivateFreeBusyId { get; set; }

		Guid ConsumerCalendarPrivateDetailId { get; set; }

		PublishVisibility ConsumerCalendarPublishVisibility { get; set; }

		string ConsumerCalendarSharingInvitations { get; set; }

		SharingPermissionLevel ConsumerCalendarPermissionLevel { get; set; }

		string ConsumerCalendarSynchronizationState { get; set; }

		object[][] GetCalendarView(ExDateTime startTime, ExDateTime endTime, params PropertyDefinition[] dataColumns);

		object[][] GetSyncView(ExDateTime startTime, ExDateTime endTime, CalendarViewBatchingStrategy batchingStrategy, PropertyDefinition[] dataColumns, bool includeNprMasters);
	}
}
