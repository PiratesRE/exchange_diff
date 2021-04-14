using System;
using System.Diagnostics;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Entities.Calendaring.DataProviders;
using Microsoft.Exchange.Entities.Calendaring.EntitySets;
using Microsoft.Exchange.Entities.DataModel.Calendaring;
using Microsoft.Exchange.Entities.DataModel.ReliableActions;
using Microsoft.Exchange.Entities.DataProviders;
using Microsoft.Exchange.Entities.EntitySets.Commands;

namespace Microsoft.Exchange.Entities.Calendaring.Interop
{
	internal abstract class SeriesInteropCommand : KeyedEntityCommand<Events, Event>
	{
		protected SeriesInteropCommand(EventSeriesPropagationConfig interopConfiguration, ICalendarInteropLog logger)
		{
			this.InteropConfiguration = interopConfiguration;
			this.Logger = logger;
		}

		public ICalendarInteropLog Logger { get; private set; }

		private protected EventSeriesPropagationConfig InteropConfiguration { protected get; private set; }

		public abstract Event CleanUp(Event master);

		public abstract void ExecuteOnInstance(Event seriesInstance);

		internal Event RemoveActionFromPendingActionQueue(Event masterToCleanup, Guid actionId)
		{
			Event @event = new Event
			{
				Id = masterToCleanup.Id,
				ChangeKey = masterToCleanup.ChangeKey
			};
			IActionQueue actionQueue = @event;
			actionQueue.ActionsToRemove = new Guid[]
			{
				actionId
			};
			return this.ExecuteOnMasterWithConflictRetries((Event theEvent) => this.Scope.EventDataProvider.Update(theEvent, null), @event, false);
		}

		protected sealed override Event OnExecute()
		{
			this.master = this.InitialMasterOperation();
			this.PropagateToSeriesInstances();
			this.TryCleanUp();
			return this.master;
		}

		protected abstract Event InitialMasterOperation();

		protected abstract bool ShouldExecuteOnInstance(Event seriesInstance);

		protected Event ExecuteOnMasterWithConflictRetries(Func<Event, Event> masterInitialAction, Event eventToUpdate, bool throwOnNonInternalPropertyConflict = false)
		{
			Event result = null;
			this.ExecuteWithConflictRetries(delegate
			{
				result = masterInitialAction(eventToUpdate);
			}, delegate
			{
				Event @event = this.Scope.Read(eventToUpdate.Id, null);
				eventToUpdate.ChangeKey = @event.ChangeKey;
			}, throwOnNonInternalPropertyConflict);
			return result;
		}

		protected virtual bool IsPropagationComplete()
		{
			return this.propagationAttemptedOnAllInstances;
		}

		protected virtual void PropagateToSeriesInstances()
		{
			try
			{
				SortBy sortOrder = SeriesInteropCommand.InstancesQuerySortByDate;
				if (this.InteropConfiguration.ReversePropagationOrder)
				{
					sortOrder = SeriesInteropCommand.InstancesQueryReverseSortByDate;
				}
				this.elapsedPropagationTime.Start();
				this.remainingPropagationCount = this.InteropConfiguration.PropagationCountLimit;
				this.propagationAttemptedOnAllInstances = this.Scope.EventDataProvider.ForEachSeriesItem(this.master, new Func<Event, bool>(this.PropagateToInstance), new Func<IStorePropertyBag, Event>(this.GetInstancePropagationData), null, sortOrder, null, new PropertyDefinition[]
				{
					CalendarItemSchema.LastExecutedCalendarInteropAction
				});
			}
			catch (Exception ex)
			{
				this.propagationAttemptedOnAllInstances = false;
				this.Trace.TraceError<string, Exception>((long)this.GetHashCode(), "Error propagating changes to series instances for series {0}. Error {1}", this.master.SeriesId, ex);
				this.Logger.SafeLogEntry(this.Scope.Session, ex, false, "Error propagating changes to series instances for series {0}", new object[]
				{
					this.master.SeriesId
				});
				if (!this.InteropConfiguration.IgnorePropagationErrors)
				{
					throw;
				}
			}
		}

		protected virtual void TryCleanUp()
		{
			if (this.IsPropagationComplete() && this.InteropConfiguration.ShouldCleanup)
			{
				this.master = this.CleanUp(this.master);
			}
		}

		private static bool IsInternalProperty(PropertyDefinition propertyDefinition)
		{
			return propertyDefinition == CalendarItemSeriesSchema.CalendarInteropActionQueueHasDataInternal || propertyDefinition == CalendarItemSeriesSchema.CalendarInteropActionQueueInternal;
		}

		private void ExecuteWithConflictRetries(Action action, Action dataRefresh, bool throwOnNonInternalPropertyConflict = false)
		{
			uint num = this.InteropConfiguration.MaxCollisionRetryCount;
			try
			{
				IL_0C:
				action();
			}
			catch (IrresolvableConflictException ex)
			{
				if (num == 0U)
				{
					throw;
				}
				num -= 1U;
				if (throwOnNonInternalPropertyConflict && ex.ConflictResolutionResult != null)
				{
					foreach (PropertyConflict propertyConflict in ex.ConflictResolutionResult.PropertyConflicts)
					{
						if (!propertyConflict.ConflictResolvable && !SeriesInteropCommand.IsInternalProperty(propertyConflict.PropertyDefinition))
						{
							throw;
						}
					}
				}
				dataRefresh();
				goto IL_0C;
			}
		}

		private void TryExecuteOnInstance(Event seriesInstance)
		{
			this.ExecuteWithConflictRetries(delegate
			{
				this.ExecuteOnInstance(seriesInstance);
			}, delegate
			{
				seriesInstance = this.Scope.Read(seriesInstance.Id, null);
			}, false);
		}

		private Event GetInstancePropagationData(IStorePropertyBag propertyBag)
		{
			Event basicSeriesEventData = EventExtensions.GetBasicSeriesEventData(propertyBag, this.Scope);
			IActionPropagationState actionPropagationState = basicSeriesEventData;
			actionPropagationState.LastExecutedAction = propertyBag.GetValueOrDefault<Guid?>(CalendarItemSchema.LastExecutedCalendarInteropAction, null);
			return basicSeriesEventData;
		}

		private bool PropagateToInstance(Event seriesInstance)
		{
			if (this.remainingPropagationCount == 0U || this.elapsedPropagationTime.Elapsed > this.InteropConfiguration.PropagationTimeLimit)
			{
				return false;
			}
			this.TryExecuteOnInstance(seriesInstance);
			this.remainingPropagationCount -= 1U;
			return true;
		}

		private static readonly SortBy InstancesQueryReverseSortByDate = new SortBy(CalendarItemInstanceSchema.StartTime, SortOrder.Descending);

		private static readonly SortBy InstancesQuerySortByDate = new SortBy(CalendarItemInstanceSchema.StartTime, SortOrder.Ascending);

		private readonly Stopwatch elapsedPropagationTime = new Stopwatch();

		private uint remainingPropagationCount;

		private bool propagationAttemptedOnAllInstances;

		private Event master;
	}
}
