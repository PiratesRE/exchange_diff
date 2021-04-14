using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.ActivityLog;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Entities.Calendaring;
using Microsoft.Exchange.Entities.Calendaring.DataProviders;
using Microsoft.Exchange.Entities.DataModel.Calendaring;

namespace Microsoft.Exchange.Entities.Calendaring.EntitySets.EventCommands
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class RespondToEvent : RespondToEventBase
	{
		internal Event UpdateToEvent { get; set; }

		internal bool SkipDeclinedEventRemoval { get; set; }

		protected override ITracer Trace
		{
			get
			{
				return ExTraceGlobals.RespondToEventTracer;
			}
		}

		protected override VoidResult OnExecute()
		{
			StoreId entityStoreId = this.GetEntityStoreId();
			EventDataProvider eventDataProvider = this.Scope.EventDataProvider;
			Event eventObject = eventDataProvider.Read(entityStoreId);
			this.Validate(eventObject);
			eventDataProvider.RespondToEvent(entityStoreId, base.Parameters, this.UpdateToEvent);
			this.Scope.EventDataProvider.TryLogCalendarEventActivity(ActivityId.UpdateCalendarEvent, StoreId.GetStoreObjectId(entityStoreId));
			this.CleanUpDeclinedEvent(entityStoreId);
			this.DeleteMeetingRequestIfRequired(eventObject);
			return VoidResult.Value;
		}

		protected virtual bool DeleteMeetingRequestIfRequired(Event eventObject)
		{
			if (!string.IsNullOrEmpty(base.Parameters.MeetingRequestIdToBeDeleted))
			{
				eventObject.DeleteRelatedMessage(base.Parameters.MeetingRequestIdToBeDeleted, DeleteItemFlags.MoveToDeletedItems, this.Scope.XsoFactory, this.Scope.IdConverter, this.Scope.Session, true);
				return true;
			}
			return false;
		}

		protected override bool CleanUpDeclinedEvent(StoreId id)
		{
			return !this.SkipDeclinedEventRemoval && base.CleanUpDeclinedEvent(id);
		}
	}
}
