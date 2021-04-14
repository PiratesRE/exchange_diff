using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Entities.Calendaring;
using Microsoft.Exchange.Entities.DataModel.Calendaring;

namespace Microsoft.Exchange.Entities.Calendaring.EntitySets.EventCommands
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class ForwardEvent : ForwardEventBase
	{
		public int? SeriesSequenceNumber { get; set; }

		public string OccurrencesViewPropertiesBlob { get; set; }

		internal Event UpdateToEvent { get; set; }

		protected override ITracer Trace
		{
			get
			{
				return ExTraceGlobals.ForwardEventTracer;
			}
		}

		protected override VoidResult OnExecute()
		{
			StoreId entityStoreId = this.GetEntityStoreId();
			this.Scope.EventDataProvider.ForwardEvent(entityStoreId, base.Parameters, this.UpdateToEvent, this.SeriesSequenceNumber, this.OccurrencesViewPropertiesBlob, this.Context);
			return VoidResult.Value;
		}
	}
}
