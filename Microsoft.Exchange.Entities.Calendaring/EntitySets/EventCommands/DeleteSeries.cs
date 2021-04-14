using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Entities.Calendaring;
using Microsoft.Exchange.Entities.Calendaring.Interop;
using Microsoft.Exchange.Entities.DataModel.Calendaring;

namespace Microsoft.Exchange.Entities.Calendaring.EntitySets.EventCommands
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[DataContract]
	internal class DeleteSeries : DeleteEventBase, ICalendarInteropSeriesAction
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
				return ExTraceGlobals.DeleteSeriesTracer;
			}
		}

		void ICalendarInteropSeriesAction.RestoreExecutionContext(Events entitySet, SeriesInteropCommand seriesInteropCommand)
		{
			this.seriesPropagation = seriesInteropCommand;
			base.RestoreScope(entitySet);
		}

		Event ICalendarInteropSeriesAction.CleanUp(Event master)
		{
			this.Scope.EventDataProvider.Delete(master.StoreId, DeleteItemFlags.MoveToDeletedItems);
			return null;
		}

		void ICalendarInteropSeriesAction.ExecuteOnInstance(Event seriesInstance)
		{
			this.Scope.EventDataProvider.Delete(seriesInstance.StoreId, DeleteItemFlags.MoveToDeletedItems);
		}

		Event ICalendarInteropSeriesAction.GetInitialMasterValue()
		{
			return this.Scope.Read(base.EntityKey, null);
		}

		Event ICalendarInteropSeriesAction.InitialMasterOperation(Event updateToMaster)
		{
			return this.Scope.EventDataProvider.Update(updateToMaster, this.Context);
		}

		protected override void DeleteCancelledEventFromAttendeesCalendar(Event eventToDelete)
		{
			this.seriesPropagation = this.CreateInteropPropagationCommand(null);
			this.seriesPropagation.Execute(null);
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
	}
}
