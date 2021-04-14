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

namespace Microsoft.Exchange.Entities.Calendaring.EntitySets.EventCommands
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[DataContract]
	internal class UpdateSeries : UpdateEventBase, ICalendarInteropSeriesAction
	{
		public virtual Guid CommandId
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
				return ExTraceGlobals.UpdateSeriesTracer;
			}
		}

		public void ExecuteOnInstance(Event seriesInstanceInformation)
		{
			Event @event = new Event
			{
				Id = seriesInstanceInformation.Id,
				ChangeKey = seriesInstanceInformation.ChangeKey,
				Type = EventType.Exception
			};
			this.PrepareDataForInstance(@event);
			IActionPropagationState actionPropagationState = @event;
			actionPropagationState.LastExecutedAction = new Guid?(this.CommandId);
			UpdateEvent updateEvent = new UpdateEvent
			{
				EntityKey = @event.Id,
				Entity = @event,
				Scope = this.Scope,
				SeriesSequenceNumber = new int?(this.seriesSequenceNumber),
				SendMeetingMessagesOnSave = true,
				MasterGoid = this.masterGoid,
				PropagationInProgress = true
			};
			Event event2 = updateEvent.Execute(new CommandContext
			{
				IfMatchETag = seriesInstanceInformation.ChangeKey
			});
			seriesInstanceInformation.ChangeKey = event2.ChangeKey;
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

		public Event GetInitialMasterValue()
		{
			return base.Entity;
		}

		public Event InitialMasterOperation(Event updateToMaster)
		{
			UpdateEvent updateEvent = new UpdateEvent
			{
				Entity = updateToMaster,
				EntityKey = updateToMaster.Id,
				Scope = this.Scope,
				SendMeetingMessagesOnSave = true,
				SeriesSequenceNumber = new int?(this.seriesSequenceNumber)
			};
			return updateEvent.Execute(null);
		}

		protected virtual void PrepareDataForInstance(Event instanceDelta)
		{
			base.Entity.CopyMasterPropertiesTo(instanceDelta, false, null, true);
		}

		protected override Event OnExecute()
		{
			Event initialMasterValue = this.GetInitialMasterValue();
			Event @event = this.Scope.Read(initialMasterValue.Id, this.Context);
			base.MergeAttendeesList(@event.Attendees);
			IEventInternal eventInternal = initialMasterValue;
			IEventInternal eventInternal2 = @event;
			eventInternal.SeriesSequenceNumber = eventInternal2.SeriesSequenceNumber + 1;
			this.seriesSequenceNumber = eventInternal.SeriesSequenceNumber;
			this.masterGoid = ((eventInternal2.GlobalObjectId != null) ? new GlobalObjectId(eventInternal2.GlobalObjectId).Bytes : null);
			this.seriesPropagation = this.CreateInteropPropagationCommand(null);
			return this.seriesPropagation.Execute(null);
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

		[DataMember]
		private byte[] masterGoid;
	}
}
