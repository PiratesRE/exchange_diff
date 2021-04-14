using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Entities.Calendaring;
using Microsoft.Exchange.Entities.Calendaring.DataProviders;
using Microsoft.Exchange.Entities.Calendaring.Interop;
using Microsoft.Exchange.Entities.DataModel;
using Microsoft.Exchange.Entities.DataModel.Calendaring;
using Microsoft.Exchange.Entities.DataModel.ReliableActions;
using Microsoft.Exchange.Entities.DataProviders;

namespace Microsoft.Exchange.Entities.Calendaring.EntitySets.EventCommands
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[DataContract]
	internal class RespondToSeries : RespondToEventBase, ICalendarInteropSeriesAction
	{
		public Guid CommandId
		{
			get
			{
				return base.Id;
			}
		}

		protected override ITracer Trace
		{
			get
			{
				return ExTraceGlobals.RespondToSeriesTracer;
			}
		}

		public void RestoreExecutionContext(Events entitySet, SeriesInteropCommand seriesInteropCommand)
		{
			this.seriesPropagation = seriesInteropCommand;
			base.RestoreScope(entitySet);
		}

		public Event CleanUp(Event master)
		{
			StoreObjectId id = this.Scope.IdConverter.ToStoreObjectId(master.Id);
			if (!this.CleanUpDeclinedEvent(id))
			{
				return this.seriesPropagation.RemoveActionFromPendingActionQueue(master, this.CommandId);
			}
			return null;
		}

		public void ExecuteOnInstance(Event seriesInstance)
		{
			base.Parameters.MeetingRequestIdToBeDeleted = null;
			Event @event = new Event
			{
				Id = seriesInstance.Id,
				ChangeKey = seriesInstance.ChangeKey
			};
			IActionPropagationState actionPropagationState = @event;
			actionPropagationState.LastExecutedAction = new Guid?(this.CommandId);
			IEventInternal eventInternal = @event;
			eventInternal.SeriesToInstancePropagation = true;
			RespondToEvent respondToEvent = new RespondToEvent
			{
				EntityKey = @event.Id,
				UpdateToEvent = @event,
				Parameters = base.Parameters,
				Scope = this.Scope
			};
			respondToEvent.Execute(new CommandContext
			{
				IfMatchETag = seriesInstance.ChangeKey
			});
			if (base.Parameters.Response != ResponseType.Declined)
			{
				Event event2 = this.Scope.Read(base.EntityKey, null);
				seriesInstance.ChangeKey = event2.ChangeKey;
			}
		}

		public Event GetInitialMasterValue()
		{
			return new Event
			{
				Id = base.EntityKey
			};
		}

		public Event InitialMasterOperation(Event updateToMaster)
		{
			StoreId entityStoreId = this.GetEntityStoreId();
			EventDataProvider eventDataProvider = this.Scope.EventDataProvider;
			Event eventObject = eventDataProvider.Read(entityStoreId);
			this.Validate(eventObject);
			RespondToEvent respondToEvent = new RespondToEvent
			{
				EntityKey = updateToMaster.Id,
				UpdateToEvent = updateToMaster,
				SkipDeclinedEventRemoval = true,
				Parameters = base.Parameters,
				Scope = this.Scope
			};
			respondToEvent.Execute(this.Context);
			return this.Scope.Read(base.EntityKey, null);
		}

		protected override VoidResult OnExecute()
		{
			this.seriesPropagation = this.CreateInteropPropagationCommand(null);
			this.seriesPropagation.Execute(null);
			return VoidResult.Value;
		}

		protected virtual SeriesInlineInterop CreateInteropPropagationCommand(ICalendarInteropLog logger = null)
		{
			return new SeriesInlineInterop(this, logger)
			{
				EntityKey = base.EntityKey,
				Scope = this.Scope
			};
		}

		protected override void Validate(Event eventObject)
		{
			base.Validate(eventObject);
			if (base.Parameters.ProposedEndTime != null || base.Parameters.ProposedStartTime != null)
			{
				throw new InvalidRequestException(CalendaringStrings.ErrorProposedNewTimeNotSupportedForNpr);
			}
		}

		private SeriesInteropCommand seriesPropagation;
	}
}
