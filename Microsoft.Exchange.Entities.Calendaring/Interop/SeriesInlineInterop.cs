using System;
using Microsoft.Exchange.Data.Storage.ReliableActions;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Entities.Calendaring;
using Microsoft.Exchange.Entities.DataModel.Calendaring;
using Microsoft.Exchange.Entities.DataModel.ReliableActions;
using Microsoft.Exchange.Entities.Serialization;

namespace Microsoft.Exchange.Entities.Calendaring.Interop
{
	internal class SeriesInlineInterop : SeriesInteropCommand
	{
		public SeriesInlineInterop(ICalendarInteropSeriesAction actionToPropagate, ICalendarInteropLog logger) : base(EventSeriesPropagationConfig.GetInlinePropagationConfig(), logger ?? CalendarInteropLog.Default)
		{
			this.actionToPropagate = actionToPropagate;
		}

		protected override ITracer Trace
		{
			get
			{
				return ExTraceGlobals.SeriesInlineInteropTracer;
			}
		}

		public override Event CleanUp(Event master)
		{
			return this.actionToPropagate.CleanUp(master);
		}

		public override void ExecuteOnInstance(Event seriesInstance)
		{
			if (this.ShouldExecuteOnInstance(seriesInstance))
			{
				this.actionToPropagate.ExecuteOnInstance(seriesInstance);
				return;
			}
			this.propagationWasSkippedOnSomeInstances = true;
		}

		protected override bool IsPropagationComplete()
		{
			return base.IsPropagationComplete() && !this.propagationWasSkippedOnSomeInstances;
		}

		protected override Event InitialMasterOperation()
		{
			string rawData = EntitySerializer.Serialize<ICalendarInteropSeriesAction>(this.actionToPropagate);
			Event initialMasterValue = this.actionToPropagate.GetInitialMasterValue();
			IActionQueue actionQueue = initialMasterValue;
			ActionInfo actionInfo = new ActionInfo(this.actionToPropagate.CommandId, DateTime.UtcNow, this.actionToPropagate.GetType().Name, rawData);
			actionQueue.ActionsToAdd = new ActionInfo[]
			{
				actionInfo
			};
			Event @event = base.ExecuteOnMasterWithConflictRetries(new Func<Event, Event>(this.actionToPropagate.InitialMasterOperation), initialMasterValue, true);
			this.precedingAction = SeriesInlineInterop.GetPrecedingActionFromQueue(@event);
			return @event;
		}

		protected override bool ShouldExecuteOnInstance(Event seriesInstance)
		{
			return ((IActionPropagationState)seriesInstance).LastExecutedAction == this.precedingAction || (this.precedingAction == null && ((IActionPropagationState)seriesInstance).LastExecutedAction != null && ((IActionPropagationState)seriesInstance).LastExecutedAction.Value != this.actionToPropagate.CommandId);
		}

		private static Guid? GetPrecedingActionFromQueue(IActionQueue queue)
		{
			if (queue.OriginalActions != null && queue.OriginalActions.Length > 1)
			{
				return new Guid?(queue.OriginalActions[queue.OriginalActions.Length - 2].Id);
			}
			return null;
		}

		private readonly ICalendarInteropSeriesAction actionToPropagate;

		private Guid? precedingAction;

		private bool propagationWasSkippedOnSomeInstances;
	}
}
