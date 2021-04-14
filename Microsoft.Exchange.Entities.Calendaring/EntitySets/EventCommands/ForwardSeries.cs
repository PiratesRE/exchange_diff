using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Entities.Calendaring;
using Microsoft.Exchange.Entities.Calendaring.Interop;
using Microsoft.Exchange.Entities.DataModel;
using Microsoft.Exchange.Entities.DataModel.Calendaring;
using Microsoft.Exchange.Entities.DataModel.ReliableActions;
using Microsoft.Exchange.Entities.DataProviders;

namespace Microsoft.Exchange.Entities.Calendaring.EntitySets.EventCommands
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[DataContract]
	internal class ForwardSeries : ForwardEventBase, ICalendarInteropSeriesAction
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
				return ExTraceGlobals.ForwardSeriesTracer;
			}
		}

		public void RestoreExecutionContext(Events entitySet, SeriesInteropCommand seriesInteropCommand)
		{
			this.seriesPropagation = seriesInteropCommand;
			base.RestoreScope(entitySet);
		}

		public Event CleanUp(Event master)
		{
			return this.seriesPropagation.RemoveActionFromPendingActionQueue(master, this.CommandId);
		}

		public void ExecuteOnInstance(Event seriesInstance)
		{
			Event @event = new Event
			{
				Id = seriesInstance.Id,
				ChangeKey = seriesInstance.ChangeKey
			};
			IActionPropagationState actionPropagationState = @event;
			actionPropagationState.LastExecutedAction = new Guid?(this.CommandId);
			ForwardEvent forwardEvent = new ForwardEvent
			{
				EntityKey = @event.Id,
				UpdateToEvent = @event,
				Parameters = base.Parameters,
				Scope = this.Scope,
				SeriesSequenceNumber = new int?(this.seriesSequenceNumber)
			};
			forwardEvent.Execute(new CommandContext
			{
				IfMatchETag = seriesInstance.ChangeKey
			});
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
			ForwardEvent forwardEvent = new ForwardEvent
			{
				EntityKey = updateToMaster.Id,
				UpdateToEvent = updateToMaster,
				Parameters = base.Parameters,
				Scope = this.Scope,
				SeriesSequenceNumber = new int?(this.seriesSequenceNumber),
				OccurrencesViewPropertiesBlob = this.occurrencesViewPropertiesBlob
			};
			forwardEvent.Execute(this.Context);
			return this.Scope.Read(base.EntityKey, null);
		}

		protected override VoidResult OnExecute()
		{
			Event @event = this.Scope.Read(base.EntityKey, this.Context);
			if (!@event.HasAttendees)
			{
				throw new InvalidRequestException(CalendaringStrings.ErrorForwardNotSupportedForNprAppointment);
			}
			this.occurrencesViewPropertiesBlob = this.Scope.EventDataProvider.CreateOccurrenceViewPropertiesBlob(@event);
			this.seriesSequenceNumber = ((IEventInternal)@event).SeriesSequenceNumber;
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

		private SeriesInteropCommand seriesPropagation;

		[DataMember]
		private int seriesSequenceNumber;

		private string occurrencesViewPropertiesBlob;
	}
}
