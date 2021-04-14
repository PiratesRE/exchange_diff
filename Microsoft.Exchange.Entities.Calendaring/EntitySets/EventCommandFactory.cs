using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Entities.Calendaring.EntitySets.EventCommands;
using Microsoft.Exchange.Entities.DataModel;
using Microsoft.Exchange.Entities.DataModel.Calendaring;
using Microsoft.Exchange.Entities.DataModel.Calendaring.CustomActions;
using Microsoft.Exchange.Entities.EntitySets;
using Microsoft.Exchange.Entities.EntitySets.Commands;

namespace Microsoft.Exchange.Entities.Calendaring.EntitySets
{
	internal sealed class EventCommandFactory : IEventCommandFactory, IEntityCommandFactory<Events, Event>
	{
		private EventCommandFactory()
		{
		}

		public ICreateEntityCommand<Events, Event> CreateCreateCommand(Event entity, Events scope)
		{
			switch (entity.Type)
			{
			case EventType.SingleInstance:
				return this.singleInstanceEventCommandFactory.CreateCreateCommand(entity, scope);
			case EventType.Occurrence:
			case EventType.Exception:
				return this.nprInstanceCommandFactory.CreateCreateCommand(entity, scope);
			case EventType.SeriesMaster:
				return this.seriesEventCommandFactory.CreateCreateCommand(entity, scope);
			default:
				throw new ArgumentOutOfRangeException("entity.Type");
			}
		}

		public IDeleteEntityCommand<Events> CreateDeleteCommand(string key, Events scope)
		{
			return this.CreateCommand<IDeleteEntityCommand<Events>, VoidResult>(key, scope, () => new DeleteSeries(), () => new DeleteEvent());
		}

		public IFindEntitiesCommand<Events, Event> CreateFindCommand(IEntityQueryOptions queryOptions, Events scope)
		{
			return this.singleInstanceEventCommandFactory.CreateFindCommand(queryOptions, scope);
		}

		public IReadEntityCommand<Events, Event> CreateReadCommand(string key, Events scope)
		{
			return this.CreateCommand<IReadEntityCommand<Events, Event>, Event>(key, scope, () => this.seriesEventCommandFactory.CreateReadCommand(key, scope), () => this.singleInstanceEventCommandFactory.CreateReadCommand(key, scope));
		}

		public IUpdateEntityCommand<Events, Event> CreateUpdateCommand(string key, Event entity, Events scope)
		{
			return this.CreateUpdateCommand(key, entity, scope, null);
		}

		public UpdateEventBase CreateUpdateCommand(string key, Event entity, Events scope, UpdateEventParameters updateEventParameters)
		{
			return this.CreateCommand<UpdateEventBase, Event>(key, scope, () => new UpdateSeries
			{
				Entity = entity,
				UpdateEventParameters = updateEventParameters
			}, () => new UpdateEvent
			{
				Entity = entity,
				UpdateEventParameters = updateEventParameters
			});
		}

		public ConvertSingleEventToNprSeries CreateConvertSingleEventToNprCommand(string key, Events scope)
		{
			return new ConvertSingleEventToNprSeries
			{
				EntityKey = key,
				Scope = scope
			};
		}

		public CancelEventBase CreateCancelCommand(string key, Events scope)
		{
			return this.CreateCommand<CancelEventBase, VoidResult>(key, scope, () => new CancelSeries(), () => new CancelEvent());
		}

		public GetCalendarView CreateGetCalendarViewCommand(ICalendarViewParameters parameters, Events scope)
		{
			return new GetCalendarView(parameters)
			{
				Scope = scope
			};
		}

		public RespondToEventBase CreateRespondToCommand(string key, Events scope)
		{
			return this.CreateCommand<RespondToEventBase, VoidResult>(key, scope, () => new RespondToSeries(), () => new RespondToEvent());
		}

		public ForwardEventBase CreateForwardCommand(string key, Events scope)
		{
			return this.CreateCommand<ForwardEventBase, VoidResult>(key, scope, () => new ForwardSeries(), () => new ForwardEvent());
		}

		public ExpandSeries CreateExpandCommand(string key, Events scope)
		{
			return this.CreateCommand<ExpandSeries, ExpandedEvent>(key, scope, () => new ExpandSeries(), () => new ExpandSeries());
		}

		private TCommand CreateCommand<TCommand, TResult>(string key, Events scope, Func<TCommand> createSeriesCommand, Func<TCommand> createInstanceCommand) where TCommand : IKeyedEntityCommand<Events, TResult>
		{
			StoreObjectId storeObjectId = scope.IdConverter.ToStoreObjectId(key);
			TCommand tcommand = (storeObjectId.ObjectType == StoreObjectType.CalendarItemSeries) ? createSeriesCommand() : createInstanceCommand();
			this.InitializeKeyedEntityCommand<TResult>(tcommand, key, scope);
			return tcommand;
		}

		private void InitializeKeyedEntityCommand<TResult>(IKeyedEntityCommand<Events, TResult> command, string key, Events scope)
		{
			command.EntityKey = key;
			command.Scope = scope;
		}

		public static readonly IEventCommandFactory Instance = new EventCommandFactory();

		private readonly IEntityCommandFactory<Events, Event> singleInstanceEventCommandFactory = EntityCommandFactory<Events, Event, CreateEvent, DeleteEvent, FindEvents, ReadEvent, UpdateEvent>.Instance;

		private readonly IEntityCommandFactory<Events, Event> seriesEventCommandFactory = EntityCommandFactory<Events, Event, CreateSeries, DeleteSeries, FindEvents, ReadEvent, UpdateSeries>.Instance;

		private readonly IEntityCommandFactory<Events, Event> nprInstanceCommandFactory = EntityCommandFactory<Events, Event, CreateNprInstance, DeleteEvent, FindEvents, ReadEvent, UpdateEvent>.Instance;
	}
}
