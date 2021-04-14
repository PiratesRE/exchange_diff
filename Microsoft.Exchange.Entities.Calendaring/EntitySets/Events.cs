using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Entities.Calendaring.DataProviders;
using Microsoft.Exchange.Entities.Calendaring.EntitySets.EventCommands;
using Microsoft.Exchange.Entities.DataModel;
using Microsoft.Exchange.Entities.DataModel.Calendaring;
using Microsoft.Exchange.Entities.DataModel.Calendaring.CustomActions;
using Microsoft.Exchange.Entities.EntitySets;

namespace Microsoft.Exchange.Entities.Calendaring.EntitySets
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class Events : StorageEntitySet<Events, Event, IEventCommandFactory, IStoreSession>, IEvents, IEntitySet<Event>
	{
		protected internal Events(IStorageEntitySetScope<IStoreSession> parentScope, ILocalCalendarReference localCalendar) : base(parentScope, "Events", EventCommandFactory.Instance)
		{
			this.LocalCalendar = localCalendar;
		}

		public ILocalCalendarReference LocalCalendar { get; private set; }

		public virtual EventDataProvider EventDataProvider
		{
			get
			{
				EventDataProvider result;
				if ((result = this.eventDataProvider) == null)
				{
					result = (this.eventDataProvider = new EventDataProvider(this, this.LocalCalendar.GetCalendarFolderId()));
				}
				return result;
			}
		}

		public virtual SeriesEventDataProvider SeriesEventDataProvider
		{
			get
			{
				SeriesEventDataProvider result;
				if ((result = this.seriesEventDataProvider) == null)
				{
					result = (this.seriesEventDataProvider = new SeriesEventDataProvider(this, this.LocalCalendar.GetCalendarFolderId()));
				}
				return result;
			}
		}

		public virtual EventTimeAdjuster TimeAdjuster
		{
			get
			{
				EventTimeAdjuster result;
				if ((result = this.timeAdjuster) == null)
				{
					result = (this.timeAdjuster = new EventTimeAdjuster(new DateTimeHelper()));
				}
				return result;
			}
		}

		public IEventReference this[string eventId]
		{
			get
			{
				return new EventReference(this, eventId);
			}
		}

		public void Cancel(string key, CancelEventParameters parameters, CommandContext context = null)
		{
			CancelEventBase cancelEventBase = base.CommandFactory.CreateCancelCommand(key, this);
			cancelEventBase.Parameters = parameters;
			cancelEventBase.Execute(context);
		}

		public void Forward(string key, ForwardEventParameters parameters, CommandContext context = null)
		{
			ForwardEventBase forwardEventBase = base.CommandFactory.CreateForwardCommand(key, this);
			forwardEventBase.Parameters = parameters;
			forwardEventBase.Execute(context);
		}

		public ExpandedEvent Expand(string key, ExpandEventParameters parameters, CommandContext context = null)
		{
			ExpandSeries expandSeries = base.CommandFactory.CreateExpandCommand(key, this);
			expandSeries.Parameters = parameters;
			return expandSeries.Execute(context);
		}

		public IEnumerable<Event> GetCalendarView(ICalendarViewParameters parameters, CommandContext context = null)
		{
			GetCalendarView getCalendarView = base.CommandFactory.CreateGetCalendarViewCommand(parameters, this);
			return getCalendarView.Execute(context);
		}

		public void Respond(string key, RespondToEventParameters parameters, CommandContext context = null)
		{
			RespondToEventBase respondToEventBase = base.CommandFactory.CreateRespondToCommand(key, this);
			respondToEventBase.Parameters = parameters;
			respondToEventBase.Execute(context);
		}

		public Event Update(string key, Event entity, UpdateEventParameters updateEventParameters, CommandContext context = null)
		{
			UpdateEventBase updateEventBase = base.CommandFactory.CreateUpdateCommand(key, entity, this, updateEventParameters);
			return updateEventBase.Execute(context);
		}

		public Event ConvertSingleEventToNprSeries(string key, IList<Event> additionalInstancesToAdd, string clientId, CommandContext context = null)
		{
			ConvertSingleEventToNprSeries convertSingleEventToNprSeries = base.CommandFactory.CreateConvertSingleEventToNprCommand(key, this);
			convertSingleEventToNprSeries.AdditionalInstancesToAdd = additionalInstancesToAdd;
			convertSingleEventToNprSeries.ClientId = clientId;
			return convertSingleEventToNprSeries.Execute(context);
		}

		internal virtual EventDataProvider GetDataProvider(StoreId storeId)
		{
			StoreObjectId storeObjectId = StoreId.GetStoreObjectId(storeId);
			if (storeObjectId.ObjectType != StoreObjectType.CalendarItemSeries)
			{
				return this.EventDataProvider;
			}
			return this.SeriesEventDataProvider;
		}

		private EventDataProvider eventDataProvider;

		private SeriesEventDataProvider seriesEventDataProvider;

		private EventTimeAdjuster timeAdjuster;
	}
}
