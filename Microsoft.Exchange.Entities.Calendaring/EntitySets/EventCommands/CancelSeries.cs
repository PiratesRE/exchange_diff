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
	internal class CancelSeries : CancelEventBase, ICalendarInteropSeriesAction
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
				return ExTraceGlobals.CancelSeriesTracer;
			}
		}

		public void RestoreExecutionContext(Events entitySet, SeriesInteropCommand seriesInteropCommand)
		{
			this.seriesPropagation = seriesInteropCommand;
			base.RestoreScope(entitySet);
		}

		public Event CleanUp(Event master)
		{
			this.Scope.EventDataProvider.Delete(master.StoreId, DeleteItemFlags.MoveToDeletedItems);
			return null;
		}

		public void ExecuteOnInstance(Event seriesInstance)
		{
			this.Scope.EventDataProvider.CancelEvent(seriesInstance.StoreId, base.Parameters, new int?(this.seriesSequenceNumber), true, null, this.masterGoid);
		}

		public Event GetInitialMasterValue()
		{
			return this.Scope.Read(base.EntityKey, null);
		}

		public Event InitialMasterOperation(Event updateToMaster)
		{
			this.Scope.EventDataProvider.CancelEvent(updateToMaster.StoreId, base.Parameters, new int?(this.seriesSequenceNumber), false, updateToMaster, null);
			return this.Scope.SeriesEventDataProvider.Read(this.Scope.IdConverter.ToStoreObjectId(updateToMaster.Id));
		}

		protected override VoidResult OnExecute()
		{
			Event initialMasterValue = this.GetInitialMasterValue();
			IEventInternal eventInternal = initialMasterValue;
			IEventInternal eventInternal2 = this.Scope.Read(initialMasterValue.Id, this.Context);
			eventInternal.SeriesSequenceNumber = eventInternal2.SeriesSequenceNumber + 1;
			this.seriesSequenceNumber = eventInternal.SeriesSequenceNumber;
			this.masterGoid = ((eventInternal2.GlobalObjectId != null) ? new GlobalObjectId(eventInternal2.GlobalObjectId).Bytes : null);
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

		[DataMember]
		private byte[] masterGoid;
	}
}
