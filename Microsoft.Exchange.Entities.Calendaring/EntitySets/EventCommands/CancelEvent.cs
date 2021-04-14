using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Entities.Calendaring;

namespace Microsoft.Exchange.Entities.Calendaring.EntitySets.EventCommands
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class CancelEvent : CancelEventBase
	{
		public int? SeriesSequenceNumber { get; set; }

		protected override ITracer Trace
		{
			get
			{
				return ExTraceGlobals.CancelEventTracer;
			}
		}

		protected override VoidResult OnExecute()
		{
			StoreObjectId id = this.Scope.IdConverter.ToStoreObjectId(base.EntityKey);
			this.Scope.EventDataProvider.CancelEvent(id, base.Parameters, this.SeriesSequenceNumber, true, null, null);
			return VoidResult.Value;
		}
	}
}
