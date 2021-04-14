using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.ReliableActions;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Entities.Calendaring;
using Microsoft.Exchange.Entities.DataModel.Calendaring;
using Microsoft.Exchange.Entities.DataModel.ReliableActions;

namespace Microsoft.Exchange.Entities.Calendaring.Interop
{
	internal class SeriesPendingActionsInterop : SeriesInteropCommand
	{
		public SeriesPendingActionsInterop(ICalendarInteropLog logger, ISeriesActionParser parser = null) : base(EventSeriesPropagationConfig.GetBackgroundPropagationConfig(), logger)
		{
			this.seriesActionParser = parser;
		}

		public Event Entity { get; set; }

		protected override ITracer Trace
		{
			get
			{
				return ExTraceGlobals.SeriesPendingActionsInteropTracer;
			}
		}

		private ISeriesActionParser SeriesActionParser
		{
			get
			{
				if (this.seriesActionParser == null)
				{
					this.seriesActionParser = new SeriesActionParser(base.Logger, this.Scope.Session);
				}
				return this.seriesActionParser;
			}
		}

		public override Event CleanUp(Event master)
		{
			foreach (ICalendarInteropSeriesAction calendarInteropSeriesAction in this.pendingActions)
			{
				master = calendarInteropSeriesAction.CleanUp(master);
			}
			return master;
		}

		public override void ExecuteOnInstance(Event seriesInstance)
		{
			try
			{
				this.ApplyPendingActionsOnInstance(seriesInstance);
			}
			catch (ObjectNotFoundException)
			{
				ExTraceGlobals.SeriesPendingActionsInteropTracer.TraceWarning<string, string>((long)this.GetHashCode(), "Series instance {0} from series {1} cannot be found any more - processing will be skipped for it.", seriesInstance.Id, this.Entity.SeriesId);
			}
		}

		protected override bool ShouldExecuteOnInstance(Event seriesInstance)
		{
			return true;
		}

		protected override Event InitialMasterOperation()
		{
			Event entity = this.Entity;
			IActionQueue actionQueue = entity;
			if (actionQueue.OriginalActions != null)
			{
				bool flag = true;
				foreach (ActionInfo actionInfo in actionQueue.OriginalActions)
				{
					if (flag)
					{
						if (DateTime.UtcNow - actionInfo.Timestamp > base.InteropConfiguration.MaxActionLifetime)
						{
							this.pendingActions.Add(new MarkInstancesAsExceptionsCommand(actionInfo.Id)
							{
								EntityKey = entity.Id,
								Entity = entity
							});
						}
						else
						{
							this.pendingActions.Add(this.SeriesActionParser.DeserializeCommand(actionInfo, entity));
						}
						flag = false;
					}
					else
					{
						this.pendingActions.Add(this.SeriesActionParser.DeserializeCommand(actionInfo, entity));
					}
				}
				foreach (ICalendarInteropSeriesAction calendarInteropSeriesAction in this.pendingActions)
				{
					calendarInteropSeriesAction.RestoreExecutionContext(this.Scope, this);
				}
			}
			return entity;
		}

		private void ApplyPendingActionsOnInstance(Event seriesInstance)
		{
			foreach (ICalendarInteropSeriesAction calendarInteropSeriesAction in this.GetRemainingActions(((IActionPropagationState)seriesInstance).LastExecutedAction))
			{
				try
				{
					calendarInteropSeriesAction.ExecuteOnInstance(seriesInstance);
				}
				catch (Exception ex)
				{
					if (this.Trace.IsTraceEnabled(TraceType.ErrorTrace))
					{
						this.Trace.TraceError<string, string, Exception>((long)this.GetHashCode(), "Error propagating action " + calendarInteropSeriesAction.CommandId + " to instance {0} of series {1}. Error: {2}.", seriesInstance.Id, this.Entity.SeriesId, ex);
					}
					base.Logger.SafeLogEntry(this.Scope.Session, ex, false, "Error propagating action {0} to instance {1} of series {2}.", new object[]
					{
						calendarInteropSeriesAction.CommandId,
						seriesInstance.Id,
						this.Entity.SeriesId
					});
					throw;
				}
			}
		}

		private IEnumerable<ICalendarInteropSeriesAction> GetRemainingActions(Guid? lastInteropActionAppliedOnTarget)
		{
			List<ICalendarInteropSeriesAction> list = new List<ICalendarInteropSeriesAction>();
			bool flag = false;
			foreach (ICalendarInteropSeriesAction calendarInteropSeriesAction in this.pendingActions)
			{
				if (!flag)
				{
					if (lastInteropActionAppliedOnTarget != null && lastInteropActionAppliedOnTarget.Value == calendarInteropSeriesAction.CommandId)
					{
						flag = true;
					}
				}
				else
				{
					list.Add(calendarInteropSeriesAction);
				}
			}
			if (!flag)
			{
				return this.pendingActions;
			}
			return list;
		}

		private readonly List<ICalendarInteropSeriesAction> pendingActions = new List<ICalendarInteropSeriesAction>();

		private ISeriesActionParser seriesActionParser;
	}
}
